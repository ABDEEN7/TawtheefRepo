import { HttpErrorResponse, HttpRequest } from '@angular/common/http';
import { fakeAsync, flushMicrotasks, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { of, throwError } from 'rxjs';
import { errorInterceptor } from '../../../../core/interceptors/error.interceptor';
import { LanguageService } from '../../../../core/services/language.service';
import { NotificationService } from '../../../../core/services/notification.service';
import {
  AssignmentStatuses, AssignmentWorkspace, MaintenanceBaseQuestion, QuestionChangeTypes,
  QuestionInput, RequestItemStatuses,
} from './models/question-bank-assignment.models';
import { QuestionEntryPage } from './question-entry.page';
import { QuestionBankAssignmentsService } from './services/question-bank-assignments.service';

describe('QuestionEntryPage maintenance errors', () => {
  const claimCode = 'QUESTION_BANK_QUESTION_ALREADY_ASSIGNED_FOR_MAINTENANCE';
  const question = { questionId: 'question' } as MaintenanceBaseQuestion;
  let page: QuestionEntryPage;
  let failure: HttpErrorResponse;
  let service: jasmine.SpyObj<QuestionBankAssignmentsService>;
  let notification: jasmine.SpyObj<NotificationService>;

  beforeEach(() => {
    service = jasmine.createSpyObj('assignments', ['workspace', 'updateBaseQuestion', 'deleteBaseQuestion']);
    service.workspace.and.returnValue(of(null as any));
    notification = jasmine.createSpyObj('notification', ['error', 'success']);
    TestBed.configureTestingModule({
      providers: [
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: new Map([['assignmentId', 'assignment']]) } } },
        { provide: Router, useValue: { navigateByUrl: jasmine.createSpy('navigate') } },
        { provide: QuestionBankAssignmentsService, useValue: service },
        { provide: NotificationService, useValue: notification },
        { provide: LanguageService, useValue: { get: () => 'en' } },
        { provide: TranslateService, useValue: {
          instant: (key: string) => key === `server-error.${claimCode}`
            ? 'QUESTION_ASSIGNMENTS.QUESTION_CLAIMED' : key,
        } },
        { provide: DialogService, useValue: { open: () => ({ onClose: of({} as QuestionInput) }) } },
        { provide: ConfirmationService, useValue: { confirm: (config: { accept: () => void }) => config.accept() } },
      ],
    });
    const request = () => TestBed.runInInjectionContext(() => errorInterceptor(
      new HttpRequest('POST', '/question-bank-assignments/assignment/base-questions/question', {}),
      () => throwError(() => failure),
    ));
    service.updateBaseQuestion.and.callFake(() => request() as any);
    service.deleteBaseQuestion.and.callFake(() => request() as any);
    page = TestBed.runInInjectionContext(() => new QuestionEntryPage());
  });

  for (const action of ['editBase', 'deleteBase', 'proposeDeletion'] as const) {
    it(`${action} refreshes a genuine claim conflict and shows one claim notification`, fakeAsync(() => {
      // The API currently returns FluentResults error codes even on a 500 response.
      failure = new HttpErrorResponse({ status: 500, error: { error: [{ message: claimCode }] } });
      page[action](question as any);
      flushMicrotasks();
      expect(service.workspace).toHaveBeenCalledTimes(2);
      expect(notification.error).toHaveBeenCalledOnceWith('• QUESTION_ASSIGNMENTS.QUESTION_CLAIMED');
    }));

    for (const status of [400, 500]) {
      it(`${action} leaves an unrelated ${status} error to the interceptor without a second toast`, fakeAsync(() => {
        failure = new HttpErrorResponse({ status, error: { error: [{ message: 'UNRELATED_FAILURE' }] } });
        page[action](question as any);
        flushMicrotasks();
        expect(service.workspace).toHaveBeenCalledTimes(1);
        expect(notification.error).toHaveBeenCalledOnceWith('• UNRELATED_FAILURE');
      }));
    }
  }
});

describe('QuestionEntryPage maintenance row styling', () => {
  for (const language of ['en', 'ar']) {
    it(`keeps ADD white, UPDATE amber, DELETE red, and the base table unchanged in ${language}`, async () => {
      const workspace = {
        assignment: { statusId: AssignmentStatuses.inProgress },
        questionBank: {},
        progress: { minimumQuestionCount: 5, currentQuestionCount: 2, remainingQuestionCount: 3, canFinish: false },
        baseQuestions: [{ questionId: 'base', ownership: 'Available' }],
        questions: [QuestionChangeTypes.add, QuestionChangeTypes.update, QuestionChangeTypes.delete].map((changeTypeId) => ({
          changeTypeId, statusId: RequestItemStatuses.draft,
        })),
      } as unknown as AssignmentWorkspace;
      TestBed.configureTestingModule({
        imports: [QuestionEntryPage, TranslateModule.forRoot()],
        providers: [
          { provide: ActivatedRoute, useValue: { snapshot: { paramMap: new Map([['assignmentId', 'assignment']]) } } },
          { provide: Router, useValue: {} },
          { provide: QuestionBankAssignmentsService, useValue: { workspace: () => of(workspace) } },
          { provide: NotificationService, useValue: {} },
          { provide: LanguageService, useValue: { get: () => language } },
        ],
      });
      const translate = TestBed.inject(TranslateService);
      const labels = language === 'ar' ? ['إضافة', 'تعديل', 'حذف'] : ['Add', 'Update', 'Delete'];
      translate.setTranslation(language, {
        'QUESTION_ASSIGNMENTS.CHANGE_ADD': labels[0],
        'QUESTION_ASSIGNMENTS.CHANGE_UPDATE': labels[1],
        'QUESTION_ASSIGNMENTS.CHANGE_DELETE': labels[2],
      });
      translate.use(language);
      const fixture = TestBed.createComponent(QuestionEntryPage);
      fixture.detectChanges();
      await fixture.whenStable();
      fixture.detectChanges();
      const tables = fixture.nativeElement.querySelectorAll('p-table');
      const baseRow = tables[0].querySelector('tbody tr');
      expect(baseRow.className).not.toContain('maintenance-');
      const rows = tables[1].querySelectorAll('tbody tr');
      const classes = ['maintenance-add', 'maintenance-update', 'maintenance-delete'];
      const colors = ['rgb(255, 255, 255)', 'rgb(255, 251, 235)', 'rgb(255, 241, 242)'];
      expect(rows.length).toBe(3);
      rows.forEach((row: HTMLTableRowElement, index: number) => {
        expect(row.classList.contains(classes[index])).toBeTrue();
        expect(getComputedStyle(row.cells[0]).backgroundColor).toBe(colors[index]);
        expect(row.cells[3].textContent?.trim()).toBe(labels[index]);
      });
      expect(fixture.componentInstance.percent()).toBe(40);
    });
  }
});
