import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ToggleSwitch } from 'primeng/toggleswitch';
import { InputTextModule } from 'primeng/inputtext';

import {PaginationComponent} from '../../../../../../shared/components/pagination/pagination.component';
import { OrganizationStructuresStore } from '../../organization-structures.store';
import { OrganizationStructuresFacade } from '../../organization-structures.facade';
import {Select} from 'primeng/select';

@Component({
  selector: 'app-sectors-tab',
  standalone: true,
  templateUrl: './sectors-tab.html',
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
export class SectorsTabComponent {
  store = inject(OrganizationStructuresStore);
  service = inject(OrganizationStructuresFacade);

  statusOptions = [
    { label: 'ORG_STRUCTURES.STATUS_ALL', value: null },
    { label: 'ORG_STRUCTURES.ACTIVE', value: true },
    { label: 'ORG_STRUCTURES.INACTIVE', value: false },
  ];
}
