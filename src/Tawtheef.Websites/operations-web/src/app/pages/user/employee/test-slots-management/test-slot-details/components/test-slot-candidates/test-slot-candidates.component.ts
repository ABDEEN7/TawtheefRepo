import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { Permissions } from '../../../../../../../core/constants/permissions';
import { TestSlotCandidatesDialogComponent } from '../../dialogs/test-slot-candidates-dialog/test-slot-candidates-dialog.component';
import { HasPermissionDirective } from '../../../../../../../shared/directives/has-permission.directive';

@Component({
  selector: 'app-test-slot-candidates',
  standalone: true,
  templateUrl: './test-slot-candidates.component.html',
  styleUrl: './test-slot-candidates.component.scss',
  imports: [TranslatePipe, HasPermissionDirective],
  providers: [DialogService],
})
export class TestSlotCandidatesComponent {
  readonly permissions = Permissions;
  @Input({ required: true }) testSlotId!: string;
  @Input({ required: true }) isCurrentUserRoomHead!: boolean;
  @Input() sendingNotifications = false;
  @Output() sendNotifications = new EventEmitter<void>();
  private readonly dialogs = inject(DialogService);
  private readonly translate = inject(TranslateService);

  openCandidates(testSessionId?: string): void {
    this.dialogs.open(TestSlotCandidatesDialogComponent, {
      header: this.translate.instant('TEST_SLOT_DETAILS.CANDIDATES'),
      width: 'min(90rem, 96vw)',
      modal: true,
      closable: true,
      dismissableMask: false,
      data: { testSlotId: this.testSlotId, isCurrentUserRoomHead: this.isCurrentUserRoomHead, testSessionId },
    });
  }

  sendNotification(): void {
    this.sendNotifications.emit();
  }
}
