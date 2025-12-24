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
        titleKey: 'profileView.sections.prerequisites',
        icon: 'pi pi-file',
        missingCount: missingCountOf(v.basic),
        missing: toMissingItems(v.basic.errors),
        highlights: [
          { labelKey: 'profileView.fields.candidateType', value: safeText(s.candidateType?.name) },
          { labelKey: 'profileView.fields.targetEntity', value: safeText(s.targetEntity?.name) },
          ...(isResident ? [] : [{ labelKey: 'profileView.fields.office', value: safeText(s.office?.name) }]),
          ...(isResident ? [{ labelKey: 'profileView.fields.qidExpiry', value: safeText(s.qidExpiry) }] : []),
        ],
        requiredFiles: [
          { labelKey: 'profileView.files.resume', fileName: s.cvName ?? null },
          { labelKey: 'profileView.files.nationalCard', fileName: s.idName ?? null },
          ...(needsMarriage ? [{ labelKey: 'profileView.files.marriageCertificate', fileName: s.marriageCertificateName ?? null }] : []),
          ...(needsBirth ? [{ labelKey: 'profileView.files.birthdayCertificate', fileName: s.birthCertificateName ?? null }] : []),
        ],
      },

      {
        id: 'personal',
        titleKey: 'profileView.sections.personal',
        icon: 'pi pi-id-card',
        missingCount: missingCountOf(v.personal),
        missing: toMissingItems(v.personal.errors),
        highlights: [
          { labelKey: 'profileView.fields.fullNameAr', value: safeText(s.fullNameAr) },
          { labelKey: 'profileView.fields.fullNameEn', value: safeText(s.fullNameEn) },
          { labelKey: 'profileView.fields.nationalNumber', value: safeText(s.qid) },
          { labelKey: 'profileView.fields.birthDate', value: safeText(s.dob) },
        ],
        requiredFiles: [
          ...(needsSponsor ? [{ labelKey: 'profileView.files.sponsorCard', fileName: s.sponsorCardName ?? null }] : []),
        ],
      },

      {
        id: 'contact',
        titleKey: 'profileView.sections.contact',
        icon: 'pi pi-map-marker',
        missingCount: missingCountOf(v.contact),
        missing: toMissingItems(v.contact.errors),
        highlights: [
          { labelKey: 'profileView.fields.residenceCountry', value: safeText(s.country?.name) },
          { labelKey: 'profileView.fields.interviewLocation', value: safeText(s.interviewPlace?.name) },
          { labelKey: 'profileView.fields.email', value: safeText(s.email) },
          { labelKey: 'profileView.fields.phone', value: safeText(s.phone) },
          ...(isResident
            ? [
              { labelKey: 'profileView.fields.naZone', value: safeText(s.naZone) },
              { labelKey: 'profileView.fields.naStreet', value: safeText(s.naStreet) },
            ]
            : [
              { labelKey: 'profileView.fields.address', value: safeText(s.address) },
            ]),
        ],
        requiredFiles: isResident
          ? [{ labelKey: 'profileView.files.residenceAddressCertificate', fileName: s.naFileName ?? null }]
          : [],
      },

      {
        id: 'degrees',
        titleKey: 'profileView.sections.qualifications',
        icon: 'pi pi-graduation-cap',
        missingCount: missingCountOf(v.degrees),
        missing: toMissingItems(v.degrees.errors),
        highlights: [
          { labelKey: 'profileView.summary.count', value: safeText(s.degrees?.length ?? 0) },
        ],
      },

      {
        id: 'experience',
        titleKey: 'profileView.sections.experiences',
        icon: 'pi pi-briefcase',
        missingCount: missingCountOf(v.experience),
        missing: toMissingItems(v.experience.errors),
        highlights: [
          { labelKey: 'profileView.summary.count', value: safeText(s.experiences?.length ?? 0) },
          { labelKey: 'profileView.summary.trainingCount', value: safeText(s.courses?.length ?? 0) },
        ],
      },

      {
        id: 'achievements',
        titleKey: 'profileView.sections.certificatesAndAwards',
        icon: 'pi pi-star',
        missingCount: missingCountOf(v.achievements),
        missing: toMissingItems(v.achievements.errors),
        highlights: [
          { labelKey: 'profileView.summary.count', value: safeText(s.achievements?.length ?? 0) },
        ],
      },

      {
        id: 'skills',
        titleKey: 'profileView.sections.skills',
        icon: 'pi pi-bolt',
        missingCount: missingCountOf(v.skills),
        missing: toMissingItems(v.skills.errors),
        highlights: [
          { labelKey: 'profileView.summary.count', value: safeText(s.skills?.length ?? 0) },
        ],
      },

      {
        id: 'languages',
        titleKey: 'profileView.sections.languages',
        icon: 'pi pi-language',
        missingCount: missingCountOf(v.languages),
        missing: toMissingItems(v.languages.errors),
        highlights: [
          { labelKey: 'profileView.summary.count', value: safeText(s.languages?.length ?? 0) },
        ],
      },

      {
        id: 'attachments',
        titleKey: 'profileView.sections.attachments',
        icon: 'pi pi-paperclip',
        missingCount: missingCountOf(v.attachments),
        missing: toMissingItems(v.attachments.errors),
        highlights: [
          { labelKey: 'profileView.summary.count', value: safeText(s.attachments?.length ?? 0) },
        ],
      },
    ];

    return cards;
  });
}
