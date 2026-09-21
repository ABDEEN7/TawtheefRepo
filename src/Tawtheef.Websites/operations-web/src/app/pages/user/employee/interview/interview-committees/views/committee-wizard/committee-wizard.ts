import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { Select } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';

import { InterviewCommitteesStore, WizardStep } from '../../interview-committees.store';
import { InterviewCommitteesFacade } from '../../interview-committees.facade';
import { LazySelectComponent } from '../../components/lazy-select/lazy-select';
import { LazySelectOption } from '../../models/lazy-select-option.model';
import {
  COMMITTEE_ROLE_LABELS,
  CommitteeRole,
  EvaluationScope,
  MEMBER_ROLES,
  MINIMUM_COMMITTEE_MEMBERS,
} from '../../models/enums';
import {
  participatesInEvaluation,
  WizardJobContext,
  WizardMemberDraft,
  WizardTemplateAxis,
  WizardUser,
} from '../../models/wizard-draft.model';

// Everything the review table shows is precomputed: a p-table row is untyped in the template, so the row carries the
// ready-made translation key instead of the template indexing a lookup with an `any`.
interface ReviewRow {
  name: string;
  roleKey: string;
  evaluationKey: string;
  axes: string[];
}

@Component({
  selector: 'app-committee-wizard',
  templateUrl: './committee-wizard.html',
  styleUrls: ['./committee-wizard.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, TranslatePipe, Select, InputTextModule, TableModule, LazySelectComponent],
})
export class CommitteeWizardComponent {
  store = inject(InterviewCommitteesStore);
  service = inject(InterviewCommitteesFacade);

  readonly EvaluationScope = EvaluationScope;
  readonly minimumMembers = MINIMUM_COMMITTEE_MEMBERS;

  readonly steps: WizardStep[] = [1, 2, 3];
  readonly stepLabels: Record<WizardStep, string> = {
    1: 'INTERVIEW_COMMITTEES.WIZARD.STEP1_LABEL',
    2: 'INTERVIEW_COMMITTEES.WIZARD.STEP2_LABEL',
    3: 'INTERVIEW_COMMITTEES.WIZARD.STEP3_LABEL',
  };

  readonly roleOptions = MEMBER_ROLES.map((role) => ({ value: role, label: COMMITTEE_ROLE_LABELS[role] }));
  readonly scopeOptions = [
    { value: EvaluationScope.AllAxes, label: 'INTERVIEW_COMMITTEES.SCOPE.ALL_AXES' },
    { value: EvaluationScope.SelectedAxes, label: 'INTERVIEW_COMMITTEES.SCOPE.SELECTED_AXES' },
  ];

  readonly searchJobs = this.service.searchJobs;
  readonly searchMembers = this.service.searchEligibleMembers;

  // Rows hold live controls (dropdowns, checkboxes); tracking by id keeps a row's DOM alive when its draft object is replaced.
  readonly trackByLocalId = (_index: number, member: WizardMemberDraft) => member.localId;

  // Picked users are rendered from these; caching keeps each option's identity stable between change-detection
  // passes so the child dropdown isn't handed a "new" selection every time.
  private userOptionCache = new Map<string, LazySelectOption<WizardUser>>();

  // Job and template are fixed once the committee exists - the backend's update command doesn't carry them.
  readonly isEditMode = computed(() => this.store.wizardMode() === 'edit');

  readonly titleKey = computed(() =>
    this.isEditMode() ? 'INTERVIEW_COMMITTEES.WIZARD.EDIT_TITLE' : 'INTERVIEW_COMMITTEES.WIZARD.CREATE_TITLE',
  );

  readonly axes = computed<WizardTemplateAxis[]>(() => this.store.wizardTemplate()?.axes ?? []);

  readonly jobOption = computed<LazySelectOption<WizardJobContext> | null>(() => {
    const job = this.store.wizardJob();
    if (!job) return null;
    return { value: job.id, label: this.localized(job.titleAr, job.titleEn), hint: job.jobNumber, data: job };
  });

  readonly templateOptions = computed(() => {
    const options = this.store.approvedTemplates().map((t) => ({
      value: t.id,
      label: this.localized(t.titleAr, t.titleEn) + (t.latestVersionNo ? ` (v${t.latestVersionNo})` : ''),
    }));

    // An edited committee's template may no longer be the newest approved one; keep it selectable so it still shows.
    const current = this.store.wizardTemplate();
    if (current && !options.some((o) => o.value === current.id)) {
      options.unshift({ value: current.id, label: `${this.localized(current.titleAr, current.titleEn)} (v${current.versionNo})` });
    }
    return options;
  });

  readonly committeeName = computed(() => {
    const info = this.store.wizardInfo();
    return this.localized(info.nameAr, info.nameEn);
  });

  readonly reviewRows = computed<ReviewRow[]>(() => {
    const rows: ReviewRow[] = [];

    const chair = this.store.wizardChair();
    if (chair) {
      rows.push({
        name: this.service.userLabel(chair),
        roleKey: COMMITTEE_ROLE_LABELS[CommitteeRole.Chair],
        evaluationKey: 'INTERVIEW_COMMITTEES.SCOPE.ALL_AXES',
        axes: [],
      });
    }

    for (const member of this.store.wizardMembers()) {
      const evaluates = participatesInEvaluation(member.role);
      const selected = evaluates && member.evaluationScope === EvaluationScope.SelectedAxes;
      rows.push({
        name: member.user ? this.service.userLabel(member.user) : '—',
        roleKey: COMMITTEE_ROLE_LABELS[member.role],
        evaluationKey: !evaluates
          ? 'INTERVIEW_COMMITTEES.SCOPE.NOT_EVALUATING'
          : selected
            ? 'INTERVIEW_COMMITTEES.SCOPE.SELECTED_AXES'
            : 'INTERVIEW_COMMITTEES.SCOPE.ALL_AXES',
        axes: selected ? member.axisIds.map((id) => this.axisName(id)) : [],
      });
    }
    return rows;
  });

  localized(ar?: string | null, en?: string | null): string {
    return this.store.isRtl() ? ar || en || '' : en || ar || '';
  }

  axisName(axisId: string): string {
    const axis = this.axes().find((a) => a.id === axisId);
    return axis ? this.localized(axis.nameAr, axis.nameEn) : '';
  }

  axisLabel(axis: WizardTemplateAxis): string {
    return this.localized(axis.nameAr, axis.nameEn);
  }

  userOption(user: WizardUser | null): LazySelectOption<WizardUser> | null {
    if (!user) return null;

    const key = `${user.id}|${this.store.currentLang()}`;
    let option = this.userOptionCache.get(key);
    if (!option) {
      option = { value: user.id, label: this.service.userLabel(user), hint: user.email, data: user };
      this.userOptionCache.set(key, option);
    }
    return option;
  }

  evaluates(member: WizardMemberDraft): boolean {
    return participatesInEvaluation(member.role);
  }

  onChairSelected(option: LazySelectOption<WizardUser> | null) {
    this.service.setChair(option?.data ?? null);
  }

  onMemberSelected(member: WizardMemberDraft, option: LazySelectOption<WizardUser> | null) {
    this.service.updateMember(member.localId, { user: option?.data ?? null });
  }

  onAxisToggle(member: WizardMemberDraft, axisId: string, event: Event) {
    this.service.toggleMemberAxis(member, axisId, (event.target as HTMLInputElement).checked);
  }

  goToStep(step: WizardStep) {
    this.service.goToStep(step);
  }

  saveDraft() {
    void this.service.save(false);
  }

  sendForApproval() {
    void this.service.save(true);
  }
}
