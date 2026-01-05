import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ToggleSwitch } from 'primeng/toggleswitch';
import { InputTextModule } from 'primeng/inputtext';

import { OrganizationStructuresStore } from '../../organization-structures.store';
import { OrganizationStructuresFacade } from '../../organization-structures.facade';
import {Select} from 'primeng/select';
import {PaginationComponent} from '../../../../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-departments-tab',
  standalone: true,
  templateUrl: './departments-tab.html',
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    ToggleSwitch,
    InputTextModule,
    PaginationComponent,
    Select
  ]
})
export class DepartmentsTabComponent {
  store = inject(OrganizationStructuresStore);
  service = inject(OrganizationStructuresFacade);

  statusOptions = [
    { label: 'ORG_STRUCTURES.ACTIVE', value: true },
    { label: 'ORG_STRUCTURES.INACTIVE', value: false },
  ];
}
