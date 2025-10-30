export interface Degree {
  level?: string;
  major?: string;
  uni?: string;
  year?: string;
  fileName?: string;
}


export interface Experience {
  org?: string;
  title?: string;
  from?: string;
  to?: string;
  tasks?: string;
  fileName?: string;
}


export interface Attachment {
  title?: string;
  fileName?: string;
}


export interface LanguageSkill {
  name: string;
  level: string;
}


export interface UserProfile {
  fullName?: string;
  fullNameEn?: string;
  qid?: string;
  passport?: string;
  dob?: string;
  country?: string;
  dialCode?: string;
  phone?: string;
  address?: string;
  email?: string;
  targetEntity?: string;
  subEntity?: string;
  position?: string;
  cvName?: string;
  idName?: string;
  degrees?: Degree[];
  experiences?: Experience[];
  attachments?: Attachment[];
  skills?: string[];
  languages?: LanguageSkill[];
}
