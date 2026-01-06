import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { OrganizationStructuresStore } from './organization-structures.store';
import { OrganizationStructuresFacade } from './organization-structures.facade';

import { SectorsTabComponent } from './tabs/sectors-tab/sectors-tab';
import { ManagementsTabComponent } from './tabs/managements-tab/managements-tab';
import { DepartmentsTabComponent } from './tabs/departments-tab/departments-tab';
import { DialogService } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-organization-structures',
  standalone: true,
  templateUrl: './organization-structures.page.html',
  styleUrls: ['./organization-structures.page.scss'],
  imports: [
    CommonModule,
    TranslatePipe,
    I18nNamespaceDirective,
    SectorsTabComponent,
    ManagementsTabComponent,
    DepartmentsTabComponent
  ],
  providers: [
    OrganizationStructuresStore,
    OrganizationStructuresFacade,
    DialogService
  ]
})
export class OrganizationStructuresPage implements OnInit {
  store = inject(OrganizationStructuresStore);
  service = inject(OrganizationStructuresFacade);

  ngOnInit(): void {
    this.service.init();
  }
}
