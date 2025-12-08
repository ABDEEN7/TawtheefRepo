import { Injectable, inject } from '@angular/core';
import {HttpParams} from '@angular/common/http';
import {EndpointsService} from '../../../../core/http/endpoints.service';
import {SaveProfilePrereqRequestModel} from '../models/save-profile-prereq-request.model';
import {SaveProfilePersonalRequestDto} from '../models/save-profile-personal-request.model';
import {SaveProfileContactRequestDto} from '../models/save-user-contact-request.model';
import {GUID} from '../../../../shared/types/guid.type';
import {HttpService} from '../../../../core/http/http.service';
import {Experience, TrainingCourse} from '../models/experience.model';
import {Achievement} from '../models/achievement.model';
import {of} from 'rxjs';
import {MoiPersonalInfo} from '../models/moi-personal-info.model';
import {ProfileStatusDto} from '../../../../core/models/auth/auth-response.model';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

// ========== PREREQ ==========
  savePrereq(
    dto: SaveProfilePrereqRequestModel,
    files: {
      cvFile?: File | null;
      idFile?: File | null;
      birthCertificateFile?: File | null;
      marriageCertificateFile?: File | null;
    }
  ) {
    const formData = this.buildFormData(dto, {
      cvFile: files.cvFile,
      idFile: files.idFile,
      birthCertificateFile: files.birthCertificateFile,
      marriageCertificateFile: files.marriageCertificateFile
    });

    return this.http.post(this.endpoints.user.profile.savePrereq, formData);
  }

  // ========== PERSONAL ==========
  savePersonalSection(
    dto: SaveProfilePersonalRequestDto,
    files?: { sponsorCard?: File | null }
  ) {
    const formData = this.buildFormData(dto, {
      sponsorCard: files?.sponsorCard ?? null
    });

    return this.http.post(this.endpoints.user.profile.savePersonal, formData);
  }

  checkProfile(qid: string, expiryDate: string) {
    return this.http.get<MoiPersonalInfo>(this.endpoints.user.profile.checkProfile, { qid, expiryDate });
  }

  // ========== CONTACT ==========
  saveContactSection(
    dto: SaveProfileContactRequestDto,
    files?: { nationalAddressFile?: File | null }
  ) {
    const formData = new FormData();

    if (dto.submit !== null && dto.submit !== undefined) {
      formData.append('submit', String(dto.submit));
    }
    if (dto.residenceCountryId) {
      formData.append('residenceCountryId', dto.residenceCountryId);
    }
    if (dto.interviewLocationId) {
      formData.append('interviewLocationId', dto.interviewLocationId);
    }
    if (dto.address) {
      formData.append('address', dto.address);
    }

    if (dto.nationalAddress) {
      const na = dto.nationalAddress;
      if (na.zone !== null && na.zone !== undefined) {
        formData.append('nationalAddress.zone', String(na.zone));
      }
      if (na.street !== null && na.street !== undefined) {
        formData.append('nationalAddress.street', String(na.street));
      }
      if (na.building !== null && na.building !== undefined) {
        formData.append('nationalAddress.building', String(na.building));
      }
      if (na.unit !== null && na.unit !== undefined) {
        formData.append('nationalAddress.unit', String(na.unit));
      }
      if (na.nationalAddressFileName) {
        formData.append('nationalAddress.nationalAddressFileName', na.nationalAddressFileName);
      }
    }

    if (files?.nationalAddressFile) {
      formData.append('nationalAddress.nationalAddress', files.nationalAddressFile);
    }

    return this.http.post(this.endpoints.user.profile.saveContact, formData);
  }

  getProfileStatus() {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.bootstrap);
  }

  // ========== Degrees ==========
  saveEducationSection(degrees: any[]) {
    let fileCursor = 0;
    const degreeFiles: (File | null | undefined)[] = [];

    const payload = (degrees ?? []).map(d => {
      const fileIndex = d.file ? fileCursor++ : null;
      if (d.file) {
        degreeFiles.push(d.file);
      }

      return {
        id: d.id ?? null,
        degreeId: d.degree.id,
        gradCountryId: d.gradCountry.id,
        universityId: d.university.id,
        majorId: d.major.id,
        subMajorId: d.subMajor.id,
        gradYear: d.gradYear,
        studyTypeId: d.studySystem.id,
        gpa: d.gpa,
        gradeId: d.grade.id,
        certificateId: d.attachmentId ?? null,
        fileIndex,
        existingFileName: d.certificate?.resourceName ?? d.certificateName ?? d.fileName ?? null,
      };
    });

    const formData = this.buildFormData({ degreesJson: payload });
    degreeFiles.forEach(f => {
      if (f) {
        formData.append('DegreeFiles', f);
      }
    });
    return this.http.post(this.endpoints.user.profile.saveEducation, formData);
  }
  deleteEduction(degreeId: GUID){
    return this.http.delete(this.endpoints.user.profile.deleteEducation(degreeId));
  }

  // ========== EXPERIENCE ==========
  saveExperienceSection(experiences: Experience[], courses: TrainingCourse[]) {
    const experienceFiles: (File | null | undefined)[] = [];
    const experiencesDto = (experiences ?? []).filter(e=> !e.id).map(e => {
      const fileIndex = e.file ? experienceFiles.push(e.file) - 1 : null;

      return {
        id: e.id ?? null,
        employerName: e.employerName,
        jobTitle: e.jobTitle,
        startDate: e.from,
        endDate: e.current ? null : e.to,
        countryId: e.country?.id,
        certificateId: e.attachmentId ?? null,
        certificateFileIndex: fileIndex,
        description: e.description,
        qualificationId: e.qualificationId ?? null,
      };
    });
    const trainingCourseFiles: (File | null | undefined)[] = [];
    const coursesDto = (courses ?? []).filter(e=> !e.id).map(c => {
      const fileIndex = c.file ? trainingCourseFiles.push(c.file) - 1 : null;

      return {
        id: c.id ?? null,
        title: c.title,
        provider: c.provider,
        startDate: c.from,
        endDate: c.to,
        countryId: c.country?.id,
        description: c.description,
        certificateId: c.attachmentId ?? null,
        certificateFileIndex: fileIndex,
      };
    });

    if(experiencesDto.length === 0 && coursesDto.length === 0)
      return of(null);

    const formData = this.buildFormData({
      submit: false,
      experiencesJson: experiencesDto,
      trainingCoursesJson: coursesDto
    });
    experienceFiles.forEach(f => {
      if (f) {
        formData.append('ExperienceFiles', f);
      }
    });
    trainingCourseFiles.forEach(f => {
      if (f) {
        formData.append('TrainingCourseFiles', f);
      }
    });

    return this.http.post(this.endpoints.user.profile.saveExperience, formData);
  }
  deleteExperience(experienceId: GUID){
    return this.http.delete(this.endpoints.user.profile.deleteExperience(experienceId));
  }
  deleteTrainingCourse(courseId: GUID){
    return this.http.delete(this.endpoints.user.profile.deleteTrainingCourse(courseId));
  }

  saveAchievementsSection(achievements: Achievement[]) {
    const files: (File | null | undefined)[] = [];
    const payload = (achievements ?? []).filter(e=> !e.id).map(a => {
      const fileIndex = a.file ? files.push(a.file) - 1 : null;
      return {
        id: a.id ?? null,
        achievementTypeId: a.achievementType?.id,
        title: a.title,
        issuingAuthority: a.issuingAuthority,
        countryId: a.country?.id,
        issueDate: a.issueDate,
        description: a.description,
        attachmentId: a.attachmentId ?? null,
        certificateFileIndex: fileIndex,
        relatedToSpecialization: a.relatedToSpecialization ?? null,
      };
    });

    const formData = this.buildFormData({
      submit: false,
      achievementsJson: payload,
    });

    files.forEach(f => {
      if (f) {
        formData.append('AchievementFiles', f);
      }
    });

    return this.http.post(this.endpoints.user.profile.saveAchievements, formData);
  }

  deleteAchievement(id: GUID){
    return this.http.delete(this.endpoints.user.profile.deleteAchievement(id));
  }

  // ========== SKILLS & LANGUAGES ==========
  saveSkillsSection(skills: any[]) {
    const dto = {
      submit: false,
      skills: (skills ?? []).map(s => ({
        skillId: s.skillId ?? s.id ?? s,
        levelId: s.levelId ?? s.level?.id,
      })),
    };

    return this.http.post(this.endpoints.user.profile.saveSkills, dto);
  }
  deleteSkill(skillId: GUID){
    return this.http.delete(this.endpoints.user.profile.deleteSkill(skillId));
  }
  saveLanguagesSection(languages: any[]) {
    const dto = {
      submit: false,
      languages: (languages ?? []).map(l => ({
        languageId: l.langId ?? l.languageId ?? l.id ?? l,
        speakingLevelId: l.speakingLevelId ?? l.speakingLevel?.id ?? l.levelId ?? l.level?.id ?? l.level,
        writingLevelId: l.writingLevelId ?? l.writingLevel?.id ?? l.levelId ?? l.level?.id ?? l.level,
        readingLevelId: l.readingLevelId ?? l.readingLevel?.id ?? l.levelId ?? l.level?.id ?? l.level,
      })),
    };

    return this.http.post(this.endpoints.user.profile.saveLanguages, dto);
  }
  deleteLanguage(languageId: GUID){
    return this.http.delete(this.endpoints.user.profile.deleteLanguage(languageId));
  }

  // ========== ATTACHMENTS ==========
  saveAttachmentsSection(attachments: any[]) {
    let fileCursor = 0;
    const files: (File | null | undefined)[] = [];

    const payload = (attachments ?? []).map(a => {
      const item: any = {
        id: a.id ?? null,
        fileName: a.fileName ?? a.name,
        attachmentId: a.attachmentId ?? null,
      };

      if (a?.file) {
        item.fileIndex = fileCursor;
        files[fileCursor] = a.file;
        fileCursor += 1;
      }

      return item;
    });

    const formData = new FormData();
    formData.append('Submit', 'false');
    formData.append('AttachmentsJson', JSON.stringify(payload));
    files.forEach(f => {
      if (f) {
        formData.append('AttachmentFiles', f);
      }
    });

    return this.http.post(this.endpoints.user.profile.saveReferences, formData);
  }

  // ========== FINAL SUBMISSION ==========
  finalizeProfile() {
    return this.http.post(this.endpoints.user.profile.submit, {});
  }

  private buildFormData(dto: any, files?: Record<string, File | null | undefined>): FormData {
    const formData = new FormData();
    Object.entries(dto ?? {}).forEach(([key, value]) => {
      if (value === null || value === undefined) return;
      if (Array.isArray(value) || typeof value === 'object') {
        formData.append(key, JSON.stringify(value));
      } else {
        formData.append(key, value as any);
      }
    });
    Object.entries(files ?? {}).forEach(([key, file]) => {
      if (file) {
        formData.append(key, file);
      }
    });
    return formData;
  }
}
