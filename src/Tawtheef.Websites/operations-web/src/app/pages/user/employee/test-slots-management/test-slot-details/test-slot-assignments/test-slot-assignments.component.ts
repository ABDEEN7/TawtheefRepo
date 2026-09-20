import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { TableModule } from 'primeng/table';
import { Permissions } from '../../../../../../core/constants/permissions';
import { HasPermissionDirective } from '../../../../../../shared/directives/has-permission.directive';
import { TestSlotStaffDetailsDto } from '../../models/test-slot-details.dto';
import { TestSlotAssignmentDialogComponent } from '../test-slot-assignment-dialog/test-slot-assignment-dialog.component';
@Component({
  selector: 'app-test-slot-assignments',
  standalone: true,
  templateUrl: './test-slot-assignments.component.html',
  styleUrl: './test-slot-assignments.component.scss',
  imports: [TableModule, TranslatePipe, HasPermissionDirective],
  providers: [DialogService],
})
export class TestSlotAssignmentsComponent {
  readonly permissions = Permissions;
  @Input({ required: true }) staff!: TestSlotStaffDetailsDto[];
  @Input({ required: true }) testSlotId!: string;
  @Output() assignmentsUpdated = new EventEmitter<void>();

  private readonly dialogs = inject(DialogService);
  private readonly translate = inject(TranslateService);

  openAssignmentDialog(): void {
    const dialog = this.dialogs.open(TestSlotAssignmentDialogComponent, {
      header: this.translate.instant('TEST_SLOT_DETAILS.ADD_ASSIGNMENT'),
      width: 'min(72rem, 95vw)',
      modal: true,
      closable: true,
      dismissableMask: false,
      data: { testSlotId: this.testSlotId },
    });

    dialog?.onClose.subscribe(updated => {
      if (updated) this.assignmentsUpdated.emit();
    });
  }
}
