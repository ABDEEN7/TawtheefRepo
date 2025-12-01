import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {EndpointsService} from '../../../../core/http/endpoints.service';
import {SaveProfilePrereqRequestModel} from '../models/save-profile-prereq-request.model';
import {SaveProfilePersonalRequestDto} from '../models/save-profile-personal-request.model';
import {SaveProfileContactRequestDto} from '../models/save-user-contact-request.model';
import {GUID} from '../../../../shared/types/guid.type';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private http = inject(HttpClient);
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
    files?: { sponsorCardFile?: File | null }
  ) {
    const formData = this.buildFormData(dto, {
      sponsorCardFile: files?.sponsorCardFile ?? null
    });

    return this.http.post(this.endpoints.user.profile.savePersonal, formData);
  }

  // ========== CONTACT ==========
  saveContactSection(
    dto: SaveProfileContactRequestDto,
    files?: { nationalAddressFile?: File | null }
  ) {
    const formData = this.buildFormData(dto, {
      nationalAddressFile: files?.nationalAddressFile ?? null
    });

    return this.http.post(this.endpoints.user.profile.saveContact, formData);
  }

  // ========== Degrees ==========
  saveEducationSection(degrees: any[]) {
    const newDegrees = degrees.filter(d=> !d.attachmentId);
    const dto = {
      degreesJson: newDegrees.filter(d=> !d.attachmentId).map(d => ({
        degreeId: d.degree.id,
        gradCountryId: d.gradCountry.id,
        universityId: d.university.id,
        majorId: d.major.id,
        subMajorId: d.subMajor.id,
        gradYear: d.gradYear,
        studyTypeId: d.studySystem.id,
        gpa: d.gpa,
        gradeId: d.grade.id,
        fileName: d.fileName
      })),
    };
    const formData = this.buildFormData(dto);
    newDegrees.forEach(d => {
      if (d.file) {
        formData.append('DegreeFiles', d.file);
      }
    });
    return this.http.post(this.endpoints.user.profile.saveEducation, formData);
  }
  deleteEduction(degreeId: GUID){
    return this.http.delete(this.endpoints.user.profile.deleteEducation(degreeId));
  }

  // ========== EXPERIENCE ==========
  saveExperienceSection(experiences: any[], courses: any[]) {
    const experienceFiles: (File | null | undefined)[] = [];
    const experiencesDto = (experiences ?? []).map(e => {
      const fileIndex = e.file ? experienceFiles.push(e.file) - 1 : null;

      return {
        id: e.id ?? null,
        organization: e.org,
        position: e.title,
        startDate: e.from,
        endDate: e.current ? null : e.to,
        certificateId: e.attachmentId ?? null,
        certificateFileIndex: fileIndex,
        achievements: e.tasks,
      };
    });

    const trainingCourseFiles: (File | null | undefined)[] = [];
    const coursesDto = (courses ?? []).map(c => {
      const fileIndex = c.file ? trainingCourseFiles.push(c.file) - 1 : null;

      return {
        id: c.id ?? null,
        organization: c.org,
        position: c.title,
        startDate: c.from,
        endDate: c.to,
        certificateId: c.attachmentId ?? null,
        certificateFileIndex: fileIndex,
      };
    });

    const formData = this.buildFormData({
      submit: false,
      experiencesJson: experiencesDto,
      trainingCoursesJson: coursesDto,
      achievements: [],
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

  // ========== SKILLS ==========
  saveSkillsSection(skills: any[], languages: any[]) {
    const dto = {
      submit: false,
      skills: (skills ?? []).map(s => ({
        skillId: s.skillId ?? s.id ?? s,
        levelId: s.levelId ?? s.level?.id,
      })),
      languages: (languages ?? []).map(l => ({
        languageId: l.langId ?? l.languageId ?? l.id ?? l,
        levelId: l.levelId ?? l.level?.id ?? l.level,
      })),
    };

    return this.http.post(this.endpoints.user.profile.saveSkills, dto);
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
