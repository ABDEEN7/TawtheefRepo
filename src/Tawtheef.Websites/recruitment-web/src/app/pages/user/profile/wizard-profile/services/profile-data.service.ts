import {computed, inject, Injectable, signal} from '@angular/core';
import {ProfileState} from '../models/profile-state.model';
import {Language} from '../models/language.model';
import {Degree} from '../models/degree.model';
import {Experience, TrainingCourse} from '../models/experience.model';
import {Achievement} from '../models/achievement.model';
import {Attachment} from '../models/attachment.model';
import {Skill} from '../models/skill.model';
import {PhoneMapperService} from './phone-mapper.service';
import {ProfileLookupsService} from './profile-lookups.service';
import {buildArabicFullName, buildEnglishFullName, MoiPersonalInfo} from '../models/moi-personal-info.model';
import {ProfileService} from './profile.service';
import {normalizeMoiResponse} from './moi-response-normalizer';
import {
  candidateTypeFromState,
  candidateTypeIsResident,
  candidateTypeNeedsBirthCertificate,
  candidateTypeNeedsMarriageCertificate,
  candidateTypeNeedsSponsor,
  createStepValiditySignal,
} from '../state/profile-step-validity.signal';
import {NationalityMapperService} from './nationality-mapper.service';
import {take, tap} from 'rxjs';
import {mapProfileStatusToState} from './profile.mapper';
import {UserService} from '../../../../../core/auth/user.service';
import {CandidateType, SponsorType} from '../../../../../core/enums/lookups.enum';

@Injectable()
export class ProfileDataService {
  nationalityMapperService = inject(NationalityMapperService);
  phoneMapperService = inject(PhoneMapperService);
  userService = inject(UserService);
  lookups = inject(ProfileLookupsService);
  profileService = inject(ProfileService);
  state = signal<ProfileState>({
    provider: 'Google',
    isKawaderQid: false,
    degrees: [], experiences: [], courses: [], achievements: [],
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
    return candidateTypeIsResident(candidateTypeFromState(this.state()), this.state().provider);
  }
  get isIndividualSponsor(): boolean {
    return this.state().sponsorType?.backendName === SponsorType.Individual;
  }

  get isCandidateTypeLocked(): boolean {
    return this.shouldLockCandidateType();
  }

  private locked = signal<Partial<Record<keyof ProfileState, boolean>>>({});

  isLocked<K extends keyof ProfileState>(key: K): boolean {
    const l = this.locked();
    if (l[key]) return true;

    if (this.isResidentQatar && (key === 'fullNameAr' || key === 'fullNameEn')) return true;
    if (this.isIndividualSponsor && key === 'sponsorEmployerName') return true;
    if (this.shouldLockCandidateType() && key === 'candidateType') return true;

    return false;
  }

  private lockableKeys: (keyof ProfileState)[] = ['qid','dob','nationality','gender','phone','email','fullNameAr','fullNameEn','sponsorEmployerName','sponsorEmployerNumber','candidateType'];
  lockFields(keys: (keyof ProfileState)[]) {
    this.locked.update(m => {
      const copy = { ...m };
      keys.forEach(k => copy[k] = true);
      return copy;
    });
  }
  prefillFromBootstrap(userData: Partial<ProfileState>) {
    this.state.update(s => ({ ...s, ...userData }));
    this.lockedPrefillData();
    this.applyKawaderCandidateType();
    this.prefillFromCheckProfile();
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
      achievements: v.achievements.valid,
      skills: v.skills.valid,
      languages: v.languages.valid,
      attachments: v.attachments.valid,
    } as const;
  });

  up<K extends keyof ProfileState>(key: K, val: ProfileState[K] | null) {
    if (key === 'candidateType' && this.shouldLockCandidateType()) return;
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
      next.sponsorQidExpiry = null;
      next.sponsorCardName = null;
      next.sponsorCard = null;
    }

    if (!candidateTypeNeedsBirthCertificate(type)) {
      next.birthCertificateName = null;
      next.birthCertificateFile = null;
    }

    if (!candidateTypeNeedsMarriageCertificate(type)) {
      next.marriageCertificateName = null;
      next.marriageCertificateFile = null;
    }

    if (!candidateTypeIsResident(type, state.provider)) {
      next.naZone = null;
      next.naStreet = null;
      next.naBuilding = null;
      next.naUnit = null;
      next.naFileName = null;
      next.naFile = null;
      next.qidExpiry = null;
    } else {
      next.address = undefined;
    }

    return next;
  }

  private shouldLockCandidateType(): boolean {
    const provider = (this.state().provider ?? '').toString().toLowerCase();
    const isPreferredProvider = ['qatarpass', 'qatarresidentotp'].includes(provider);
    return isPreferredProvider && !!this.state().isKawaderQid;
  }

  private applyKawaderCandidateType(): void {
    if (!this.shouldLockCandidateType()) return;

    const qatariOption = this.lookups
      .candidateTypes()
      .find(type => type.backendName === CandidateType.Qatari);

    if (!qatariOption) return;

    this.state.update(s => {
      const updated = { ...s, candidateType: qatariOption } as ProfileState;
      return this.cleanCandidateTypeDependents(updated);
    });

    this.locked.update(m => ({ ...m, candidateType: true }));
  }

  applyMoiPersonalInfo(info: MoiPersonalInfo) {
    const nationality = this.nationalityMapperService.toNationalityObject(String(info.nationalityCode));
    const gender = this.lookups.genders().find(g => g.backendName?.toUpperCase() === info.gender?.toUpperCase());
    const arabicFullName = buildArabicFullName(info);
    const englishFullName = buildEnglishFullName(info);
    const shouldLockCheckProfile = this.isResidentQatar;

    this.state.update(s => ({
      ...s,
      fullNameAr: arabicFullName || s.fullNameAr,
      fullNameEn: englishFullName || s.fullNameEn,
      qid: info.qid || s.qid,
      qidExpiry: info.qidExpiry || s.qidExpiry,
      dob: info.dateOfBirth || s.dob,
      nationality: nationality ?? s.nationality,
      gender: gender ?? s.gender,
    }));

    this.locked.update(m => ({
      ...m,
      fullNameAr: (shouldLockCheckProfile && !!arabicFullName) || m.fullNameAr,
      fullNameEn: (shouldLockCheckProfile && !!englishFullName) || m.fullNameEn,
      qid: (shouldLockCheckProfile && !!info.qid) || m.qid,
      dob: (shouldLockCheckProfile && !!info.dateOfBirth) || m.dob,
      nationality: (shouldLockCheckProfile && !!nationality) || m.nationality,
      gender: (shouldLockCheckProfile && !!gender) || m.gender,
    }));
  }

  private prefillFromCheckProfile() {
    if (!this.isResidentQatar) return;

    const { qid, qidExpiry } = this.state();
    if (!qid || !qidExpiry) return;

    this.profileService
      .checkProfile(qid, qidExpiry)
      .pipe(take(1))
      .subscribe({
        next: res => this.applyMoiPersonalInfo(normalizeMoiResponse(res))
      });
  }

  refreshDegreesFromBackend() {
    return this.profileService
      .getProfileStatus()
      .pipe(
        take(1),
        tap(dto => {
          const mapped = mapProfileStatusToState(
            this.phoneMapperService,
            this.lookups,
            dto,
            this.userService.getPrefill()
          );
          this.state.update(s => ({ ...s, degrees: mapped.degrees }));
        })
      );
  }

  applySponsorPersonalInfo(info: MoiPersonalInfo) {
    const arabicFullName = buildArabicFullName(info);
    const englishFullName = buildEnglishFullName(info);
    const sponsorName = arabicFullName || englishFullName || this.state().sponsorEmployerName;

    this.state.update(s => ({
      ...s,
      sponsorEmployerName: sponsorName || s.sponsorEmployerName,
      sponsorEmployerNumber: info.qid || s.sponsorEmployerNumber,
      sponsorQidExpiry: info.qidExpiry || s.sponsorQidExpiry,
    }));

    this.locked.update(m => ({
      ...m,
      sponsorEmployerName: !!sponsorName || m.sponsorEmployerName,
      sponsorEmployerNumber: !!info.qid || m.sponsorEmployerNumber,
      sponsorQidExpiry: !!info.qidExpiry || m.sponsorQidExpiry,
    }));
  }

  addDegree(d: Degree){ this.state.update(s => ({...s, degrees:[...s.degrees, d]})); }
  delDegree(i:number){ this.state.update(s => ({...s, degrees: s.degrees.filter((_,x)=>x!==i)})); }

  addExp(e: Experience){ this.state.update(s => ({...s, experiences:[...s.experiences, e]})); }
  delExp(i:number){ this.state.update(s => ({...s, experiences: s.experiences.filter((_,x)=>x!==i)})); }

  addCourse(e: TrainingCourse){ this.state.update(s => ({...s, courses:[...s.courses, e]})); }
  delCourse(i:number){ this.state.update(s => ({...s, courses: s.courses.filter((_,x)=>x!==i)})); }

  addAchievement(a: Achievement){ this.state.update(s => ({...s, achievements:[...s.achievements, a]})); }
  updateAchievement(i: number, a: Achievement){ this.state.update(s => ({...s, achievements: s.achievements.map((item,idx)=> idx===i ? a : item)})); }
  delAchievement(i:number){ this.state.update(s => ({...s, achievements: s.achievements.filter((_,x)=>x!==i)})); }

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

  setState(next: ProfileState): void {
    this.state.set(next);
  }

  patch(partial: Partial<ProfileState>): void {
    this.state.update(s => ({ ...s, ...partial }));
  }

  clear(): void {
    this.state.set(this.createEmptyState());
  }

  private createEmptyState(): ProfileState {
    return {
      provider: 'Google',
      isKawaderQid: false,
      prerequisites: {},
      personal: {},
      contact: {},
      qualifications: [],
      experiences: [],
      trainingCourses: [],
      achievements: [],
      skills: [],
      languages: [],
      attachments: []
    } as unknown as ProfileState;
  }
}
