import { Component, computed, signal, inject } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { PointsConfig } from '../../models/points-config.model';
import { JobService } from '../../services/job.service';
import { PointsConfigService } from '../../services/points-config.service';
import { POINTS_CONFIG_CONSTANTS } from '../../constants/points-config.constants';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { TranslatePipe } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { GUID } from '../../../../shared/types/guid.type';
import { NotificationService } from '../../../../core/services/notification.service';
import { PointsGroup } from '../../types/points-group.type';

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
  private notificationService = inject(NotificationService);
  private pointsConfigService = inject(PointsConfigService);
  
  readonly TOTAL_POINTS = POINTS_CONFIG_CONSTANTS.TOTAL_POINTS;
  readonly groups = this.pointsConfigService.getGroupLabels();
  
  jobId: GUID;
  jobTitle: string;
  pointsConfig = signal<PointsConfig>(this.getInitialPointsConfig());

  // Computed signals with proper typing
  sumTotals = computed(() => 
    this.groups.reduce((sum, group) => sum + this.getTotalValue(group), 0)
  );
  
  groupSums = computed(() => {
    const result: Record<PointsGroup, number> = {} as Record<PointsGroup, number>;
    this.groups.forEach(group => {
      result[group] = this.calculateGroupSum(group);
    });
    return result;
  });
  
  isValid = computed(() => {
    if (this.sumTotals() !== this.TOTAL_POINTS) return false;
    return this.groups.every(group => this.groupSums()[group] === this.getTotalValue(group));
  });

  constructor(public ref: DynamicDialogRef, public config: DynamicDialogConfig) {
    this.jobId = this.config.data.jobId;
    this.jobTitle = this.config.data.jobTitle;
  }

  // Type-safe methods for template access
  updateTotal(group: PointsGroup, value: number) {
    this.pointsConfig.update(cfg => ({
      ...cfg,
      totals: { ...cfg.totals, [group]: value }
    }));
  }

  updateRubric(group: PointsGroup, option: string, value: number) {
    this.pointsConfig.update(cfg => ({
      ...cfg,
      rubrics: {
        ...cfg.rubrics,
        [group]: { ...cfg.rubrics[group], [option]: value }
      }
    }));
  }

  getTotalValue(group: PointsGroup): number {
    return this.pointsConfigService.getTotalValue(this.pointsConfig(), group);
  }

  getRubricValue(group: PointsGroup, option: string): number {
    return this.pointsConfigService.getRubricValue(this.pointsConfig(), group, option);
  }

  getRubricOptions(group: PointsGroup): string[] {
    return this.pointsConfigService.getRubricOptions(this.pointsConfig(), group);
  }

  getRubricLabel(group: PointsGroup): string {
    return this.pointsConfigService.getRubricLabel(group);
  }

  private calculateGroupSum(group: PointsGroup): number {
    const rubricValues = this.pointsConfig().rubrics[group];
    return Object.values(rubricValues).reduce((sum: number, value: number) => sum + value, 0);
  }

  async save() {
    if (this.isValid()) {
      try {
        await this.jobService.saveJobPointsConfig(this.jobId, this.pointsConfig());
        this.ref.close(true);
      } catch (error) {
        this.notificationService.error(error as string);
      }
    }
  }

  resetToDefaults() {
    this.pointsConfig.set(this.pointsConfigService.getDefaultPointsConfig());
  }

  private getInitialPointsConfig(): PointsConfig {
    return  this.pointsConfigService.getDefaultPointsConfig();
  }
}