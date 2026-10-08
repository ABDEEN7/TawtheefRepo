import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { RadioButton } from 'primeng/radiobutton';
import { TagModule } from 'primeng/tag';
import { TextareaModule } from 'primeng/textarea';
import { finalize } from 'rxjs';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { portalRoutes } from '../../../../routes/portal-routes';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { RichContentRendererComponent } from '../../../../shared/rich-content/rich-content-renderer.component';
import { QuestionBankAssignmentsService } from '../question-bank-assignments/services/question-bank-assignments.service';
import {
  QuestionBankRequestReview,
  QuestionChangeTypeIds,
  QuestionReviewDecisionIds,
  QuestionReviewInput,
} from './models/question-bank-request.models';
import { QuestionBankRequestsService } from './services/question-bank-requests.service';
import { questionBankRequestStatusSeverity } from '../../../../shared/utils/question-bank-status.util';

interface ReviewFormValue {
  decisionId?: string;
  reviewNote: string;
}

@Component({
  selector: 'app-question-bank-request-review',
  standalone: true,
  templateUrl: './question-bank-request-review.page.html',
  styleUrl: './question-bank-request-review.page.scss',
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    I18nNamespaceDirective,
    ButtonModule,
    RadioButton,
    TagModule,
    TextareaModule,
    RichContentRendererComponent,
  ],
})
export class QuestionBankRequestReviewPage implements OnInit, OnDestroy {
  private readonly service = inject(QuestionBankRequestsService);
  private readonly assignmentService = inject(QuestionBankAssignmentsService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly language = inject(LanguageService);
  private readonly notification = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  private readonly destroyRef = inject(DestroyRef);
  readonly requestId = this.route.snapshot.paramMap.get('id')!;
  readonly review = signal<QuestionBankRequestReview | null>(null);
  readonly loading = signal(false);
  readonly submitting = signal(false);
  readonly submitted = signal(false);
  readonly currentLang = signal<Lang>(this.language.get());
  readonly decisions = QuestionReviewDecisionIds;
  readonly changeTypes = QuestionChangeTypeIds;

  changeTypeLabel(changeTypeId: string): string {
    if (changeTypeId === this.changeTypes.Update) return 'QUESTION_BANK_REQUESTS.CHANGE_UPDATE';
    if (changeTypeId === this.changeTypes.Delete) return 'QUESTION_BANK_REQUESTS.CHANGE_DELETE';
    return 'QUESTION_BANK_REQUESTS.CHANGE_ADD';
  }

  imageChangeLabel(item: QuestionBankRequestReview['items'][number]): string {
    if (!item.originalImageUrl && item.imageUrl) return 'QUESTION_BANK_REQUESTS.IMAGE_ADDED';
    if (item.originalImageUrl && !item.imageUrl) return 'QUESTION_BANK_REQUESTS.IMAGE_REMOVED';
    if (item.originalImageUrl && item.imageUrl && item.originalImageUrl !== item.imageUrl)
      return 'QUESTION_BANK_REQUESTS.IMAGE_REPLACED';
    return 'QUESTION_BANK_REQUESTS.IMAGE_KEPT';
  }
  readonly requestStatusSeverity = questionBankRequestStatusSeverity;
  readonly values = new Map<string, ReviewFormValue>();
  readonly imageUrls = new Map<string, string>();

  ngOnInit(): void {
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((language) => this.currentLang.set(language));
    this.load();
  }

  name(ar?: string | null, en?: string | null): string {
    return (this.currentLang() === 'ar' ? ar : en) || '-';
  }

  value(itemId: string): ReviewFormValue {
    if (!this.values.has(itemId)) this.values.set(itemId, { reviewNote: '' });
    return this.values.get(itemId)!;
  }

  noteRequired(itemId: string): boolean {
    const decision = this.value(itemId).decisionId;
    return decision === this.decisions.NeedsModification || decision === this.decisions.Rejected;
  }

  itemInvalid(itemId: string): boolean {
    const value = this.value(itemId);
    return !value.decisionId || (this.noteRequired(itemId) && !value.reviewNote.trim());
  }

  reviewIncomplete(): boolean {
    const review = this.review();
    return !review || review.items.some((item) => this.itemInvalid(item.requestItemId));
  }

  allApproved(): boolean {
    const review = this.review();
    return (
      !!review &&
      review.items.length > 0 &&
      !this.reviewIncomplete() &&
      review.items.every(
        (item) => this.value(item.requestItemId).decisionId === this.decisions.Approved,
      )
    );
  }

  submitLabel(): string {
    return this.allApproved()
      ? 'QUESTION_BANK_REQUESTS.APPROVE_AND_ISSUE'
      : 'QUESTION_BANK_REQUESTS.SUBMIT_REVIEW_RESULT';
  }

  back(): void {
    void this.router.navigateByUrl(portalRoutes.questionBankRequestDetails(this.requestId));
  }

  submit(): void {
    this.submitted.set(true);
    const review = this.review();
    if (!review || this.reviewIncomplete()) return;
    const reviews: QuestionReviewInput[] = review.items.map((item) => ({
      requestItemId: item.requestItemId,
      reviewedRevisionId: item.currentProposedRevisionId,
      decisionId: this.value(item.requestItemId).decisionId!,
      reviewNote: this.value(item.requestItemId).reviewNote.trim() || null,
    }));
    this.submitting.set(true);
    this.service
      .submitReview(this.requestId, reviews)
      .pipe(
        finalize(() => this.submitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (result) => {
          this.notification.success(
            this.translate.instant(
              result.changesRequired || !result.issued
                ? 'QUESTION_BANK_REQUESTS.REVIEW_CHANGES_SUCCESS'
                : 'QUESTION_BANK_REQUESTS.REVIEW_APPROVED_SUCCESS',
            ),
          );
          this.back();
        },
        error: () =>
          this.notification.error(
            this.translate.instant('QUESTION_BANK_REQUESTS.REVIEW_SUBMIT_ERROR'),
          ),
      });
  }

  ngOnDestroy(): void {
    this.imageUrls.forEach((url) => URL.revokeObjectURL(url));
  }

  private load(): void {
    this.loading.set(true);
    this.service
      .review(this.requestId)
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (review) => {
          this.review.set(review);
          review.items.forEach((item) => {
            this.values.set(item.requestItemId, { reviewNote: '' });
            if (item.imageUrl) this.loadImage(item.requestItemId, item.imageUrl);
            if (item.originalImageUrl)
              this.loadImage(`original-${item.requestItemId}`, item.originalImageUrl);
          });
        },
        error: () => {
          this.notification.error(
            this.translate.instant('QUESTION_BANK_REQUESTS.REVIEW_NOT_AVAILABLE'),
          );
          this.back();
        },
      });
  }

  private loadImage(itemId: string, blobKey: string): void {
    this.assignmentService
      .image(blobKey)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (blob) => this.imageUrls.set(itemId, URL.createObjectURL(blob)),
      });
  }
}
