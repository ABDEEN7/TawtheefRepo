import {Injectable, signal, computed, inject} from '@angular/core';
import {ProfileState} from '../models/profile-state.model';
import {Language} from '../models/language.model';
import {Degree} from '../models/degree.model';
import {Experience, TrainingCourse} from '../models/experience.model';
import {Attachment} from '../models/attachment.model';
import {Skill} from '../models/skill.model';
import {UserService} from '../../../../core/auth/user.service';
import {NationalityMapperService, PhoneMapperService} from './phone-mapper.service';
import {CountryISO} from 'ngx-intl-tel-input';
import {
  candidateTypeFromState,
  candidateTypeIsResident,
  candidateTypeNeedsBirthCertificate,
  candidateTypeNeedsMarriageCertificate,
  candidateTypeNeedsSponsor,
  createStepValiditySignal,
} from '../state/profile-step-validity.signal';



@Injectable({ providedIn: 'root' })
export class DataService {
  nationalityMapperService = inject(NationalityMapperService);
  phoneMapperService = inject(PhoneMapperService);
  userService = inject(UserService);
  state = signal<ProfileState>({
    degrees: [], experiences: [], courses: [],
    skills: [], languages: [], attachments: [],
    available: true, hasDisability: false,
    emailVerified: false, phoneVerified: false,
  });

  get isNeedSponsor(){
    return candidateTypeNeedsSponsor(candidateTypeFromState(this.state()));
  }
  get isNeedBirthCertificate() {
    const t = candidateTypeFromState(this.state());
    if (!t) return false;
    return candidateTypeNeedsBirthCertificate(t);
  }

  get isNeedMarriageCertificate() {
    const t = candidateTypeFromState(this.state());
    if (!t) return false;
    return candidateTypeNeedsMarriageCertificate(t);
  }
  get isResidentQatar(): boolean {
    return candidateTypeIsResident(candidateTypeFromState(this.state()));
  }

  private locked = signal<Partial<Record<keyof ProfileState, boolean>>>({});

  isLocked<K extends keyof ProfileState>(key: K): boolean {
    const l = this.locked();
    return !!l[key];
  }

  private lockableKeys: (keyof ProfileState)[] = ['qid','dob','nationality','gender','phone','email'];
  prefillFromBootstrap(userData: Partial<ProfileState>) {
    this.state.update(s => ({ ...s, ...userData }));
    this.lockedPrefillData();
  }
  private lockedPrefillData() {
    const prefill = this.userService.getPrefill();
    if (!prefill) return;

    const state = this.state();
    const prefillMap: Partial<ProfileState> = {
      email: prefill.email,
      emailVerified: prefill.emailVerified,
      phone: this.phoneMapperService.toPhoneObject(prefill.phone),
      phoneVerified: prefill.phoneVerified,
      nationality: this.nationalityMapperService.toNationalityObject(prefill.nationality),
      qid: prefill.qid,
    };

    this.locked.update(m => {
      const copy = { ...m };

      for (const k of Object.keys(prefillMap) as (keyof ProfileState)[]) {
        if (!this.lockableKeys.includes(k)) continue;

        const prefillValue = prefillMap[k];
        if (prefillValue === null || prefillValue === undefined) {
          copy[k] = false;
          continue;
        }

        const currentValue = state[k];
        const matches =
          typeof currentValue === 'string' && typeof prefillValue === 'string'
            ? currentValue.trim().toLowerCase() === prefillValue.trim().toLowerCase()
            : JSON.stringify(currentValue) === JSON.stringify(prefillValue);
        copy[k] = matches;
      }

      return copy;
    });
  }

  stepValidationDetailed = createStepValiditySignal(this.state);

  stepValidity = computed(() => {
    const v = this.stepValidationDetailed();
    return {
      basic: v.basic.valid,
      personal: v.personal.valid,
      contact: v.contact.valid,
      degrees: v.degrees.valid,
      experience: v.experience.valid,
      skills: v.skills.valid,
      languages: v.languages.valid,
      attachments: v.attachments.valid,
    } as const;
  });

  up<K extends keyof ProfileState>(key: K, val: ProfileState[K] | null) {
    this.state.update(s => {
      const updated = { ...s, [key]: val } as ProfileState;

      if (key === 'candidateType') {
        return this.cleanCandidateTypeDependents(updated);
      }

      if (key === 'hasDisability') {
        return this.cleanDisabilityDependents(updated);
      }

      return updated;
    });
  }

  private cleanDisabilityDependents(state: ProfileState): ProfileState {
    const next: ProfileState = { ...state };

    if (!next.hasDisability) {
      next.disabilityDetails = null;
    }

    return next;
  }

  private cleanCandidateTypeDependents(state: ProfileState): ProfileState {
    const type = candidateTypeFromState(state);
    const next: ProfileState = { ...state };

    if (!candidateTypeNeedsSponsor(type)) {
      next.sponsorType = null;
      next.sponsorEmployerName = null;
      next.sponsorEmployerNumber = null;
      next.sponsorCardName = null;
      next.sponsorCardFile = null;
    }

    if (!candidateTypeNeedsBirthCertificate(type)) {
      next.birthCertificateName = null;
      next.birthCertificateFile = null;
    }

    if (!candidateTypeNeedsMarriageCertificate(type)) {
      next.marriageCertificateName = null;
      next.marriageCertificateFile = null;
    }

    if (!candidateTypeIsResident(type)) {
      next.naZone = null;
      next.naStreet = null;
      next.naBuilding = null;
      next.naUnit = null;
      next.naFileName = null;
      next.naFile = null;
    } else {
      next.address = undefined;
    }

    return next;
  }

  addDegree(d: Degree){ this.state.update(s => ({...s, degrees:[...s.degrees, d]})); }
  delDegree(i:number){ this.state.update(s => ({...s, degrees: s.degrees.filter((_,x)=>x!==i)})); }

  addExp(e: Experience){ this.state.update(s => ({...s, experiences:[...s.experiences, e]})); }
  delExp(i:number){ this.state.update(s => ({...s, experiences: s.experiences.filter((_,x)=>x!==i)})); }

  addCourse(e: TrainingCourse){ this.state.update(s => ({...s, courses:[...s.courses, e]})); }
  delCourse(i:number){ this.state.update(s => ({...s, courses: s.courses.filter((_,x)=>x!==i)})); }

  addLang(l: Language){ this.state.update(s => ({...s, languages:[...s.languages, l]})); }
  delLang(i:number){ this.state.update(s => ({...s, languages: s.languages.filter((_,x)=>x!==i)})); }

  addSkill(skill: Skill){
    this.state.update(s => s.skills.some(t => t.skillId === skill.skillId)
      ? s
      : ({...s, skills:[...s.skills, skill]})
    );
  }
  delSkill(i: number){ this.state.update(s => ({...s, skills: s.skills.filter((_,x)=>x!==i)})); }

  addAttachment(a: Attachment){ this.state.update(s => ({...s, attachments:[...s.attachments, a]})); }
  delAttachment(i:number){ this.state.update(s => ({...s, attachments: s.attachments.filter((_,x)=>x!==i)})); }
}
