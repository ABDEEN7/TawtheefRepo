import {Injectable, signal, computed, inject} from '@angular/core';
import {ProfileState} from '../models/profile-state.model';
import {Language} from '../models/language.model';
import {Degree} from '../models/degree.model';
import {Experience, TrainingCourse} from '../models/experience.model';
import {Attachment} from '../models/attachment.model';
import {CandidateType} from '../../../../core/enums/lookups.enum';
import {Skill} from '../models/skill.model';
import {UserService} from '../../../../core/auth/user.service';
import {NationalityMapperService, PhoneMapperService} from './phone-mapper.service';
import {CountryISO} from 'ngx-intl-tel-input';



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

  private candidateTypeFromState(state: ProfileState): CandidateType | undefined {
    return state.candidateType?.backendName as CandidateType | undefined;
  }

  private candidateTypeNeedsSponsor(type: CandidateType | undefined): boolean {
    return !!type && [CandidateType.ResidentQatar].includes(type);
  }

  private candidateTypeNeedsBirthCertificate(type: CandidateType | undefined): boolean {
    return !!type && [CandidateType.SonOfQatariMother].includes(type);
  }

  private candidateTypeNeedsMarriageCertificate(type: CandidateType | undefined): boolean {
    return !!type && [CandidateType.WifeOfQatari].includes(type);
  }

  private candidateTypeIsResident(type: CandidateType | undefined): boolean {
    if (!type) return false;

    return [
      CandidateType.ResidentQatar,
      CandidateType.Qatari,
      CandidateType.SonOfQatariMother,
      CandidateType.WifeOfQatari
    ].includes(type);
  }

  get isNeedSponsor(){
    return this.candidateTypeNeedsSponsor(this.candidateTypeFromState(this.state()));
  }
  get isNeedBirthCertificate() {
    const t = this.candidateTypeFromState(this.state());
    if (!t) return false;
    return this.candidateTypeNeedsBirthCertificate(t);
  }

  get isNeedMarriageCertificate() {
    const t = this.candidateTypeFromState(this.state());
    if (!t) return false;
    return this.candidateTypeNeedsMarriageCertificate(t);
  }
  get isResidentQatar(): boolean {
    return this.candidateTypeIsResident(this.candidateTypeFromState(this.state()));
  }

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

  stepValidity = computed(() => {
    const s = this.state();

    const basicValidExceptionCase =
      (!this.isNeedBirthCertificate && !this.isNeedMarriageCertificate) ||
      (this.isNeedMarriageCertificate && this.isFilledScalar(s.marriageCertificateName)) ||
      (this.isNeedBirthCertificate && this.isFilledScalar(s.birthCertificateName));


    const basicValid =
      this.isFilledScalar(s.candidateType) &&
      this.isFilledScalar(s.targetEntity) &&
      this.isFilledScalar(s.cvName) &&
      this.isFilledScalar(s.idName) &&
      basicValidExceptionCase;

    const disabilityTypeValid =
      !s.hasDisability || this.isFilledScalar(s.disabilityDetails);

    const sponsorValid =
      !this.isNeedSponsor || (
      this.isFilledScalar(s.sponsorType) &&
      this.isFilledScalar(s.sponsorEmployerName) &&
      this.isFilledScalar(s.sponsorEmployerNumber) &&
      this.isFilledScalar(s.sponsorCardName));

    const personalValid =
      this.isFilledScalar(s.fullNameAr) &&
      this.isFilledScalar(s.fullNameEn) &&
      this.isFilledScalar(s.qid) &&
      this.isFilledScalar(s.nationality) &&
      this.isFilledScalar(s.dob) &&
      this.isFilledScalar(s.gender) &&
      this.isFilledScalar(s.religion) &&
      this.isFilledScalar(s.marital) &&
      disabilityTypeValid &&
      sponsorValid;

    const addressValid =
      (this.isResidentQatar && this.isFilledScalar(s.naZone) &&this.isFilledScalar(s.naStreet) &&this.isFilledScalar(s.naBuilding) &&this.isFilledScalar(s.naFileName)) ||
      (!this.isResidentQatar && this.isFilledScalar(s.address));

    const contactValid =
      this.isFilledScalar(s.country) &&
      this.isFilledScalar(s.phone) &&
      this.isFilledScalar(s.phoneVerified) &&
      this.isFilledScalar(s.email) &&
      this.isFilledScalar(s.emailVerified) &&
      addressValid;

    const degreesValid   = Array.isArray(s.degrees) && s.degrees.length > 0&&
      s.degrees.every(a => !!a.certificate && this.isFilledScalar(a.certificate.resourceName) && (!!a.file || !!a.attachmentId));
    const expValid       = Array.isArray(s.experiences) && s.experiences.length > 0&&
      s.experiences.every(a => !!a.attachment && this.isFilledScalar(a.attachment.resourceName) && (!!a.file || !!a.attachmentId));
    const courseValid       = Array.isArray(s.courses) && s.courses.length > 0&&
      s.courses.every(a => !!a.attachment && this.isFilledScalar(a.attachment.resourceName) && (!!a.file || !!a.attachmentId));

    const skillsValid    = Array.isArray(s.skills) && s.skills.length > 0;
    const languagesValid = Array.isArray(s.languages) && s.languages.length  > 0;

    const attachmentsValid = Array.isArray(s.attachments) &&
      s.attachments.length > 0 &&
      s.attachments.every(a => this.isFilledScalar(a.fileName ?? a.name) && (!!a.file || !!a.attachmentId));

    return {
      basic: basicValid,
      personal: personalValid,
      contact: contactValid,
      degrees: degreesValid,
      experience: expValid && courseValid,
      skills: skillsValid,
      languages: languagesValid,
      attachments: attachmentsValid,
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
    const type = this.candidateTypeFromState(state);
    const next: ProfileState = { ...state };

    if (!this.candidateTypeNeedsSponsor(type)) {
      next.sponsorType = null;
      next.sponsorEmployerName = null;
      next.sponsorEmployerNumber = null;
      next.sponsorCardName = null;
      next.sponsorCardFile = null;
    }

    if (!this.candidateTypeNeedsBirthCertificate(type)) {
      next.birthCertificateName = null;
      next.birthCertificateFile = null;
    }

    if (!this.candidateTypeNeedsMarriageCertificate(type)) {
      next.marriageCertificateName = null;
      next.marriageCertificateFile = null;
    }

    if (!this.candidateTypeIsResident(type)) {
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
