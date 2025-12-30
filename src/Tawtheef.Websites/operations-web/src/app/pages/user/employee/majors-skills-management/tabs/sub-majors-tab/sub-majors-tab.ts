import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ToggleSwitch } from 'primeng/toggleswitch';
import {RemoteSelectComponent} from '../../../../../../shared/components/remote-select/remote-select';
import {MajorsSkillsManagementStore} from '../../majors-skills-management.store';
import {PaginationComponent} from '../../../../../../shared/components/pagination/pagination.component';


@Component({
  selector: 'app-sub-majors-tab',
  standalone: true,
  templateUrl: './sub-majors-tab.html',
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    ToggleSwitch,
    RemoteSelectComponent,
    PaginationComponent
  ]
})
export class SubMajorsTabComponent {
  store = inject(MajorsSkillsManagementStore);
}
