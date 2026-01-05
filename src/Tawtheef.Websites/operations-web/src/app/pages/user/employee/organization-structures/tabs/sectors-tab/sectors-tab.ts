import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ToggleSwitch } from 'primeng/toggleswitch';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';

import { OrganizationStructuresStore } from '../../organization-structures.store';
import { OrganizationStructuresFacade } from '../../organization-structures.facade';
import { PaginationComponent } from '../../../../../shared/components/pagination/pagination.component';

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
    DropdownModule,
    PaginationComponent
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
