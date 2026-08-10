import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ToggleSwitch } from 'primeng/toggleswitch';
import {MajorsSkillsManagementStore} from '../../majors-skills-management.store';
import { MajorsSkillsManagementFacade } from '../../majors-skills-management.facade';
import {PaginationComponent} from '../../../../../../shared/components/pagination/pagination.component';
import {HasPermissionDirective} from '../../../../../../shared/directives/has-permission.directive';
import {Permissions} from '../../../../../../core/constants/permissions';


@Component({
  selector: 'app-main-majors-tab',
  standalone: true,
  templateUrl: './main-majors-tab.html',
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    ToggleSwitch,
    PaginationComponent,
    HasPermissionDirective
  ]
})
export class MainMajorsTabComponent {
  readonly Permissions = Permissions;
  store = inject(MajorsSkillsManagementStore);
  service = inject(MajorsSkillsManagementFacade);
}
