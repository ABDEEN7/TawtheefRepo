import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToggleSwitchModule } from 'primeng/toggleswitch';

import { OrganizationStructuresStore } from '../../organization-structures.store';
import { OrganizationStructuresFacade } from '../../organization-structures.facade';
import {PaginationComponent} from '../../../../../../shared/components/pagination/pagination.component';
import {RemoteSelectComponent} from '../../../../../../shared/components/remote-select/remote-select';
import {Select} from 'primeng/select';
import {Tooltip} from 'primeng/tooltip';

@Component({
  selector: 'app-managements-tab',
  standalone: true,
  templateUrl: './managements-tab.html',
  styleUrls: ['../../organization-structures-actions.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    InputTextModule,
    PaginationComponent,
    RemoteSelectComponent,
    Select,
    ToggleSwitchModule,
    Tooltip
  ]
})
export class ManagementsTabComponent {
  store = inject(OrganizationStructuresStore);
  service = inject(OrganizationStructuresFacade);

  statusOptions = [
    { label: 'ORG_STRUCTURES.ACTIVE', value: true },
    { label: 'ORG_STRUCTURES.INACTIVE', value: false },
  ];
}
