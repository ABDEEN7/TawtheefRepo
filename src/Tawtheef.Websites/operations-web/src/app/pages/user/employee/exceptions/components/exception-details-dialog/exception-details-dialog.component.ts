import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  inject,
  signal
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslatePipe } from '@ngx-translate/core';
import {
  DynamicDialogConfig,
  DynamicDialogRef
} from 'primeng/dynamicdialog';
import { Skeleton } from 'primeng/skeleton';
import { finalize } from 'rxjs';

import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { I18nNamespaceDirective } from '../../../../../../shared/directives/i18n-namespace.directive';
import { GUID } from '../../../../../../shared/types/guid.type';
import { InvitationExceptionDetails } from '../../models/invitation-exception.models';
import { invitationExceptionStatusPresentations } from '../../models/invitation-exception-status.presentation';
import { ExceptionsService } from '../../services/exceptions.service';

export interface ExceptionDetailsDialogData {
  exceptionId: GUID;
}

@Component({
  selector: 'app-exception-details-dialog',
  standalone: true,
  imports: [
    CommonModule,
    TranslatePipe,
    Skeleton,
    I18nNamespaceDirective
  ],
  templateUrl: './exception-details-dialog.component.html',
  styleUrl: './exception-details-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ExceptionDetailsDialogComponent implements OnInit {
  private readonly config =
    inject(DynamicDialogConfig<ExceptionDetailsDialogData>);

  private readonly dialogRef = inject(DynamicDialogRef);
  private readonly exceptionsService = inject(ExceptionsService);
  private readonly fileUtils = inject(FileUtilsService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly details =
    signal<InvitationExceptionDetails | null>(null);

  protected readonly loading = signal(false);
  protected readonly downloading = signal(false);

  protected readonly statusPresentations =
    invitationExceptionStatusPresentations;

  private readonly exceptionId =
    this.config.data?.exceptionId;

  ngOnInit(): void {
    if (!this.exceptionId) {
      this.dialogRef.close();
      return;
    }

    this.loading.set(true);

    this.exceptionsService
      .getDetails(this.exceptionId)
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: details => this.details.set(details)
      });
  }

  protected downloadProof(): void {
    const details = this.details();
    const proof = details?.proof;

    if (
      !this.exceptionId ||
      !proof ||
      this.downloading()
    ) {
      return;
    }

    this.downloading.set(true);

    this.exceptionsService
      .getProof(this.exceptionId)
      .pipe(
        finalize(() => this.downloading.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: response =>
          void this.fileUtils.downloadResponse(
            response,
            proof.fileName
          )
      });
  }

  protected close(): void {
    this.dialogRef.close();
  }
}