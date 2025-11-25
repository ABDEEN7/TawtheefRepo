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

  uploadFile(file: File): Observable<UploadedFileRef > {
    const form = new FormData();
    form.append('file', file);

    return this.http.post<UploadedFileRef>(this.endpoints.user.profile.upload,form);
  }

  savePrereq(dto: SaveProfilePrereqRequestModel) {
    return this.http.post(this.endpoints.user.profile.savePrereq,dto);
  }
  savePersonalSection(dto: SaveProfilePersonalRequestDto) {
    return this.http.post(
      this.endpoints.user.profile.savePersonal,
      dto
    );
  }
  saveContactSection(dto: SaveProfileContactRequestDto) {
    return this.http.post(this.endpoints.user.profile.saveContact, dto);
  }
}
