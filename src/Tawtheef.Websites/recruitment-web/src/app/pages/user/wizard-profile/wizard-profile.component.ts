import {Component, inject, OnInit} from '@angular/core';
import {DataService} from './services/data.service';
import {TranslateService} from '@ngx-translate/core';
import {LanguageService} from '../../../core/services/language.service';
import {ProfileState, UploadedFileRef} from './models/profile-state.model';
import {Router} from '@angular/router';
import {AuthService} from '../../../core/auth/auth.service';
import {take} from 'rxjs';
import {FileRefDto, PrefillData, ProfileStatusDto} from '../../../core/models/auth/auth-response.model';
import {routes} from '../../../routes/routes';
import {finalize} from 'rxjs/operators';
import {ProfileLookupsService} from './services/profile-lookups.service';
import {AvatarModal} from './steps/step-personal/dialogs/avatar.modal/avatar.modal';
import {DialogService} from 'primeng/dynamicdialog';
import {PhoneMapperService} from './services/phone-mapper.service';
import {HttpClient} from '@angular/common/http';
import {EndpointsService} from '../../../core/http/endpoints.service';
import {MessageService} from 'primeng/api';
import {dropdownOptionsModel} from '../../../shared/models/dropdown-options.model';

@Component({
  selector: 'app-wizard-profile',
  templateUrl: './wizard-profile.component.html',
  styleUrls: ['./wizard-profile.component.scss'],
  standalone: false,
})
export class WizardProfileComponent implements OnInit {
  private router = inject(Router);
  private auth = inject(AuthService);
  private dialog = inject(DialogService);
  private translate = inject(TranslateService);
  ds = inject(DataService);
  lookups = inject(ProfileLookupsService);
  language = inject(LanguageService);
  phoneMapper = inject(PhoneMapperService);

  avatarPreviewUrl: string | null = null;

  step = 1;
  total = 8;

  loading = true;

  private stepKeyMap: Record<number,
    keyof ReturnType<typeof this.ds.stepValidity>> = {
    1: 'basic',
    2: 'personal',
    3: 'contact',
    4: 'degrees',
    5: 'experience',
    6: 'skills',
    7: 'attachments',
  };

  isCurrentStepValid(): boolean {
    const validity = this.ds.stepValidity();
    const key = this.stepKeyMap[this.step];
    if (!key) return true;
    return validity[key];
  }

  ngOnInit(): void {
    this.lookups.loadAll().subscribe(() => {
      const nav = this.router.currentNavigation();
      const state = nav?.extras.state as ProfileStatusDto | null;
      if (state) {
        this.avatarPreviewUrl = state.avatar ?? null;
        this.ds.prefillFromBootstrap(this.mapProfileStatusToState(state));
        this.loading = false;
        return;
      }
      this.auth.getAuthBootstrap$()
        .pipe(take(1))
        .pipe(finalize(() => {
          this.loading = false;
        }))
        .subscribe((b: Partial<ProfileStatusDto>) => {
          if (b.isComplete) {
            this.router.navigate([routes.user.dashboard]);
            return;
          }
          this.avatarPreviewUrl = b.avatar ?? null;
          this.ds.prefillFromBootstrap(this.mapProfileStatusToState(b as ProfileStatusDto));
        });
    })

  }

  canGoTo(targetStep: number): boolean {
    // const v = this.ds.stepValidity();
    // const orderedSteps: (keyof typeof v)[] = [
    //   'personal',
    //   'contact',
    //   'degrees',
    //   'experience',
    //   'skills',
    //   'attachments'
    // ];

    // if (targetStep === 1) return true;
    // for (let i = 0; i < targetStep - 1 && i < orderedSteps.length; i++) {
    //   if (!v[orderedSteps[i]]) {
    //     return false;
    //   }
    // }

    return true;
  }

  go(step: number) {
    if (!this.canGoTo(step)) return;
    this.step = step;
  }

  next() {
    if (!this.isCurrentStepValid()) {
      return;
    }

    if (this.step < this.total) {
      this.step++;
    }
  }

  prev() {
    if (this.step > 1) {
      this.step--;
    }
  }

  openAvatarDialog() {
    this.dialog.open(AvatarModal, {
      header: this.translate.instant('wizard.personal.avatar.title'),
      width: '80%',
      contentStyle: {'max-height': '80vh', 'overflow': 'visible'},
      baseZIndex: 10000,
      closable: true,
    })?.onClose.subscribe((croppedImage: string | null) => {
      if (croppedImage) {
        this.ds.up('avatarUrl', croppedImage);
        this.avatarPreviewUrl = croppedImage;
      }
    });
  }

  // saveDraft() {
  //   this.savingDraft = true;
  //   const state = this.ds.state();
  //   const formData = buildProfileFormData(state, false);
  //   this.http.post(this.endpoints.user.profile.save, formData)
  //     .pipe(finalize(() => this.savingDraft = false))
  //     .subscribe({
  //       next: () => {
  //         this.messages.add({
  //           severity: 'success',
  //           summary: this.i18n.instant('wizard.review.savedDraftTitle'),
  //           detail: this.i18n.instant('wizard.review.savedDraft'),
  //         });
  //       },
  //       error: (err) => {
  //         console.error(err);
  //         this.messages.add({
  //           severity: 'error',
  //           summary: this.i18n.instant('common.error'),
  //           detail: this.i18n.instant('wizard.review.saveDraftFailed'),
  //         });
  //       }
  //     });
  // }
  mapProfileStatusToState(
    dto: ProfileStatusDto,
    prefill?: PrefillData | null
  ): ProfileState {
    return {
      // ----------- Prereq -----------
      candidateType: this.mapIdToDropdown('candidateType', dto.candidateTypeId) as dropdownOptionsModel,
      targetEntity: this.mapIdToDropdown('targetEntity', dto.targetEntityId) as dropdownOptionsModel,

      // Attachments
      cvFile: this.mapFile(dto.resumeAttachment),
      cvName: dto.resumeAttachment?.fileName ?? null,

      idFile: this.mapFile(dto.nationalCard),
      idName: dto.nationalCard?.fileName ?? null,

      birthCertificateFile: this.mapFile(dto.birthdayCertificate),
      birthCertificateName: dto.birthdayCertificate?.fileName ?? null,

      marriageCertificateFile: this.mapFile(dto.marriageCertificate),
      marriageCertificateName: dto.marriageCertificate?.fileName ?? null,

      // ----------- Personal -----------
      fullNameAr: dto.fullNameAr ?? prefill?.fullName ?? undefined,
      fullNameEn: dto.fullNameEn ?? undefined,

      qid: dto.nationalNumber ?? prefill?.qid ?? undefined,

      nationality: this.mapIdToDropdown('nationality', dto.nationalityId ?? prefill?.nationality ?? undefined),
      gender: this.mapIdToDropdown('gender', dto.genderId ?? prefill?.gender ?? undefined),
      religion: this.mapIdToDropdown('religion', dto.religionId ?? undefined),
      marital: this.mapIdToDropdown('marital', dto.maritalStatusId ?? undefined),
      sponsorType: this.mapIdToDropdown('sponsorType', dto.sponsorTypeId ?? undefined),

      children: dto.childrenCount,

      dob: dto.birthDate ?? prefill?.dob ?? undefined,

      hasDisability: dto.hasDisability,
      disabilityDetails: dto.disabilityDetails ?? null,

      sponsorEmployerName: dto.sponsorEmployerName,
      sponsorEmployerNumber: dto.sponsorEmployerNumber,
      sponsorCardName: dto.sponsorCard?.fileName ?? null,
      sponsorCardFile: this.mapFile(dto.sponsorCard),

      // ----------- Contact -----------
      country: this.mapIdToDropdown('countries', dto.residenceCountryId),
      dialCode: undefined,
      address: dto.address ?? undefined,

      phone: dto.phone ? this.phoneMapper.toPhoneObject(dto.phone) :
        prefill?.phone ? this.phoneMapper.toPhoneObject(prefill.phone) : null,

      phoneVerified: dto.phoneVerified ?? prefill?.phoneVerified ?? false,

      email: dto.email ?? prefill?.email ?? undefined,
      emailVerified: dto.emailVerified ?? prefill?.emailVerified ?? false,

      interviewPlace: this.mapIdToDropdown('interviewLocation', dto.interviewLocationId),

      // National Address
      naZone: dto.naZone,
      naStreet: dto.naStreet,
      naBuilding: dto.naBuilding,
      naUnit: dto.naUnit,

      naFileName: dto.residenceAddressCertificate?.fileName ?? null,
      naFile: this.mapFile(dto.residenceAddressCertificate),

      // ----------- Collections -----------
      degrees: (dto.qualifications ?? []).map(q => ({
        id: q.id,
        levelId: q.degreeId ?? '',
        level: q.degreeName ?? '',
        majorId: q.major ?? '',
        major: q.major ?? undefined,
        uniId: q.universityName ?? undefined,
        uni: q.universityName ?? undefined,
        graduationYear: q.graduationYear ?? undefined,
        attachment: this.mapFile(q.attachment),
      })),

      experiences: (dto.experiences ?? []).map(e => ({
        id: e.id,
        org: e.employerName ?? '',
        title: e.jobTitle ?? '',
        startDate: e.startDate ?? undefined,
        endDate: e.endDate ?? undefined,
        isCurrent: e.isCurrent,
        attachment: this.mapFile(e.attachment),
      })),

      courses: (dto.trainingCourses ?? []).map(t => ({
        id: t.id,
        title: t.title ?? '',
        org: t.provider ?? '',
        startDate: t.startDate ?? undefined,
        endDate: t.endDate ?? undefined,
        attachment: this.mapFile(t.attachment),
      })),

      achievements: [],

      skills: (dto.skills ?? []).map(s => s.skillId),

      languages: (dto.languages ?? []).map(l => ({
        id: l.id,
        langId: l.languageId,
        langName: this.mapIdToDropdown('language', l.languageId)?.name ?? '',
        levelId: l.levelId,
        levelName: this.mapIdToDropdown('languageLevel', l.levelId)?.name ?? '',
      })),

      attachments: (dto.additionalAttachments ?? []).map(a => ({
        id: a.id,
        name: a.title ?? '',
        // file: this.mapFile(a.file)!,
      })),

      // ----------- UI fields -----------
      available: true,
      avatarUrl: dto.avatar ?? prefill?.avatar ?? undefined,
    };
  }

  mapFile(ref?: FileRefDto | null): UploadedFileRef | null {
    if (!ref) return null;

    return {
      resourceId: ref.resourceId,
      resourceName: ref.fileName,
    };
  }

// Convert backend ID → dropdownOptionsModel
  mapIdToDropdown(kind: 'candidateType' | 'targetEntity' | 'countries' | 'language' | 'languageLevel' |
                    'nationality' | 'gender' | 'religion' | 'marital' | 'studyType' | 'degree' | 'university' | 'major' | 'ratingGrade' |
                    'interviewLocation' | 'residenceCountry' | 'graduationCountry' | 'sponsorType',
                  id?: string | null): dropdownOptionsModel | undefined {
    if (!id) return undefined;
    switch (kind) {
      case 'candidateType':
        return this.lookups.candidateTypes().find(ct => ct.id === id);
      case 'targetEntity':
        return this.lookups.targetEntities().find(te => te.id === id);
      case 'nationality':
        return this.lookups.nationalities().find(nat => nat.id === id);
      case 'gender':
        return this.lookups.genders().find(g => g.id === id);
      case 'religion':
        return this.lookups.religions().find(r => r.id === id);
      case 'marital':
        return this.lookups.maritalStatuses().find(m => m.id === id);
      case 'studyType':
        return this.lookups.studyTypes().find(st => st.id === id);
      case 'degree':
        return this.lookups.degrees().find(d => d.id === id);
      case 'university':
        return this.lookups.universities().find(u => u.id === id);
      case 'major':
        return this.lookups.majors().find(m => m.id === id);
      case 'ratingGrade':
        return this.lookups.ratingGrades().find(rg => rg.id === id);
      case 'language':
        return this.lookups.languages().find(l => l.id === id);
      case 'languageLevel':
        return this.lookups.languageLevels().find(ll => ll.id === id);
      case 'interviewLocation':
        return this.lookups.interviewLocation().find(il => il.id === id);
      case 'residenceCountry':
        return this.lookups.residenceCountry().find(rc => rc.id === id);
      case 'graduationCountry':
        return this.lookups.graduationCountry().find(gc => gc.id === id);
      case 'sponsorType':
        return this.lookups.sponsorTypes().find(st => st.id === id);
      case 'countries':
        return this.lookups.countries().find(c => c.id === id);
      default:
        throw Error(`Unknown dropdown: ${kind}`);
    }
  }
}
