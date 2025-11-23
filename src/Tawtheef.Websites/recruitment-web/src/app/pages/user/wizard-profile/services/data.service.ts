import { Injectable, signal, computed } from '@angular/core';
import {ProfileState} from '../models/profile-state.model';
import {Language} from '../models/language.model';
import {Degree} from '../models/degree.model';
import {Experience} from '../models/experience.model';
import {Attachment} from '../models/attachment.model';
import {CandidateType} from '../../../../core/enums/lookups.enum';



@Injectable({ providedIn: 'root' })
export class DataService {
  state = signal<ProfileState>({
    degrees: [], experiences: [], courses: [], achievements: [],
    skills: [], languages: [], attachments: [],
    available: true, hasDisability: false,
    emailVerified: false, phoneVerified: false,
  });

  private isFilledScalar = (val: unknown) => {
    if (typeof val === 'string') return val.trim().length > 0;
    if (typeof val === 'number') return Number.isFinite(val); // counts even 0
    if (typeof val === 'boolean') return val as boolean; // counts even 0
    return !!val;
  };

  private locked = signal<Partial<Record<keyof ProfileState, boolean>>>({});

  isLocked<K extends keyof ProfileState>(key: K): boolean {
    const l = this.locked();
    return !!l[key];
  }

  private lockableKeys: (keyof ProfileState)[] = ['qid','dob','nationality','gender','phone','email'];
  prefillFromBootstrap(prefill: Partial<ProfileState>) {
    this.state.update(s => ({ ...s, ...prefill }));
    this.locked.update(m => {
      const copy = { ...m };
      for (const k of Object.keys(prefill) as (keyof ProfileState)[]) {
        if (!this.lockableKeys.includes(k)) continue;
        const value = prefill[k];
        const hasValue =
          value !== null && value !== undefined &&
          (typeof value !== 'string' || value.trim().length > 0);

        if (hasValue) {
          copy[k] = true;
        }
      }
      return copy;
    });
  }
  stepValidity = computed(() => {
    const s = this.state();

    const basicValidExceptionCase =
      ![CandidateType.WifeOfQatari, CandidateType.SonOfQatariMother].includes(s.candidateType?.backendName as CandidateType) ||
      (s.candidateType?.backendName == CandidateType.WifeOfQatari && this.isFilledScalar(s.marriageCertificateName)) ||
      (s.candidateType?.backendName == CandidateType.SonOfQatariMother && this.isFilledScalar(s.birthCertificateName));


    const basicValid =
      this.isFilledScalar(s.candidateType) &&
      this.isFilledScalar(s.targetEntity) &&
      this.isFilledScalar(s.cvName) &&
      this.isFilledScalar(s.idName) &&
      basicValidExceptionCase;

    const hasDisabilityValid = s.hasDisability !== null && s.hasDisability !== undefined;
    const disabilityTypeValid =
      !s.hasDisability || this.isFilledScalar(s.disabilityDetails);

    const sponsorValid =
      this.isFilledScalar(s.sponsorType) &&
      this.isFilledScalar(s.sponsorEmployerName) &&
      this.isFilledScalar(s.sponsorEmployerNumber) &&
      this.isFilledScalar(s.sponsorCardName);

    const personalValid =
      this.isFilledScalar(s.fullName) &&
      this.isFilledScalar(s.fullNameEn) &&
      this.isFilledScalar(s.qid) &&
      this.isFilledScalar(s.dob) &&
      this.isFilledScalar(s.nationality) &&
      this.isFilledScalar(s.gender) &&
      this.isFilledScalar(s.religion) &&
      this.isFilledScalar(s.marital) &&
      hasDisabilityValid &&
      disabilityTypeValid &&
      sponsorValid;

    const contactValid =
      this.isFilledScalar(s.country) &&
      this.isFilledScalar(s.dialCode) &&
      this.isFilledScalar(s.phone) &&
      this.isFilledScalar(s.phoneVerified) &&
      this.isFilledScalar(s.email) &&
      this.isFilledScalar(s.emailVerified) &&
      this.isFilledScalar(s.address);

    const degreesValid   = Array.isArray(s.degrees) && s.degrees.length > 0;
    const expValid       = Array.isArray(s.experiences) && s.experiences.length > 0;
    const skillsValid    = (Array.isArray(s.skills) && s.skills.length > 0) || (Array.isArray(s.languages)  && s.languages.length  > 0);

    return {
      basic: basicValid,
      personal: personalValid,
      contact: contactValid,
      degrees: degreesValid,
      experience: expValid,
      skills: skillsValid,
      attachments: true,
    } as const;
  });

  up<K extends keyof ProfileState>(key: K, val: ProfileState[K] | null) {
    this.state.update(s => ({ ...s, [key]: val }));
  }

  addDegree(d: Degree){ this.state.update(s => ({...s, degrees:[...s.degrees, d]})); }
  delDegree(i:number){ this.state.update(s => ({...s, degrees: s.degrees.filter((_,x)=>x!==i)})); }

  addExp(e: Experience){ this.state.update(s => ({...s, experiences:[...s.experiences, e]})); }
  delExp(i:number){ this.state.update(s => ({...s, experiences: s.experiences.filter((_,x)=>x!==i)})); }

  addCourse(e: Experience){ this.state.update(s => ({...s, courses:[...s.courses, e]})); }
  delCourse(i:number){ this.state.update(s => ({...s, courses: s.courses.filter((_,x)=>x!==i)})); }
  addAchievement(e: Experience){ this.state.update(s => ({...s, achievements:[...s.achievements, e]})); }
  delAchievement(i:number){ this.state.update(s => ({...s, achievements: s.achievements.filter((_,x)=>x!==i)})); }

  addLang(l: Language){ this.state.update(s => ({...s, languages:[...s.languages, l]})); }
  delLang(i:number){ this.state.update(s => ({...s, languages: s.languages.filter((_,x)=>x!==i)})); }

  addSkill(tag: string){
    this.state.update(s => s.skills.includes(tag)
      ? s
      : ({...s, skills:[...s.skills, tag]})
    );
  }
  delSkill(tag: string){ this.state.update(s => ({...s, skills: s.skills.filter(t=>t!==tag)})); }

  addAttachment(a: Attachment){ this.state.update(s => ({...s, attachments:[...s.attachments, a]})); }
  delAttachment(i:number){ this.state.update(s => ({...s, attachments: s.attachments.filter((_,x)=>x!==i)})); }
}
