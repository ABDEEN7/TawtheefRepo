import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';

import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { InterviewAxesCriteriaStore } from './interview-axes-criteria.store';
import { InterviewAxesCriteriaFacade } from './interview-axes-criteria.facade';

import { AxesTabComponent } from './tabs/axes-tab/axes-tab';
import { CriteriaTabComponent } from './tabs/criteria-tab/criteria-tab';

@Component({
  selector: 'app-interview-axes-criteria',
  standalone: true,
  templateUrl: './interview-axes-criteria.page.html',
  styleUrls: ['./interview-axes-criteria.page.scss'],
  imports: [
    CommonModule,
    TranslatePipe,
    I18nNamespaceDirective,
    AxesTabComponent,
    CriteriaTabComponent
  ],
  providers: [
    InterviewAxesCriteriaStore,
    InterviewAxesCriteriaFacade,
    DialogService
  ]
})
export class InterviewAxesCriteriaPage implements OnInit {
  store = inject(InterviewAxesCriteriaStore);
  service = inject(InterviewAxesCriteriaFacade);

  ngOnInit(): void {
    this.service.init();
  }
}
