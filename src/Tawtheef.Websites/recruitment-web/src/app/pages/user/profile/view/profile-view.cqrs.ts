import { inject, Injectable } from '@angular/core';
import { defer, Observable } from 'rxjs';
import { shareReplay } from 'rxjs/operators';

import { HttpService } from '../../../../core/http/http.service';
import { EndpointsService } from '../../../../core/http/endpoints.service';
import { ProfileStatusDto } from '../../../../core/models/auth/auth-response.model';
import { ProfileSectionEnum } from './models/profile-overview.model';

@Injectable({ providedIn: 'root' })
export class ProfileViewActionApi {
  private readonly http = inject(HttpService);
  private readonly endpoints = inject(EndpointsService);

  basics(): Observable<ProfileStatusDto> {
    return this.http.get<ProfileStatusDto>(this.endpoints.user.profile.basics);
  }

  section(section: ProfileSectionEnum): Observable<ProfileStatusDto> {
    return this.http.get<ProfileStatusDto>(this.sectionUrl(section));
  }

  private sectionUrl(section: ProfileSectionEnum): string {
    switch (section) {
      case ProfileSectionEnum.Prerequisites:
        return this.endpoints.user.profile.sections.prereq;
      case ProfileSectionEnum.Personal:
        return this.endpoints.user.profile.sections.personal;
      case ProfileSectionEnum.Contact:
        return this.endpoints.user.profile.sections.contact;
      case ProfileSectionEnum.Qualifications:
        return this.endpoints.user.profile.sections.education;
      case ProfileSectionEnum.Experience:
        return this.endpoints.user.profile.sections.experience;
      case ProfileSectionEnum.TrainingCourses:
        return this.endpoints.user.profile.sections.training;
      case ProfileSectionEnum.CertificatesAndAwards:
        return this.endpoints.user.profile.sections.achievements;
      case ProfileSectionEnum.Skills:
        return this.endpoints.user.profile.sections.skills;
      case ProfileSectionEnum.Languages:
        return this.endpoints.user.profile.sections.languages;
      case ProfileSectionEnum.Attachments:
      default:
        return this.endpoints.user.profile.sections.attachments;
    }
  }
}

@Injectable({ providedIn: 'root' })
export class ProfileViewCqrs {
  private readonly api = inject(ProfileViewActionApi);
  private readonly basics$ = defer(() => this.api.basics()).pipe(shareReplay(1));
  private readonly sectionCache = new Map<ProfileSectionEnum, Observable<ProfileStatusDto>>();

  basics(): Observable<ProfileStatusDto> {
    return this.basics$;
  }

  section(section: ProfileSectionEnum): Observable<ProfileStatusDto> {
    if (!this.sectionCache.has(section)) {
      this.sectionCache.set(section, defer(() => this.api.section(section)).pipe(shareReplay(1)));
    }

    return this.sectionCache.get(section)!;
  }
}
