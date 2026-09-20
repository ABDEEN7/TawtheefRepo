import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';

import { HasPermissionDirective } from '../../../../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../../../../core/constants/permissions';
import { DialogHelperService } from '../../../../../../../core/services/dialog-helper.service';

import { InterviewCommitteesStore } from '../../interview-committees.store';
import { InterviewCommitteesFacade } from '../../interview-committees.facade';
import { CommitteeMemberModel } from '../../models/committee-member.model';
import {
  COMMITTEE_ROLE_LABELS,
  COMMITTEE_STATUS_LABELS,
  COMMITTEE_STATUS_PILL,
  CommitteeRole,
  CommitteeStatus,
  EvaluationScope,
} from '../../models/enums';

interface MemberRow {
  id: string;
  name: string;
  roleKey: string;
  evaluationKey: string;
  axes: string[];
  permissionKeys: string[];
}

@Component({
  selector: 'app-committee-detail',
  templateUrl: './committee-detail.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, TranslatePipe, HasPermissionDirective],
})
export class CommitteeDetailComponent {
  readonly Permissions = Permissions;

  store = inject(InterviewCommitteesStore);
  service = inject(InterviewCommitteesFacade);
  private dialogHelper = inject(DialogHelperService);

  private localized(ar?: string | null, en?: string | null): string {
    return this.store.isRtl() ? ar || en || '' : en || ar || '';
  }

  readonly memberRows = computed<MemberRow[]>(() =>
    this.store.detailMembers().map((member) => ({
      id: member.id,
      name: this.localized(member.memberNameAr, member.memberNameEn),
      roleKey: COMMITTEE_ROLE_LABELS[member.role],
      ...this.evaluation(member),
      permissionKeys: this.permissionKeys(member),
    })),
  );

  committeeName = computed(() => {
    const c = this.store.detailCommittee();
    return c ? this.localized(c.nameAr, c.nameEn) : '';
  });

  jobTitle = computed(() => {
    const c = this.store.detailCommittee();
    return c ? this.localized(c.jobTitleNameAr, c.jobTitleNameEn) : '';
  });

  templateTitle = computed(() => {
    const c = this.store.detailCommittee();
    return c ? this.localized(c.interviewTemplateTitleAr, c.interviewTemplateTitleEn) : '';
  });

  chairName = computed(() => {
    const c = this.store.detailCommittee();
    return c ? this.localized(c.chairNameAr, c.chairNameEn) : '';
  });

  committeeType = computed(() => {
    const c = this.store.detailCommittee();
    return c ? this.localized(c.committeeTypeNameAr, c.committeeTypeNameEn) : '';
  });

  statusLabel(status: CommitteeStatus): string {
    return COMMITTEE_STATUS_LABELS[status];
  }

  statusPill(status: CommitteeStatus): string {
    return COMMITTEE_STATUS_PILL[status];
  }

  // Which lifecycle actions apply mirrors the backend's state machine (see InterviewCommittee entity).
  canEdit(status: CommitteeStatus): boolean {
    return status === CommitteeStatus.Draft || status === CommitteeStatus.Returned;
  }

  canSubmit(status: CommitteeStatus): boolean {
    return status === CommitteeStatus.Draft;
  }

  canDecide(status: CommitteeStatus): boolean {
    return status === CommitteeStatus.PendingApproval;
  }

  canCancel(status: CommitteeStatus): boolean {
    return status === CommitteeStatus.Draft || status === CommitteeStatus.PendingApproval;
  }

  private evaluation(member: CommitteeMemberModel): { evaluationKey: string; axes: string[] } {
    if (member.role === CommitteeRole.Chair) return { evaluationKey: 'INTERVIEW_COMMITTEES.SCOPE.ALL_AXES', axes: [] };
    if (!member.participatesInEvaluation) return { evaluationKey: 'INTERVIEW_COMMITTEES.SCOPE.NOT_EVALUATING', axes: [] };
    if (member.evaluationScope === EvaluationScope.SelectedAxes) {
      return {
        evaluationKey: 'INTERVIEW_COMMITTEES.SCOPE.SELECTED_AXES',
        axes: member.evaluationAxes.map((a) => this.localized(a.axisNameAr, a.axisNameEn)),
      };
    }
    return { evaluationKey: 'INTERVIEW_COMMITTEES.SCOPE.ALL_AXES', axes: [] };
  }

  private permissionKeys(member: CommitteeMemberModel): string[] {
    const keys: string[] = [];
    if (member.canViewCandidates) keys.push('INTERVIEW_COMMITTEES.PERMISSION.VIEW_CANDIDATES');
    if (member.canAddNotes) keys.push('INTERVIEW_COMMITTEES.PERMISSION.ADD_NOTES');
    if (member.canSubmitEvaluation) keys.push('INTERVIEW_COMMITTEES.PERMISSION.SUBMIT_EVALUATION');
    if (member.canViewOtherEvaluations) keys.push('INTERVIEW_COMMITTEES.PERMISSION.VIEW_OTHER_EVALUATIONS');
    if (member.canViewCommitteeSummary) keys.push('INTERVIEW_COMMITTEES.PERMISSION.VIEW_COMMITTEE_SUMMARY');
    return keys;
  }

  edit() {
    const committee = this.store.detailCommittee();
    if (committee) this.service.openEditWizard(committee);
  }

  submit() {
    const committee = this.store.detailCommittee();
    if (!committee) return;

    this.dialogHelper
      .openConfirmDialog({
        type: 'submit',
        title: 'INTERVIEW_COMMITTEES.DETAIL.CONFIRM_SUBMIT_TITLE',
        description: 'INTERVIEW_COMMITTEES.DETAIL.CONFIRM_SUBMIT_DESCRIPTION',
        confirmText: 'INTERVIEW_COMMITTEES.DETAIL.SUBMIT',
        cancelText: 'INTERVIEW_COMMITTEES.WIZARD.CANCEL',
      })
      ?.onClose.subscribe((confirmed: boolean | undefined) => {
        if (confirmed) this.service.submitCommittee(committee.id);
      });
  }

  approve() {
    const committee = this.store.detailCommittee();
    if (!committee) return;

    this.dialogHelper
      .openConfirmDialog({
        type: 'submit',
        title: 'INTERVIEW_COMMITTEES.DETAIL.CONFIRM_APPROVE_TITLE',
        description: 'INTERVIEW_COMMITTEES.DETAIL.CONFIRM_APPROVE_DESCRIPTION',
        confirmText: 'INTERVIEW_COMMITTEES.DETAIL.APPROVE',
        cancelText: 'INTERVIEW_COMMITTEES.WIZARD.CANCEL',
        showInputField: true,
        inputType: 'textarea',
        inputLabel: 'INTERVIEW_COMMITTEES.DETAIL.DECISION_NOTES_LABEL',
        inputPlaceholder: 'INTERVIEW_COMMITTEES.DETAIL.DECISION_NOTES_PLACEHOLDER',
      })
      ?.onClose.subscribe((notes: string | undefined) => {
        if (notes === undefined) return;
        this.service.approveCommittee(committee.id, notes?.trim() || null);
      });
  }

  returnForEdit() {
    const committee = this.store.detailCommittee();
    if (!committee) return;

    this.dialogHelper
      .openConfirmDialog({
        type: 'warning',
        title: 'INTERVIEW_COMMITTEES.DETAIL.CONFIRM_RETURN_TITLE',
        description: 'INTERVIEW_COMMITTEES.DETAIL.CONFIRM_RETURN_DESCRIPTION',
        confirmText: 'INTERVIEW_COMMITTEES.DETAIL.RETURN',
        cancelText: 'INTERVIEW_COMMITTEES.WIZARD.CANCEL',
        showInputField: true,
        inputType: 'textarea',
        inputLabel: 'INTERVIEW_COMMITTEES.DETAIL.REASON_LABEL',
        inputPlaceholder: 'INTERVIEW_COMMITTEES.DETAIL.REASON_PLACEHOLDER',
      })
      ?.onClose.subscribe((reason: string | undefined) => {
        if (!reason?.trim()) return;
        this.service.returnCommittee(committee.id, reason.trim());
      });
  }

  cancelCommittee() {
    const committee = this.store.detailCommittee();
    if (!committee) return;

    this.dialogHelper
      .openConfirmDialog({
        type: 'delete',
        title: 'INTERVIEW_COMMITTEES.DETAIL.CONFIRM_CANCEL_TITLE',
        description: 'INTERVIEW_COMMITTEES.DETAIL.CONFIRM_CANCEL_DESCRIPTION',
        confirmText: 'INTERVIEW_COMMITTEES.DETAIL.CANCEL_COMMITTEE',
        cancelText: 'INTERVIEW_COMMITTEES.WIZARD.CANCEL',
        showInputField: true,
        inputType: 'textarea',
        inputLabel: 'INTERVIEW_COMMITTEES.DETAIL.REASON_LABEL',
        inputPlaceholder: 'INTERVIEW_COMMITTEES.DETAIL.REASON_PLACEHOLDER',
      })
      ?.onClose.subscribe((reason: string | undefined) => {
        if (!reason?.trim()) return;
        this.service.cancelCommittee(committee.id, reason.trim());
      });
  }
}
