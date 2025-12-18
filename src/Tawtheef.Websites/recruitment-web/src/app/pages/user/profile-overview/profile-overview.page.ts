import {Component, computed, inject, signal} from "@angular/core";
import {CommonModule} from '@angular/common';
import {ProfileService} from '../wizard-profile/services/profile.service';
import {ProfileLookupsService} from '../wizard-profile/services/profile-lookups.service';
import {Router} from '@angular/router';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {
  AchievementDto, AdditionalAttachmentDto,
  ExperienceDto, FileRefDto, LanguageDto,
  ProfileStatusDto,
  QualificationDto, SkillDto,
  TrainingCourseDto
} from '../../../core/models/auth/auth-response.model';
import {forkJoin} from 'rxjs';
import {finalize} from 'rxjs/operators';
import {FileUtilsService} from '../../../core/utils/file-utils';
import {mapIdToDropdown} from '../wizard-profile/services/profile.mapper';
import {PrimeTemplate} from 'primeng/api';
import {ButtonDirective} from 'primeng/button';
import {Tag} from 'primeng/tag';
import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';
import {Skeleton} from 'primeng/skeleton';
import {Accordion, AccordionHeader, AccordionPanel} from 'primeng/accordion';
import {TableModule} from 'primeng/table';
import {Chip} from 'primeng/chip';

@Component({
  standalone: true,
  selector: 'app-profile-view-page',
  imports: [
    CommonModule,
    TranslatePipe,
    PrimeTemplate,
    ButtonDirective,
    Tag,
    I18nNamespaceDirective,
    Skeleton,
    Accordion,
    TableModule,
    Chip,
    AccordionPanel,
    AccordionHeader,
  ],
  templateUrl: './profile-overview.page.html',
  styleUrls: ['./profile-overview.page.scss']
})
export class ProfileOverviewPage {
  private fileUtils = inject(FileUtilsService);
  private profileService = inject(ProfileService);
  private lookups = inject(ProfileLookupsService);
  private router = inject(Router);
  private i18n = inject(TranslateService);

  loading = signal<boolean>(true);
  error = signal<string | null>(null);

  profile = signal<ProfileStatusDto | null>(null);

  vm = computed(() => {
    const p = this.profile();
    if (!p) return null;

    const missing = p.missing ?? [];
    const missingCount = missing.length;

    return {
      header: {
        avatar: p.avatar ?? null,
        fullName: (p.fullNameEn || p.fullNameAr || '').trim(),
        email: p.email ?? '',
        phone: p.phone ?? '',
        emailVerified: p.emailVerified,
        phoneVerified: p.phoneVerified,
        isComplete: p.isComplete,
        isDraft: p.isDraft,
        missingCount,
        missing
      },

      prereq: {
        candidateType: mapIdToDropdown(this.lookups, 'candidateType', p.candidateTypeId),
        targetEntity: mapIdToDropdown(this.lookups,'targetEntity', p.targetEntityId),
        office: mapIdToDropdown(this.lookups,'office', p.officeId),

        resume: p.resumeAttachment ?? null,
        nationalCard: p.nationalCard ?? null,
        birthdayCertificate: p.birthdayCertificate ?? null,
        marriageCertificate: p.marriageCertificate ?? null,
        sponsorCard: p.sponsorCard ?? null,
        residenceAddressCertificate: p.residenceAddressCertificate ?? null
      },

      personal: {
        nationalNumber: p.nationalNumber ?? null,
        qidExpiry: p.qidExpiry ?? null,
        birthDate: p.birthDate ?? null,
        nationality: mapIdToDropdown(this.lookups,'countries', p.nationalityId),
        gender: mapIdToDropdown(this.lookups,'gender', p.genderId),
        religion: mapIdToDropdown(this.lookups,'religion', p.religionId),
        maritalStatus: mapIdToDropdown(this.lookups,'marital', p.maritalStatusId),
        childrenCount: p.childrenCount ?? 0,
        hasDisability: p.hasDisability,
        disabilityDetails: p.disabilityDetails ?? null,

        sponsorType: mapIdToDropdown(this.lookups,'sponsorType', p.sponsorTypeId),
        sponsorEmployerName: p.sponsorEmployerName ?? null,
        sponsorEmployerNumber: p.sponsorEmployerNumber ?? null,
        sponsorQidExpiry: p.sponsorQidExpiry ?? null
      },

      contact: {
        residenceCountry: mapIdToDropdown(this.lookups,'countries', p.residenceCountryId),
        interviewLocation: mapIdToDropdown(this.lookups,'countries', p.interviewLocationId),
        address: p.address ?? null,

        naZone: p.naZone ?? null,
        naStreet: p.naStreet ?? null,
        naBuilding: p.naBuilding ?? null,
        naUnit: p.naUnit ?? null
      },

      qualifications: (p.qualifications ?? []) as QualificationDto[],
      experiences: (p.experiences ?? []) as ExperienceDto[],
      trainingCourses: (p.trainingCourses ?? []) as TrainingCourseDto[],
      achievements: (p.achievements ?? []) as AchievementDto[],
      skills: (p.skills ?? []) as SkillDto[],
      languages: (p.languages ?? []) as LanguageDto[],
      attachments: (p.additionalAttachments ?? []) as AdditionalAttachmentDto[]
    };
  });

  // Section “health” (complete/missing) based on missing keys convention.
  // If your missing[] uses different keys, update the prefix map.
  private missingPrefix = {
    prerequisites: ['prereq.', 'prerequisites.'],
    personal: ['personal.'],
    contact: ['contact.'],
    qualifications: ['qualifications.'],
    experiences: ['experiences.', 'experience.'],
    training: ['trainingCourses.', 'training.'],
    achievements: ['achievements.', 'certificatesAndAwards.', 'certificates.'],
    skills: ['skills.'],
    languages: ['languages.'],
    attachments: ['attachments.']
  } as const;

  sectionMissingCount(section: keyof typeof this.missingPrefix): number {
    const p = this.profile();
    if (!p?.missing?.length) return 0;
    const prefixes = this.missingPrefix[section];
    return p.missing.filter(k => prefixes.some(pref => k.startsWith(pref))).length;
  }

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      lookups: this.lookups.loadAll(),
      profile: this.profileService.getProfileStatus()
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (res) => this.profile.set(res.profile),
        error: (err) => {
          console.error(err);
          this.error.set(this.i18n.instant('common.loadFailed'));
        }
      });
  }

  // Navigation to edit pages (you will implement these routes)
  editSection(section: string) {
    this.router.navigate(['/profile/edit', section]);
  }

  // File actions
  openFile(file: FileRefDto | null) {
    if (!file?.url) return;
    this.fileUtils.previewUrl(file?.url);
  }
}
