import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';

import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { InterviewScheduleStore } from './interview-schedule.store';
import { InterviewScheduleFacade } from './interview-schedule.facade';

import { SchedulesListComponent } from './views/schedules-list/schedules-list';
import { ScheduleWizardComponent } from './views/schedule-wizard/schedule-wizard';
import { ScheduleDetailComponent } from './views/schedule-detail/schedule-detail';

@Component({
  selector: 'app-interview-schedule',
  templateUrl: './interview-schedule.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [I18nNamespaceDirective, SchedulesListComponent, ScheduleWizardComponent, ScheduleDetailComponent],
  providers: [InterviewScheduleStore, InterviewScheduleFacade, DialogService],
})
export class InterviewSchedulePage implements OnInit {
  store = inject(InterviewScheduleStore);
  service = inject(InterviewScheduleFacade);

  ngOnInit(): void {
    this.service.init();
  }
}
