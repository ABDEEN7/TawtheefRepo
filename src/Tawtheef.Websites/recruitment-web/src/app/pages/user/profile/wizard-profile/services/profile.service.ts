import { Injectable, inject } from '@angular/core';
import { of } from 'rxjs';

import { SaveProfilePrereqRequestModel } from '../models/save-profile-prereq-request.model';
import { SaveProfilePersonalRequestDto } from '../models/save-profile-personal-request.model';
import { SaveProfileContactRequestDto } from '../models/save-user-contact-request.model';
import { MoiPersonalInfo } from '../models/moi-personal-info.model';
import { Degree } from '../models/degree.model';
import { Experience, TrainingCourse } from '../models/experience.model';
import { Achievement } from '../models/achievement.model';
import { Skill } from '../models/skill.model';
import { Language } from '../models/language.model';
import { Attachment } from '../models/attachment.model';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {ProfileStatusDto} from '../../../../../core/models/auth/auth-response.model';
import {GUID} from '../../../../../shared/types/guid.type';

type FileLike = File | null | undefined;

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

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
    const fd = this.createFormData();
    this.fdAppendJsonObject(fd, dto);

    this.fdAppendFile(fd, 'cvFile', files.cvFile);
    this.fdAppendFile(fd, 'idFile', files.idFile);
    this.fdAppendFile(fd, 'birthCertificateFile', files.birthCertificateFile);
    this.fdAppendFile(fd, 'marriageCertificateFile', files.marriageCertificateFile);

    return this.http.post(this.endpoints.user.profile.savePrereq, fd);
  }

  // ========== PERSONAL ==========
  savePersonalSection(dto: SaveProfilePersonalRequestDto, files?: { sponsorCard?: FileLike }) {
    const fd = this.createFormData();
    this.fdAppendJsonObject(fd, dto);
    this.fdAppendFile(fd, 'sponsorCard', files?.sponsorCard);

    return this.http.post(this.endpoints.user.profile.savePersonal, fd);
  }

  checkProfile(qid: string, expiryDate: string) {
    return this.http.get<MoiPersonalInfo>(this.endpoints.user.profile.checkProfile, { qid, expiryDate });
  }

  // ========== CONTACT ==========
  saveContactSection(dto: SaveProfileContactRequestDto, files?: { nationalAddressFile?: FileLike }) {
    const fd = this.createFormData();

    // keep your exact binding keys for nested DTO
    this.fdAppendScalar(fd, 'submit', dto.submit);
    this.fdAppendScalar(fd, 'residenceCountryId', dto.residenceCountryId);
    this.fdAppendScalar(fd, 'interviewLocationId', dto.interviewLocationId);
    this.fdAppendScalar(fd, 'address', dto.address);

    if (dto.nationalAddress) {
      const na = dto.nationalAddress;
      this.fdAppendScalar(fd, 'nationalAddress.zone', na.zone);
      this.fdAppendScalar(fd, 'nationalAddress.street', na.street);
      this.fdAppendScalar(fd, 'nationalAddress.building', na.building);
      this.fdAppendScalar(fd, 'nationalAddress.unit', na.unit);
      this.fdAppendScalar(fd, 'nationalAddress.nationalAddressFileName', na.nationalAddressFileName);
    }

    // keep your original field name
    if (files?.nationalAddressFile) {
      fd.append('nationalAddress.nationalAddress', files.nationalAddressFile);
    }

    return this.http.post(this.endpoints.user.profile.saveContact, fd);
  }

  getProfileStatus() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.bootstrap);
  }

  // ========== EDUCATION (Degrees) ==========
  // Policy: no updates -> we still allow sending items with id if your backend uses it for delete/ignore,
  saveEducationSection(degrees: Degree[]) {
    const files: File[] = [];
    let cursor = 0;

    const payload = (degrees ?? [])
      .filter(d => !d.id)
      .map(d => {
        const fileIndex = d.file ? cursor++ : null;
        if (d.file) files.push(d.file);

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

    const fd = this.createFormData();
    this.fdAppendJsonObject(fd, { degreesJson: payload });

    this.fdAppendFiles(fd, 'DegreeFiles', files);
    return this.http.post(this.endpoints.user.profile.saveEducation, fd);
  }

  deleteEduction(degreeId: GUID) {
    return this.http.delete(this.endpoints.user.profile.deleteEducation(degreeId));
  }

  // ========== EXPERIENCE + TRAINING COURSES ==========
  // Policy: no updates -> ONLY new items (no id)
  saveExperienceSection(experiences: Experience[], courses: TrainingCourse[]) {
    const experienceFiles: File[] = [];
    const trainingFiles: File[] = [];

    const experiencesDto = (experiences ?? [])
      .filter(e => !e.id)
      .map(e => ({
        id: null,
        employerName: e.employerName,
        jobTitle: e.jobTitle,
        startDate: e.from,
        endDate: e.current ? null : e.to,
        countryId: e.country?.id,
        certificateId: e.attachmentId ?? null,
        certificateFileIndex: this.collectFileIndex(experienceFiles, e.file),
        description: e.description,
        qualificationId: e.qualificationId ?? null,
      }));

    const coursesDto = (courses ?? [])
      .filter(c => !c.id)
      .map(c => ({
        id: null,
        title: c.title,
        provider: c.provider,
        startDate: c.from,
        endDate: c.to,
        countryId: c.country?.id,
        description: c.description,
        certificateId: c.attachmentId ?? null,
        certificateFileIndex: this.collectFileIndex(trainingFiles, c.file),
      }));

    if (experiencesDto.length === 0 && coursesDto.length === 0) return of(null);

    const fd = this.createFormData();
    this.fdAppendJsonObject(fd, {
      submit: false,
      experiencesJson: experiencesDto,
      trainingCoursesJson: coursesDto,
    });

    this.fdAppendFiles(fd, 'ExperienceFiles', experienceFiles);
    this.fdAppendFiles(fd, 'TrainingCourseFiles', trainingFiles);

    return this.http.post(this.endpoints.user.profile.saveExperience, fd);
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
    const files: File[] = [];

    const payload = (achievements ?? [])
      .filter(a => !a.id)
      .map(a => ({
        id: null,
        achievementTypeId: a.achievementType?.id,
        title: a.title,
        issuingAuthority: a.issuingAuthority,
        countryId: a.country?.id,
        issueDate: a.issueDate,
        description: a.description,
        attachmentId: a.attachmentId ?? null,
        certificateFileIndex: this.collectFileIndex(files, a.file),
        relatedToSpecialization: a.relatedToSpecialization ?? null,
      }));

    const fd = this.createFormData();
    this.fdAppendJsonObject(fd, { submit: false, achievementsJson: payload });
    this.fdAppendFiles(fd, 'AchievementFiles', files);

    return this.http.post(this.endpoints.user.profile.saveAchievements, fd);
  }

  deleteAchievement(id: GUID) {
    return this.http.delete(this.endpoints.user.profile.deleteAchievement(id));
  }

  // ========== SKILLS ==========
  saveSkillsSection(skills: Skill[]) {
    // Keeping your flexible mapping (Skill may be object or id); no updates rule not relevant here.
    const dto = {
      submit: false,
      skills: (skills ?? []).map((s: any) => ({
        skillId: s.skillId ?? s.id ?? s,
        levelId: s.levelId ?? s.level?.id,
      })),
    };

    return this.http.post(this.endpoints.user.profile.saveSkills, dto);
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

    return this.http.post(this.endpoints.user.profile.saveLanguages, dto);
  }

  deleteLanguage(languageId: GUID) {
    return this.http.delete(this.endpoints.user.profile.deleteLanguage(languageId));
  }

  // ========== ATTACHMENTS ==========
  saveAttachmentsSection(attachments: Attachment[]) {
    const files: File[] = [];
    let cursor = 0;

    const payload = (attachments ?? []).map(a => {
      const item: any = {
        id: a.id ?? null,
        fileName: a.fileName ?? a.name,
        attachmentId: a.attachmentId ?? null,
      };

      if (a?.file) {
        item.fileIndex = cursor++;
        files.push(a.file);
      }

      return item;
    });

    const fd = this.createFormData();
    this.fdAppendScalar(fd, 'submit', false);
    this.fdAppendScalar(fd, 'attachmentsJson', JSON.stringify(payload));
    this.fdAppendFiles(fd, 'AttachmentFiles', files);

    return this.http.post(this.endpoints.user.profile.saveReferences, fd);
  }

  // ========== FINAL SUBMISSION ==========
  finalizeProfile() {
    return this.http.post(this.endpoints.user.profile.submit, {});
  }

  // ================= Helpers =================

  private createFormData(): FormData {
    return new FormData();
  }

  private fdAppendScalar(fd: FormData, key: string, value: any) {
    if (value === null || value === undefined || value === '') return;
    fd.append(key, String(value));
  }

  private fdAppendFile(fd: FormData, key: string, file: FileLike) {
    if (!file) return;
    fd.append(key, file);
  }

  private fdAppendFiles(fd: FormData, key: string, files: File[]) {
    (files ?? []).forEach(f => fd.append(key, f));
  }

  // Appends keys exactly as provided; objects/arrays are JSON-stringified.
  private fdAppendJsonObject(fd: FormData, dto: any) {
    Object.entries(dto ?? {}).forEach(([key, value]) => {
      if (value === null || value === undefined) return;

      const t = typeof value;
      if (t === 'string' || t === 'number' || t === 'boolean') {
        fd.append(key, String(value));
        return;
      }

      fd.append(key, JSON.stringify(value));
    });
  }

  // Push file into bucket and return its index; returns null when no file
  private collectFileIndex(bucket: File[], file: FileLike): number | null {
    if (!file) return null;
    bucket.push(file);
    return bucket.length - 1;
  }
}
