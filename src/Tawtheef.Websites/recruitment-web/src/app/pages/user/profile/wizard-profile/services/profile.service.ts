import { Injectable, inject } from '@angular/core';
import { of } from 'rxjs';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {PROFILE_WRITE_MODE, ProfileWriteMode} from './profile-write-mode.token';
import {SaveProfilePrereqRequestModel} from '../models/save-profile-prereq-request.model';
import {SaveProfilePersonalRequestDto} from '../models/save-profile-personal-request.model';
import {MoiPersonalInfo} from '../models/moi-personal-info.model';
import {SaveProfileContactRequestDto} from '../models/save-user-contact-request.model';
import {ProfileStatusDto} from '../../../../../core/models/auth/auth-response.model';
import {GUID} from '../../../../../shared/types/guid.type';
import {Experience, TrainingCourse} from '../models/experience.model';
import {Achievement} from '../models/achievement.model';
import {Skill} from '../models/skill.model';
import {Attachment} from '../models/attachment.model';
import {Degree} from '../models/degree.model';
import {Language} from '../models/language.model';

type FileLike = File | null | undefined;

type SectionKey =
  | 'prereq'
  | 'personal'
  | 'contact'
  | 'education'
  | 'experience'
  | 'achievements'
  | 'skills'
  | 'languages'
  | 'attachments';

type UrlPair = { normal: string; change: string };

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private writeMode: ProfileWriteMode =
    inject(PROFILE_WRITE_MODE, { optional: true }) ?? 'create';

  setWriteMode(mode: ProfileWriteMode) {
    this.writeMode = mode;
  }

  isChangeRequestMode(): boolean {
    return this.writeMode === 'change-request';
  }

  // ========== PREREQ ==========
  savePrereq(
    dto: SaveProfilePrereqRequestModel,
    files: {
      cvFile?: FileLike;
      idFile?: FileLike;
      birthCertificateFile?: FileLike;
      marriageCertificateFile?: FileLike;
    }
  ) {
    const fd = this.fd()
      .json(dto)
      .file('cvFile', files.cvFile)
      .file('idFile', files.idFile)
      .file('birthCertificateFile', files.birthCertificateFile)
      .file('marriageCertificateFile', files.marriageCertificateFile)
      .build();

    return this.http.post(this.url('prereq'), fd);
  }

  // ========== PERSONAL ==========
  savePersonalSection(dto: SaveProfilePersonalRequestDto, files?: { sponsorCard?: FileLike }) {
    const fd = this.fd()
      .json(dto)
      .file('sponsorCard', files?.sponsorCard)
      .build();

    return this.http.post(this.url('personal'), fd);
  }

  checkProfile(qid: string, expiryDate: string) {
    return this.http.post<MoiPersonalInfo>(this.endpoints.user.profile.checkProfile, { qid, expiryDate });
  }

  // ========== CONTACT ==========
  saveRecruitmentAvailability(available: boolean) {
    return this.http.post(this.endpoints.user.profile.saveAvailability, { availableForRecruitment: available });
  }

  saveContactSection(dto: SaveProfileContactRequestDto, files?: { nationalAddressFile?: FileLike }) {
    const b = this.fd()
      .json(dto);

    if (dto.nationalAddress) {
      const na = dto.nationalAddress;
      b.scalar('nationalAddress.zone', na.zone)
        .scalar('nationalAddress.street', na.street)
        .scalar('nationalAddress.building', na.building)
        .scalar('nationalAddress.unit', na.unit)
        .scalar('nationalAddress.nationalAddressFileName', na.nationalAddressFileName);
    }

    // keep your original field name
    if (files?.nationalAddressFile) {
      b.rawAppend('nationalAddress.nationalAddress', files.nationalAddressFile);
    }

    return this.http.post(this.url('contact'), b.build());
  }

  getProfileBasics() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.basics);
  }

  getProfileStatus() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.bootstrap);
  }

  getPrereqSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.prereq);
  }

  getPersonalSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.personal);
  }

  getContactSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.contact);
  }

  getQualificationsSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.education);
  }

  getExperienceSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.experience);
  }

  getAchievementsSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.achievements);
  }

  getSkillsSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.skills);
  }

  getLanguagesSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.languages);
  }

  getAttachmentsSection() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.sections.attachments);
  }

  // ========== EDUCATION (Degrees) ==========
  // Policy: no updates -> only new items (no id)
  saveEducationSection(degrees: Degree[]) {
    const allowUpdates = this.writeMode === 'change-request';
    const fileBucket: File[] = [];
    let cursor = 0;

    const payload = (degrees ?? [])
      .filter(d => allowUpdates || !d.id)
      .map(d => {
        const fileIndex = d.file ? cursor++ : null;
        if (d.file) fileBucket.push(d.file);

        return {
          id: d.id ?? null,
          degreeId: d.degree!.id,
          gradCountryId: d.gradCountry!.id,
          universityId: d.university?.id,
          majorId: d.major?.id,
          subMajorId: d.subMajor?.id,
          gradYear: d.gradYear,
          studyTypeId: d.studySystem?.id,
          gpa: d.gpa,
          gradeId: d.grade?.id,
          certificateId: d.attachmentId ?? null,
          fileIndex,
          existingFileName: d.certificate?.resourceName ?? d.fileName ?? null,
        };
      });

    if(payload.length === 0) return of(null);

    const fd = this.fd()
      .json({ degreesJson: payload })
      .files('DegreeFiles', fileBucket)
      .build();

    return this.http.post(this.url('education'), fd);
  }

  deleteEducation(degreeId: GUID) {
    return this.http.delete(this.endpoints.user.profile.deleteEducation(degreeId));
  }

  // ========== EXPERIENCE + TRAINING COURSES ==========
  // Policy: no updates -> ONLY new items (no id)
  saveExperienceSection(experiences: Experience[], courses: TrainingCourse[]) {
    const allowUpdates = this.writeMode === 'change-request';
    const experienceFiles: File[] = [];
    const trainingFiles: File[] = [];

    const experiencesDto = (experiences ?? [])
      .filter(e => allowUpdates || !e.id)
      .map(e => ({
        id: allowUpdates ? e.id ?? null : null,
        employerName: e.employerName,
        jobTitle: e.jobTitle,
        startDate: e.from,
        endDate: e.current ? null : e.to,
        countryId: e.country?.id,
        certificateId: e.attachmentId ?? null,
        certificateFileIndex: this.fileIndex(experienceFiles, e.file),
        description: e.description,
        qualificationId: e.qualificationId ?? null,
      }));

    const coursesDto = (courses ?? [])
      .filter(c => allowUpdates || !c.id)
      .map(c => ({
        id: allowUpdates ? c.id ?? null : null,
        title: c.title,
        provider: c.provider,
        startDate: c.from,
        endDate: c.to,
        countryId: c.country?.id,
        description: c.description,
        certificateId: c.attachmentId ?? null,
        certificateFileIndex: this.fileIndex(trainingFiles, c.file),
      }));

    if (experiencesDto.length === 0 && coursesDto.length === 0) return of(null);

    const fd = this.fd()
      .json({
        submit: false,
        experiencesJson: experiencesDto,
        trainingCoursesJson: coursesDto,
      })
      .files('ExperienceFiles', experienceFiles)
      .files('TrainingCourseFiles', trainingFiles)
      .build();

    return this.http.post(this.url('experience'), fd);
  }

  deleteExperience(experienceId: GUID) {
    return this.http.delete(this.endpoints.user.profile.deleteExperience(experienceId));
  }

  deleteTrainingCourse(courseId: GUID) {
    return this.http.delete(this.endpoints.user.profile.deleteTrainingCourse(courseId));
  }

  // ========== ACHIEVEMENTS ==========
  // Policy: no updates -> ONLY new items (no id)
  saveAchievementsSection(achievements: Achievement[]) {
    const allowUpdates = this.writeMode === 'change-request';
    const files: File[] = [];

    const payload = (achievements ?? [])
      .filter(a => allowUpdates || !a.id)
      .map(a => ({
        id: allowUpdates ? a.id ?? null : null,
        achievementTypeId: a.achievementType?.id,
        title: a.title,
        issuingAuthority: a.issuingAuthority,
        countryId: a.country?.id,
        issueDate: a.issueDate,
        description: a.description,
        attachmentId: a.attachmentId ?? null,
        certificateFileIndex: this.fileIndex(files, a.file),
        relatedToSpecialization: a.relatedToSpecialization ?? null,
      }));

    const fd = this.fd()
      .json({ submit: false, achievementsJson: payload })
      .files('AchievementFiles', files)
      .build();

    return this.http.post(this.url('achievements'), fd);
  }

  deleteAchievement(id: GUID) {
    return this.http.delete(this.endpoints.user.profile.deleteAchievement(id));
  }

  // ========== SKILLS ==========
  saveSkillsSection(skills: Skill[]) {
    const dto = {
      submit: false,
      skills: (skills ?? []).filter(s=> !s.id).map((s: any) => ({
        skillId: s.skillId,
        levelId: s.levelId,
      })),
    };

    return this.http.post(this.url('skills'), dto);
  }

  deleteSkill(skillId: GUID) {
    return this.http.delete(this.endpoints.user.profile.deleteSkill(skillId));
  }

  // ========== LANGUAGES ==========
  saveLanguagesSection(languages: Language[]) {
    const dto = {
      submit: false,
      languages: (languages ?? []).map(l => ({
        languageId: l.langId,
        speakingLevelId: l.speakingLevelId ?? l.speakingLevel?.id,
        writingLevelId: l.writingLevelId ?? l.writingLevel?.id,
        readingLevelId: l.readingLevelId ?? l.readingLevel?.id,
      })),
    };

    return this.http.post(this.url('languages'), dto);
  }

  deleteLanguage(languageId: GUID) {
    return this.http.delete(this.endpoints.user.profile.deleteLanguage(languageId));
  }

  // ========== ATTACHMENTS ==========
  saveAttachmentsSection(attachments: Attachment[]) {
    const allowUpdates = this.writeMode === 'change-request';
    const files: File[] = [];
    let cursor = 0;

    const payload = (attachments ?? [])
      .filter(a => allowUpdates || !a.id)
      .map(a => {
        const item: any = {
          id: allowUpdates ? a.id ?? null : a.id ?? null,
          title: a.title,
          fileName: a.fileName ?? a.title,
          attachmentId: a.attachmentId ?? null,
        };

        if (a?.file) {
          item.fileIndex = cursor++;
          files.push(a.file);
        }

        return item;
      });

    const fd = this.fd()
      .scalar('submit', false)
      .scalar('attachmentsJson', JSON.stringify(payload))
      .files('AttachmentFiles', files)
      .build();

    return this.http.post(this.url('attachments'), fd);
  }

  // ========== FINAL SUBMISSION ==========
  finalizeProfile() {
    return this.http.post(this.endpoints.user.profile.submit, {});
  }

  // ================= URL RESOLUTION =================

  private url(section: SectionKey): string {
    const map: Record<SectionKey, UrlPair> = {
      prereq: {
        normal: this.endpoints.user.profile.savePrereq,
        change: this.endpoints.user.profile.requestChanges.prereq,
      },
      personal: {
        normal: this.endpoints.user.profile.savePersonal,
        change: this.endpoints.user.profile.requestChanges.personal,
      },
      contact: {
        normal: this.endpoints.user.profile.saveContact,
        change: this.endpoints.user.profile.requestChanges.contact,
      },
      education: {
        normal: this.endpoints.user.profile.saveEducation,
        change: this.endpoints.user.profile.requestChanges.education,
      },
      experience: {
        normal: this.endpoints.user.profile.saveExperience,
        change: this.endpoints.user.profile.requestChanges.experience,
      },
      achievements: {
        normal: this.endpoints.user.profile.saveAchievements,
        change: this.endpoints.user.profile.requestChanges.achievements,
      },
      skills: {
        normal: this.endpoints.user.profile.saveSkills,
        change: this.endpoints.user.profile.requestChanges.skills,
      },
      languages: {
        normal: this.endpoints.user.profile.saveLanguages,
        change: this.endpoints.user.profile.requestChanges.languages,
      },
      attachments: {
        normal: this.endpoints.user.profile.saveReferences,
        change: this.endpoints.user.profile.requestChanges.references,
      },
    };

    const pair = map[section];
    return this.writeMode === 'change-request' ? pair.change : pair.normal;
  }

  // ================= FORM DATA BUILDER =================

  private fd() {
    return new FormDataBuilder();
  }

  // Push file into bucket and return its index; returns null when no file
  private fileIndex(bucket: File[], file: FileLike): number | null {
    if (!file) return null;
    bucket.push(file);
    return bucket.length - 1;
  }
}

class FormDataBuilder {
  private readonly _fd = new FormData();

  build(): FormData {
    return this._fd;
  }

  scalar(key: string, value: any): this {
    if (value === null || value === undefined || value === '') return this;
    this._fd.append(key, String(value));
    return this;
  }

  json(dto: any): this {
    Object.entries(dto ?? {}).forEach(([key, value]) => {
      if (value === null || value === undefined) return;

      const t = typeof value;
      if (t === 'string' || t === 'number' || t === 'boolean') {
        this._fd.append(key, String(value));
        return;
      }

      this._fd.append(key, JSON.stringify(value));
    });

    return this;
  }

  file(key: string, file: FileLike): this {
    if (!file) return this;
    this._fd.append(key, file);
    return this;
  }

  files(key: string, files: File[]): this {
    (files ?? []).forEach(f => this._fd.append(key, f));
    return this;
  }

  /** Use when you must preserve the exact append behavior (e.g., nested key + raw file append). */
  rawAppend(key: string, value: Blob | string): this {
    this._fd.append(key, value);
    return this;
  }
}
