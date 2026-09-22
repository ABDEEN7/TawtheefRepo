import { Component, inject, Input } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { TestSlotCandidatesDialogComponent } from '../test-slot-candidates-dialog/test-slot-candidates-dialog.component';

@Component({
  selector: 'app-test-slot-candidates',
  standalone: true,
  templateUrl: './test-slot-candidates.component.html',
  styleUrl: './test-slot-candidates.component.scss',
  imports: [TranslatePipe],
  providers: [DialogService],
})
export class TestSlotCandidatesComponent {
  @Input({ required: true }) testSlotId!: string;
  private readonly dialogs = inject(DialogService);
  private readonly translate = inject(TranslateService);

  openCandidates(): void {
    this.dialogs.open(TestSlotCandidatesDialogComponent, {
      header: this.translate.instant('TEST_SLOT_DETAILS.CANDIDATES'),
      width: 'min(90rem, 96vw)',
      modal: true,
      closable: true,
      dismissableMask: false,
      data: { testSlotId: this.testSlotId },
    });
  }
}
