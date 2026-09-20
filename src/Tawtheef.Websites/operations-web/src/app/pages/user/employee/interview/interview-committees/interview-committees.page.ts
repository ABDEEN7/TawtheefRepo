import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';

import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { InterviewCommitteesStore } from './interview-committees.store';
import { InterviewCommitteesFacade } from './interview-committees.facade';

import { CommitteesListComponent } from './views/committees-list/committees-list';
import { CommitteeWizardComponent } from './views/committee-wizard/committee-wizard';
import { CommitteeDetailComponent } from './views/committee-detail/committee-detail';

@Component({
  selector: 'app-interview-committees',
  templateUrl: './interview-committees.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [I18nNamespaceDirective, CommitteesListComponent, CommitteeWizardComponent, CommitteeDetailComponent],
  providers: [InterviewCommitteesStore, InterviewCommitteesFacade, DialogService],
})
export class InterviewCommitteesPage implements OnInit {
  store = inject(InterviewCommitteesStore);
  service = inject(InterviewCommitteesFacade);

  ngOnInit(): void {
    this.service.init();
  }
}
