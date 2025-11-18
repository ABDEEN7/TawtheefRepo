import { Injectable, signal, computed } from '@angular/core';
import {ProfileState} from '../models/profile-state.model';
import {Language} from '../models/language.model';
import {Degree} from '../models/degree.model';
import {Experience} from '../models/experience.model';
import {Attachment} from '../models/attachment.model';



@Injectable({ providedIn: 'root' })
export class DataService {
  state = signal<ProfileState>({
    degrees: [], experiences: [], courses: [], achievements: [],
    skills: [], languages: [], attachments: [],
    available: true,
  });

  /** Fields that count once when non-empty (strings trimmed; numbers count if defined, even 0) */
  private scalarKeys: (keyof ProfileState)[] = [
    'fullName','fullNameEn','qid','dob',
    'country','dialCode','phone','address','email',
    'nationality','gender','religion','marital','children',
    'interviewPlace','naZone','naStreet','naBuilding','naUnit','naFileName',
    'avatarUrl'
  ];

  /**
   * Arrays contribute by their length up to a cap.
   * Tweak caps to change how much each collection can add to progress.
   */
  private arrayCaps: Record<keyof ProfileState, number> = {
    degrees: 1,
    experiences: 1,
    courses: 1,
    achievements: 1,
    skills: 1,
    languages: 1,
    attachments: 1,
    // non-array keys set to 0 or omitted
    candidateType: 0, targetEntity: 0, cvName: 0, idName: 0,
    fullName: 0, fullNameEn: 0, qid: 0, nationality: 0, gender: 0, religion: 0, marital: 0, children: 0,
    dob: 0, country: 0, dialCode: 0, phone: 0, address: 0, email: 0, interviewPlace: 0,
    naZone: 0, naStreet: 0, naBuilding: 0, naUnit: 0, naFileName: 0,
    available: 0, avatarUrl: 0
  };

  private isFilledScalar = (val: unknown) => {
    if (typeof val === 'string') return val.trim().length > 0;
    if (typeof val === 'number') return Number.isFinite(val); // counts even 0
    return !!val;
  };

  progress = computed(() => {
    const s = this.state();

    // 1) Scalars
    const scalarFilled = this.scalarKeys.reduce((n, k) => n + (this.isFilledScalar(s[k]) ? 1 : 0), 0);
    const scalarTotal = this.scalarKeys.length;

    // 2) Arrays with caps
    const arrayEntries = Object.entries(this.arrayCaps)
      .filter(([_, cap]) => cap > 0) as Array<[keyof ProfileState, number]>;

    let arrayFilled = 0;
    let arrayTotal = 0;

    for (const [key, cap] of arrayEntries) {
      const arr = s[key] as unknown;
      const len = Array.isArray(arr) ? arr.length : 0;
      arrayFilled += Math.min(len, cap); // contribute by count up to cap
      arrayTotal += cap;                 // total possible points for this array
    }

    const filled = scalarFilled + arrayFilled;
    const total = scalarTotal + arrayTotal;

    return total === 0 ? 0 : Math.min(100, Math.round((filled / total) * 100));
  });
  private locked = signal<Partial<Record<keyof ProfileState, boolean>>>({});

  isLocked<K extends keyof ProfileState>(key: K): boolean {
    const l = this.locked();
    return !!l[key];
  }

  private lockableKeys: (keyof ProfileState)[] = [
    'qid',
    'dob',
    'nationality',
    'gender',
    'phone',
    'email'
  ];
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

    const personalValid =
      this.isFilledScalar(s.fullName) &&
      this.isFilledScalar(s.fullNameEn) &&
      this.isFilledScalar(s.qid) &&
      this.isFilledScalar(s.dob) &&
      this.isFilledScalar(s.nationality) &&
      this.isFilledScalar(s.gender) &&
      this.isFilledScalar(s.religion) &&
      this.isFilledScalar(s.marital) &&
      this.isFilledScalar(s.candidateType) &&
      this.isFilledScalar(s.targetEntity) &&
      this.isFilledScalar(s.cvName) &&
      this.isFilledScalar(s.idName);

    const contactValid =
      this.isFilledScalar(s.country) &&
      this.isFilledScalar(s.dialCode) &&
      this.isFilledScalar(s.phone) &&
      this.isFilledScalar(s.email) &&
      this.isFilledScalar(s.address);

    const degreesValid   = Array.isArray(s.degrees) && s.degrees.length > 0;
    const expValid       = Array.isArray(s.experiences) && s.experiences.length > 0;
    const skillsValid    = (Array.isArray(s.skills) && s.skills.length > 0) || (Array.isArray(s.languages)  && s.languages.length  > 0);

    return {
      personal: personalValid,
      contact: contactValid,
      degrees: degreesValid,
      experience: expValid,
      skills: skillsValid,
      attachments: true,
    } as const;
  });

  up<K extends keyof ProfileState>(key: K, val: ProfileState[K]) {
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
