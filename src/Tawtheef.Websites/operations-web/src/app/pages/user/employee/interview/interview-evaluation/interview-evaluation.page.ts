import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';

import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { InterviewEvaluationStore } from './interview-evaluation.store';
import { InterviewEvaluationFacade } from './interview-evaluation.facade';

import { SessionsListComponent } from './views/sessions-list/sessions-list';
import { SessionDetailComponent } from './views/session-detail/session-detail';
import { CandidateEvaluationComponent } from './views/candidate-evaluation/candidate-evaluation';

@Component({
  selector: 'app-interview-evaluation',
  templateUrl: './interview-evaluation.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [I18nNamespaceDirective, SessionsListComponent, SessionDetailComponent, CandidateEvaluationComponent],
  providers: [InterviewEvaluationStore, InterviewEvaluationFacade, DialogService],
})
export class InterviewEvaluationPage implements OnInit {
  store = inject(InterviewEvaluationStore);
  service = inject(InterviewEvaluationFacade);

  ngOnInit(): void {
    this.service.init();
  }
}
