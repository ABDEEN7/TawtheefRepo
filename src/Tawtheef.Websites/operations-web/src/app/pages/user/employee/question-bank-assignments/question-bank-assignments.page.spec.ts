import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { of } from 'rxjs';
import { LanguageService } from '../../../../core/services/language.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { I18nFeatureLoader } from '../../../../shared/loaders/i18n-feature.loader';
import { AssignmentStatuses, MyAssignment } from './models/question-bank-assignment.models';
import { QuestionBankAssignmentsPage } from './question-bank-assignments.page';
import { QuestionBankAssignmentsService } from './services/question-bank-assignments.service';

describe('Employee question assignment actions', () => {
  it('offers an action only for accessible assignments and refuses finalized navigation', async () => {
    const active = { assignmentId: 'active', canAccess: true, statusId: AssignmentStatuses.assigned } as MyAssignment;
    const final = { assignmentId: 'final', canAccess: false, statusId: AssignmentStatuses.workflowCompleted } as MyAssignment;
    const issued = { assignmentId: 'issued', canAccess: false, statusId: AssignmentStatuses.completed } as MyAssignment;
    const navigate = jasmine.createSpy('navigate');
    TestBed.configureTestingModule({
      imports: [QuestionBankAssignmentsPage, TranslateModule.forRoot()],
      providers: [
        { provide: Router, useValue: { navigateByUrl: navigate } },
        { provide: QuestionBankAssignmentsService, useValue: { list: () => of({ items: [active, final, issued] }) } },
        { provide: LanguageService, useValue: { get: () => 'en' } },
        { provide: NotificationService, useValue: {} },
        { provide: I18nFeatureLoader, useValue: { ensureLoaded: () => Promise.resolve() } },
      ],
    });
    const fixture = TestBed.createComponent(QuestionBankAssignmentsPage);
    fixture.detectChanges();
    await fixture.whenStable();
    expect(fixture.nativeElement.querySelectorAll('tbody .action-buttons button').length).toBe(1);
    const page = fixture.componentInstance;
    page.open(final);
    page.open(issued);
    expect(navigate).not.toHaveBeenCalled();
    page.open(active);
    expect(navigate).toHaveBeenCalledTimes(1);
  });
});
