using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Migration;

public sealed class SmartUserProfileSeeder
{
    private readonly DbContext _db;

    public SmartUserProfileSeeder(DbContext db) => _db = db;

    public enum AttachmentMode
    {
        None,
        Pooled,          // reuse a limited pool of Resource rows (fast, low DB growth)
        UniquePerProfile // creates new Resource rows per profile (realistic, heavy)
    }

    public sealed class SeedOptions
    {
        public int Count { get; init; } = 10_000;
        public int BatchSize { get; init; } = 800; // 500..1500 is typically good
        public double CompletionRate { get; init; } = 0.80;

        public AttachmentMode ProfileAttachmentMode { get; init; } = AttachmentMode.Pooled;
        public AttachmentMode CertificateAttachmentMode { get; init; } = AttachmentMode.Pooled;

        // Realism knobs
        public double SponsorProbabilityForNonQatari { get; init; } = 0.35;
        public double ResidenceAddressProbability { get; init; } = 0.70;
        public double BirthCertificateProbability { get; init; } = 0.20;
        public double MarriageCertificateProbabilityIfMarried { get; init; } = 0.25;

        public int Seed { get; init; } = 20260106;
        public bool UseTransactionPerBatch { get; init; } = true;
    }

    public async Task SeedSmartApplicantsAsync(
        Guid createdById,
        SeedOptions? options = null,
        CancellationToken ct = default)
    {
        options ??= new SeedOptions();
        if (options.Count <= 0) return;
        if (options.CompletionRate is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(options.CompletionRate));

        // Preload lookup IDs once
        var lookups = await LookupCache.LoadAsync(_db, ct);

        // Prepare attachment pools (optional)
        var pools = await ResourcePools.BuildAsync(_db, createdById, options, ct);

        // Weighted distributions
        var candidateTypeWeighted = new WeightedPicker<Guid>([
            (CandidateTypeIds.Qatari, 25),
            (CandidateTypeIds.GCC, 15),
            (CandidateTypeIds.ResidentQatar, 30),
            (CandidateTypeIds.NonQatari, 30)
        ]);

        var targetEntityWeighted = new WeightedPicker<Guid>([
            (TargetEntityIds.Schools, 65),
            (TargetEntityIds.Ministry, 35)
        ]);

        var rnd = new SmartRandom(options.Seed);

        // Performance switches
        var previousDetectChanges = _db.ChangeTracker.AutoDetectChangesEnabled;
        var previousQueryTracking = _db.ChangeTracker.QueryTrackingBehavior;

        _db.ChangeTracker.AutoDetectChangesEnabled = false;
        _db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        try
        {
            for (var offset = 0; offset < options.Count; offset += options.BatchSize)
            {
                var take = Math.Min(options.BatchSize, options.Count - offset);

                if (options.UseTransactionPerBatch)
                {
                    await using var tx = await _db.Database.BeginTransactionAsync(ct);
                    await SeedBatchAsync(offset, take, createdById, options, lookups, pools, candidateTypeWeighted, targetEntityWeighted, rnd, ct);
                    await tx.CommitAsync(ct);
                }
                else
                {
                    await SeedBatchAsync(offset, take, createdById, options, lookups, pools, candidateTypeWeighted, targetEntityWeighted, rnd, ct);
                }

                _db.ChangeTracker.Clear();
            }
        }
        finally
        {
            _db.ChangeTracker.AutoDetectChangesEnabled = previousDetectChanges;
            _db.ChangeTracker.QueryTrackingBehavior = previousQueryTracking;
        }
    }

    private async Task SeedBatchAsync(
        int offset,
        int take,
        Guid createdById,
        SeedOptions options,
        LookupCache lookups,
        ResourcePools pools,
        WeightedPicker<Guid> candidateTypeWeighted,
        WeightedPicker<Guid> targetEntityWeighted,
        SmartRandom rnd,
        CancellationToken ct)
    {
        var users = new List<ApplicantUser>(take);

        for (var i = 0; i < take; i++)
        {
            var seq = offset + i + 1;

            var name = NameFactory.Generate(rnd);
            var email = EmailFactory.MakeEmail(name, seq);
            var displayName = $"{name.FirstEn} {name.LastEn}";

            var userResult = ApplicantUser.Register(email, displayName);
            if (userResult.IsFailed)
                throw new InvalidOperationException($"Failed to create user #{seq}: {string.Join(", ", userResult.Errors.Select(e => e.Message))}");

            var user = (ApplicantUser)userResult.Value;

            if (user.Id == Guid.Empty) user.Id = Guid.NewGuid();
            user.CreatedById = createdById;
            user.CreatedDate = DateTimeOffset.UtcNow;

            user.FullNameEn = $"{name.FirstEn} {name.LastEn}";
            user.FullNameAr = $"{name.FirstAr} {name.LastAr}";

            var makeComplete = rnd.NextDouble() < options.CompletionRate;

            var profile = await BuildProfileAsync(
                rnd: rnd,
                lookups: lookups,
                pools: pools,
                createdById: createdById,
                userId: user.Id,
                candidateTypeId: candidateTypeWeighted.Pick(rnd),
                targetEntityId: targetEntityWeighted.Pick(rnd),
                provider: ProviderFactory.PickProvider(rnd),
                makeComplete: makeComplete,
                options: options,
                ct: ct
            );

            user.Profile = profile;
            profile.User = user;

            users.Add(user);
        }

        // AddRange + single SaveChanges per batch
        await _db.Set<ApplicantUser>().AddRangeAsync(users, ct);
        await _db.SaveChangesAsync(ct);
    }

    private async Task<UserProfile> BuildProfileAsync(
        SmartRandom rnd,
        LookupCache lookups,
        ResourcePools pools,
        Guid createdById,
        Guid userId,
        Guid candidateTypeId,
        Guid targetEntityId,
        string provider,
        bool makeComplete,
        SeedOptions options,
        CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        var age = AgeFactory.PickAge(rnd);
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-age).AddDays(-rnd.NextInt(0, 365)));

        var (maritalStatusId, childrenCount) = FamilyFactory.PickFamily(rnd, age);

        var nationalityId = NationalityFactory.PickNationality(rnd, lookups.CountryIds);
        var residenceCountryId = ResidenceFactory.PickResidenceCountry(rnd, nationalityId, lookups.CountryIds);
        var interviewLocationId = ResidenceFactory.PickInterviewCountry(rnd, residenceCountryId, lookups.CountryIds);

        var genderId = rnd.NextDouble() < 0.55 ? GenderIds.Male : GenderIds.Female;
        var religionId = ReligionIds.Islam;

        var nationalNumber = NationalIdFactory.MakeNationalNumber(rnd, candidateTypeId);
        var qidExpiry = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(rnd.NextInt(1, 10)).AddDays(rnd.NextInt(0, 60)));

        Guid? officeId = null;
        if (lookups.OfficeIds.Count > 0 && rnd.NextDouble() < 0.35)
            officeId = lookups.OfficeIds[rnd.NextInt(0, lookups.OfficeIds.Count)];

        // Attachments (CV + NationalCard) — controlled by ProfileAttachmentMode
        Resource? resume = await pools.GetProfileResumeAsync(options.ProfileAttachmentMode, createdById, rnd, ct);
        Resource? nationalCard = await pools.GetProfileNationalCardAsync(options.ProfileAttachmentMode, createdById, rnd, ct);

        var useResidenceAddress = rnd.NextDouble() < options.ResidenceAddressProbability;
        ResidenceAddress? residenceAddress = null;
        string? freeTextAddress = null;

        if (useResidenceAddress)
        {
            // ResidenceAddress certificate: controlled by CertificateAttachmentMode
            var addressCert = await pools.GetCertificateAsync(options.CertificateAttachmentMode, createdById, "residence-address-certificate.pdf", "application/pdf", rnd, ct);

            residenceAddress = ResidenceAddress.Create(
                buildingNo: rnd.NextInt(1, 400),
                streetNo: rnd.NextInt(1, 900),
                zoneNo: rnd.NextInt(1, 250),
                unitNo: rnd.NextInt(0, 80),
                certificateId: addressCert?.Id ?? Guid.Empty // If your domain requires non-empty, set mode to Pooled/Unique
            );
        }
        else
        {
            freeTextAddress = AddressFactory.MakeAddress(rnd);
        }

        // Sponsor: only for some Resident/NonQatari
        SponsorProfile? sponsor = null;
        if ((candidateTypeId == CandidateTypeIds.ResidentQatar || candidateTypeId == CandidateTypeIds.NonQatari)
            && rnd.NextDouble() < options.SponsorProbabilityForNonQatari)
        {
            var sponsorCard = await pools.GetCertificateAsync(options.CertificateAttachmentMode, createdById, "sponsor-card.jpg", "image/jpeg", rnd, ct);

            sponsor = new SponsorProfile
            {
                Id = Guid.NewGuid(),
                SponsorTypeId = SponsorTypeIds.Individual, // If FK required, load SponsorTypeIds and pick
                SponsorName = SponsorFactory.MakeSponsorName(rnd),
                SponsorNumber = SponsorFactory.MakeSponsorNumber(rnd),
                QIDExpiry = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(rnd.NextInt(1, 8))),
                SponsorCardId = sponsorCard?.Id // allow null if schema allows
            };
        }

        Guid? birthCertId = null;
        Guid? marriageCertId = null;

        if (rnd.NextDouble() < options.BirthCertificateProbability)
        {
            var birthCert = await pools.GetCertificateAsync(options.CertificateAttachmentMode, createdById, "birth-certificate.pdf", "application/pdf", rnd, ct);
            birthCertId = birthCert?.Id;
        }

        if (maritalStatusId == MaritalStatusIds.Married && rnd.NextDouble() < options.MarriageCertificateProbabilityIfMarried)
        {
            var marriageCert = await pools.GetCertificateAsync(options.CertificateAttachmentMode, createdById, "marriage-certificate.pdf", "application/pdf", rnd, ct);
            marriageCertId = marriageCert?.Id;
        }

        // Qualifications / Experiences / Courses / Achievements / Languages / Skills
        var quals = await QualificationFactory.MakeQualificationsAsync(rnd, lookups, age, createdById, pools, options, ct);
        var exps = await ExperienceFactory.MakeExperiencesAsync(rnd, lookups, age, quals, createdById, pools, options, ct);
        var courses = await TrainingFactory.MakeCoursesAsync(rnd, lookups, createdById, pools, options, ct);
        var achievements = await AchievementFactory.MakeAchievementsAsync(rnd, lookups, createdById, pools, options, ct);
        var langs = LanguageFactory.MakeLanguages(rnd);
        var skills = SkillFactory.MakeSkills(rnd, lookups.SkillTypeIds);

        // Completion shaping
        if (!makeComplete)
        {
            var omitRoll = rnd.NextInt(0, 4);
            switch (omitRoll)
            {
                case 0: resume = null; break;
                case 1: nationalNumber = null; break;
                case 2: quals.Clear(); break;
                case 3: langs.Clear(); break;
            }
        }

        var profile = new UserProfile
        {
            CreatedById = createdById,
            CreatedDate = now,
            IsDeleted = false,

            UserId = userId,
            Provider = provider,

            CandidateTypeId = candidateTypeId,
            TargetEntityId = targetEntityId,
            OfficeId = officeId,

            ResumeAttachmentId = resume?.Id,
            NationalCardId = nationalCard?.Id, // if required in DB, ensure mode != None

            NationalNumber = nationalNumber,
            QIDExpiry = qidExpiry,

            BirthDate = birthDate,

            NationalityId = nationalityId,
            GenderId = genderId,
            ReligionId = religionId,
            MaritalStatusId = maritalStatusId,
            ChildrenCount = childrenCount,

            ResidenceCountryId = residenceCountryId,
            InterviewLocationId = interviewLocationId,

            Address = freeTextAddress,
            ResidenceAddress = residenceAddress,

            HasDisability = rnd.NextDouble() < 0.04,
            DisabilityDetails = null,

            SponsorProfile = sponsor,
            // DO NOT set SponsorProfileId from sponsor.Id manually unless your domain explicitly requires it.

            BirthdayCertificateId = birthCertId,
            MarriageCertificateId = marriageCertId,

            Qualifications = quals,
            Experiences = exps,
            TrainingCourses = courses,
            Achievements = achievements,
            Skills = skills,
            Languages = langs,

            Status = makeComplete ? UserProfileStatus.Submitted : UserProfileStatus.InCreation,
            AvailableForRecruitment = rnd.NextDouble() < 0.90
        };

        // Back-references
        foreach (var q in profile.Qualifications ?? Enumerable.Empty<Qualification>()) q.UserProfile = profile;
        foreach (var e in profile.Experiences ?? Enumerable.Empty<Experience>()) e.UserProfile = profile;
        foreach (var c in profile.TrainingCourses ?? Enumerable.Empty<TrainingCourse>()) c.UserProfile = profile;
        foreach (var a in profile.Achievements ?? Enumerable.Empty<Achievement>()) a.UserProfile = profile;
        foreach (var s in profile.Skills ?? Enumerable.Empty<ProfileSkill>()) s.UserProfile = profile;
        foreach (var l in profile.Languages ?? Enumerable.Empty<ProfileLanguage>()) l.UserProfile = profile;

        return profile;
    }

    // ==========================
    // Resource pools (to prevent resource explosion)
    // ==========================
    private sealed class ResourcePools
    {
        private readonly DbContext _db;
        private readonly List<Resource> _resumePool = new();
        private readonly List<Resource> _nationalPool = new();
        private readonly Dictionary<string, List<Resource>> _certPools = new(StringComparer.OrdinalIgnoreCase);

        private ResourcePools(DbContext db) => _db = db;

        public static async Task<ResourcePools> BuildAsync(DbContext db, Guid createdById, SeedOptions options, CancellationToken ct)
        {
            var pools = new ResourcePools(db);

            if (options.ProfileAttachmentMode == AttachmentMode.Pooled)
            {
                pools._resumePool.AddRange(await CreatePoolAsync(db, createdById, "cv_pool", "application/pdf", poolSize: 50, ct));
                pools._nationalPool.AddRange(await CreatePoolAsync(db, createdById, "national_pool", "image/jpeg", poolSize: 50, ct));
            }

            if (options.CertificateAttachmentMode == AttachmentMode.Pooled)
            {
                pools._certPools["residence-address-certificate.pdf"] = await CreatePoolAsync(db, createdById, "residence-cert_pool", "application/pdf", 30, ct);
                pools._certPools["sponsor-card.jpg"] = await CreatePoolAsync(db, createdById, "sponsor-card_pool", "image/jpeg", 30, ct);
                pools._certPools["birth-certificate.pdf"] = await CreatePoolAsync(db, createdById, "birth-cert_pool", "application/pdf", 30, ct);
                pools._certPools["marriage-certificate.pdf"] = await CreatePoolAsync(db, createdById, "marriage-cert_pool", "application/pdf", 30, ct);
            }

            // Persist pooled resources once (fast)
            var allPooled = pools._resumePool
                .Concat(pools._nationalPool)
                .Concat(pools._certPools.Values.SelectMany(x => x))
                .ToList();

            if (allPooled.Count > 0)
            {
                await db.Set<Resource>().AddRangeAsync(allPooled, ct);
                await db.SaveChangesAsync(ct);
                db.ChangeTracker.Clear();
            }

            return pools;
        }

        public Task<Resource?> GetProfileResumeAsync(AttachmentMode mode, Guid createdById, SmartRandom rnd, CancellationToken ct)
            => GetProfileAsync(mode, createdById, rnd, ct, pool: _resumePool, fileName: $"cv_{Guid.NewGuid():N}.pdf", contentType: "application/pdf");

        public Task<Resource?> GetProfileNationalCardAsync(AttachmentMode mode, Guid createdById, SmartRandom rnd, CancellationToken ct)
            => GetProfileAsync(mode, createdById, rnd, ct, pool: _nationalPool, fileName: $"national_{Guid.NewGuid():N}.jpg", contentType: "image/jpeg");

        public async Task<Resource?> GetCertificateAsync(AttachmentMode mode, Guid createdById, string fileName, string contentType, SmartRandom rnd, CancellationToken ct)
        {
            if (mode == AttachmentMode.None) return null;

            if (mode == AttachmentMode.Pooled)
            {
                if (_certPools.TryGetValue(fileName, out var pool) && pool.Count > 0)
                    return pool[rnd.NextInt(0, pool.Count)];

                // fallback to any cert pool if missing
                var any = _certPools.Values.FirstOrDefault(x => x.Count > 0);
                return any is null ? null : any[rnd.NextInt(0, any.Count)];
            }

            // UniquePerProfile
            var res = CreateResource(createdById, fileName, contentType);
            await _db.Set<Resource>().AddAsync(res, ct);
            return res;
        }

        private async Task<Resource?> GetProfileAsync(
            AttachmentMode mode,
            Guid createdById,
            SmartRandom rnd,
            CancellationToken ct,
            List<Resource> pool,
            string fileName,
            string contentType)
        {
            if (mode == AttachmentMode.None) return null;

            if (mode == AttachmentMode.Pooled && pool.Count > 0)
                return pool[rnd.NextInt(0, pool.Count)];

            // UniquePerProfile
            var res = CreateResource(createdById, fileName, contentType);
            await _db.Set<Resource>().AddAsync(res, ct);
            return res;
        }

        private static async Task<List<Resource>> CreatePoolAsync(DbContext db, Guid createdById, string prefix, string contentType, int poolSize, CancellationToken ct)
        {
            var list = new List<Resource>(poolSize);
            for (var i = 0; i < poolSize; i++)
            {
                list.Add(CreateResource(createdById, $"{prefix}_{i:D3}.bin", contentType));
            }
            return await Task.FromResult(list);
        }

        private static Resource CreateResource(Guid createdById, string fileName, string contentType)
        {
            return new Resource
            {
                Id = Guid.NewGuid(),
                CreatedById = createdById,
                CreatedDate = DateTimeOffset.UtcNow,
                IsDeleted = false,

                Name = fileName,
                Description = null,
                Url = $"private/seed/{Guid.NewGuid():N}/{fileName}",
                Key = Guid.NewGuid().ToString(),
                Type = contentType,
                Size = (ulong)RandomNumberGenerator.GetInt32(15_000, 900_000),
                AdditionalData = null,
                ContentHash = HashFactory.Sha256Hex($"{fileName}:{DateTime.UtcNow.Ticks}:{Guid.NewGuid()}")
            };
        }
    }

    // ==========================
    // Lookup cache
    // ==========================
    private sealed class LookupCache
    {
        public List<Guid> CountryIds { get; private set; } = [];
        public List<Guid> UniversityIds { get; private set; } = [];
        public List<Guid> MajorIds { get; private set; } = [];
        public List<Guid> OfficeIds { get; private set; } = [];
        public List<Guid> SkillTypeIds { get; private set; } = [];

        public static async Task<LookupCache> LoadAsync(DbContext db, CancellationToken ct)
        {
            var cache = new LookupCache();

            cache.CountryIds = await db.Set<Country>().AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => x.Id)
                .Take(250)
                .ToListAsync(ct);

            cache.UniversityIds = await db.Set<University>().AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => x.Id)
                .Take(250)
                .ToListAsync(ct);

            cache.MajorIds = await db.Set<Major>().AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => x.Id)
                .Take(500)
                .ToListAsync(ct);

            cache.OfficeIds = await db.Set<Office>().AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => x.Id)
                .Take(100)
                .ToListAsync(ct);

            try
            {
                cache.SkillTypeIds = await db.Set<SkillType>().AsNoTracking()
                    .Select(x => x.Id)
                    .Take(500)
                    .ToListAsync(ct);
            }
            catch
            {
                cache.SkillTypeIds = [];
            }

            if (cache.CountryIds.Count == 0)
                throw new InvalidOperationException("No Countries found. Seed lookups first.");

            return cache;
        }
    }

    // ==========================
    // Random helpers
    // ==========================
    private sealed class SmartRandom(int seed)
    {
        private readonly Random _r = new(seed);

        public int NextInt(int minInclusive, int maxExclusive) => _r.Next(minInclusive, maxExclusive);
        public double NextDouble() => _r.NextDouble();
    }

    private sealed class WeightedPicker<T>
    {
        private readonly (T item, int weight)[] _items;
        private readonly int _total;

        public WeightedPicker(IEnumerable<(T item, int weight)> items)
        {
            _items = items.Where(x => x.weight > 0).ToArray();
            _total = _items.Sum(x => x.weight);
            if (_items.Length == 0 || _total <= 0) throw new ArgumentException("No weighted items.");
        }

        public T Pick(SmartRandom rnd)
        {
            var roll = rnd.NextInt(1, _total + 1);
            var acc = 0;
            foreach (var (item, w) in _items)
            {
                acc += w;
                if (roll <= acc) return item;
            }
            return _items[^1].item;
        }
    }

    // ==========================
    // Factories
    // ==========================
    private static class ProviderFactory
    {
        private static readonly string[] Providers = ["qatar-pass", "azure-ad", "local", "google"];
        public static string PickProvider(SmartRandom rnd) => Providers[rnd.NextInt(0, Providers.Length)];
    }

    private static class AgeFactory
    {
        public static int PickAge(SmartRandom rnd)
        {
            var p = rnd.NextDouble();
            if (p < 0.10) return rnd.NextInt(18, 22);
            if (p < 0.80) return rnd.NextInt(22, 41);
            return rnd.NextInt(41, 56);
        }
    }

    private static class FamilyFactory
    {
        public static (Guid maritalStatusId, int childrenCount) PickFamily(SmartRandom rnd, int age)
        {
            var marriedLikelihood =
                age < 23 ? 0.08 :
                age < 28 ? 0.30 :
                age < 35 ? 0.55 :
                age < 45 ? 0.65 :
                0.60;

            var married = rnd.NextDouble() < marriedLikelihood;
            if (!married) return (MaritalStatusIds.Single, 0);

            int children =
                age < 28 ? rnd.NextInt(0, 2) :
                age < 35 ? rnd.NextInt(0, 4) :
                age < 45 ? rnd.NextInt(0, 6) :
                rnd.NextInt(0, 7);

            return (MaritalStatusIds.Married, children);
        }
    }

    private static class NationalityFactory
    {
        public static Guid PickNationality(SmartRandom rnd, IReadOnlyList<Guid> countryIds)
        {
            var p = rnd.NextDouble();
            if (p < 0.60) return countryIds[rnd.NextInt(0, Math.Min(30, countryIds.Count))];
            return countryIds[rnd.NextInt(0, countryIds.Count)];
        }
    }

    private static class ResidenceFactory
    {
        public static Guid PickResidenceCountry(SmartRandom rnd, Guid nationalityId, IReadOnlyList<Guid> countryIds)
            => rnd.NextDouble() < 0.60 ? nationalityId : countryIds[rnd.NextInt(0, countryIds.Count)];

        public static Guid PickInterviewCountry(SmartRandom rnd, Guid residenceCountryId, IReadOnlyList<Guid> countryIds)
            => rnd.NextDouble() < 0.85 ? residenceCountryId : countryIds[rnd.NextInt(0, countryIds.Count)];
    }

    private static class NationalIdFactory
    {
        public static string MakeNationalNumber(SmartRandom rnd, Guid candidateTypeId)
        {
            if (candidateTypeId == CandidateTypeIds.Qatari || candidateTypeId == CandidateTypeIds.ResidentQatar)
            {
                var first = rnd.NextDouble() < 0.7 ? "2" : "3";
                var rest = string.Concat(Enumerable.Range(0, 10).Select(_ => rnd.NextInt(0, 10).ToString(CultureInfo.InvariantCulture)));
                return first + rest;
            }

            var len = rnd.NextInt(8, 13);
            return string.Concat(Enumerable.Range(0, len).Select(_ => rnd.NextInt(0, 10).ToString(CultureInfo.InvariantCulture)));
        }
    }

    private static class AddressFactory
    {
        private static readonly string[] Streets =
        [
            "Al Sadd", "Al Waab", "West Bay", "Al Rayyan", "Al Dafna", "Al Thumama", "Madinat Khalifa", "Umm Ghuwailina"
        ];

        public static string MakeAddress(SmartRandom rnd)
        {
            var street = Streets[rnd.NextInt(0, Streets.Length)];
            var building = rnd.NextInt(1, 250);
            var apt = rnd.NextInt(1, 80);
            return $"{street}, Building {building}, Apt {apt}";
        }
    }

    private static class SponsorFactory
    {
        private static readonly string[] SponsorNames =
        [
            "Ahmed Al-Mansoori", "Mohammed Al-Kuwari", "Hassan Al-Nuaimi", "Khalid Al-Ansari"
        ];

        public static string MakeSponsorName(SmartRandom rnd) => SponsorNames[rnd.NextInt(0, SponsorNames.Length)];

        public static string MakeSponsorNumber(SmartRandom rnd)
            => "3" + string.Concat(Enumerable.Range(0, 10).Select(_ => rnd.NextInt(0, 10).ToString(CultureInfo.InvariantCulture)));
    }

    // Certificate creation inside sub-entities is now controlled by CertificateAttachmentMode
    private static class QualificationFactory
    {
        public static async Task<List<Qualification>> MakeQualificationsAsync(
            SmartRandom rnd,
            LookupCache lookups,
            int age,
            Guid createdById,
            ResourcePools pools,
            SeedOptions options,
            CancellationToken ct)
        {
            var count = rnd.NextDouble() < 0.75 ? 1 : 2;

            var list = new List<Qualification>(count);
            for (var i = 0; i < count; i++)
            {
                var degreeId = PickDegree(rnd, age);
                var graduationYear = PickGraduationYear(rnd, age);

                var cert = await pools.GetCertificateAsync(options.CertificateAttachmentMode, createdById, "qualification-certificate.pdf", "application/pdf", rnd, ct);

                list.Add(new Qualification
                {
                    DegreeId = degreeId,
                    CountryId = lookups.CountryIds[rnd.NextInt(0, lookups.CountryIds.Count)],
                    UniversityId = lookups.UniversityIds.Count > 0 ? lookups.UniversityIds[rnd.NextInt(0, lookups.UniversityIds.Count)] : null,
                    MajorId = lookups.MajorIds.Count > 0 ? lookups.MajorIds[rnd.NextInt(0, lookups.MajorIds.Count)] : null,
                    GraduationYear = graduationYear,
                    StudyTypeId = PickStudyType(rnd),
                    GPA = PickGpa(rnd),
                    RatingId = PickRating(rnd),
                    CertificateId = cert?.Id, // if required, keep mode != None
                    CreatedById = createdById,
                    CreatedDate = DateTimeOffset.UtcNow
                });
            }

            return list;
        }

        private static Guid PickDegree(SmartRandom rnd, int age)
        {
            var p = rnd.NextDouble();
            if (age < 26) return DegreeIds.Bachelor;
            if (p < 0.70) return DegreeIds.Bachelor;
            if (p < 0.93) return DegreeIds.Master;
            return DegreeIds.Doctorate;
        }

        private static int PickGraduationYear(SmartRandom rnd, int age)
        {
            var min = DateTime.UtcNow.Year - age + 20;
            var max = DateTime.UtcNow.Year - 1;
            if (min >= max) return max;
            return rnd.NextInt(min, max + 1);
        }

        private static Guid PickStudyType(SmartRandom rnd)
        {
            var p = rnd.NextDouble();
            if (p < 0.78) return StudyTypeIds.Regular;
            if (p < 0.90) return StudyTypeIds.DistanceLearning;
            return StudyTypeIds.Affiliation;
        }

        private static decimal PickGpa(SmartRandom rnd)
        {
            var x = (decimal)((rnd.NextDouble() + rnd.NextDouble()) / 2.0);
            var gpa = 2.0m + (x * 2.0m);
            return Math.Round(gpa, 2);
        }

        private static Guid PickRating(SmartRandom rnd)
        {
            var p = rnd.NextDouble();
            if (p < 0.10) return RatingGradeIds.Acceptable;
            if (p < 0.40) return RatingGradeIds.Good;
            if (p < 0.72) return RatingGradeIds.VeryGood;
            if (p < 0.92) return RatingGradeIds.Excellent;
            return RatingGradeIds.AboveExcellent;
        }
    }

    private static class ExperienceFactory
    {
        private static readonly string[] Employers = ["Qatar Holding", "Doha Tech", "Gulf Services", "National Trading", "Edu Services", "HealthCare Group"];
        private static readonly string[] JobTitles = ["Assistant", "Specialist", "Coordinator", "Officer", "Analyst", "Supervisor"];

        public static async Task<List<Experience>> MakeExperiencesAsync(
            SmartRandom rnd,
            LookupCache lookups,
            int age,
            List<Qualification> quals,
            Guid createdById,
            ResourcePools pools,
            SeedOptions options,
            CancellationToken ct)
        {
            var max = age < 24 ? 1 : age < 30 ? 3 : age < 40 ? 5 : 7;
            var count = rnd.NextDouble() < 0.30 ? 0 : rnd.NextInt(1, max + 1);

            var list = new List<Experience>(count);
            if (count == 0) return list;

            var gradYear = quals.Where(q => q.GraduationYear.HasValue)
                .Select(q => q.GraduationYear!.Value)
                .DefaultIfEmpty(DateTime.UtcNow.Year - 1)
                .Max();

            var startYearMin = Math.Min(DateTime.UtcNow.Year - 1, gradYear + 1);
            var cursor = new DateOnly(startYearMin, rnd.NextInt(1, 13), rnd.NextInt(1, 28));

            for (var i = 0; i < count; i++)
            {
                var months = rnd.NextInt(6, 36);
                var end = cursor.AddMonths(months);

                var now = DateOnly.FromDateTime(DateTime.UtcNow);
                if (end > now) end = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-rnd.NextInt(1, 6)));

                var cert = await pools.GetCertificateAsync(options.CertificateAttachmentMode, createdById, "experience-certificate.pdf", "application/pdf", rnd, ct);

                list.Add(new Experience
                {
                    EmployerName = Employers[rnd.NextInt(0, Employers.Length)],
                    JobTitle = JobTitles[rnd.NextInt(0, JobTitles.Length)],
                    CountryId = lookups.CountryIds[rnd.NextInt(0, lookups.CountryIds.Count)],
                    StartDate = cursor,
                    EndDate = rnd.NextDouble() < 0.20 ? null : end,
                    Description = rnd.NextDouble() < 0.35 ? "Handled daily operational tasks and reporting." : null,
                    SpecializationRelation = (SpecializationRelationLevel)rnd.NextInt(1, 4),
                    CertificateId = cert?.Id ?? Guid.Empty,
                    CreatedById = createdById,
                    CreatedDate = DateTimeOffset.UtcNow
                });

                cursor = end.AddMonths(rnd.NextInt(0, 6));
                if (cursor > now) break;
            }

            return list;
        }
    }

    private static class TrainingFactory
    {
        private static readonly string[] Titles = ["Project Management", "Excel Advanced", "Communication Skills", "Customer Service", "Data Analysis"];
        private static readonly string[] Providers = ["Coursera", "Udemy", "Local Institute", "Company Training"];

        public static async Task<List<TrainingCourse>> MakeCoursesAsync(
            SmartRandom rnd,
            LookupCache lookups,
            Guid createdById,
            ResourcePools pools,
            SeedOptions options,
            CancellationToken ct)
        {
            var count = rnd.NextDouble() < 0.55 ? 0 : rnd.NextInt(1, 4);
            var list = new List<TrainingCourse>(count);

            for (var i = 0; i < count; i++)
            {
                var start = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-rnd.NextInt(1, 72)));
                var end = start.AddDays(rnd.NextInt(3, 30));
                var cert = await pools.GetCertificateAsync(options.CertificateAttachmentMode, createdById, "training-certificate.pdf", "application/pdf", rnd, ct);

                list.Add(new TrainingCourse
                {
                    Title = Titles[rnd.NextInt(0, Titles.Length)],
                    Provider = Providers[rnd.NextInt(0, Providers.Length)],
                    CountryId = lookups.CountryIds[rnd.NextInt(0, lookups.CountryIds.Count)],
                    StartDate = start,
                    EndDate = rnd.NextDouble() < 0.15 ? null : end,
                    Description = null,
                    SpecializationRelation = (SpecializationRelationLevel?)rnd.NextInt(1, 4),
                    CertificateId = cert?.Id ?? Guid.Empty,
                    CreatedById = createdById,
                    CreatedDate = DateTimeOffset.UtcNow
                });
            }

            return list;
        }
    }

    private static class AchievementFactory
    {
        private static readonly string[] Titles = ["Employee of the Month", "Hackathon Winner", "Best Volunteer", "Attendance Award"];
        private static readonly string[] Authorities = ["Employer", "University", "Community Org", "Training Center"];

        public static async Task<List<Achievement>> MakeAchievementsAsync(
            SmartRandom rnd,
            LookupCache lookups,
            Guid createdById,
            ResourcePools pools,
            SeedOptions options,
            CancellationToken ct)
        {
            var count = rnd.NextDouble() < 0.70 ? 0 : rnd.NextInt(1, 3);
            var list = new List<Achievement>(count);

            for (var i = 0; i < count; i++)
            {
                var issue = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-rnd.NextInt(2, 120)));
                var attach = await pools.GetCertificateAsync(options.CertificateAttachmentMode, createdById, "achievement-attachment.pdf", "application/pdf", rnd, ct);

                list.Add(new Achievement
                {
                    AchievementTypeId = rnd.NextDouble() < 0.60 ? AchievementTypeIds.Certificate : AchievementTypeIds.Award,
                    Title = Titles[rnd.NextInt(0, Titles.Length)],
                    IssuingAuthority = Authorities[rnd.NextInt(0, Authorities.Length)],
                    CountryId = lookups.CountryIds[rnd.NextInt(0, lookups.CountryIds.Count)],
                    IssueDate = issue,
                    Description = null,
                    RelatedToSpecialization = rnd.NextDouble() < 0.50,
                    AttachmentId = attach?.Id ?? Guid.Empty,
                    CreatedById = createdById,
                    CreatedDate = DateTimeOffset.UtcNow
                });
            }

            return list;
        }
    }

    private static class LanguageFactory
    {
        public static List<ProfileLanguage> MakeLanguages(SmartRandom rnd)
        {
            var p = rnd.NextDouble();
            var list = new List<ProfileLanguage>();

            if (p < 0.90)
            {
                list.Add(MakeArabic(rnd));
                list.Add(MakeEnglish(rnd));
            }
            else if (p < 0.97)
            {
                list.Add(MakeArabic(rnd));
            }
            else
            {
                list.Add(MakeEnglish(rnd));
            }

            return list.GroupBy(x => x.LanguageId).Select(g => g.First()).ToList();
        }

        private static ProfileLanguage MakeArabic(SmartRandom rnd) => new()
        {
            LanguageId = LanguageIds.Arabic,
            SpeakingLevelId = LanguageLevelIds.Native,
            ReadingLevelId = LanguageLevelIds.Native,
            WritingLevelId = rnd.NextDouble() < 0.65 ? LanguageLevelIds.Advanced : LanguageLevelIds.Expert
        };

        private static ProfileLanguage MakeEnglish(SmartRandom rnd)
        {
            var p = rnd.NextDouble();
            var level =
                p < 0.25 ? LanguageLevelIds.Basic :
                p < 0.65 ? LanguageLevelIds.Intermediate :
                p < 0.92 ? LanguageLevelIds.Advanced :
                LanguageLevelIds.Expert;

            return new ProfileLanguage
            {
                LanguageId = LanguageIds.English,
                SpeakingLevelId = level,
                ReadingLevelId = level,
                WritingLevelId = p < 0.55 ? LanguageLevelIds.Basic : level
            };
        }
    }

    private static class SkillFactory
    {
        public static List<ProfileSkill> MakeSkills(SmartRandom rnd, IReadOnlyList<Guid> skillTypeIds)
        {
            if (skillTypeIds == null || skillTypeIds.Count == 0) return [];

            var count = rnd.NextDouble() < 0.25 ? 0 : rnd.NextInt(3, 11);

            var picked = new HashSet<Guid>();
            var list = new List<ProfileSkill>(count);

            while (list.Count < count && picked.Count < skillTypeIds.Count)
            {
                var id = skillTypeIds[rnd.NextInt(0, skillTypeIds.Count)];
                if (!picked.Add(id)) continue;

                list.Add(new ProfileSkill
                {
                    SkillId = id,
                    LevelId = PickSkillLevel(rnd)
                });
            }

            return list;
        }

        private static Guid PickSkillLevel(SmartRandom rnd)
        {
            var p = rnd.NextDouble();
            if (p < 0.20) return SkillLevelIds.Basic;
            if (p < 0.55) return SkillLevelIds.Intermediate;
            if (p < 0.85) return SkillLevelIds.Advanced;
            return SkillLevelIds.Expert;
        }
    }

    private static class NameFactory
    {
        private static readonly (string En, string Ar)[] First =
        [
            ("Ahmed", "أحمد"), ("Mohammed", "محمد"), ("Hamad", "حمد"), ("Ali", "علي"), ("Omar", "عمر"),
            ("Sara", "سارة"), ("Fatima", "فاطمة"), ("Aisha", "عائشة"), ("Mariam", "مريم"), ("Noor", "نور"),
            ("Khalid", "خالد"), ("Yousef", "يوسف"), ("Abdullah", "عبدالله"), ("Salem", "سالم"), ("Hanan", "حنان")
        ];

        private static readonly (string En, string Ar)[] Last =
        [
            ("Al-Kuwari", "الكواري"), ("Al-Mansoori", "المنصوري"), ("Al-Ansari", "الأنصاري"),
            ("Al-Nuaimi", "النعيمي"), ("Haddad", "حداد"), ("Saleh", "صالح"), ("Hassan", "حسن"),
            ("Al-Hashmi", "الهاشمي"), ("Al-Sayed", "السيد"), ("Al-Ali", "العلي")
        ];

        public static (string FirstEn, string FirstAr, string LastEn, string LastAr) Generate(SmartRandom rnd)
        {
            var f = First[rnd.NextInt(0, First.Length)];
            var l = Last[rnd.NextInt(0, Last.Length)];
            return (f.En, f.Ar, l.En, l.Ar);
        }
    }

    private static class EmailFactory
    {
        public static string MakeEmail((string FirstEn, string FirstAr, string LastEn, string LastAr) name, int seq)
        {
            var local = $"{ToSlug(name.FirstEn)}.{ToSlug(name.LastEn)}.{seq:D6}";
            return $"{local}@example.local";
        }

        private static string ToSlug(string s)
        {
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s.ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(ch)) sb.Append(ch);
                else if (ch is '-' or '_' or '.') sb.Append(ch);
            }
            return sb.ToString().Trim('.');
        }
    }

    private static class HashFactory
    {
        public static string Sha256Hex(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }
    }
}
