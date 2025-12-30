// majors-skills-management.page.ts
import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';

import { MajorsSkillsManagementStore } from './majors-skills-management.store';

import { DialogService } from 'primeng/dynamicdialog';
import {MappingTabComponent} from './tabs/mapping-tab/mapping-tab';
import {MainMajorsTabComponent} from './tabs/main-majors-tab/main-majors-tab';
import {SubMajorsTabComponent} from './tabs/sub-majors-tab/sub-majors-tab';
import {SkillsTabComponent} from './tabs/skills-tab/skills-tab';

@Component({
  selector: 'app-majors-skills-management',
  standalone: true,
  templateUrl: './majors-skills-management.html',
  styleUrls: ['./majors-skills-management.scss'],
  imports: [
    CommonModule,
    TranslatePipe,
    I18nNamespaceDirective,
    MappingTabComponent,
    MainMajorsTabComponent,
    SubMajorsTabComponent,
    SkillsTabComponent,
  ],
  providers: [
    MajorsSkillsManagementStore,
    DialogService
  ]
})
export class MajorsSkillsManagementPage implements OnInit {
  store = inject(MajorsSkillsManagementStore);

  ngOnInit(): void {
    this.store.init();
  }
}
