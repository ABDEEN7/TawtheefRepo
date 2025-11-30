import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {UploadedFileRef} from '../models/profile-state.model';
import {EndpointsService} from '../../../../core/http/endpoints.service';
import {SaveProfilePrereqRequestModel} from '../models/save-profile-prereq-request.model';
import {SaveProfilePersonalRequestDto} from '../models/save-profile-personal-request.model';
import {SaveProfileContactRequestDto} from '../models/save-user-contact-request.model';

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

    const files: Record<string, File | null> = {};
    newDegrees.forEach((d, index) => {
      if (d.file) {
        files[`DegreeFiles${index}`] = d.file as File;
      }
    });

    const formData = this.buildFormData(dto, files);
    return this.http.post(this.endpoints.user.profile.saveEducation, formData);
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
