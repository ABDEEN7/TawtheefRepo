import {MoiPersonalInfo} from '../models/moi-personal-info.model';

export function normalizeMoiResponse(raw: any): MoiPersonalInfo {
  const pick = (key: string) => raw?.[key] ?? raw?.[key.charAt(0).toUpperCase() + key.slice(1)];

  return {
    qid: pick('qid') ?? '',
    arabicName1: pick('arabicName1') ?? pick('arabicFirstName') ?? '',
    arabicName2: pick('arabicName2') ?? pick('arabicSecondName') ?? null,
    arabicName3: pick('arabicName3') ?? pick('arabicThirdName') ?? null,
    arabicName4: pick('arabicName4') ?? pick('arabicFourthName') ?? null,
    arabicName5: pick('arabicName5') ?? pick('arabicFamilyName') ?? '',
    englishName1: pick('englishName1') ?? pick('englishFirstName') ?? '',
    englishName2: pick('englishName2') ?? pick('englishSecondName') ?? null,
    englishName3: pick('englishName3') ?? pick('englishThirdName') ?? null,
    englishName4: pick('englishName4') ?? pick('englishFourthName') ?? null,
    englishName5: pick('englishName5') ?? pick('englishFamilyName') ?? '',
    dateOfBirth: pick('dateOfBirth') ?? '',
    qidExpiry: pick('qidExpiry') ?? '',
    nationalityCode: pick('nationalityCode') ?? '',
    gender: pick('gender') ?? '',
  };
}
