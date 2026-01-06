import { computed, Signal } from '@angular/core';
import {FieldError, StepValidityResult} from '../../wizard-profile/models/profile-validation.model';
import {ProfileState} from '../../wizard-profile/models/profile-state.model';
import {
  candidateTypeFromState,
  candidateTypeIsResident, candidateTypeNeedsBirthCertificate, candidateTypeNeedsMarriageCertificate,
  candidateTypeNeedsSponsor
} from '../../wizard-profile/state/profile-step-validity.signal';

export type SectionId =
  | 'prerequisites'
  | 'personal'
  | 'contact'
  | 'degrees'
  | 'experience'
  | 'achievements'
  | 'skills'
  | 'languages'
  | 'attachments';

export interface MissingItemVm {
  field: string;
  i18nKey: string;
}

export interface SectionHighlightVm {
  labelKey: string;
  value: string;
}

export interface RequiredFileVm {
  labelKey: string;
  fileName?: string | null;
}

export interface SectionCardVm {
  id: SectionId;
  titleKey: string;
  icon: string;
  missingCount: number;
  missing: MissingItemVm[];
  highlights: SectionHighlightVm[];
  requiredFiles?: RequiredFileVm[];
}

function toMissingItems(errors: FieldError[] | undefined): MissingItemVm[] {
  return (errors ?? []).map(e => ({ field: e.field, i18nKey: e.i18nKey }));
}

function missingCountOf(step: { errors: FieldError[] } | undefined): number {
  return step?.errors?.length ?? 0;
}

function safeText(v: unknown): string {
  if (v === null || v === undefined) return '—';
  if (typeof v === 'string') return v.trim() ? v : '—';
  return String(v);
}

export function createProfileOverviewCardsSignal(
  state$: Signal<ProfileState>,
  validity$: Signal<StepValidityResult>
) {
  return computed<SectionCardVm[]>(() => {
    const s = state$();
    const v = validity$();

    const type = candidateTypeFromState(s);
    const needsSponsor = candidateTypeNeedsSponsor(type);
    const needsBirth = candidateTypeNeedsBirthCertificate(type);
    const needsMarriage = candidateTypeNeedsMarriageCertificate(type);
    const isResident = candidateTypeIsResident(type, s.provider);


    const cards: SectionCardVm[] = [
      {
        id: 'prerequisites',
        titleKey: 'profileOverview.sections.prerequisites',
        icon: 'pi pi-file',
        missingCount: missingCountOf(v.basic),
        missing: toMissingItems(v.basic.errors),
        highlights: [
          { labelKey: 'profileOverview.fields.candidateType', value: safeText(s.candidateType?.name) },
          { labelKey: 'profileOverview.fields.targetEntity', value: safeText(s.targetEntity?.name) },
          ...(isResident ? [{ labelKey: 'profileOverview.fields.qidExpiry', value: safeText(s.qidExpiry) }] : []),
        ],
        requiredFiles: [
          { labelKey: 'profileOverview.files.resume', fileName: s.cvName ?? null },
          { labelKey: 'profileOverview.files.nationalCard', fileName: s.idName ?? null },
          ...(needsMarriage ? [{ labelKey: 'profileOverview.files.marriageCertificate', fileName: s.marriageCertificateName ?? null }] : []),
          ...(needsBirth ? [{ labelKey: 'profileOverview.files.birthdayCertificate', fileName: s.birthCertificateName ?? null }] : []),
        ],
      },

      {
        id: 'personal',
        titleKey: 'profileOverview.sections.personal',
        icon: 'pi pi-id-card',
        missingCount: missingCountOf(v.personal),
        missing: toMissingItems(v.personal.errors),
        highlights: [
          { labelKey: 'profileOverview.fields.fullNameAr', value: safeText(s.fullNameAr) },
          { labelKey: 'profileOverview.fields.fullNameEn', value: safeText(s.fullNameEn) },
          { labelKey: 'profileOverview.fields.nationalNumber', value: safeText(s.qid) },
          { labelKey: 'profileOverview.fields.birthDate', value: safeText(s.dob) },
        ],
        requiredFiles: [
          ...(needsSponsor ? [{ labelKey: 'profileOverview.files.sponsorCard', fileName: s.sponsorCardName ?? null }] : []),
        ],
      },

      {
        id: 'contact',
        titleKey: 'profileOverview.sections.contact',
        icon: 'pi pi-map-marker',
        missingCount: missingCountOf(v.contact),
        missing: toMissingItems(v.contact.errors),
        highlights: [
          { labelKey: 'profileOverview.fields.residenceCountry', value: safeText(s.country?.name) },
          { labelKey: 'profileOverview.fields.interviewLocation', value: safeText(s.interviewPlace?.name) },
          { labelKey: 'profileOverview.fields.email', value: safeText(s.email) },
          { labelKey: 'profileOverview.fields.phone', value: safeText(s.phone) },
          ...(isResident
            ? [
              { labelKey: 'profileOverview.fields.naZone', value: safeText(s.naZone) },
              { labelKey: 'profileOverview.fields.naStreet', value: safeText(s.naStreet) },
            ]
            : [
              { labelKey: 'profileOverview.fields.address', value: safeText(s.address) },
            ]),
        ],
        requiredFiles: isResident
          ? [{ labelKey: 'profileOverview.files.residenceAddressCertificate', fileName: s.naFileName ?? null }]
          : [],
      },

      {
        id: 'degrees',
        titleKey: 'profileOverview.sections.qualifications',
        icon: 'pi pi-graduation-cap',
        missingCount: missingCountOf(v.degrees),
        missing: toMissingItems(v.degrees.errors),
        highlights: [
          { labelKey: 'profileOverview.summary.count', value: safeText(s.degrees?.length ?? 0) },
        ],
      },

      {
        id: 'experience',
        titleKey: 'profileOverview.sections.experiences',
        icon: 'pi pi-briefcase',
        missingCount: missingCountOf(v.experience),
        missing: toMissingItems(v.experience.errors),
        highlights: [
          { labelKey: 'profileOverview.summary.count', value: safeText(s.experiences?.length ?? 0) },
          { labelKey: 'profileOverview.summary.trainingCount', value: safeText(s.courses?.length ?? 0) },
        ],
      },

      {
        id: 'achievements',
        titleKey: 'profileOverview.sections.certificatesAndAwards',
        icon: 'pi pi-star',
        missingCount: missingCountOf(v.achievements),
        missing: toMissingItems(v.achievements.errors),
        highlights: [
          { labelKey: 'profileOverview.summary.count', value: safeText(s.achievements?.length ?? 0) },
        ],
      },

      {
        id: 'skills',
        titleKey: 'profileOverview.sections.skills',
        icon: 'pi pi-bolt',
        missingCount: missingCountOf(v.skills),
        missing: toMissingItems(v.skills.errors),
        highlights: [
          { labelKey: 'profileOverview.summary.count', value: safeText(s.skills?.length ?? 0) },
        ],
      },

      {
        id: 'languages',
        titleKey: 'profileOverview.sections.languages',
        icon: 'pi pi-language',
        missingCount: missingCountOf(v.languages),
        missing: toMissingItems(v.languages.errors),
        highlights: [
          { labelKey: 'profileOverview.summary.count', value: safeText(s.languages?.length ?? 0) },
        ],
      },

      {
        id: 'attachments',
        titleKey: 'profileOverview.sections.attachments',
        icon: 'pi pi-paperclip',
        missingCount: missingCountOf(v.attachments),
        missing: toMissingItems(v.attachments.errors),
        highlights: [
          { labelKey: 'profileOverview.summary.count', value: safeText(s.attachments?.length ?? 0) },
        ],
      },
    ];

    return cards;
  });
}
