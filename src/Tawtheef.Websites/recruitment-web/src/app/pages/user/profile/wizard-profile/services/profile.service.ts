import { Injectable, inject } from '@angular/core';
import { of } from 'rxjs';
import { HttpService } from '../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { PROFILE_WRITE_MODE, ProfileWriteMode } from './profile-write-mode.token';
import { SaveProfilePrereqRequestModel } from '../models/save-profile-prereq-request.model';
import { SaveProfilePersonalRequestDto } from '../models/save-profile-personal-request.model';
import { MoiPersonalInfo } from '../models/moi-personal-info.model';
import { SaveProfileContactRequestDto } from '../models/save-user-contact-request.model';
import { ProfileStatusDto } from '../../../../../core/models/auth/auth-response.model';
import { GUID } from '../../../../../shared/types/guid.type';
import { Experience, TrainingCourse } from '../models/experience.model';
import { Achievement } from '../models/achievement.model';
import { Skill } from '../models/skill.model';
import { Attachment } from '../models/attachment.model';
import { Degree } from '../models/degree.model';
import { Language } from '../models/language.model';
type FileLike = File | null | undefined;

type SectionKey =
  | 'prereq'
  | 'personal'
  | 'contact'
  | 'education'
  | 'experience'
  | 'achievements'
  | 'skills'
  | 'languages'
  | 'attachments';

type UrlSet = { create: string; changeRequest: string; revision: string, attachment?: string };

@Injectable()
export class ProfileService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private writeMode: ProfileWriteMode =
    inject(PROFILE_WRITE_MODE, { optional: true }) ?? 'create';

  setWriteMode(mode: ProfileWriteMode) {
    this.writeMode = mode;
  }

  isChangeRequestMode(): boolean {
    return this.writeMode === 'change-request';
  }

  isRevisionMode(): boolean {
    return this.writeMode === 'review-edit';
  }

  // ========== PREREQ ==========
  savePrereq(
    dto: SaveProfilePrereqRequestModel,
    files: {
      cvFile?: FileLike;
      idFile?: FileLike;
      birthCertificateFile?: FileLike;
      marriageCertificateFile?: FileLike;
    }
  ) {
    const fd = this.fd()
      .json(dto)
      .file('cvFile', files.cvFile)
      .file('idFile', files.idFile)
      .file('birthCertificateFile', files.birthCertificateFile)
      .file('marriageCertificateFile', files.marriageCertificateFile)
      .build();

    return this.http.post(this.url('prereq'), fd);
  }
  savePereqAttachmentsSection(info: any, files: any) {
    const b = this.fd();

    if (this.isRevisionMode()) {
      if (files.birth) {
        b.scalar('Birthday.Id', info.birth?.id || info.birth?.attachmentId || info.birth?.resourceId || info.birthdayCertificate?.resourceId);
        b.scalar('Birthday.Title', info.birth?.title || 'Birth Certificate');
        b.rawAppend('BirthdayCertificate', files.birth);
      }
      if (files.marriage) {
        b.scalar('Marriage.Id', info.marriage?.id || info.marriage?.attachmentId || info.marriage?.resourceId || info.marriageCertificate?.resourceId);
        b.scalar('Marriage.Title', info.marriage?.title || 'Marriage Certificate');
        b.rawAppend('MarriageCertificate', files.marriage);
      }
      return this.http.post(this.endpoints.user.profile.revisions.prereqAttachment, b.build());
    }

    // Change Request (Approved) Mode
    b.scalar('CandidateTypeId', info.candidateTypeId || info.candidateType?.id);
    b.scalar('TargetEntityId', info.targetEntityId || info.targetEntity?.id);
    b.scalar('QIDExpiry', info.qidExpiry || info.qidExpiry);
    b.scalar('Submit', false);

    if (files.birth) b.rawAppend('BirthCertificateFile', files.birth);
    if (files.marriage) b.rawAppend('MarriageCertificateFile', files.marriage);
    if (files.resume) b.rawAppend('CvFile', files.resume);
    if (files.nationalCard) b.rawAppend('IdFile', files.nationalCard);

    return this.http.post(this.url('prereq'), b.build());
  }

  // ========== PERSONAL ==========
  savePersonalSection(dto: SaveProfilePersonalRequestDto, files?: { sponsorCard?: FileLike }) {
    const fd = this.fd()
      .json(dto)
      .file('sponsorCard', files?.sponsorCard)
      .build();

    return this.http.post(this.url('personal'), fd);
  }
  savePersonalAttachmentsSection(info: any, files: any) {
    const b = this.fd();

    if (this.isRevisionMode()) {
      if (files.resume) {
        b.scalar('Resume.Id', info.resume?.id || info.resume?.attachmentId || info.resume?.resourceId || info.resumeAttachment?.resourceId);
        b.scalar('Resume.Title', info.resume?.title || 'Resume');
        b.rawAppend('ResumeAttachment', files.resume);
      }
      if (files.nationalCard) {
        b.scalar('NationalCard.Id', info.nationalCard?.id || info.nationalCard?.attachmentId || info.nationalCard?.resourceId || info.nationalCard?.resourceId);
        b.scalar('NationalCard.Title', info.nationalCard?.title || 'National Card');
        b.rawAppend('NationalCardAttachment', files.nationalCard);
      }
      if (files.sponsorCard) {
        b.scalar('SponsorCard.Id', info.sponsorCard?.id || info.sponsorCard?.attachmentId || info.sponsorCard?.resourceId || info.sponsorCard?.resourceId);
        b.scalar('SponsorCard.Title', info.sponsorCard?.title || 'Sponsor Card');
        b.rawAppend('SponsorCardAttachment', files.sponsorCard);
      }
      return this.http.post(this.endpoints.user.profile.revisions.personalAttachment, b.build());
    }

    // Change Request (Approved) Mode
    // Resume and NationalCard MUST go to Prereq endpoint in Change Request mode
    if (files.resume || files.nationalCard) {
      return this.savePereqAttachmentsSection(info, files);
    }

    // Personal Metadata
    b.scalar('NationalityId', info.nationalityId || info.nationality?.id);
    b.scalar('GenderId', info.genderId || info.gender?.id);
    b.scalar('ReligionId', info.religionId || info.religion?.id);
    b.scalar('MaritalStatusId', info.maritalStatusId || info.maritalStatus?.id);
    b.scalar('BirthDate', info.birthDate);
    b.scalar('ChildrenCount', info.childrenCount);
    b.scalar('HasDisability', info.hasDisability);
    b.scalar('DisabilityDetails', info.disabilityDetails);

    // Sponsor Metadata
    b.scalar('SponsorTypeId', info.sponsorTypeId || info.sponsorType?.id);
    b.scalar('SponsorEmployerName', info.sponsorEmployerName || info.sponsorName);
    b.scalar('SponsorEmployerNumber', info.sponsorEmployerNumber || info.sponsorNumber);
    b.scalar('SponsorQidExpiry', info.sponsorQidExpiry);

    if (files.sponsorCard) b.rawAppend('SponsorCard', files.sponsorCard);
    b.scalar('Submit', false);

    return this.http.post(this.url('personal'), b.build());
  }

  checkProfile(qid: string, expiryDate: string) {
    return this.http.post<MoiPersonalInfo>(this.endpoints.user.profile.checkProfile, { qid, expiryDate });
  }

  // ========== CONTACT ==========
  saveRecruitmentAvailability(available: boolean) {
    const endpoint = this.isRevisionMode()
      ? this.endpoints.user.profile.revisions.availability
      : this.endpoints.user.profile.saveAvailability;

    return this.http.post(endpoint, { availableForRecruitment: available });
  }

  saveContactSection(dto: SaveProfileContactRequestDto, files?: { nationalAddressFile?: FileLike }) {
    const b = this.fd()
      .json(dto);

    if (dto.nationalAddress) {
      const na = dto.nationalAddress;
      b.scalar('nationalAddress.zone', na.zone)
        .scalar('nationalAddress.street', na.street)
        .scalar('nationalAddress.building', na.building)
        .scalar('nationalAddress.unit', na.unit)
        .scalar('nationalAddress.nationalAddressFileName', na.nationalAddressFileName);
    }

    // keep your original field name
    if (files?.nationalAddressFile) {
      b.rawAppend('nationalAddress.nationalAddress', files.nationalAddressFile);
    }

    return this.http.post(this.url('contact'), b.build());
  }
  saveContactAttachmentsSection(info: any, files: any) {
    const b = this.fd();

    if (this.isRevisionMode() && files.nationalAddress) {
      const meta = info.nationalAddress;
      b.scalar('ResidenceAddress.Id', meta?.id || meta?.attachmentId || meta?.resourceId || info.residenceAddressCertificate?.resourceId);
      b.scalar('ResidenceAddress.Title', meta?.title || 'Residence Address');
      b.rawAppend('ResidenceAddressCertificate', files.nationalAddress);
      return this.http.post(this.endpoints.user.profile.revisions.contactAttachment, b.build());
    }

    // Change Request (Approved) Mode
    b.scalar('ResidenceCountryId', info.residenceCountryId || info.residenceCountry?.id);
    b.scalar('InterviewLocationId', info.interviewLocationId || info.interviewLocation?.id);
    b.scalar('Address', info.address);
    b.scalar('Submit', false);

    if (files.nationalAddress) {
      b.scalar('NationalAddress.Zone', info.naZone || info.residenceAddress?.zoneNo || 0);
      b.scalar('NationalAddress.Street', info.naStreet || info.residenceAddress?.streetNo || 0);
      b.scalar('NationalAddress.Building', info.naBuilding || info.residenceAddress?.buildingNo || 0);
      b.scalar('NationalAddress.Unit', info.naUnit || info.residenceAddress?.unitNo || 0);
      b.rawAppend('NationalAddress.NationalAddress', files.nationalAddress);
    }

    return this.http.post(this.url('contact'), b.build());
  }

  getProfileBasics() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.basics);
  }

  getProfileStatus() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.bootstrap);
  }

  getPrereqSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.prereq);
  }

  getPersonalSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.personal);
  }

  getContactSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.contact);
  }

  getQualificationsSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.education);
  }

  getExperienceSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.experience);
  }

  getAchievementsSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.achievements);
  }

  getSkillsSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.skills);
  }

  getLanguagesSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.languages);
  }

  getAttachmentsSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.attachments);
  }

  // ========== EDUCATION (Degrees) ==========
  saveEducationSection(degrees: Degree[]) {
    const fileBucket: File[] = [];

    const payload = (degrees ?? [])
      .map(d => {
        const fileIndex = this.fileIndex(fileBucket, d.file);

        return {
          id: d.id ?? null,
          degreeId: d.degree!.id,
          gradCountryId: d.gradCountry!.id,
          universityId: d.university?.id,
          majorId: d.major?.id,
          subMajorId: d.subMajor?.id,
          gradYear: d.gradYear,
          studyTypeId: d.studySystem?.id,
          gpa: d.gpa,
          gradeId: d.grade?.id,
          certificateId: d.attachmentId ?? null,
          fileIndex,
          existingFileName: d.certificate?.resourceName ?? d.fileName ?? null,
        };
      });

    if (payload.length === 0) return of(null);

    const fd = this.fd()
      .json({ degreesJson: payload })
      .files('DegreeFiles', fileBucket)
      .build();

    return this.http.post(this.url('education'), fd);
  }

  deleteEducation(degreeId: GUID) {
    const endpoint = this.isRevisionMode()
      ? this.endpoints.user.profile.revisions.deleteEducation(degreeId)
      : this.endpoints.user.profile.deleteEducation(degreeId);

    return this.http.delete(endpoint);
  }

  // ========== EXPERIENCE + TRAINING COURSES ==========
  saveExperienceSection(experiences: Experience[], courses: TrainingCourse[]) {
    const experienceFiles: File[] = [];
    const trainingFiles: File[] = [];

    const experiencesDto = (experiences ?? [])
      .map(e => ({
        id: e.id ?? null,
        employerName: e.employerName,
        jobTitle: e.jobTitle,
        startDate: e.from,
        endDate: e.current ? null : e.to,
        countryId: e.country?.id,
        certificateId: e.attachmentId ?? null,
        certificateFileIndex: this.fileIndex(experienceFiles, e.file),
        description: e.description,
        qualificationId: e.qualificationId ?? null,
      }));

    const coursesDto = (courses ?? [])
      .map(c => ({
        id: c.id ?? null,
        title: c.title,
        provider: c.provider,
        startDate: c.from,
        endDate: c.to,
        countryId: c.country?.id,
        description: c.description,
        certificateId: c.attachmentId ?? null,
        certificateFileIndex: this.fileIndex(trainingFiles, c.file),
      }));

    if (experiencesDto.length === 0 && coursesDto.length === 0) return of(null);

    const fd = this.fd()
      .json({
        submit: false,
        experiencesJson: experiencesDto,
        trainingCoursesJson: coursesDto,
      })
      .files('ExperienceFiles', experienceFiles)
      .files('TrainingCourseFiles', trainingFiles)
      .build();

    return this.http.post(this.url('experience'), fd);
  }

  deleteExperience(experienceId: GUID) {
    const endpoint = this.isRevisionMode()
      ? this.endpoints.user.profile.revisions.deleteExperience(experienceId)
      : this.endpoints.user.profile.deleteExperience(experienceId);

    return this.http.delete(endpoint);
  }

  deleteTrainingCourse(courseId: GUID) {
    const endpoint = this.isRevisionMode()
      ? this.endpoints.user.profile.revisions.deleteTrainingCourse(courseId)
      : this.endpoints.user.profile.deleteTrainingCourse(courseId);

    return this.http.delete(endpoint);
  }

  // ========== ACHIEVEMENTS ==========
  saveAchievementsSection(achievements: Achievement[]) {
    const files: File[] = [];

    const payload = (achievements ?? [])
      .map(a => ({
        id: a.id ?? null,
        achievementTypeId: a.achievementType?.id,
        title: a.title,
        issuingAuthority: a.issuingAuthority,
        countryId: a.country?.id,
        issueDate: a.issueDate,
        description: a.description,
        attachmentId: a.attachmentId ?? null,
        certificateFileIndex: this.fileIndex(files, a.file),
        relatedToSpecialization: a.relatedToSpecialization ?? null,
      }));

    const fd = this.fd()
      .json({ submit: false, achievementsJson: payload })
      .files('AchievementFiles', files)
      .build();

    return this.http.post(this.url('achievements'), fd);
  }

  deleteAchievement(id: GUID) {
    const endpoint = this.isRevisionMode()
      ? this.endpoints.user.profile.revisions.deleteAchievement(id)
      : this.endpoints.user.profile.deleteAchievement(id);

    return this.http.delete(endpoint);
  }

  deleteAttachment(id: GUID) {
    const endpoint = this.isRevisionMode()
      ? this.endpoints.user.profile.revisions.deleteReference(id)
      : this.endpoints.user.profile.deleteReference(id);

    return this.http.delete(endpoint);
  }

  // ========== SKILLS ==========
  saveSkillsSection(skills: Skill[]) {
    const dto = {
      submit: false,
      skills: (skills ?? []).map((s: any) => ({
        skillId: s.skillId,
        levelId: s.levelId,
      })),
    };

    return this.http.post(this.url('skills'), dto);
  }

  deleteSkill(id: GUID) {
    const endpoint = this.isRevisionMode()
      ? this.endpoints.user.profile.revisions.deleteSkill(id)
      : this.endpoints.user.profile.deleteSkill(id);

    return this.http.delete(endpoint);
  }

  // ========== LANGUAGES ==========
  saveLanguagesSection(languages: Language[]) {
    const dto = {
      submit: false,
      languages: (languages ?? []).map(l => ({
        languageId: l.langId,
        speakingLevelId: l.speakingLevelId ?? l.speakingLevel?.id,
        writingLevelId: l.writingLevelId ?? l.writingLevel?.id,
        readingLevelId: l.readingLevelId ?? l.readingLevel?.id,
      })),
    };

    return this.http.post(this.url('languages'), dto);
  }

  deleteLanguage(languageId: GUID) {
    const endpoint = this.isRevisionMode()
      ? this.endpoints.user.profile.revisions.deleteLanguage(languageId)
      : this.endpoints.user.profile.deleteLanguage(languageId);

    return this.http.delete(endpoint);
  }

  // ========== ATTACHMENTS ==========
  saveSectionAttachmentsSection(attachments: Attachment[]) {
    const files: File[] = [];

    const payload = (attachments ?? [])
      .map(a => {
        const item: any = {
          id: a.id ?? null,
          title: a.title,
          fileName: a.fileName ?? a.title,
          attachmentId: a.attachmentId ?? null,
        };

        item.fileIndex = this.fileIndex(files, a.file);

        return item;
      });

    const fd = this.fd()
      .scalar('submit', false)
      .scalar('attachmentsJson', JSON.stringify(payload))
      .files('AttachmentFiles', files)
      .build();

    return this.http.post(this.url('attachments'), fd);
  }
  saveAttachmentsSection(attachments: Attachment[]) {
    const files: File[] = [];

    const payload = (attachments ?? [])
      .map(a => {
        const item: any = {
          id: a.id ?? null,
          title: a.title,
          fileName: a.fileName ?? a.title,
          attachmentId: a.attachmentId ?? null,
        };

        item.fileIndex = this.fileIndex(files, a.file);

        return item;
      });

    const fd = this.fd()
      .scalar('submit', false)
      .scalar('attachmentsJson', JSON.stringify(payload))
      .files('AttachmentFiles', files)
      .build();

    return this.http.post(this.url('attachments'), fd);
  }

  // ========== FINAL SUBMISSION ==========
  finalizeProfile() {
    return this.http.post(this.endpoints.user.profile.submit, {});
  }

  resubmitProfile() {
    return this.http.post(this.endpoints.user.profile.resubmit, {});
  }

  // ================= URL RESOLUTION =================

  private url(section: SectionKey): string {
    const map: Record<SectionKey, UrlSet> = {
      prereq: {
        create: this.endpoints.user.profile.savePrereq,
        changeRequest: this.endpoints.user.profile.requestChanges.prereq,
        revision: this.endpoints.user.profile.revisions.prereq,
      },
      personal: {
        create: this.endpoints.user.profile.savePersonal,
        changeRequest: this.endpoints.user.profile.requestChanges.personal,
        revision: this.endpoints.user.profile.revisions.personal,
      },
      contact: {
        create: this.endpoints.user.profile.saveContact,
        changeRequest: this.endpoints.user.profile.requestChanges.contact,
        revision: this.endpoints.user.profile.revisions.contact,
      },
      education: {
        create: this.endpoints.user.profile.saveEducation,
        changeRequest: this.endpoints.user.profile.requestChanges.education,
        revision: this.endpoints.user.profile.revisions.education,
      },
      experience: {
        create: this.endpoints.user.profile.saveExperience,
        changeRequest: this.endpoints.user.profile.requestChanges.experience,
        revision: this.endpoints.user.profile.revisions.experience,
      },
      achievements: {
        create: this.endpoints.user.profile.saveAchievements,
        changeRequest: this.endpoints.user.profile.requestChanges.achievements,
        revision: this.endpoints.user.profile.revisions.achievements,
      },
      skills: {
        create: this.endpoints.user.profile.saveSkills,
        changeRequest: this.endpoints.user.profile.requestChanges.skills,
        revision: this.endpoints.user.profile.revisions.skills,
      },
      languages: {
        create: this.endpoints.user.profile.saveLanguages,
        changeRequest: this.endpoints.user.profile.requestChanges.languages,
        revision: this.endpoints.user.profile.revisions.languages,
      },
      attachments: {
        create: this.endpoints.user.profile.saveReferences,
        changeRequest: this.endpoints.user.profile.requestChanges.references,
        revision: this.endpoints.user.profile.revisions.references,
      },
    };

    const urls = map[section];
    if (this.writeMode === 'change-request') return urls.changeRequest;
    if (this.writeMode === 'review-edit') return urls.revision;
    return urls.create;
  }

  // ================= FORM DATA BUILDER =================

  private fd() {
    return new FormDataBuilder();
  }

  // Push file into bucket and return its index; returns null when no file
  private fileIndex(bucket: File[], file: FileLike): number | null {
    if (!file) return null;
    bucket.push(file);
    return bucket.length - 1;
  }
}

class FormDataBuilder {
  private readonly _fd = new FormData();

  build(): FormData {
    return this._fd;
  }

  scalar(key: string, value: any): this {
    if (value === null || value === undefined || value === '') return this;
    this._fd.append(key, String(value));
    return this;
  }

  json(dto: any): this {
    Object.entries(dto ?? {}).forEach(([key, value]) => {
      if (value === null || value === undefined) return;

      const t = typeof value;
      if (t === 'string' || t === 'number' || t === 'boolean') {
        this._fd.append(key, String(value));
        return;
      }

      this._fd.append(key, JSON.stringify(value));
    });

    return this;
  }

  file(key: string, file: FileLike): this {
    if (!file) return this;
    this._fd.append(key, file);
    return this;
  }

  files(key: string, files: File[]): this {
    (files ?? []).forEach(f => this._fd.append(key, f));
    return this;
  }

  /** Use when you must preserve the exact append behavior (e.g., nested key + raw file append). */
  rawAppend(key: string, value: Blob | string): this {
    this._fd.append(key, value);
    return this;
  }
}
