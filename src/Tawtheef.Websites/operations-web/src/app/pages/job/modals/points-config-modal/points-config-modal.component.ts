import { Component, computed, signal, inject } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { PointsConfig } from '../../models/points-config.model';
import { JobService } from '../../services/job.service';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {TranslatePipe} from '@ngx-translate/core';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-points-config-modal',
  standalone: true,
  templateUrl: './points-config-modal.component.html',
  imports: [
    I18nNamespaceDirective,
    TranslatePipe,
    FormsModule
  ],
  styleUrl: './points-config-modal.component.scss'
})
export class PointsConfigModalComponent {
  private jobService = inject(JobService);

  jobId: number;
  jobTitle: string;
  pointsConfig = signal<PointsConfig>(this.getDefaultPointsConfig());

  groups = ['degree', 'exp', 'langs', 'skills', 'avail', 'geo'] as const;

  sumTotals = computed(() => Object.values(this.pointsConfig().totals).reduce((a, b) => a + b, 0));
  groupSums = computed(() => {
    const rubrics = this.pointsConfig().rubrics;
    return this.groups.reduce((acc, key) => {
      acc[key] = Object.values(rubrics[key]).reduce((a, b) => a + b, 0);
      return acc;
    }, {} as Record<typeof this.groups[number], number>);
  });
  isValid = computed(() => {
    if (this.sumTotals() !== 1000) return false;
    const totals = this.pointsConfig().totals;
    return this.groups.every(key => this.groupSums()[key] === totals[key]);
  });

  rubricLabels: Record<typeof this.groups[number], string> = {
    degree: 'lbl_degree',
    exp: 'lbl_experience',
    langs: 'lbl_languages',
    skills: 'lbl_skills',
    avail: 'lbl_availability',
    geo: 'lbl_geography'
  };

  constructor(public ref: DynamicDialogRef, public config: DynamicDialogConfig) {
    this.jobId = this.config.data.jobId;
    this.jobTitle = this.config.data.jobTitle;
    this.pointsConfig.set(this.loadPointsConfig(this.jobId));
  }

  updateTotal(key: typeof this.groups[number], value: number) {
    this.pointsConfig.update(cfg => ({
      ...cfg,
      totals: { ...cfg.totals, [key]: value }
    }));
  }

  updateRubric(group: typeof this.groups[number], opt: string, value: number) {
    this.pointsConfig.update(cfg => ({
      ...cfg,
      rubrics: {
        ...cfg.rubrics,
        [group]: { ...cfg.rubrics[group], [opt]: value }
      }
    }));
  }

  async save() {
    if (this.isValid()) {
      try {
        // Save to localStorage
        this.savePointsConfig(this.jobId, this.pointsConfig());

        // Call API to save job settings
        await this.jobService.saveJobPointsConfig(this.jobId, this.pointsConfig());

        this.ref.close(true);
      } catch (error) {
        console.error(error);
        // Handle error (show toast message, etc.)
      }
    }
  }

  resetToDefaults() {
    this.pointsConfig.set(this.getDefaultPointsConfig());
  }

  private getDefaultPointsConfig(): PointsConfig {
    return {
      totals: { degree: 250, exp: 200, langs: 150, skills: 150, avail: 100, geo: 150 },
      rubrics: {
        degree: { 'دكتوراه': 70, 'ماجستير': 60, 'بكالوريوس': 50, 'دبلوم': 35, 'ثانوي': 20, 'الإعدادية': 10, 'الابتدائية': 5 },
        exp: { '10+': 60, '7-9': 50, '4-6': 40, '1-3': 30, '0-1': 20 },
        langs: { ' لغتان': 60, 'لغة واحدة قوية': 45, 'لغة متوسطة': 30, 'أساسية': 15 },
        skills: { '>=80%': 60, '60-79%': 45, '40-59%': 30, '<40%': 15 },
        avail: { 'فوري+داخل قطر': 40, 'خلال شهر': 30, 'خارج الدولة': 20, 'غير متاح': 10 },
        geo: { 'مطابق للسياسة': 70, 'أولوية': 50, 'عام': 30 }
      }
    };
  }

  private loadPointsConfig(jobId: number): PointsConfig {
    const stored = localStorage.getItem(`eduhire_points_cfg_${jobId}`);
    return stored ? JSON.parse(stored) : this.getDefaultPointsConfig();
  }

  private savePointsConfig(jobId: number, config: PointsConfig) {
    localStorage.setItem(`eduhire_points_cfg_${jobId}`, JSON.stringify(config));
  }

  protected readonly Object = Object;
}
