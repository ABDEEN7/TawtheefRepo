import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';

import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { InterviewTemplatesStore } from './interview-templates.store';
import { InterviewTemplatesFacade } from './interview-templates.facade';

import { TemplatesListComponent } from './views/templates-list/templates-list';
import { TemplateWizardComponent } from './views/template-wizard/template-wizard';
import { TemplateDetailComponent } from './views/template-detail/template-detail';

@Component({
  selector: 'app-interview-templates',
  standalone: true,
  templateUrl: './interview-templates.page.html',
  imports: [
    CommonModule,
    I18nNamespaceDirective,
    TemplatesListComponent,
    TemplateWizardComponent,
    TemplateDetailComponent,
  ],
  providers: [InterviewTemplatesStore, InterviewTemplatesFacade, DialogService],
})
export class InterviewTemplatesPage implements OnInit {
  store = inject(InterviewTemplatesStore);
  service = inject(InterviewTemplatesFacade);

  ngOnInit(): void {
    this.service.init();
  }
}
