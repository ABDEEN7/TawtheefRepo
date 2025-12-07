export type StepName =
  | 'basic'
  | 'personal'
  | 'contact'
  | 'degrees'
  | 'experience'
  | 'achievements'
  | 'skills'
  | 'languages'
  | 'attachments';

export interface FieldError {
  /** Form control name or logical field name */
  field: string;
  /** ngx-translate key: e.g. 'wizard.profile.basic.candidateType.required' */
  i18nKey: string;
}

export interface StepValidationResult {
  valid: boolean;
  errors: FieldError[];
}

export type StepValidityResult = Record<StepName, StepValidationResult>;
