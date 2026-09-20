import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { Select } from 'primeng/select';
import { Tooltip } from 'primeng/tooltip';

import { HasPermissionDirective } from '../../../../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../../../../core/constants/permissions';
import { InterviewCommitteesStore } from '../../interview-committees.store';
import { InterviewCommitteesFacade } from '../../interview-committees.facade';
import { CommitteeModel } from '../../models/committee.model';
import { COMMITTEE_STATUS_LABELS, COMMITTEE_STATUS_PILL, CommitteeStatus } from '../../models/enums';

@Component({
  selector: 'app-committees-list',
  templateUrl: './committees-list.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, FormsModule, TranslatePipe, TableModule, ButtonModule, Select, Tooltip, HasPermissionDirective],
})
export class CommitteesListComponent {
  readonly Permissions = Permissions;
  store = inject(InterviewCommitteesStore);
  service = inject(InterviewCommitteesFacade);

  readonly statusOptions = [
    CommitteeStatus.Draft,
    CommitteeStatus.PendingApproval,
    CommitteeStatus.Returned,
    CommitteeStatus.Approved,
    CommitteeStatus.Stopped,
    CommitteeStatus.Closed,
    CommitteeStatus.Cancelled,
  ].map((status) => ({ value: status, label: COMMITTEE_STATUS_LABELS[status] }));

  private localized(ar?: string | null, en?: string | null): string {
    return this.store.isRtl() ? ar || en || '' : en || ar || '';
  }

  committeeName(committee: CommitteeModel): string {
    return this.localized(committee.nameAr, committee.nameEn);
  }

  jobTitle(committee: CommitteeModel): string {
    return this.localized(committee.jobTitleNameAr, committee.jobTitleNameEn);
  }

  chairName(committee: CommitteeModel): string {
    return this.localized(committee.chairNameAr, committee.chairNameEn);
  }

  statusLabel(status: CommitteeStatus): string {
    return COMMITTEE_STATUS_LABELS[status];
  }

  statusPill(status: CommitteeStatus): string {
    return COMMITTEE_STATUS_PILL[status];
  }

  // Only a Draft or Returned committee can be edited; the backend locks the core fields after that.
  canEdit(committee: CommitteeModel): boolean {
    return committee.status === CommitteeStatus.Draft || committee.status === CommitteeStatus.Returned;
  }
}
