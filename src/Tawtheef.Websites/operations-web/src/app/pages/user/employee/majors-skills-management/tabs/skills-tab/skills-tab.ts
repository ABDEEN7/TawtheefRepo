// tabs/sub-majors-tab.component.ts
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
import {Select} from 'primeng/select';
import {HasPermissionDirective} from '../../../../../../shared/directives/has-permission.directive';
import {Permissions} from '../../../../../../core/constants/permissions';

@Component({
  selector: 'app-skills-tab',
  standalone: true,
  templateUrl: './skills-tab.html',
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    ToggleSwitch,
    PaginationComponent,
    Select,
    HasPermissionDirective
  ]
})
export class SkillsTabComponent {
  readonly Permissions = Permissions;
  store = inject(MajorsSkillsManagementStore);
  service = inject(MajorsSkillsManagementFacade);

  setSkillGeneral(v: any) {
    this.service.setSkillGeneral(v);
  }
}
