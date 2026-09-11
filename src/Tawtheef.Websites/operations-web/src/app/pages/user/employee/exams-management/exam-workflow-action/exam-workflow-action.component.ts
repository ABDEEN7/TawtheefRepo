import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { HasPermissionDirective } from '../../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../../core/constants/permissions';

@Component({
  selector: 'app-exam-workflow-action',
  standalone: true,
  templateUrl: './exam-workflow-action.component.html',
  styleUrl: './exam-workflow-action.component.scss',
  imports: [ButtonModule, TranslatePipe, HasPermissionDirective],
})
export class ExamWorkflowActionComponent {
  @Input({ required: true }) statusBackendName: string | null = null;
  @Input() submitting = false;

  @Output() approveClicked = new EventEmitter<void>();
  @Output() returnClicked = new EventEmitter<void>();
  @Output() rejectClicked = new EventEmitter<void>();

  protected readonly workflowPermission = Permissions.Exams.WorkflowActions;
  protected readonly pendingApprovalStatus = 'PendingApproval';
}
