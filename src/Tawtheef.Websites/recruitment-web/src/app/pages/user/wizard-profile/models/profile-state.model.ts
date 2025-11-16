import {Degree} from "./degree.model";
import {Attachment} from './attachment.model';
import {Language} from './language.model';
import {Experience} from './experience.model';

export interface ProfileState {
  // prereq
  candidateType?: string;
  targetEntity?: string;
  cvName?: string;
  idName?: string;

  // personal
  fullName?: string;
  fullNameEn?: string;
  qid?: string;
  nationality?: string;
  gender?: string;
  religion?: string;
  marital?: string;
  children?: number;
  dob?: string;

  // contact
  country?: string;
  dialCode?: string;
  phone?: string;
  address?: string;
  email?: string;
  interviewPlace?: string;
  naZone?: string;
  naStreet?: string;
  naBuilding?: string;
  naUnit?: string;
  naFileName?: string;

  // collections
  degrees: Degree[];
  experiences: Experience[];
  courses: Experience[];
  achievements: Experience[];
  skills: string[];
  languages: Language[];
  attachments: Attachment[];

  // ui
  available: boolean;
  avatarUrl?: string;
}
