import {
  Component,
  EventEmitter,
  inject,
  Input,
  OnInit,
  Output,
  computed,
  input,
  output,
  ChangeDetectionStrategy,
  signal
} from '@angular/core';
import { CommonModule, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { FaDirArrowDirective } from '../../../../../../shared/directives/dir-arrow.directive';
import { ProfileDataService } from '../../../wizard-profile/services/profile-data.service';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { DegreeModal } from './dialogs/degree.modal/degree.modal';
import { Degree } from '../../../wizard-profile/models/degree.model';
import { finalize, switchMap } from 'rxjs/operators';
import { ProfileState } from '../../../wizard-profile/models/profile-state.model';

@Component({
  selector: 'app-step-degrees',
  templateUrl: './step-degree.component.html',
  styleUrl: './step-degree.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    FaDirArrowDirective
  ]
})
export class StepDegreeComponent implements OnInit {
  back = output<void>();
  next = output<void>();
  submitLabelKey = input<string>('wizard.buttons.next');
  showBack = input<boolean>(true);
  requireChanges = input<boolean>(false);

  ds = inject(ProfileDataService);
  dialog = inject(DialogService);
  translate = inject(TranslateService);
  profile = inject(ProfileService);
  notify = inject(NotificationService);
  fileUtils = inject(FileUtilsService);

  savingDegrees = signal(false);
  private lastSubmittedSignature: string | null = null;

  step = computed(() => this.ds.stepValidationDetailed().degrees);

  ngOnInit(): void {
    this.lastSubmittedSignature = null;
  }

  add() {
    this.dialog.open(DegreeModal, {
      header: this.translate.instant('wizard.degrees.add'),
      width: '80%',
      contentStyle: { 'max-height': '80vh', 'overflow': 'auto' },
      baseZIndex: 10000,
      closable: true,
      draggable: false,
    })?.onClose.subscribe((e: Degree) => {
      if (e) {
        this.ds.addDegree(e);
        this.ds.state().degrees = [...this.ds.state().degrees.sort((a, b) => a.gradYear - b.gradYear)];
      }
    });
  }

  edit(index: number) {
    const degree = this.ds.state().degrees[index];
    this.dialog.open(DegreeModal, {
      header: this.translate.instant('wizard.degrees.edit'),
      width: '80%',
      contentStyle: { 'max-height': '80vh', 'overflow': 'auto' },
      baseZIndex: 10000,
      closable: true,
      draggable: false,
      data: { initialValue: degree },
    })?.onClose.subscribe((result: Degree | null) => {
      if (result) {
        this.ds.updateDegree(index, result);
        const sortedDegrees = [...this.ds.state().degrees].sort((a, b) => a.gradYear - b.gradYear);
        this.ds.patch({ degrees: sortedDegrees } as Partial<ProfileState>);
      }
    });
  }

  del(i: number) {
    const degree = this.ds.state().degrees[i];
    if (degree?.id && this.ds.state().experiences?.some(exp => exp.qualificationId === degree.id)) {
      this.notify.warn(
        `${this.translate.instant('wizard.warningTitle')}: ${this.translate.instant('wizard.degrees.deleteLinkedError')}`,
      );
      return;
    }
    if (degree?.id) {
      this.profile.deleteEducation(degree.id).subscribe({
        next: () => {
          this.ds.delDegree(i);
        }
      });
    } else {
      this.ds.delDegree(i);
    }
  }

  preview(r: Degree, ev?: Event) {
    ev?.stopPropagation();
    const local = r.file ?? r.certificate?.file ?? null;
    if (local) {
      this.fileUtils.previewBlob(local);
      return;
    }

    if (r.certificate?.url) {
      this.fileUtils.previewUrl(r.certificate.url, r.certificate.resourceName || '', false);
    } else if (r.certificate?.file) {
      this.fileUtils.previewBlob(r.certificate!.file);
    }
  }

  // ====== NEW: submit to API ======
  onNext() {
    if (!this.step().valid) {
      this.notify.error(
        `${this.translate.instant('wizard.validationErrorTitle')}: ${this.step().errors
          .map(e => `* ${this.translate.instant(e.i18nKey)}`)
          .join('\n')}`,
      );
      return;
    }

    const state = this.ds.state();
    const degrees = state.degrees || [];
    const signature = this.buildSignature(degrees);

    if (signature && signature === this.lastSubmittedSignature) {
      if (this.requireChanges()) {
        this.notify.error(this.translate.instant('profileView.notifications.noChanges'));
        return;
      }
      this.notify.info(this.translate.instant('profileView.notifications.noChanges'));
      this.next.emit();
      return;
    }

    if (!degrees.length) {
      this.notify.error(`${this.translate.instant('wizard.degrees.validation.noRows')}`,
        `${this.translate.instant('wizard.validationErrorTitle')}`);
      return;
    }

    this.savingDegrees.set(true);
    this.profile
      .saveEducationSection(degrees)
      .pipe(
        switchMap(() => this.ds.refreshDegreesFromBackend()),
        finalize(() => {
          this.savingDegrees.set(false);
        })
      )
      .subscribe({
        next: () => {
          this.lastSubmittedSignature = signature;
          this.ds.markStepSubmitted('degrees');
          if (this.profile.isChangeRequestMode()) {
            this.notify.success(this.translate.instant('profileView.notifications.changeRequestSent'));
          }
          this.next.emit();
        }
      });
  }

  private buildSignature(degrees: Degree[]): string {
    return JSON.stringify(
      (degrees ?? []).map(d => ({
        id: d.id ?? null,
        degreeId: d.degree?.id ?? null,
        gradCountryId: d.gradCountry?.id ?? null,
        universityId: d.university?.id ?? null,
        majorId: d.major?.id ?? null,
        subMajorId: d.subMajor?.id ?? null,
        gradYear: d.gradYear ?? null,
        studySystemId: d.studySystem?.id ?? null,
        gpa: d.gpa ?? null,
        gradeId: d.grade?.id ?? null,
        attachmentId: d.attachmentId ?? null,
        fileName: d.file?.name ?? d.certificate?.resourceName ?? null,
      }))
    );
  }
}
