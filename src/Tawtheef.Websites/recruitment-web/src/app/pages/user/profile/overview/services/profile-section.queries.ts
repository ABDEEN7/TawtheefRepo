import { inject, Injectable } from '@angular/core';
import { shareReplay } from 'rxjs/operators';
import { ProfileService } from '../../wizard-profile/services/profile.service';
import { ProfileSectionEnum } from '../models/profile-overview.model';
import { ProfileStatusDto } from '../../../../../core/models/auth/auth-response.model';
import { Observable, defer } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ProfileSectionQueriesService {
  private readonly profileService = inject(ProfileService);
  private readonly basics$ = defer(() => this.profileService.getProfileBasics()).pipe(shareReplay(1));
  private readonly sectionCache = new Map<ProfileSectionEnum, Observable<ProfileStatusDto>>();
  private readonly sectionLoaders: Record<ProfileSectionEnum, () => Observable<ProfileStatusDto>> = {
    [ProfileSectionEnum.Prerequisites]: () => this.profileService.getPrereqSection(),
    [ProfileSectionEnum.Personal]: () => this.profileService.getPersonalSection(),
    [ProfileSectionEnum.Contact]: () => this.profileService.getContactSection(),
    [ProfileSectionEnum.Qualifications]: () => this.profileService.getQualificationsSection(),
    [ProfileSectionEnum.Experience]: () => this.profileService.getExperienceSection(),
    [ProfileSectionEnum.TrainingCourses]: () => this.profileService.getExperienceSection(),
    [ProfileSectionEnum.CertificatesAndAwards]: () => this.profileService.getAchievementsSection(),
    [ProfileSectionEnum.Skills]: () => this.profileService.getSkillsSection(),
    [ProfileSectionEnum.Languages]: () => this.profileService.getLanguagesSection(),
    [ProfileSectionEnum.Attachments]: () => this.profileService.getAttachmentsSection()
  };

  loadBasics(): Observable<ProfileStatusDto> {
    return this.basics$;
  }

  loadSection(section: ProfileSectionEnum): Observable<ProfileStatusDto> {
    if (!this.sectionCache.has(section)) {
      const loader = this.sectionLoaders[section];
      this.sectionCache.set(section, defer(() => loader()).pipe(shareReplay(1)));
    }

    return this.sectionCache.get(section)!;
  }
}
