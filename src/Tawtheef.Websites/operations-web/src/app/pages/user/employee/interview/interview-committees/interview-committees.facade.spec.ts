import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, ParamMap, Router } from '@angular/router';
import { BehaviorSubject, of, throwError } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';

import { NotificationService } from '../../../../../core/services/notification.service';
import { LanguageService } from '../../../../../core/services/language.service';
import { JobLookupService } from '../../job-management/services/job-lookup.service';
import { InterviewTemplatesService } from '../interview-templates/services/interview-templates.service';
import { TemplateVersionStatus } from '../interview-templates/models/enums';
import { TemplateVersionDetailsModel, TemplateVersionModel } from '../interview-templates/models/template-version.model';

import { InterviewCommitteesFacade } from './interview-committees.facade';
import { InterviewCommitteesStore } from './interview-committees.store';
import { InterviewCommitteesService } from './services/interview-committees.service';
import { CommitteeModel } from './models/committee.model';
import { CommitteeRole, CommitteeStatus, EvaluationScope } from './models/enums';
import { WizardMemberDraft, WizardUser } from './models/wizard-draft.model';

const user = (id: string): WizardUser => ({ id, nameAr: `مستخدم ${id}`, nameEn: `User ${id}`, email: `${id}@example.com` });

const member = (id: string, patch: Partial<WizardMemberDraft> = {}): WizardMemberDraft => ({
  localId: `row-${id}`,
  user: user(id),
  role: CommitteeRole.Evaluator,
  evaluationScope: EvaluationScope.AllAxes,
  axisIds: [],
  ...patch,
});

const committee = (id = 'c1'): CommitteeModel => ({
  id,
  code: 'COM-2026-0001',
  jobId: 'job-1',
  jobTitleNameAr: 'وظيفة',
  jobTitleNameEn: 'Job',
  interviewTemplateId: 'tpl-1',
  interviewTemplateTitleAr: 'قالب',
  interviewTemplateTitleEn: 'Template',
  committeeTypeId: 'type-1',
  committeeTypeNameAr: 'أكاديمي',
  committeeTypeNameEn: 'Academic',
  nameAr: 'لجنة',
  nameEn: 'Committee',
  memberCount: 3,
  status: CommitteeStatus.Draft,
  isActive: true,
});

describe('InterviewCommitteesFacade', () => {
  let facade: InterviewCommitteesFacade;
  let store: InterviewCommitteesStore;
  let api: jasmine.SpyObj<InterviewCommitteesService>;
  let templatesApi: jasmine.SpyObj<InterviewTemplatesService>;
  let notify: jasmine.SpyObj<NotificationService>;
  let router: jasmine.SpyObj<Router>;
  // Stands in for the router's query string: tests push a new value exactly as the router would after a navigation.
  let queryParams$: BehaviorSubject<ParamMap>;

  // A wizard that passes every check: job + approved template with two axes, a chair and two more members.
  function fillValidWizard() {
    // The wizard is the open view, so "back to the list after saving" is a real transition rather than the default.
    store.setView('wizard');
    store.resetWizardForCreate();
    store.updateWizardInfo({ jobId: 'job-1', templateId: 'tpl-1', nameAr: 'لجنة المقابلات', nameEn: 'Interview Committee' });
    store.setWizardTemplate({
      id: 'tpl-1',
      titleAr: 'قالب',
      titleEn: 'Template',
      versionNo: 1,
      finalScore: 100,
      qualificationScore: null,
      axes: [
        { id: 'axis-1', nameAr: 'محور 1', nameEn: 'Axis 1' },
        { id: 'axis-2', nameAr: 'محور 2', nameEn: 'Axis 2' },
      ],
    });
    store.setWizardChair(user('chair'));
    store.wizardMembers.set([member('m1'), member('m2', { role: CommitteeRole.Observer })]);
  }

  beforeEach(() => {
    api = jasmine.createSpyObj<InterviewCommitteesService>('InterviewCommitteesService', [
      'listCommittees',
      'createCommittee',
      'updateCommittee',
      'submitCommittee',
      'getCommittee',
      'listMembers',
    ]);
    api.listCommittees.and.returnValue(of([]));
    api.getCommittee.and.returnValue(of(committee()));
    api.listMembers.and.returnValue(of([]));
    templatesApi = jasmine.createSpyObj<InterviewTemplatesService>('InterviewTemplatesService', [
      'listTemplates',
      'listTemplateVersions',
      'getTemplateVersionDetails',
    ]);
    templatesApi.listTemplates.and.returnValue(of([]));
    notify = jasmine.createSpyObj<NotificationService>('NotificationService', ['success', 'error']);
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);
    router.navigate.and.resolveTo(true);
    queryParams$ = new BehaviorSubject<ParamMap>(convertToParamMap({}));

    TestBed.configureTestingModule({
      imports: [TranslateModule.forRoot()],
      providers: [
        InterviewCommitteesStore,
        InterviewCommitteesFacade,
        { provide: InterviewCommitteesService, useValue: api },
        { provide: InterviewTemplatesService, useValue: templatesApi },
        { provide: Router, useValue: router },
        { provide: ActivatedRoute, useValue: { queryParamMap: queryParams$.asObservable() } },
        { provide: JobLookupService, useValue: { loadJobStatus: () => of([]), getStatusIdByEnum: () => undefined } },
        { provide: NotificationService, useValue: notify },
        { provide: LanguageService, useValue: { get: () => 'en', current$: new BehaviorSubject('en') } },
      ],
    });

    facade = TestBed.inject(InterviewCommitteesFacade);
    store = TestBed.inject(InterviewCommitteesStore);
    fillValidWizard();
  });

  describe('validation (runs for both Save as Draft and Send for Approval)', () => {
    it('requires a chair', async () => {
      store.setWizardChair(null);

      await facade.save(false);

      expect(notify.error).toHaveBeenCalledWith('INTERVIEW_COMMITTEES.VALIDATION.CHAIR_REQUIRED');
      expect(api.createCommittee).not.toHaveBeenCalled();
    });

    it('requires a job and a template', async () => {
      store.updateWizardInfo({ jobId: '' });
      await facade.save(false);
      expect(notify.error).toHaveBeenCalledWith('INTERVIEW_COMMITTEES.VALIDATION.JOB_REQUIRED');

      store.updateWizardInfo({ jobId: 'job-1', templateId: '' });
      await facade.save(false);
      expect(notify.error).toHaveBeenCalledWith('INTERVIEW_COMMITTEES.VALIDATION.TEMPLATE_REQUIRED');
      expect(api.createCommittee).not.toHaveBeenCalled();
    });

    it('requires the backend minimum of three members counting the chair', async () => {
      store.wizardMembers.set([member('m1')]);

      await facade.save(true);

      expect(notify.error).toHaveBeenCalledWith('INTERVIEW_COMMITTEES.VALIDATION.MINIMUM_MEMBERS');
      expect(api.createCommittee).not.toHaveBeenCalled();
    });

    it('rejects a row without a selected user', async () => {
      store.wizardMembers.set([member('m1'), member('m2', { user: null })]);

      await facade.save(false);

      expect(notify.error).toHaveBeenCalledWith('INTERVIEW_COMMITTEES.VALIDATION.MEMBER_USER_REQUIRED');
    });

    it('rejects the same person twice, including the chair also listed as a member', async () => {
      store.wizardMembers.set([member('chair'), member('m2')]);

      await facade.save(false);

      expect(notify.error).toHaveBeenCalledWith('INTERVIEW_COMMITTEES.VALIDATION.MEMBER_DUPLICATE');
      expect(api.createCommittee).not.toHaveBeenCalled();
    });

    it('requires at least one axis for an evaluator scoped to selected axes', async () => {
      store.wizardMembers.set([
        member('m1', { evaluationScope: EvaluationScope.SelectedAxes, axisIds: [] }),
        member('m2'),
      ]);

      await facade.save(false);

      expect(notify.error).toHaveBeenCalledWith('INTERVIEW_COMMITTEES.VALIDATION.MEMBER_AXES_REQUIRED');
      expect(api.createCommittee).not.toHaveBeenCalled();
    });
  });

  describe('save', () => {
    it('Save as Draft creates the committee and does not submit it', async () => {
      api.createCommittee.and.returnValue(of('committee-1'));

      await facade.save(false);

      expect(api.createCommittee).toHaveBeenCalledTimes(1);
      expect(api.submitCommittee).not.toHaveBeenCalled();
      expect(notify.success).toHaveBeenCalledWith('INTERVIEW_COMMITTEES.DRAFT_SAVED');
      expect(store.view()).toBe('list');
    });

    it('Send for Approval creates the committee and then submits the id it got back', async () => {
      api.createCommittee.and.returnValue(of('committee-1'));
      api.submitCommittee.and.returnValue(of(undefined));

      await facade.save(true);

      expect(api.submitCommittee).toHaveBeenCalledOnceWith('committee-1');
      expect(notify.success).toHaveBeenCalledWith('INTERVIEW_COMMITTEES.SUBMIT_SUCCESS');
    });

    it('sends the chair as Chair evaluating all axes with full visibility, and scopes axes only for evaluators', async () => {
      store.wizardMembers.set([
        member('m1', { evaluationScope: EvaluationScope.SelectedAxes, axisIds: ['axis-2'] }),
        // Observer keeps a stale axis selection in its draft; it must never reach the API.
        member('m2', { role: CommitteeRole.Observer, evaluationScope: EvaluationScope.SelectedAxes, axisIds: ['axis-1'] }),
      ]);
      api.createCommittee.and.returnValue(of('committee-1'));

      await facade.save(false);

      const payload = api.createCommittee.calls.mostRecent().args[0];
      expect(payload.jobId).toBe('job-1');
      expect(payload.interviewTemplateId).toBe('tpl-1');
      expect(payload.nameAr).toBe('لجنة المقابلات');

      const [chair, evaluator, observer] = payload.members;
      expect(chair).toEqual(
        jasmine.objectContaining({
          memberUserId: 'chair',
          role: CommitteeRole.Chair,
          participatesInEvaluation: true,
          canSubmitEvaluation: true,
          canViewOtherEvaluations: true,
          canViewCommitteeSummary: true,
          evaluationAxisIds: [],
        }),
      );
      expect(evaluator).toEqual(
        jasmine.objectContaining({
          memberUserId: 'm1',
          role: CommitteeRole.Evaluator,
          participatesInEvaluation: true,
          canSubmitEvaluation: true,
          canViewOtherEvaluations: false,
          evaluationAxisIds: ['axis-2'],
        }),
      );
      expect(observer).toEqual(
        jasmine.objectContaining({
          memberUserId: 'm2',
          role: CommitteeRole.Observer,
          participatesInEvaluation: false,
          canAddNotes: true,
          canSubmitEvaluation: false,
          evaluationAxisIds: [],
        }),
      );
    });

    it('updates instead of creating a duplicate when a retry follows a created-but-not-submitted committee', async () => {
      api.createCommittee.and.returnValue(of('committee-1'));
      api.submitCommittee.and.returnValues(throwError(() => new Error('boom')), of(undefined));
      api.updateCommittee.and.returnValue(of(undefined));

      await facade.save(true); // created, then the submit call fails
      expect(store.view()).toBe('wizard');
      expect(store.wizardMode()).toBe('edit');

      await facade.save(true); // retry

      expect(api.createCommittee).toHaveBeenCalledTimes(1);
      expect(api.updateCommittee).toHaveBeenCalledOnceWith(jasmine.objectContaining({ id: 'committee-1' }));
      expect(api.submitCommittee).toHaveBeenCalledTimes(2);
      expect(store.view()).toBe('list');
    });
  });

  describe('roster rules in the store', () => {
    it('counts the chair toward the roster size and hides everyone already picked from the pickers', () => {
      expect(store.wizardRosterSize()).toBe(3);
      expect([...store.wizardSelectedUserIds()].sort()).toEqual(['chair', 'm1', 'm2']);
    });
  });

  // Switching language reloads the whole app, so "which committee is open" has to live in the URL to survive it.
  describe('detail view is driven by ?committee=<id>', () => {
    const openUrl = (id: string | null) => queryParams$.next(convertToParamMap(id ? { committee: id } : {}));
    const navigatedTo = (id: string | null) =>
      router.navigate.calls.allArgs().some(([, extras]) => extras?.queryParams?.['committee'] === id);

    beforeEach(() => {
      store.setView('list'); // the outer setup leaves the wizard open
    });

    it('opens that committee straight away when the page loads on ?committee=<id> (the language-switch reload)', () => {
      openUrl('c1');

      facade.init();

      expect(store.view()).toBe('detail');
      expect(api.getCommittee).toHaveBeenCalledOnceWith('c1');
      expect(api.listMembers).toHaveBeenCalledOnceWith('c1');
      expect(store.detailCommittee()?.id).toBe('c1');
    });

    it('stays on the list when there is no ?committee param', () => {
      facade.init();

      expect(store.view()).toBe('list');
      expect(api.getCommittee).not.toHaveBeenCalled();
    });

    it('opening a committee puts its id in the URL, and the URL change is what opens the detail', () => {
      facade.init();

      facade.openDetail(committee('c9'));

      expect(router.navigate).toHaveBeenCalledWith([], jasmine.objectContaining({ queryParamsHandling: 'merge' }));
      expect(navigatedTo('c9')).toBeTrue();
      expect(store.detailCommittee()?.id).toBe('c9'); // header pre-filled from the list row while the record loads

      openUrl('c9'); // what the router does next

      expect(store.view()).toBe('detail');
      expect(api.getCommittee).toHaveBeenCalledOnceWith('c9');
    });

    it('back-to-list removes the param', () => {
      facade.init();

      facade.backToList();

      expect(navigatedTo(null)).toBeTrue();
    });

    it('returns to the list, and reloads it, when the param goes away (back button, sidebar link)', () => {
      openUrl('c1');
      facade.init();
      api.listCommittees.calls.reset();

      openUrl(null);

      expect(store.view()).toBe('list');
      expect(api.listCommittees).toHaveBeenCalledTimes(1);
    });

    it('sends the user back to the list when the committee in the URL cannot be loaded', () => {
      api.getCommittee.and.returnValue(throwError(() => new Error('404')));
      openUrl('gone');

      facade.init();

      expect(navigatedTo(null)).toBeTrue();
    });

    it('editing from the detail clears the param only once the wizard is showing, and the wizard is not bounced to the list', () => {
      // Only the fields the facade reads; the rest of the models is irrelevant to this test.
      templatesApi.listTemplateVersions.and.returnValue(
        of([{ id: 'v1', status: TemplateVersionStatus.Approved }] as unknown as TemplateVersionModel[]),
      );
      templatesApi.getTemplateVersionDetails.and.returnValue(
        of({ versionNo: 1, finalScore: 100, qualificationScore: null, axes: [] } as unknown as TemplateVersionDetailsModel),
      );
      openUrl('c1');
      facade.init();
      router.navigate.calls.reset();

      facade.openEditWizard(committee('c1'));

      expect(store.view()).toBe('wizard');
      expect(navigatedTo(null)).toBeTrue();

      openUrl(null); // the router applies that navigation

      expect(store.view()).toBe('wizard');
    });
  });
});
