import { DestroyRef, inject, Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { catchError, distinctUntilChanged, firstValueFrom, map, Observable, of, shareReplay, Subject, switchMap } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

import { NotificationService } from '../../../../../core/services/notification.service';
import { LanguageService } from '../../../../../core/services/language.service';
import { JobStatus } from '../../../../../core/enums/lookups.enum';

import { JobLookupService } from '../../job-management/services/job-lookup.service';
import { JobResponse } from '../../job-management/models/job-response-model';

import { InterviewScheduleStore, WizardStep } from './interview-schedule.store';
import {
  InterviewScheduleService,
  ManualAssignmentPayload,
  PeriodInputPayload,
  PreviewSchedulePayload,
  RescheduleAppointmentPayload,
} from './services/interview-schedule.service';
import { InterviewType, ScheduleStatus } from './models/enums';
import { LazySelectOption } from './models/lazy-select-option.model';
import { SchedulePlanPreviewModel } from './models/plan-preview.model';
import { RoomOptionModel, ScheduleListItemModel, ScheduleModel } from './models/schedule.model';
import { newPeriodDraft, PeriodDraft, WizardJobContext } from './models/wizard-draft.model';

// Search-as-you-type dropdowns show at most this many rows; the user narrows further by typing.
const MAX_JOB_OPTIONS = 30;

// Which schedule's detail view is open lives in the URL (?schedule=<id>) rather than only in memory: switching the
// language reloads the whole app, and without it the page would come back on the list instead of the same schedule.
const SCHEDULE_QUERY_PARAM = 'schedule';

// Live preview refetch is debounced so a burst of edits (duration, buffer, add/edit/remove period) collapses
// into one PreviewScheduleSlotsQuery call.
const PREVIEW_DEBOUNCE_MS = 400;

@Injectable()
export class InterviewScheduleFacade {
  private destroyRef = inject(DestroyRef);
  private store = inject(InterviewScheduleStore);
  private api = inject(InterviewScheduleService);
  private jobLookups = inject(JobLookupService);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  private previewTrigger$ = new Subject<void>();

  // Resolved once, on the first job search: the Published status id comes from the job-status lookup.
  private publishedJobStatusId$ = this.jobLookups.loadJobStatus().pipe(
    map(() => this.jobLookups.getStatusIdByEnum(JobStatus.Published)),
    shareReplay(1),
  );

  init() {
    this.store.setCurrentLang(this.language.get());
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((lang) => this.store.setCurrentLang(lang));

    // Emits the current value right away, so a page loaded on ?schedule=<id> opens that detail before its first render.
    this.route.queryParamMap
      .pipe(
        map((params) => params.get(SCHEDULE_QUERY_PARAM)),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((id) => this.syncViewWithUrl(id));

    this.setupPreviewPipeline();
    this.loadSchedules();
  }

  // The URL decides between list and detail; the wizard is not URL-driven, so it is left alone here.
  private syncViewWithUrl(id: string | null) {
    if (id) {
      if (this.store.view() === 'detail' && this.store.detailSchedule()?.id === id) return;
      this.store.setView('detail');
      this.store.setDetailSchedule(null);
      this.loadDetail(id);
      return;
    }

    if (this.store.view() === 'detail') {
      this.store.setView('list');
      this.loadSchedules();
    }
  }

  private setUrlSchedule(id: string | null) {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { [SCHEDULE_QUERY_PARAM]: id },
      queryParamsHandling: 'merge',
    });
  }

  // ======== List ========
  loadSchedules() {
    this.store.listLoading.set(true);
    this.api.listSchedules().subscribe({
      next: (items) => {
        this.store.setSchedulesResult(items);
        this.store.listLoading.set(false);
      },
      error: () => this.store.listLoading.set(false),
    });
  }

  setSearch(value: string) {
    this.store.setSearch(value);
  }

  setStatusFilter(value: ScheduleStatus | null) {
    this.store.setStatusFilter(value);
  }

  setJobFilter(value: string | null) {
    this.store.setJobFilter(value);
  }

  setTypeFilter(value: InterviewType | null) {
    this.store.setTypeFilter(value);
  }

  onPageChange(page: number) {
    this.store.setPage(page);
  }

  onPageSizeChange(size: number) {
    this.store.setPageSize(size);
  }

  // ======== Lookups ========
  searchJobs = (term: string): Observable<LazySelectOption<WizardJobContext>[]> =>
    this.publishedJobStatusId$.pipe(
      switchMap((statusId) => (statusId ? this.api.searchPublishedJobs(statusId, term.trim()) : of(null))),
      map((result) => (result?.items ?? []).slice(0, MAX_JOB_OPTIONS).map((job) => this.toJobOption(job))),
      catchError(() => of([])),
    );

  searchRooms = (term: string): Observable<LazySelectOption<RoomOptionModel>[]> =>
    this.api.searchRooms(term.trim()).pipe(
      map((rooms) => rooms.map((room) => this.toRoomOption(room))),
      catchError(() => of([])),
    );

  private toJobOption(job: JobResponse): LazySelectOption<WizardJobContext> {
    const context: WizardJobContext = {
      id: job.id,
      titleAr: job.titleAr ?? '',
      titleEn: job.titleEn ?? null,
      jobNumber: job.jobNumber ?? null,
    };
    return {
      value: job.id,
      label: this.localized(context.titleAr, context.titleEn),
      hint: context.jobNumber,
      searchText: [context.titleAr, context.titleEn, context.jobNumber].filter(Boolean).join(' '),
      data: context,
    };
  }

  private toRoomOption(room: RoomOptionModel): LazySelectOption<RoomOptionModel> {
    return {
      value: room.id,
      label: this.localized(room.nameAr, room.nameEn),
      hint: this.localized(room.locationNameAr, room.locationNameEn),
      searchText: [room.nameAr, room.nameEn, room.locationNameAr, room.locationNameEn].filter(Boolean).join(' '),
      data: room,
    };
  }

  localized(ar?: string | null, en?: string | null): string {
    return this.store.isRtl() ? ar || en || '' : en || ar || '';
  }

  // ======== Wizard navigation ========
  openCreateWizard() {
    this.store.resetWizardForCreate();
    this.store.setView('wizard');
  }

  openEditWizard(schedule: ScheduleModel) {
    this.store.setWizardContextLoading(true);
    this.api.getCreationContext(schedule.jobId, schedule.id).subscribe({
      next: (context) => {
        const job: WizardJobContext = {
          id: schedule.jobId,
          titleAr: schedule.jobTitleNameAr ?? context.jobTitleNameAr,
          titleEn: schedule.jobTitleNameEn ?? context.jobTitleNameEn ?? null,
          jobNumber: null,
        };
        this.store.resetWizardForEdit(schedule, job, context);
        this.store.setWizardContextLoading(false);
        this.store.setView('wizard');
        // The saved periods + distribution are back in the store; fetch the capacity/slot preview for them.
        this.schedulePreview();
        // Editing from the detail view leaves ?schedule=<id> behind; clear it now that the wizard is showing (clearing
        // it earlier would make the URL sync send the user back to the list).
        this.setUrlSchedule(null);
      },
      error: () => this.store.setWizardContextLoading(false),
    });
  }

  cancelWizard() {
    this.store.setView('list');
    this.loadSchedules();
  }

  nextStep() {
    if (!this.validateStep(this.store.wizardStep())) return;
    const step = this.store.wizardStep();
    if (step < 4) this.store.setWizardStep((step + 1) as WizardStep);
  }

  prevStep() {
    const step = this.store.wizardStep();
    if (step > 1) this.store.setWizardStep((step - 1) as WizardStep);
  }

  goToStep(step: WizardStep) {
    const current = this.store.wizardStep();
    if (step === current) return;
    if (step < current) {
      this.store.setWizardStep(step);
      return;
    }
    for (let s = current; s < step; s++) {
      if (!this.validateStep(s as WizardStep)) return;
    }
    this.store.setWizardStep(step);
  }

  // ======== Wizard: step 1 ========
  selectJob(option: LazySelectOption<WizardJobContext> | null) {
    this.store.updateWizardInfo({ jobId: option?.value ?? '' });
    this.store.setWizardJob(option?.data ?? null);
    this.store.setWizardContext(null);
    this.store.setWizardContextError(null);
    if (!option) return;

    this.store.setWizardContextLoading(true);
    this.api.getCreationContext(option.value).subscribe({
      next: (context) => {
        this.store.setWizardContext(context);
        this.store.setWizardContextLoading(false);
      },
      error: (err: HttpErrorResponse) => {
        this.store.setWizardContextLoading(false);
        this.store.setWizardContextError(this.translateServerError(err));
      },
    });
  }

  selectInterviewType(type: InterviewType) {
    this.store.updateWizardInfo({ interviewType: type });
    this.store.clearWizardPeriods();
  }

  // Same lookup the global error interceptor uses for its toast: the business error code is the first entry of the
  // response's `error` list (the top-level `message` is only the API's generic fallback text), translated under
  // `server-error.<CODE>`.
  private translateServerError(err: HttpErrorResponse): string {
    const body = err.error as { error?: { message?: string }[]; detail?: string } | null;
    const code = body?.error?.[0]?.message || body?.detail || 'UN_EXPECTED_ERROR';
    const key = `server-error.${code}`;
    const translated = this.translate.instant(key);
    return translated !== key ? translated : this.translate.instant('server-error.UN_EXPECTED_ERROR');
  }

  // ======== Wizard: step 2 ========
  updateDuration(value: number) {
    this.store.setWizardDuration(value);
    this.schedulePreview();
  }

  updateBuffer(value: number) {
    this.store.setWizardBuffer(value);
    this.schedulePreview();
  }

  addPeriod(period: PeriodDraft) {
    this.store.addWizardPeriod(period);
    this.schedulePreview();
  }

  updatePeriod(localId: string, period: PeriodDraft) {
    this.store.updateWizardPeriod(localId, period);
    this.schedulePreview();
  }

  removePeriod(localId: string) {
    this.store.removeWizardPeriod(localId);
    this.schedulePreview();
  }

  newPeriodDraft(): PeriodDraft {
    return newPeriodDraft();
  }

  // ======== Wizard: step 3 ========
  reassignCandidate(invitationId: string, newSlotStartAt: string) {
    const existing = this.store.wizardManualAssignments().filter((m) => m.invitationId !== invitationId);
    this.store.setWizardManualAssignments([...existing, { slotStartAt: newSlotStartAt, invitationId }]);
    this.schedulePreview();
  }

  redistribute() {
    this.store.setWizardManualAssignments([]);
    this.schedulePreview();
  }

  // ======== Live preview (steps 2 + 3 share one server-computed source of truth) ========
  private schedulePreview() {
    this.previewTrigger$.next();
  }

  private setupPreviewPipeline() {
    this.previewTrigger$
      .pipe(
        debounceTime(PREVIEW_DEBOUNCE_MS),
        switchMap(() => {
          const payload = this.buildPreviewPayload();
          if (!payload) return of(null);
          this.store.setWizardPreviewLoading(true);
          return this.api.previewSlots(payload).pipe(catchError(() => of(null)));
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((preview: SchedulePlanPreviewModel | null) => {
        this.store.setWizardPreview(preview);
        this.store.setWizardPreviewLoading(false);
      });
  }

  private buildPreviewPayload(): PreviewSchedulePayload | null {
    const info = this.store.wizardInfo();
    const periods = this.store.wizardPeriods();
    if (!info.jobId || info.interviewType === null || periods.length === 0) return null;
    if (periods.some((p) => !this.isPeriodComplete(p))) return null;

    return {
      jobId: info.jobId,
      interviewType: info.interviewType,
      durationMinutes: this.store.wizardDuration(),
      bufferMinutes: this.store.wizardBuffer(),
      periods: this.buildPeriodPayloads(),
      manualAssignments: this.buildManualAssignmentPayloads(),
      excludeScheduleId: this.store.wizardMode() === 'edit' ? this.store.wizardScheduleId() : null,
    };
  }

  private isPeriodComplete(period: PeriodDraft): boolean {
    if (!period.date || !period.startTime || !period.endTime) return false;
    const type = this.store.wizardInfo().interviewType;
    if (type === InterviewType.InPerson) return !!period.room;
    return !!period.remoteMeetingUrl?.trim();
  }

  private buildPeriodPayloads(): PeriodInputPayload[] {
    return this.store.wizardPeriods().map((p) => ({
      date: p.date,
      startTime: p.startTime,
      endTime: p.endTime,
      roomId: p.room?.id ?? null,
      remoteMeetingUrl: p.remoteMeetingUrl,
      remoteMeetingInstructions: p.remoteMeetingInstructions,
    }));
  }

  private buildManualAssignmentPayloads(): ManualAssignmentPayload[] {
    return this.store.wizardManualAssignments().map((m) => ({ slotStartAt: m.slotStartAt, invitationId: m.invitationId }));
  }

  // ======== Validation ========
  private validateStep(step: WizardStep): boolean {
    if (step === 1) return this.validateStep1();
    if (step === 2) return this.validateStep2();
    if (step === 3) return this.validateStep3();
    return true;
  }

  private validateStep1(): boolean {
    const info = this.store.wizardInfo();
    if (!info.jobId) return this.fail('INTERVIEW_SCHEDULE.VALIDATION.JOB_REQUIRED');
    if (this.store.wizardContextLoading()) return this.fail('INTERVIEW_SCHEDULE.VALIDATION.CONTEXT_LOADING');
    if (!this.store.wizardContext()) return this.fail('INTERVIEW_SCHEDULE.VALIDATION.CONTEXT_MISSING');
    if (info.interviewType === null) return this.fail('INTERVIEW_SCHEDULE.VALIDATION.TYPE_REQUIRED');
    return true;
  }

  private validateStep2(): boolean {
    if (this.store.wizardPeriods().length === 0) return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PERIODS_REQUIRED');
    if (this.store.wizardPreviewLoading()) return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PREVIEW_PENDING');
    if (!this.store.wizardPreview()) return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PREVIEW_PENDING');
    return true;
  }

  private validateStep3(): boolean {
    const preview = this.store.wizardPreview();
    if (!preview) return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PREVIEW_PENDING');
    if (preview.unassignedCandidateCount > 0) {
      return this.fail('INTERVIEW_SCHEDULE.VALIDATION.UNASSIGNED_CANDIDATES', { count: preview.unassignedCandidateCount });
    }
    return true;
  }

  private fail(key: string, params?: Record<string, unknown>): false {
    this.notify.error(this.translate.instant(key, params));
    return false;
  }

  // ======== Save / Submit ========
  // The backend rejects a schedule whose capacity can't fit every eligible candidate even as a draft, so both
  // actions run the same full validation - "Save as Draft" only skips the approval hand-off (see submitSchedule).
  async save(sendForApproval: boolean): Promise<void> {
    if (!this.validateStep1() || !this.validateStep2() || !this.validateStep3()) return;

    this.store.setWizardSaving(true);
    try {
      const info = this.store.wizardInfo();
      const context = this.store.wizardContext()!;
      const periods = this.buildPeriodPayloads();
      const manualAssignments = this.buildManualAssignmentPayloads();
      // No session-title field anywhere in the wizard - auto-generated from the job, matching the demo (which
      // doesn't collect one either).
      const titleAr = `جدول مقابلات - ${context.jobTitleNameAr}`;
      const titleEn = context.jobTitleNameEn ? `Interview Schedule - ${context.jobTitleNameEn}` : null;
      const core = {
        interviewType: info.interviewType!,
        titleAr,
        titleEn,
        durationMinutes: this.store.wizardDuration(),
        bufferMinutes: this.store.wizardBuffer(),
        periods,
        manualAssignments,
      };

      let scheduleId = this.store.wizardScheduleId();
      if (this.store.wizardMode() === 'create') {
        scheduleId = await firstValueFrom(this.api.createSchedule({ ...core, jobId: info.jobId }));
        // From here a retry must update this schedule, not create a second one for the same job.
        this.store.markWizardCreated(scheduleId);
      } else {
        await firstValueFrom(this.api.updateSchedule({ ...core, id: scheduleId! }));
      }

      if (sendForApproval) await firstValueFrom(this.api.submitSchedule(scheduleId!));

      this.toast(sendForApproval ? 'INTERVIEW_SCHEDULE.SUBMIT_SUCCESS' : 'INTERVIEW_SCHEDULE.DRAFT_SAVED');
      this.store.setView('list');
      this.loadSchedules();
    } catch {
      // error toast already shown by the global HTTP error interceptor
    } finally {
      this.store.setWizardSaving(false);
    }
  }

  // ======== Detail ========
  openDetail(schedule: ScheduleListItemModel) {
    this.store.setDetailSchedule(null);
    this.setUrlSchedule(schedule.id);
  }

  backToList() {
    this.setUrlSchedule(null);
  }

  loadDetail(id: string) {
    this.store.setDetailLoading(true);
    this.api.getSchedule(id).subscribe({
      next: (schedule) => {
        this.store.setDetailSchedule(schedule);
        this.store.setDetailLoading(false);
      },
      // A schedule that can't be loaded (deleted, or a stale/bad ?schedule= link) leaves nothing to show - back to the list.
      error: () => {
        this.store.setDetailLoading(false);
        this.backToList();
      },
    });
  }

  submitSchedule(id: string) {
    this.runDetailAction(this.api.submitSchedule(id), 'INTERVIEW_SCHEDULE.SUBMIT_SUCCESS', id);
  }

  approveSchedule(id: string, decisionNotes: string | null) {
    this.runDetailAction(this.api.approveSchedule(id, decisionNotes), 'INTERVIEW_SCHEDULE.APPROVED', id);
  }

  returnSchedule(id: string, reason: string) {
    this.runDetailAction(this.api.returnSchedule(id, reason), 'INTERVIEW_SCHEDULE.RETURNED', id);
  }

  cancelSchedule(id: string, reason: string) {
    this.runDetailAction(this.api.cancelSchedule(id, reason), 'INTERVIEW_SCHEDULE.CANCELLED', id);
  }

  sendReminder(appointmentId: string, alreadySent: boolean) {
    this.api.sendAppointmentNotification(appointmentId).subscribe({
      next: () => {
        this.toast(alreadySent ? 'INTERVIEW_SCHEDULE.REMINDER_SENT' : 'INTERVIEW_SCHEDULE.NOTIFICATION_SENT');
        const schedule = this.store.detailSchedule();
        if (schedule) this.loadDetail(schedule.id);
      },
    });
  }

  rescheduleAppointment(payload: RescheduleAppointmentPayload) {
    this.api.rescheduleAppointment(payload).subscribe({
      next: () => {
        this.toast('INTERVIEW_SCHEDULE.APPOINTMENT_RESCHEDULED');
        const schedule = this.store.detailSchedule();
        if (schedule) this.loadDetail(schedule.id);
      },
    });
  }

  private runDetailAction(action$: Observable<void>, successKey: string, id: string) {
    action$.subscribe({
      next: () => {
        this.toast(successKey);
        this.loadDetail(id);
      },
    });
  }

  private toast(key: string) {
    this.notify.success(this.translate.instant(key));
  }
}
