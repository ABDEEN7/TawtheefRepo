import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';

import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { InterviewResultReportStore } from './interview-result-report.store';
import { InterviewResultReportFacade } from './interview-result-report.facade';

import { ReportsListComponent } from './views/reports-list/reports-list';
import { ReportDetailComponent } from './views/report-detail/report-detail';

@Component({
  selector: 'app-interview-result-report',
  templateUrl: './interview-result-report.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [I18nNamespaceDirective, ReportsListComponent, ReportDetailComponent],
  providers: [InterviewResultReportStore, InterviewResultReportFacade, DialogService],
})
export class InterviewResultReportPage implements OnInit {
  store = inject(InterviewResultReportStore);
  service = inject(InterviewResultReportFacade);

  ngOnInit(): void {
    this.service.init();
  }
}
