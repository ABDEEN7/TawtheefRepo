export const EXPERIENCE_DIALOG_LIMITS = {
  descriptionMaxLength: 2000,
  maxFileSizeBytes: 1_000_000,
  maxFileSizeLabel: '1MB',
};

export const ACHIEVEMENTS_DIALOG_LIMITS = {
  ...EXPERIENCE_DIALOG_LIMITS,
  descriptionMaxLength: 500,
};
export const COURSE_DIALOG_LIMITS = {
  ...EXPERIENCE_DIALOG_LIMITS,
  descriptionMaxLength: 250,
};
export const EXPERIENCE_DIALOG_CONFIG = {
  ...EXPERIENCE_DIALOG_LIMITS,
  descriptionMaxLength: 500,
};
