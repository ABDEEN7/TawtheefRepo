import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { Select } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { DialogService } from 'primeng/dynamicdialog';

import { InterviewScheduleStore, WizardStep } from '../../interview-schedule.store';
import { InterviewScheduleFacade } from '../../interview-schedule.facade';
import { LazySelectComponent } from '../../components/lazy-select/lazy-select';
import { PeriodDialogComponent } from '../../components/period-dialog/period-dialog';
import { ReassignSlotDialogComponent } from '../../components/reassign-slot-dialog/reassign-slot-dialog';
import { LazySelectOption } from '../../models/lazy-select-option.model';
import { SlotPreviewModel } from '../../models/plan-preview.model';
import { PeriodDraft, WizardJobContext } from '../../models/wizard-draft.model';
import {
  COMMITTEE_ROLE_LABELS,
  COMMITTEE_STATUS_LABELS,
  COMMITTEE_STATUS_PILL,
  CommitteeRole,
  INTERVIEW_TYPE_LABELS,
  InterviewType,
} from '../../models/enums';

@Component({
  selector: 'app-schedule-wizard',
  templateUrl: './schedule-wizard.html',
  styleUrls: ['./schedule-wizard.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, TranslatePipe, DatePipe, Select, InputTextModule, TableModule, LazySelectComponent],
})
export class ScheduleWizardComponent {
  store = inject(InterviewScheduleStore);
  service = inject(InterviewScheduleFacade);
  private dialogService = inject(DialogService);
  private translate = inject(TranslateService);

  readonly InterviewType = InterviewType;
  readonly steps: WizardStep[] = [1, 2, 3, 4];
  readonly stepLabels: Record<WizardStep, string> = {
    1: 'INTERVIEW_SCHEDULE.WIZARD.STEP1_LABEL',
    2: 'INTERVIEW_SCHEDULE.WIZARD.STEP2_LABEL',
    3: 'INTERVIEW_SCHEDULE.WIZARD.STEP3_LABEL',
    4: 'INTERVIEW_SCHEDULE.WIZARD.STEP4_LABEL',
  };

  readonly typeOptions = [
    { value: InterviewType.InPerson, label: INTERVIEW_TYPE_LABELS[InterviewType.InPerson] },
    { value: InterviewType.Remote, label: INTERVIEW_TYPE_LABELS[InterviewType.Remote] },
  ];

  readonly searchJobs = this.service.searchJobs;
  readonly searchRooms = this.service.searchRooms;

  readonly isEditMode = computed(() => this.store.wizardMode() === 'edit');
  readonly titleKey = computed(() =>
    this.isEditMode() ? 'INTERVIEW_SCHEDULE.WIZARD.EDIT_TITLE' : 'INTERVIEW_SCHEDULE.WIZARD.CREATE_TITLE',
  );

  readonly jobOption = computed<LazySelectOption<WizardJobContext> | null>(() => {
    const job = this.store.wizardJob();
    if (!job) return null;
    return { value: job.id, label: this.localized(job.titleAr, job.titleEn), hint: job.jobNumber, data: job };
  });

  readonly committeeStatusLabel = computed(() => {
    const ctx = this.store.wizardContext();
    return ctx ? COMMITTEE_STATUS_LABELS[ctx.committeeStatus] : '';
  });

  readonly committeeStatusPill = computed(() => {
    const ctx = this.store.wizardContext();
    return ctx ? COMMITTEE_STATUS_PILL[ctx.committeeStatus] : 'neutral';
  });

  // The eligible pool split by gender. Once a preview exists it is the live source (same pool the server distributes);
  // before that (step 1, or no periods yet) the job's creation context carries the same numbers.
  readonly eligibleStats = computed(() => {
    const preview = this.store.wizardPreview();
    const ctx = this.store.wizardContext();
    return {
      total: preview?.eligibleCandidateCount ?? ctx?.eligibleCandidateCount ?? 0,
      male: preview?.eligibleMaleCount ?? ctx?.eligibleMaleCount ?? 0,
      female: preview?.eligibleFemaleCount ?? ctx?.eligibleFemaleCount ?? 0,
    };
  });

  readonly capacityShortfall = computed(() => {
    const preview = this.store.wizardPreview();
    if (!preview) return false;
    return preview.totalCapacity < this.eligibleStats().total;
  });

  readonly reviewDays = computed(() => {
    const dates = new Set(this.store.wizardPeriods().map((p) => p.date));
    return dates.size;
  });

  localized(ar?: string | null, en?: string | null): string {
    return this.service.localized(ar, en);
  }

  // Optional facts (a job may have no specialist or department) render as "-" rather than an empty gap.
  localizedOrDash(ar?: string | null, en?: string | null): string {
    return this.localized(ar, en) || '-';
  }

  committeeMemberRole(role: CommitteeRole): string {
    return COMMITTEE_ROLE_LABELS[role];
  }

  goToStep(step: WizardStep) {
    this.service.goToStep(step);
  }

  onTypeChange(type: InterviewType) {
    this.service.selectInterviewType(type);
  }

  openAddPeriod() {
    this.openPeriodDialog(null);
  }

  openEditPeriod(period: PeriodDraft) {
    this.openPeriodDialog(period);
  }

  private openPeriodDialog(period: PeriodDraft | null) {
    const type = this.store.wizardInfo().interviewType;
    if (type === null) return;

    const ref = this.dialogService.open(PeriodDialogComponent, {
      header: this.translate.instant(
        period ? 'INTERVIEW_SCHEDULE.WIZARD.EDIT_PERIOD' : 'INTERVIEW_SCHEDULE.WIZARD.ADD_PERIOD',
      ),
      width: '32rem',
      data: {
        period,
        interviewType: type,
        searchRooms: this.searchRooms,
        localized: (ar?: string | null, en?: string | null) => this.localized(ar, en),
      },
    });

    ref?.onClose.subscribe((draft: PeriodDraft | undefined) => {
      if (!draft) return;
      if (period) this.service.updatePeriod(period.localId, draft);
      else this.service.addPeriod(draft);
    });
  }

  removePeriod(period: PeriodDraft) {
    this.service.removePeriod(period.localId);
  }

  periodLocation(period: PeriodDraft): string {
    if (period.room) return this.localized(period.room.nameAr, period.room.nameEn);
    return period.remoteMeetingUrl ?? '';
  }

  slotLocation(slot: SlotPreviewModel): string {
    if (slot.slot.roomId) return this.localized(slot.slot.roomNameAr, slot.slot.roomNameEn);
    if (slot.slot.remoteMeetingUrl) return slot.slot.remoteMeetingUrl;
    return '';
  }

  candidateName(slot: SlotPreviewModel): string {
    return this.localized(slot.candidateFullNameAr, slot.candidateFullNameEn);
  }

  // Name plus personal ID, for the places that show the candidate as a single string (the reassign dialog).
  candidateLabel(slot: SlotPreviewModel): string {
    const name = this.candidateName(slot);
    return slot.candidateQid ? `${name} (${slot.candidateQid})` : name;
  }

  openReassign(slot: SlotPreviewModel) {
    if (!slot.invitationId) return;
    const preview = this.store.wizardPreview();
    if (!preview) return;

    const ref = this.dialogService.open(ReassignSlotDialogComponent, {
      header: this.translate.instant('INTERVIEW_SCHEDULE.WIZARD.REASSIGN'),
      width: '28rem',
      data: {
        candidateInvitationId: slot.invitationId,
        candidateName: this.candidateLabel(slot),
        slots: preview.slots,
        localized: (ar?: string | null, en?: string | null) => this.localized(ar, en),
      },
    });

    ref?.onClose.subscribe((newStartAt: string | undefined) => {
      if (!newStartAt || newStartAt === slot.slot.startAt) return;
      this.service.reassignCandidate(slot.invitationId!, newStartAt);
    });
  }

  redistribute() {
    this.service.redistribute();
  }

  saveDraft() {
    void this.service.save(false);
  }

  sendForApproval() {
    void this.service.save(true);
  }
}
