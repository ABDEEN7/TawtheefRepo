import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DialogService } from 'primeng/dynamicdialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TextareaModule } from 'primeng/textarea';
import { catchError, debounceTime, finalize, forkJoin, map, of, Subject, switchMap } from 'rxjs';
import { AuthService } from '../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../core/constants/permissions';
import { LanguageService } from '../../../../../core/services/language.service';
import { NotificationService } from '../../../../../core/services/notification.service';
import { portalRoutes } from '../../../../../routes/portal-routes';
import { ConfirmationDialogComponent } from '../../../../../shared/dialogs/confirmation-dialog/confirmation-dialog.component';
import { FaDirArrowDirective } from '../../../../../shared/directives/dir-arrow.directive';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { ExamsService } from '../services/exams.service';
import { categoryForm, examForm, partForm } from './helper/exam-wizard.form';
import { validateExam, validatePart } from './helper/exam-wizard.validation';
import { ExamLookupItemDto, ExamLookupsDto } from '../models/exam-lookups.dto';
import { ExamJobDto } from '../models/exam-job.dto';
import { ExamBankDto } from '../models/exam-bank.dto';
import { ExamConfigurationDto } from '../models/exam-configuration.dto';
import { ExamWorkflowActionComponent } from '../exam-workflow-action/exam-workflow-action.component';
import { ExamCategoryComponent } from './exam-category/exam-category.component';
import { ReturnExamDialogComponent } from '../return-exam-dialog/return-exam-dialog.component';

@Component({
  selector: 'app-exam-workflow',
  standalone: true,
  templateUrl: './exam-workflow.component.html',
  styleUrl: './exam-workflow.component.scss',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    ButtonModule,
    CheckboxModule,
    InputNumberModule,
    InputTextModule,
    SelectModule,
    TableModule,
    TextareaModule,
    FaDirArrowDirective,
    I18nNamespaceDirective,
    ExamCategoryComponent,
    ExamWorkflowActionComponent,
  ],
  providers: [DialogService],
})
export class ExamWorkflowComponent implements OnInit {
  private readonly service = inject(ExamsService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly translate = inject(TranslateService);
  private readonly notifications = inject(NotificationService);
  private readonly dialogs = inject(DialogService);
  private readonly destroyRef = inject(DestroyRef);
  readonly language = inject(LanguageService);
  readonly Permissions = Permissions;
  readonly form = examForm();
  private readonly searches$ = new Subject<string>();
  private readonly jobChanges$ = new Subject<string>();
  lookups: ExamLookupsDto = {
    categories: [],
    interruptionPolicies: [],
    specializedCategoryId: '',
  };
  jobs: ExamJobDto[] = [];
  banks: ExamBankDto[] = [];
  selectedJob: ExamJobDto | null = null;
  step = 1;
  loading = true;
  jobLoading = false;
  searching = false;
  saving = false;
  workflowSubmitting = false;
  private partTwoCategoriesInitialized = false;
  loadFailed = false;
  jobFailed = false;
  submitted = false;
  draftId: string | null = null;
  examNo = '';
  configurationReturnNote: string | null = null;
  decisionByName: string | null = null;
  decisionAt: string | null = null;
  examStatusBackendName: string | null = null;
  errors: string[] = [];
  readonly wizardSteps = [
    {
      id: 1,
      label: 'EXAM_WIZARD.INFO',
      icon: 'hgi hgi-stroke hgi-briefcase-03 me-1',
    },
    {
      id: 2,
      label: 'EXAM_WIZARD.PART_ONE',
      icon: 'hgi hgi-stroke hgi-school me-1 fw-normal',
    },
    {
      id: 3,
      label: 'EXAM_WIZARD.PART_TWO',
      icon: 'hgi hgi-stroke hgi-clipboard-check-01',
    },
    {
      id: 4,
      label: 'EXAM_WIZARD.REVIEW',
      icon: 'hgi hgi-stroke hgi-sent me-1',
    },
  ];

  get examId(): string | null {
    return this.route.snapshot.paramMap.get('examId');
  }

  get mode(): 'create' | 'edit' | 'view' {
    return this.route.snapshot.data['examMode'] ?? 'create';
  }

  get isCreateMode(): boolean {
    return this.mode === 'create';
  }

  get isEditMode(): boolean {
    return this.mode === 'edit';
  }

  get isViewMode(): boolean {
    return this.mode === 'view';
  }

  get pageTitle(): string {
    if (this.isViewMode) return 'EXAM_WIZARD.VIEW';
    return this.isEditMode ? 'EXAM_WIZARD.EDIT' : 'EXAMS.CREATE';
  }

  get headerSubtitle(): string {
    if (this.isViewMode) return 'EXAM_WIZARD.SUBTITLES.VIEW';
    return this.isEditMode ? 'EXAM_WIZARD.SUBTITLES.EDIT' : 'EXAM_WIZARD.SUBTITLES.CREATE';
  }

  get parts() {
    return this.form.controls.parts;
  }
  readonly reviewStep = 4;
  get busy() {
    return this.loading || this.jobLoading || this.saving || this.submitted;
  }
  get categories() {
    return this.parts.getRawValue().flatMap((p) => p.categories);
  }
  get partOne() {
    return this.parts.at(0);
  }
  get partTwo() {
    return this.parts.at(1);
  }
  get nonSpecializedCategories() {
    return this.lookups.categories.filter(
      (category) => category.id !== this.lookups.specializedCategoryId,
    );
  }
  get totalQuestions() {
    return this.categories.reduce((sum, c) => sum + (c.questionCount || 0), 0);
  }
  get totalWeight() {
    return (
      this.categories.reduce((sum, c) => sum + Math.round((c.weightPercent || 0) * 100), 0) / 100
    );
  }
  get totalDuration() {
    return this.parts.getRawValue().reduce((sum, p) => sum + (p.durationMinutes || 0), 0);
  }

  ngOnInit(): void {
    this.load();
    this.searches$
      .pipe(
        debounceTime(300),
        switchMap((search) => {
          this.searching = true;
          return this.service.jobs(search, undefined, this.isViewMode).pipe(
            catchError(() => {
              this.notifyError();
              return of([]);
            }),
            finalize(() => (this.searching = false)),
          );
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((jobs) => {
        this.jobs =
          this.selectedJob && !jobs.some((j) => j.id === this.selectedJob!.id)
            ? [this.selectedJob, ...jobs]
            : jobs;
      });
    this.jobChanges$
      .pipe(
        switchMap((jobId) => {
          this.jobLoading = true;
          this.jobFailed = false;
          return forkJoin({
            banks: this.service.banks(jobId, this.isViewMode),
            existing: this.auth.hasPermission(Permissions.Exams.View)
              ? this.service.existing(jobId).pipe(catchError(() => of(null)))
              : of(null),
          }).pipe(
            catchError(() => {
              this.jobFailed = true;
              return of(null);
            }),
            finalize(() => (this.jobLoading = false)),
          );
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((result) => {
        if (!result) return;
        this.banks = result.banks;
        if (result.existing) this.offerCopy(result.existing);
      });
  }

  load(): void {
    this.loading = true;
    this.loadFailed = false;
    forkJoin({
      lookups: this.service.wizardLookups(this.isViewMode),
      configuration: this.examId
        ? this.service.configuration(this.examId, this.isViewMode)
        : of(null),
    })
      .pipe(
        switchMap((result) => {
          const configuration = result.configuration;
          this.jobLoading = !!configuration;
          return forkJoin({
            jobs: this.service.jobs('', configuration?.jobId, this.isViewMode),
            banks: configuration
              ? this.service.banks(configuration.jobId, this.isViewMode)
              : of(null),
          }).pipe(
            map((loaded) => ({ ...result, ...loaded })),
            finalize(() => (this.jobLoading = false)),
          );
        }),
        finalize(() => (this.loading = false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (result) => {
          this.lookups = result.lookups;
          this.jobs = result.jobs;
          this.resetFixedParts();
          if (result.configuration) this.applyConfiguration(result.configuration);
          else this.initializePartTwoCategories();
          this.banks = result.banks ?? [];
          if (this.isViewMode) this.form.disable({ emitEvent: false });
        },
        error: () => (this.loadFailed = true),
      });
  }

  private applyConfiguration(configuration: ExamConfigurationDto): void {
    this.draftId = this.examId;
    this.selectedJob = this.jobs.find((job) => job.id === configuration.jobId) ?? null;
    this.configurationReturnNote = configuration.decisionNotes;
    this.decisionByName = configuration.decisionByName ?? null;
    this.decisionAt = configuration.decisionAt ?? null;
    this.examStatusBackendName = configuration.statusBackendName;
    this.form.patchValue({
      jobId: configuration.jobId,
      titleAr: configuration.titleAr,
      titleEn: configuration.titleEn,
      allowPreviousQuestion: configuration.allowPreviousQuestion,
      interruptionPolicyId: configuration.interruptionPolicyId,
      notes: configuration.notes,
    });
    this.parts.clear();
    this.parts.push(partForm(1, configuration.parts[0]));
    this.parts.push(partForm(2, configuration.parts[1]));
    this.form.markAsPristine();
  }

  name(value: ExamLookupItemDto | undefined | null): string {
    return value ? (this.language.isRtl ? value.nameAr : value.nameEn) : '';
  }

  lookupName(id: string, options: ExamLookupItemDto[]): string {
    return this.name(options.find((x) => x.id === id));
  }

  get specializedCategories(): ExamLookupItemDto[] {
    const specialized = this.lookups.categories.find(
      (x) => x.id === this.lookups.specializedCategoryId,
    );

    return specialized ? [specialized] : [];
  }

  jobOptions() {
    return this.jobs.map((job) => ({ ...job, label: job.jobNumber + ' — ' + this.name(job) }));
  }

  categoryIds(index: number): string[] {
    return this.parts
      .at(index)
      .controls.categories.getRawValue()
      .map((c) => c.categoryId);
  }

  reviewErrors(): string[] {
    return validateExam(
      this.configuration(),
      this.banks,
      true,
      this.lookups.specializedCategoryId,
      this.lookups.categories.length,
    );
  }

  bankLabel(id: string): string {
    const bank = this.banks.find((b) => b.id === id);
    return bank
      ? this.translate.instant('EXAM_WIZARD.BANK_LABEL', {
          name: this.name(bank),
          version: bank.versionNo,
          id: bank.id,
        })
      : this.translate.instant('EXAM_WIZARD.EMPTY');
  }

  searchJobs(search: string): void {
    this.searches$.next(search);
  }

  selectJob(): void {
    const id = this.form.controls.jobId.value;
    this.selectedJob = this.jobs.find((j) => j.id === id) ?? null;
    this.banks = [];
    this.parts.controls.forEach((p) =>
      p.controls.categories.controls.forEach((c) => c.controls.questionBankVersionId.setValue('')),
    );
    this.errors = [];
    if (id) this.jobChanges$.next(id);
  }

  retryJob(): void {
    this.jobChanges$.next(this.form.controls.jobId.value);
  }

  private offerCopy(configuration: ExamConfigurationDto): void {
    const jobId = this.form.controls.jobId.value;
    this.dialogs
      .open(ConfirmationDialogComponent, {
        header: this.translate.instant('EXAM_WIZARD.EXISTING_TITLE'),
        width: 'min(32rem, 95vw)',
        modal: true,
        data: {
          type: 'submit',
          description: 'EXAM_WIZARD.COPY_MESSAGE',
          confirmText: 'EXAM_WIZARD.COPY',
          cancelText: 'EXAM_WIZARD.START_EMPTY',
        },
      })
      ?.onClose.pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((confirmed) => {
        if (confirmed !== true || this.form.controls.jobId.value !== jobId || this.saving) return;
        this.form.patchValue({
          titleAr: configuration.titleAr,
          titleEn: configuration.titleEn,
          allowPreviousQuestion: configuration.allowPreviousQuestion,
          interruptionPolicyId: configuration.interruptionPolicyId,
          notes: configuration.notes,
        });
        this.parts.clear();
        this.parts.push(partForm(1, configuration.parts[0]));
        this.parts.push(partForm(2, configuration.parts[1]));
        this.form.markAsDirty();
        this.step = 1;
        this.errors = [];
      });
  }

  addCategory(categoryId = '', markDirty = true): void {
    if (this.partTwo.controls.categories.length >= this.nonSpecializedCategories.length) return;
    const category = categoryForm();
    category.controls.categoryId.setValue(categoryId);
    this.partTwo.controls.categories.push(category);
    if (markDirty) this.form.markAsDirty();
  }

  private initializePartTwoCategories(): void {
    if (!this.isCreateMode || this.partTwoCategoriesInitialized) return;
    this.nonSpecializedCategories.forEach((category) => this.addCategory(category.id, false));
    this.partTwoCategoriesInitialized = true;
  }

  removeCategory(categoryIndex: number): void {
    this.partTwo.controls.categories.removeAt(categoryIndex);
    this.form.markAsDirty();
  }

  goTo(target: number): void {
    if (this.busy || target < 1 || target > this.reviewStep) return;
    if (this.isViewMode) {
      this.step = target;
      return;
    }
    this.errors = [];
    if (target > this.step) {
      if (this.step === 1) {
        const info = this.configuration();
        if (!info.jobId || !info.titleAr.trim() || !info.interruptionPolicyId)
          this.errors = ['INFO'];
      } else {
        this.errors = validatePart(this.parts.at(this.step - 2).getRawValue(), this.banks, true);
      }
    }
    if (!this.errors.length) this.step = target;
  }

  configuration(): ExamConfigurationDto {
    return {
      ...this.form.getRawValue(),
      totalQuestions: this.totalQuestions,
      decisionNotes: this.configurationReturnNote ?? null,
      statusBackendName: this.examStatusBackendName ?? '',
    };
  }

  save(submit: boolean): void {
    if (
      this.busy ||
      this.isViewMode ||
      this.loadFailed ||
      this.jobFailed ||
      !this.auth.hasPermission(Permissions.Exams.Create) ||
      (submit && this.step !== this.reviewStep)
    )
      return;
    const configuration = this.configuration();
    this.errors = validateExam(
      configuration,
      this.banks,
      submit,
      this.lookups.specializedCategoryId,
      this.lookups.categories.length,
    );
    if (this.errors.length) return;
    this.saving = true;
    this.form.disable({ emitEvent: false });
    this.service
      .save(configuration, this.draftId, submit)
      .pipe(
        finalize(() => {
          this.saving = false;
          if (!this.submitted) this.form.enable({ emitEvent: false });
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (result) => {
          this.draftId = result.id;
          this.examNo = result.examNo;
          this.form.markAsPristine();
          this.notifications.success(
            this.translate.instant(submit ? 'EXAM_WIZARD.SUBMITTED' : 'EXAM_WIZARD.DRAFT_SAVED'),
          );
          if (submit) {
            this.submitted = true;
            void this.router.navigateByUrl(portalRoutes.examsManagement);
          }
        },
        error: () => this.notifyError(),
      });
  }

  onApproveClicked(): void {
    const examId = this.examId;
    if (
      !examId ||
      this.workflowSubmitting ||
      this.examStatusBackendName !== 'PendingApproval' ||
      !this.auth.hasPermission(Permissions.Exams.WorkflowActions)
    )
      return;

    this.workflowSubmitting = true;
    const dialogRef = this.dialogs.open(ConfirmationDialogComponent, {
      header: this.translate.instant('EXAMS.APPROVE_TITLE'),
      width: 'min(32rem, 95vw)',
      modal: true,
      data: {
        type: 'submit',
        description: 'EXAMS.APPROVE_CONFIRMATION',
        confirmText: 'EXAMS.CONFIRM_APPROVE',
      },
    });
    if (!dialogRef) {
      this.workflowSubmitting = false;
      return;
    }

    dialogRef.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((confirmed) => {
      if (confirmed !== true || this.examStatusBackendName !== 'PendingApproval') {
        this.workflowSubmitting = false;
        return;
      }

      this.service
        .approve(examId)
        .pipe(
          finalize(() => (this.workflowSubmitting = false)),
          takeUntilDestroyed(this.destroyRef),
        )
        .subscribe({
          next: () => {
            this.notifications.success(this.translate.instant('EXAMS.APPROVE_SUCCESS'));
            this.load();
          },
          error: () => this.notifications.error(this.translate.instant('EXAMS.APPROVE_FAILED')),
        });
    });
  }

  onReturnClicked(): void {
    const examId = this.examId;
    if (
      !examId ||
      this.examStatusBackendName !== 'PendingApproval' ||
      !this.auth.hasPermission(Permissions.Exams.WorkflowActions)
    )
      return;

    const dialogRef = this.dialogs.open(ReturnExamDialogComponent, {
      header: this.translate.instant('EXAMS.RETURN_TITLE'),
      width: 'min(32rem, 95vw)',
      modal: true,
      data: { examId, action: 'return' },
    });

    if (!dialogRef) return;

    dialogRef.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((returned) => {
      if (!returned) return;

      this.notifications.success(this.translate.instant('EXAMS.RETURN_SUCCESS'));
      this.examStatusBackendName = null;
      this.load();
    });
  }

  onRejectClicked(): void {
    const examId = this.examId;
    if (
      !examId ||
      this.workflowSubmitting ||
      this.examStatusBackendName !== 'PendingApproval' ||
      !this.auth.hasPermission(Permissions.Exams.WorkflowActions)
    )
      return;

    this.workflowSubmitting = true;
    const dialogRef = this.dialogs.open(ReturnExamDialogComponent, {
      header: this.translate.instant('EXAMS.REJECT_TITLE'),
      width: 'min(32rem, 95vw)',
      modal: true,
      data: { examId, action: 'reject' },
    });
    if (!dialogRef) {
      this.workflowSubmitting = false;
      return;
    }

    dialogRef.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((rejected) => {
      this.workflowSubmitting = false;
      if (!rejected) return;

      this.notifications.success(this.translate.instant('EXAMS.REJECT_SUCCESS'));
      this.examStatusBackendName = null;
      this.load();
    });
  }

  cancel(): void {
    if (this.saving) return;
    if (!this.form.dirty) {
      void this.router.navigateByUrl(portalRoutes.examsManagement);
      return;
    }
    this.dialogs
      .open(ConfirmationDialogComponent, {
        header: this.translate.instant('EXAM_WIZARD.CANCEL'),
        width: 'min(32rem, 95vw)',
        modal: true,
        data: { type: 'warning', description: 'EXAM_WIZARD.LEAVE_MESSAGE' },
      })
      ?.onClose.pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((confirmed) => {
        if (confirmed === true) void this.router.navigateByUrl(portalRoutes.examsManagement);
      });
  }

  private notifyError(): void {
    this.notifications.error(this.translate.instant('EXAM_WIZARD.REQUEST_FAILED'));
  }

  private resetFixedParts(): void {
    this.parts.clear();
    this.parts.push(
      partForm(1, {
        partNo: 1,
        titleAr: '',
        titleEn: null,
        durationMinutes: 0,
        qualificationScore: null,
        categories: [
          {
            categoryId: this.lookups.specializedCategoryId,
            questionBankVersionId: '',
            questionCount: 0,
            weightPercent: 0,
            easyQuestionCount: 0,
            mediumQuestionCount: 0,
            hardQuestionCount: 0,
          },
        ],
      }),
    );
    this.parts.push(partForm(2));
  }
}
