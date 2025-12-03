import { computed, Signal } from '@angular/core';
import { VALIDATION_KEYS } from '../constants/profile-validation-keys';
import {
  FieldError, StepValidationResult,
  StepValidityResult,
} from '../models/profile-validation.model';
import { isFilledScalar } from '../utils/profile-validation.utils';
import {ProfileState} from '../models/profile-state.model';
import {CandidateType} from '../../../../core/enums/lookups.enum';

/** Small helper to push a "required" error using VALIDATION_KEYS */
function addRequiredError(
  errors: FieldError[],
  group: keyof typeof VALIDATION_KEYS,
  field: string
) {
  const keyGroup = VALIDATION_KEYS[group] as any;

  if (!keyGroup || !keyGroup[field]) {
    // fail-safe: don't crash if key is missing
    console.warn(`Missing validation key mapping for ${group}.${field}`);
    return;
  }

  errors.push({
    field,
    i18nKey: keyGroup[field],
  });
}

/* ========== BASIC STEP ========== */
function validateBasicStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];

  if (!isFilledScalar(s.candidateType)) {
    addRequiredError(errors, 'basic', 'candidateType');
  }

  if (!isFilledScalar(s.targetEntity)) {
    addRequiredError(errors, 'basic', 'targetEntity');
  }

  if (!isFilledScalar(s.cvName)) {
    addRequiredError(errors, 'basic', 'cvName');
  }

  if (!isFilledScalar(s.idName)) {
    addRequiredError(errors, 'basic', 'idName');
  }

  const backendType = s.candidateType?.backendName as CandidateType | undefined;
  const isWifeOfQatari = backendType === CandidateType.WifeOfQatari;
  const isSonOfQatariMother = backendType === CandidateType.SonOfQatariMother;

  if (isWifeOfQatari && !isFilledScalar(s.marriageCertificateName)) {
    addRequiredError(errors, 'basic', 'marriageCertificateName');
  }

  if (isSonOfQatariMother && !isFilledScalar(s.birthCertificateName)) {
    addRequiredError(errors, 'basic', 'birthCertificateName');
  }

  return {
    valid: errors.length === 0,
    errors,
  };
}

/* ========== PERSONAL STEP ========== */
function validatePersonalStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];

  if (!isFilledScalar(s.fullNameAr)) {
    addRequiredError(errors, 'personal', 'fullNameAr');
  }

  if (!isFilledScalar(s.fullNameEn)) {
    addRequiredError(errors, 'personal', 'fullNameEn');
  }

  if (!isFilledScalar(s.qid)) {
    addRequiredError(errors, 'personal', 'qid');
  }

  if (!isFilledScalar(s.dob)) {
    addRequiredError(errors, 'personal', 'dob');
  }

  if (!isFilledScalar(s.nationality)) {
    addRequiredError(errors, 'personal', 'nationality');
  }

  if (!isFilledScalar(s.gender)) {
    addRequiredError(errors, 'personal', 'gender');
  }

  if (!isFilledScalar(s.religion)) {
    addRequiredError(errors, 'personal', 'religion');
  }

  if (!isFilledScalar(s.marital)) {
    addRequiredError(errors, 'personal', 'marital');
  }

  if (s.hasDisability && !isFilledScalar(s.disabilityDetails)) {
    addRequiredError(errors, 'personal', 'disabilityDetails');
  }

  // sponsor block
  if (!isFilledScalar(s.sponsorType)) {
    addRequiredError(errors, 'personal', 'sponsorType');
  }

  if (!isFilledScalar(s.sponsorEmployerName)) {
    addRequiredError(errors, 'personal', 'sponsorEmployerName');
  }

  if (!isFilledScalar(s.sponsorEmployerNumber)) {
    addRequiredError(errors, 'personal', 'sponsorEmployerNumber');
  }

  if (!isFilledScalar(s.sponsorCardName)) {
    addRequiredError(errors, 'personal', 'sponsorCardName');
  }

  return {
    valid: errors.length === 0,
    errors,
  };
}

/* ========== CONTACT STEP ========== */
function validateContactStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];

  if (!isFilledScalar(s.country)) {
    addRequiredError(errors, 'contact', 'country');
  }

  // Phone object (PhoneNumber | null): نتحقق من null فقط
  if (!s.phone) {
    addRequiredError(errors, 'contact', 'phone');
  }

  // توثيق الهاتف: يجب أن يكون true
  if (!s.phoneVerified) {
    addRequiredError(errors, 'contact', 'phoneVerified');
  }

  if (!isFilledScalar(s.email)) {
    addRequiredError(errors, 'contact', 'email');
  }

  // توثيق البريد: يجب أن يكون true
  if (!s.emailVerified) {
    addRequiredError(errors, 'contact', 'emailVerified');
  }

  // هنا حسب تصميمك: إن كنت تريد العنوان إلزامي:
  if(!isResidentQatar()){
  if (!isFilledScalar(s.address))
    addRequiredError(errors, 'contact', 'address');
  }else{
    if(!isFilledScalar(s.naZone))
      addRequiredError(errors, 'contact', 'naZone');
    if(!isFilledScalar(s.naStreet))
      addRequiredError(errors, 'contact', 'naStreet');
    if(!isFilledScalar(s.naBuilding))
      addRequiredError(errors, 'contact', 'naBuilding');

    if(!isFilledScalar(s.naFileName))
      addRequiredError(errors, 'contact', 'naFileName');
  }

  return {
    valid: errors.length === 0,
    errors,
  };

  function isResidentQatar(): boolean {
    const type = s.candidateType?.backendName as CandidateType | undefined;
    if (!type) return false;

    return [
      CandidateType.ResidentQatar,
      CandidateType.Qatari,
      CandidateType.SonOfQatariMother,
      CandidateType.WifeOfQatari
    ].includes(type);
  }
}

/* ========== DEGREES / EXPERIENCE / SKILLS / ATTACHMENTS ========== */

function validateDegreesStep(s: ProfileState): StepValidationResult {
  const hasDegrees = Array.isArray(s.degrees) && s.degrees.length > 0;

  if (!hasDegrees) {
    return {
      valid: false,
      errors: [
        {
          field: 'degrees',
          i18nKey: 'wizard.profile.degrees.atLeastOne.required',
        },
      ],
    };
  }

  return { valid: true, errors: [] };
}

function validateExperienceStep(s: ProfileState): StepValidationResult {
  const hasExp = Array.isArray(s.experiences) && s.experiences.length > 0;

  if (!hasExp) {
    return {
      valid: false,
      errors: [
        {
          field: 'experiences',
          i18nKey: 'wizard.profile.experience.atLeastOne.required',
        },
      ],
    };
  }

  return { valid: true, errors: [] };
}

function validateSkillsStep(s: ProfileState): StepValidationResult {
  const hasSkills = Array.isArray(s.skills) && s.skills.length > 0;

  if (!hasSkills) {
    return {
      valid: false,
      errors: [
        {
          field: 'skills',
          i18nKey: 'wizard.profile.skills.required',
        },
      ],
    };
  }

  return { valid: true, errors: [] };
}

function validateLanguagesStep(s: ProfileState): StepValidationResult {
  const hasLanguages = Array.isArray(s.languages) && s.languages.length > 0;

  if (!hasLanguages) {
    return {
      valid: false,
      errors: [
        {
          field: 'languages',
          i18nKey: 'wizard.profile.languages.required',
        },
      ],
    };
  }

  return { valid: true, errors: [] };
}

// حالياً لا توجد قواعد إلزامية للمرفقات
function validateAttachmentsStep(s: ProfileState): StepValidationResult {
  // مثال: لو حابة نفرض مرفق واحد على الأقل:
  // const hasAttachments = Array.isArray(s.attachments) && s.attachments.length > 0;
  // if (!hasAttachments) { ... }
  return { valid: true, errors: [] };
}

/* ========== MAIN SIGNAL ========== */

export function createStepValiditySignal(
  stateSignal: Signal<ProfileState>
) {
  return computed<StepValidityResult>(() => {
    const s = stateSignal();

    return {
      basic:       validateBasicStep(s),
      personal:    validatePersonalStep(s),
      contact:     validateContactStep(s),
      degrees:     validateDegreesStep(s),
      experience:  validateExperienceStep(s),
      skills:      validateSkillsStep(s),
      languages:   validateLanguagesStep(s),
      attachments: validateAttachmentsStep(s),
    };
  });
}
