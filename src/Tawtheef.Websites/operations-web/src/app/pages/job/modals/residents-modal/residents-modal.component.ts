import { Component, inject } from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService, ScrollerOptions } from 'primeng/api';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { TranslatePipe } from '@ngx-translate/core';
import { Select } from 'primeng/select';
import { FormsModule } from '@angular/forms';
import { JobLookupService } from '../../services/job-lookup.service';
import { ResidentBreakdown } from '../../models/resident-breakdown.model';
import { GuidUtils } from '../../../../core/utils/guid-utils';
import { GUID } from '../../../../shared/types/guid.type';

@Component({
  selector: 'app-residents-modal',
  standalone: true,
  imports: [I18nNamespaceDirective, TranslatePipe, Select, FormsModule],
  templateUrl: './residents-modal.component.html',
})
export class ResidentsModalComponent {
  lookupsService = inject(JobLookupService);
  messageService = inject(MessageService)

  breakdown: ResidentBreakdown[] = [];
  nationality: GUID = GuidUtils.emptyGuid;
  percent = 0;
  target = 0;
  lazyLoading = false;
  loadLazyTimeout = 0;
  get total() {
    return this.breakdown.reduce((a, b) => a + b.percentage, 0);
  }

  get remainingPercentage(): number {
    return this.target - this.total;
  }

  get isOverTarget(): boolean {
    return this.total > this.target;
  }

  constructor(public ref: DynamicDialogRef, public config: DynamicDialogConfig) {
    if (config.data) {
      this.target = config.data.target || 0;
      this.breakdown = config.data.breakdown ? [...config.data.breakdown] : [];
    }
  }

  nationalitieOptions: ScrollerOptions = {
    delay: 250,
    showLoader: true,
    lazy: true,
    onLazyLoad: this.loadNationalitiesLazy.bind(this),
  };

  loadNationalitiesLazy(event: any) {
    this.lazyLoading = true;
    this.loadLazyTimeout = setTimeout(() => {
      const { first, last } = event;
      const items = [...this.lookupsService.nationalities()];
      for (let i = first; i < last; i++) {
        items[i] = this.lookupsService.nationalities()[i];
      }
      this.lookupsService.nationalities.set(items);
      this.lazyLoading = false;
    }, Math.random() * 1000 + 250);
  }

   add() {
    if (!this.nationality || this.percent <= 0) return;
    this.validateBreakdownAgainstTarget();
    this.breakdown.push({nationalityId: this.nationality, percentage: this.percent});
    this.nationality = GuidUtils.emptyGuid;
    this.percent = 0;
  }

  validateBreakdownAgainstTarget(): void {
    const newTotal = this.total + this.percent;
    if (newTotal > this.target) {
      var message = {severity: 'error', summary: 'Breakdown total exceeds residents target', detail: `Breakdown total (${this.total}%) exceeds residents target (${this.target}%)`}
      this.messageService.add(message)
    }
  }

  remove(i: number) {
    this.breakdown.splice(i, 1);
  }

  save() {
    this.ref.close(this.breakdown);
  }

  close() {
    this.ref.close(null);
  }
}
