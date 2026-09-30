import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { Permissions } from '../../../../../../core/constants/permissions';
import { HasPermissionDirective } from '../../../../../../shared/directives/has-permission.directive';
import { TEST_SESSION_STATUS_IDS } from '../../models/test-session-list-item.dto';

@Component({
  selector: 'app-test-session-workflow-action',
  standalone: true,
  templateUrl: './test-session-workflow-action.component.html',
  styleUrl: '../../../exams-management/exam-workflow-action/exam-workflow-action.component.scss',
  imports: [ButtonModule, TranslatePipe, HasPermissionDirective],
})
export class TestSessionWorkflowActionComponent {
  @Input() statusId: string | null = null;
  @Input() submitting = false;

  @Output() approveClicked = new EventEmitter<void>();
  @Output() returnClicked = new EventEmitter<void>();
  @Output() rejectClicked = new EventEmitter<void>();

  protected readonly workflowPermission = Permissions.TestSessions.WorkflowActions;
  protected readonly pendingApprovalStatusId = TEST_SESSION_STATUS_IDS.pendingApproval;
}
