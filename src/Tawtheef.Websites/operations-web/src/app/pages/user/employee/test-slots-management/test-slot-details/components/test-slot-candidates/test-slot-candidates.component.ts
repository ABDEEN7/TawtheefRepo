import { Component, DestroyRef, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { TestSlotCandidatesDialogComponent } from '../../dialogs/test-slot-candidates-dialog/test-slot-candidates-dialog.component';
import { Permissions } from '../../../../../../core/constants/permissions';

@Component({
  selector: 'app-test-slot-candidates',
  standalone: true,
  templateUrl: './test-slot-candidates.component.html',
  styleUrl: './test-slot-candidates.component.scss',
  imports: [TranslatePipe],
  providers: [DialogService],
})
export class TestSlotCandidatesComponent implements OnInit {
  readonly permissions = Permissions;
  @Input({ required: true }) testSlotId!: string;
  @Input({ required: true }) isCurrentUserRoomHead!: boolean;
  @Input() sendingNotifications = false;
  @Output() sendNotifications = new EventEmitter<void>();
  private readonly dialogs = inject(DialogService);
  private readonly translate = inject(TranslateService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    this.route.queryParamMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => {
        const testSessionId = params.get('testSessionId');
        if (testSessionId) this.openCandidates(testSessionId);
      });
  }

  openCandidates(testSessionId?: string): void {
    const dialogRef = this.dialogs.open(TestSlotCandidatesDialogComponent, {
      header: this.translate.instant('TEST_SLOT_DETAILS.CANDIDATES'),
      width: 'min(90rem, 96vw)',
      modal: true,
      closable: true,
      dismissableMask: false,
      data: { testSlotId: this.testSlotId, isCurrentUserRoomHead: this.isCurrentUserRoomHead, testSessionId },
    });
    if (testSessionId) {
      dialogRef?.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => {
        void this.router.navigate([], {
          relativeTo: this.route,
          queryParams: { testSessionId: null },
          queryParamsHandling: 'merge',
        });
      });
    }
  }

  sendNotification(): void {
    this.sendNotifications.emit();
  }
}
