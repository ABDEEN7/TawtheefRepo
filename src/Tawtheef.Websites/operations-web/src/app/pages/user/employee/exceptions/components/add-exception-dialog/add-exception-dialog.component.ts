import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputTextModule } from 'primeng/inputtext';
import { Textarea } from 'primeng/textarea';
import { Observable, finalize, map, of, takeUntil } from 'rxjs';

import { ProfileStatusNumber } from '../../../../../../core/enums/lookups.enum';
import { LanguageService } from '../../../../../../core/services/language.service';
import { NotificationService } from '../../../../../../core/services/notification.service';
import {
  RemoteSelectComponent,
  RemoteSelectLoadRequest,
} from '../../../../../../shared/components/remote-select/remote-select';
import { I18nNamespaceDirective } from '../../../../../../shared/directives/i18n-namespace.directive';
import { GUID } from '../../../../../../shared/types/guid.type';
import { ExceptionCandidateLookup } from '../../models/invitation-exception.models';
import { CreateInvitationExceptionRequest } from '../../models/invitation-exception.requests';
import { ExceptionsService } from '../../services/exceptions.service';

@Component({
  selector: 'app-add-exception-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    TranslatePipe,
    InputTextModule,
    Textarea,
    RemoteSelectComponent,
    I18nNamespaceDirective,
  ],
  templateUrl: './add-exception-dialog.component.html',
  styleUrl: './add-exception-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AddExceptionDialogComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly exceptionsService = inject(ExceptionsService);
  private readonly dialogRef = inject(DynamicDialogRef);
  private readonly destroyRef = inject(DestroyRef);
  private readonly languageService = inject(LanguageService);
  private readonly notificationService = inject(NotificationService);
  private readonly translate = inject(TranslateService);

  protected readonly form = this.formBuilder.group({
    qid: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.pattern(/^\d{11}$/),
    ]),
    managementId: new FormControl<GUID | null>(null, Validators.required),
    departmentId: new FormControl<GUID | null>(null),
    jobId: new FormControl<GUID | null>(null, Validators.required),
    reason: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.pattern(/\S/),
      Validators.maxLength(2000),
    ]),
    proof: new FormControl<File | null>(null, Validators.required),
  });

  protected readonly candidate = signal<ExceptionCandidateLookup | null>(null);
  protected readonly lookupLoading = signal(false);
  protected readonly createLoading = signal(false);
  protected readonly proofDragging = signal(false);

  protected readonly approvedProfileStatus = ProfileStatusNumber.Approved;
  protected readonly isRtl = this.languageService.isRtl;
  protected readonly reasonMaxLength = 2000;

  protected jobDependencyKey(): string | null {
    const managementId = this.form.controls.managementId.value;
    if (managementId === null) {
      return null;
    }

    return `${managementId}:${this.form.controls.departmentId.value ?? ''}`;
  }

  protected readonly loadManagements = (request: RemoteSelectLoadRequest): Observable<readonly object[]> =>
    this.exceptionsService
      .getManagements({
        search: request.searchTerm || undefined,
        pageNumber: request.pageNumber,
        pageSize: request.pageSize,
      })
      .pipe(map((response) => response.items));

  protected readonly loadDepartments = (request: RemoteSelectLoadRequest): Observable<readonly object[]> => {
    const managementId = this.form.controls.managementId.value;
    if (managementId === null) {
      return of([]);
    }

    return this.exceptionsService
      .getDepartments({
        managementId,
        search: request.searchTerm || undefined,
        pageNumber: request.pageNumber,
        pageSize: request.pageSize,
      })
      .pipe(map((response) => response.items));
  };

  protected readonly loadJobs = (request: RemoteSelectLoadRequest): Observable<readonly object[]> => {
    const managementId = this.form.controls.managementId.value;
    if (managementId === null) {
      return of([]);
    }

    return this.exceptionsService
      .getJobs({
        managementId,
        departmentId: this.form.controls.departmentId.value ?? undefined,
        searchTerm: request.searchTerm || undefined,
        pageNumber: request.pageNumber,
        pageSize: request.pageSize,
      })
      .pipe(map((response) => response.items));
  };

  private readonly resolvedQid = signal<string | null>(null);

  constructor() {
    this.form.controls.qid.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((value) => {
        const resolvedQid = this.resolvedQid();

        if (resolvedQid !== null && value.trim() !== resolvedQid) {
          this.clearCandidate();
          this.resetCreationFields();
        }
      });

    this.form.controls.managementId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.form.controls.departmentId.reset();
        this.form.controls.jobId.reset();
      });

    this.form.controls.departmentId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.form.controls.jobId.reset());
  }

  protected lookupCandidate(): void {
    const qidControl = this.form.controls.qid;
    const qid = qidControl.value.trim();

    qidControl.markAsTouched();

    if (qidControl.invalid || this.lookupLoading() || this.createLoading()) {
      return;
    }

    qidControl.setValue(qid, { emitEvent: false });

    this.clearCandidate();
    this.lookupLoading.set(true);

    this.exceptionsService
      .getCandidateByQid(qid)
      .pipe(
        takeUntil(qidControl.valueChanges),
        finalize(() => this.lookupLoading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (candidate) => {
          if (qidControl.value.trim() !== qid) {
            return;
          }

          this.candidate.set(candidate);
          this.resolvedQid.set(qid);
        },
        error: () => this.clearCandidate(),
      });
  }

  protected selectProof(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;

    this.setProof(file, input);
  }

  protected onProofDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();

    if (this.createLoading()) {
      return;
    }

    this.proofDragging.set(true);
  }

  protected onProofDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();

    this.proofDragging.set(false);
  }

  protected onProofDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();

    this.proofDragging.set(false);

    if (this.createLoading()) {
      return;
    }

    const file = event.dataTransfer?.files?.[0] ?? null;

    this.setProof(file);
  }

  protected getFileSize(size: number): string {
    if (size < 1024) {
      return `${size} B`;
    }

    if (size < 1024 * 1024) {
      return `${(size / 1024).toFixed(1)} KB`;
    }

    return `${(size / (1024 * 1024)).toFixed(1)} MB`;
  }

  private setProof(file: File | null, input?: HTMLInputElement): void {
    const proofControl = this.form.controls.proof;

    if (file === null) {
      proofControl.setValue(null);
      proofControl.markAsTouched();
      return;
    }

    if (!file.name.toLowerCase().endsWith('.pdf')) {
      proofControl.setValue(null);
      proofControl.setErrors({ invalidPdf: true });
      proofControl.markAsTouched();

      if (input) {
        input.value = '';
      }

      return;
    }

    proofControl.setValue(file);
    proofControl.markAsTouched();
  }

  protected removeProof(input: HTMLInputElement): void {
    input.value = '';
    this.form.controls.proof.reset();
  }

  protected canCreate(): boolean {
    const qid = this.form.controls.qid.value.trim();

    return (
      this.form.valid &&
      this.candidate() !== null &&
      this.resolvedQid() === qid &&
      !this.createLoading()
    );
  }

  protected createException(): void {
    this.form.markAllAsTouched();

    if (!this.canCreate()) {
      return;
    }

    const value = this.form.getRawValue();
    const qid = value.qid.trim();
    const reason = value.reason.trim();

    if (value.jobId === null || value.proof === null || !reason || this.resolvedQid() !== qid) {
      return;
    }

    const request: CreateInvitationExceptionRequest = {
      qid,
      jobId: value.jobId,
      reason,
      proof: value.proof,
    };

    this.createLoading.set(true);

    this.exceptionsService
      .createException(request)
      .pipe(
        finalize(() => this.createLoading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.notificationService.success(
            this.translate.instant('EXCEPTIONS.ADD_DIALOG.CREATE_SUCCESS'),
          );

          this.dialogRef.close(true);
        },
      });
  }

  protected close(): void {
    this.dialogRef.close();
  }

  private clearCandidate(): void {
    this.candidate.set(null);
    this.resolvedQid.set(null);
  }

  private resetCreationFields(): void {
    this.form.controls.managementId.reset();
    this.form.controls.departmentId.reset();
    this.form.controls.jobId.reset();
    this.form.controls.reason.reset();
    this.form.controls.proof.reset();
  }
}
