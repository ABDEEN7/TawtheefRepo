import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { Select } from 'primeng/select';
import { ButtonModule } from 'primeng/button';
import { ToggleSwitch } from 'primeng/toggleswitch';
import { TooltipModule } from 'primeng/tooltip';
import {RemoteSelectComponent} from '../../../../../../shared/components/remote-select/remote-select';
import {MajorsSkillsManagementStore} from '../../majors-skills-management.store';
import { MajorsSkillsManagementFacade } from '../../majors-skills-management.facade';
import {PaginationComponent} from '../../../../../../shared/components/pagination/pagination.component';
import {Checkbox} from 'primeng/checkbox';
@Component({
  selector: 'app-mapping-tab',
  standalone: true,
  templateUrl: './mapping-tab.html',
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TableModule,
    Select,
    ButtonModule,
    ToggleSwitch,
    TooltipModule,
    RemoteSelectComponent,
    PaginationComponent,
    Checkbox
  ]
})
export class MappingTabComponent {
  store = inject(MajorsSkillsManagementStore);
  service = inject(MajorsSkillsManagementFacade);

  setMajorSkillActiveOnly(evt: { checked: boolean; }){
    this.service.setMajorSkillActiveOnly(evt.checked)
  }
}
