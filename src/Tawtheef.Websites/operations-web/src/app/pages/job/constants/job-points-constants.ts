import { DetailItem } from '../models/detail-Item';

// ================= Applicant Category =================
export const APPLICANT_CATEGORY_ITEMS: DetailItem[] = [
  { key: 'qatari', label: 'JOB_POINTS.APPLICANT_CATEGORY.QATARI' },
  { key: 'qatarMother', label: 'JOB_POINTS.APPLICANT_CATEGORY.QATAR_MOTHER' },
  { key: 'gcc', label: 'JOB_POINTS.APPLICANT_CATEGORY.GCC' },
  { key: 'qatarGraduate', label: 'JOB_POINTS.APPLICANT_CATEGORY.QATAR_GRADUATE' },
  { key: 'qatarGraduatePrev', label: 'JOB_POINTS.APPLICANT_CATEGORY.QATAR_GRADUATE_PREV' },
];

// ================= Education =================
export const EDUCATION_ITEMS: DetailItem[] = []; 

// ================= Training =================
export const TRAINING_ITEMS: DetailItem[] = [
  { key: 'highLinked', label: 'JOB_POINTS.TRAINING.HIGH_LINKED' },
  { key: 'mediumLinked', label: 'JOB_POINTS.TRAINING.MEDIUM_LINKED' },
  { key: 'lowLinked', label: 'JOB_POINTS.TRAINING.LOW_LINKED' },
];

// ================= Skills =================
export const SKILLS_ITEMS: DetailItem[] = []; 

// ================= Certificates =================
export const CERTIFICATES_ITEMS: DetailItem[] = [
  { key: 'certificatesLinked', label: 'JOB_POINTS.CERTIFICATES_LINKED' },
  { key: 'certificatesNotLinked', label: 'JOB_POINTS.CERTIFICATES_NOT_LINKED' },
  { key: 'prize', label: 'JOB_POINTS.PRIZE_POINTS' },
];

// ================= Languages =================
export const LANGUAGE_ITEMS: DetailItem[] = [
  { key: 'speaking.max', label: 'JOB_POINTS.LANGUAGE_ABILITY.SPEAKING_MAX' },
  { key: 'speaking.excellent', label: 'JOB_POINTS.LANGUAGE_LEVEL.EXCELLENT' },
  { key: 'speaking.veryGood', label: 'JOB_POINTS.LANGUAGE_LEVEL.VERY_GOOD' },
  { key: 'speaking.good', label: 'JOB_POINTS.LANGUAGE_LEVEL.GOOD' },

  { key: 'reading.max', label: 'JOB_POINTS.LANGUAGE_ABILITY.READING_MAX' },
  { key: 'reading.excellent', label: 'JOB_POINTS.LANGUAGE_LEVEL.EXCELLENT' },
  { key: 'reading.veryGood', label: 'JOB_POINTS.LANGUAGE_LEVEL.VERY_GOOD' },
  { key: 'reading.good', label: 'JOB_POINTS.LANGUAGE_LEVEL.GOOD' },

  { key: 'conversation.max', label: 'JOB_POINTS.LANGUAGE_ABILITY.CONVERSATION_MAX' },
  { key: 'conversation.excellent', label: 'JOB_POINTS.LANGUAGE_LEVEL.EXCELLENT' },
  { key: 'conversation.veryGood', label: 'JOB_POINTS.LANGUAGE_LEVEL.VERY_GOOD' },
  { key: 'conversation.good', label: 'JOB_POINTS.LANGUAGE_LEVEL.GOOD' },

  { key: 'native', label: 'JOB_POINTS.LANGUAGE_LEVEL.NATIVE' },
];

export const EXPERIENCE_ITEMS: DetailItem[] = [
  { key: 'pointsPerYear', label: 'JOB_POINTS.EXPERIENCE.POINTS_PER_YEAR' },
  { key: 'maxYears', label: 'JOB_POINTS.EXPERIENCE.MAX_YEARS' },
];
