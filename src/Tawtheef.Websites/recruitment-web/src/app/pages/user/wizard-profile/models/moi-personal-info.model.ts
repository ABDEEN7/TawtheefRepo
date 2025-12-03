export interface MoiPersonalInfo {
  qid: string;
  arabicName1: string;
  arabicName2?: string | null;
  arabicName3?: string | null;
  arabicName4?: string | null;
  arabicName5: string;
  englishName1: string;
  englishName2?: string | null;
  englishName3?: string | null;
  englishName4?: string | null;
  englishName5: string;
  dateOfBirth: string;
  qidExpiry: string;
  nationalityCode: number | string;
  gender: string;
}

export function buildArabicFullName(info: MoiPersonalInfo): string {
  return [info.arabicName1, info.arabicName2, info.arabicName3, info.arabicName4, info.arabicName5]
    .filter(part => !!part)
    .join(' ');
}

export function buildEnglishFullName(info: MoiPersonalInfo): string {
  return [info.englishName1, info.englishName2, info.englishName3, info.englishName4, info.englishName5]
    .filter(part => !!part)
    .join(' ');
}
