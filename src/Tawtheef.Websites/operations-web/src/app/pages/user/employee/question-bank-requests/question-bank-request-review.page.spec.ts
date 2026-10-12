import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { of } from 'rxjs';
import { LanguageService } from '../../../../core/services/language.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { I18nFeatureLoader } from '../../../../shared/loaders/i18n-feature.loader';
import { QuestionBankAssignmentsService } from '../question-bank-assignments/services/question-bank-assignments.service';
import {
  QuestionBankRequestReview, QuestionBankRequestStatusIds, QuestionReviewDecisionIds,
} from './models/question-bank-request.models';
import { QuestionBankRequestReviewPage } from './question-bank-request-review.page';
import { QuestionBankRequestsService } from './services/question-bank-requests.service';

describe('QuestionBankRequestReviewPage initial decisions', () => {
  let service: jasmine.SpyObj<QuestionBankRequestsService>;
  let review: QuestionBankRequestReview;

  beforeEach(() => {
    review = {
      requestId: 'request',
      requestStatusId: QuestionBankRequestStatusIds.PendingReview,
      currentReviewRound: 0,
      items: Array.from({ length: 105 }, (_, index) => ({
        requestItemId: `item-${index}`,
        currentProposedRevisionId: `revision-${index}`,
        changeTypeId: 'add',
        originalOptions: [],
        options: [],
      })),
    } as unknown as QuestionBankRequestReview;
    service = jasmine.createSpyObj('requests', ['review', 'submitReview']);
    service.review.and.callFake(() => of(review));
    service.submitReview.and.returnValue(of({ issued: true, changesRequired: false, reviewRound: 1 }));
    TestBed.configureTestingModule({
      imports: [QuestionBankRequestReviewPage, TranslateModule.forRoot()],
      providers: [
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: new Map([['id', 'request']]) } } },
        { provide: Router, useValue: { navigateByUrl: jasmine.createSpy('navigate') } },
        { provide: QuestionBankRequestsService, useValue: service },
        { provide: QuestionBankAssignmentsService, useValue: { image: jasmine.createSpy('image') } },
        { provide: LanguageService, useValue: { get: () => 'en', current$: of('en') } },
        { provide: NotificationService, useValue: { success: () => {}, error: () => {} } },
        { provide: I18nFeatureLoader, useValue: { ensureLoaded: () => Promise.resolve() } },
      ],
    });
  });

  it('checks all 105 Approved radios and sends untouched defaults in the actual submission', async () => {
    const fixture = TestBed.createComponent(QuestionBankRequestReviewPage);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
    // PrimeNG keeps its value on the component rather than the native input.
    const checked = fixture.nativeElement.querySelectorAll('input[type="radio"]:checked');
    expect(checked.length).toBe(105);
    expect(fixture.componentInstance.reviewIncomplete()).toBeFalse();
    fixture.componentInstance.submit();
    const [id, inputs] = service.submitReview.calls.mostRecent().args;
    expect(id).toBe('request');
    expect(inputs.length).toBe(105);
    expect(inputs.every(input => input.decisionId === QuestionReviewDecisionIds.Approved)).toBeTrue();
    expect(service.review).toHaveBeenCalledTimes(1);
    expect(TestBed.inject(QuestionBankAssignmentsService).image).not.toHaveBeenCalled();
  });

  it('lets a reviewer change one radio and submits only that changed decision', async () => {
    const fixture = TestBed.createComponent(QuestionBankRequestReviewPage);
    fixture.detectChanges();
    await fixture.whenStable();
    const radios = fixture.nativeElement.querySelectorAll('input[type="radio"]');
    (radios[1] as HTMLInputElement).click();
    fixture.detectChanges();
    await fixture.whenStable();
    const page = fixture.componentInstance;
    expect(page.value('item-0').decisionId).toBe(QuestionReviewDecisionIds.NeedsModification);
    page.value('item-0').reviewNote = 'Please update this question';
    expect(page.reviewIncomplete()).toBeFalse();
    page.submit();
    const inputs = service.submitReview.calls.mostRecent().args[1];
    expect(inputs[0].decisionId).toBe(QuestionReviewDecisionIds.NeedsModification);
    expect(inputs.slice(1).every(input => input.decisionId === QuestionReviewDecisionIds.Approved)).toBeTrue();
  });

  it('preserves saved decisions and notes in correction rounds and defaults only undecided items', () => {
    review.items[0].decisionId = QuestionReviewDecisionIds.NeedsModification;
    review.items[0].reviewNote = 'Existing correction note';
    review.items[1].decisionId = QuestionReviewDecisionIds.Rejected;
    review.items[1].reviewNote = 'Existing rejection note';
    review.items[2].decisionId = QuestionReviewDecisionIds.Approved;
    review.items[2].reviewNote = 'Existing approval note';
    review.items[3].decisionId = null;
    const fixture = TestBed.createComponent(QuestionBankRequestReviewPage);
    fixture.detectChanges();
    const page = fixture.componentInstance;
    expect(page.value('item-0')).toEqual({
      decisionId: QuestionReviewDecisionIds.NeedsModification, reviewNote: 'Existing correction note',
    });
    expect(page.value('item-1').decisionId).toBe(QuestionReviewDecisionIds.Rejected);
    expect(page.value('item-2').reviewNote).toBe('Existing approval note');
    expect(page.value('item-3').decisionId).toBe(QuestionReviewDecisionIds.Approved);
    fixture.detectChanges();
    page.submit();
    const inputs = service.submitReview.calls.mostRecent().args[1];
    expect(inputs[0].decisionId).toBe(QuestionReviewDecisionIds.NeedsModification);
    expect(inputs[1].decisionId).toBe(QuestionReviewDecisionIds.Rejected);
  });
});
