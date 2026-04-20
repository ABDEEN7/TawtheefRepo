import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { JobDetailsService } from '../services/job-details.service';

@Component({
  selector: 'app-job-qualifications',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  template: `
    <div class="row g-3">
      @if (degreeRequirements()) {
      <div class="col-md-6 col-lg-4">
        <div class="requirement-card">
          <div class="d-flex align-items-center gap-2 mb-2">
            <i class="hgi-stroke hgi-graduation-cap-01"></i>
            <span class="label">{{ 'JOB_DETAILS.QUALIFICATION_REQUIREMENTS' | translate }}</span>
          </div>
          <span class="value">{{ degreeRequirements() }}</span>
        </div>
      </div>
      }
      @if (job()?.major) {
      <div class="col-md-6 col-lg-4">
        <div class="requirement-card">
          <div class="d-flex align-items-center gap-2 mb-2">
            <i class="hgi-stroke hgi-book-01"></i>
            <span class="label">{{ 'JOB_DETAILS.MAJOR' | translate }}</span>
          </div>
          <span class="value">{{ job()!.major!.name }}</span>
        </div>
      </div>
      }
      @if (job()?.subMajor) {
      <div class="col-md-6 col-lg-4">
        <div class="requirement-card">
          <div class="d-flex align-items-center gap-2 mb-2">
            <i class="hgi-stroke hgi-book-02"></i>
            <span class="label">{{ 'JOB_DETAILS.SUB_MAJOR' | translate }}</span>
          </div>
          <span class="value">{{ job()!.subMajor!.name }}</span>
        </div>
      </div>
      }
      @if (specializationRequirements()) {
      <div class="col-12">
        <div class="requirement-card">
          <div class="d-flex align-items-center gap-2 mb-2">
            <i class="hgi-stroke hgi-star"></i>
            <span class="label">{{ 'JOB_DETAILS.SPECIALIZATIONS' | translate }}</span>
          </div>
          <span class="value">{{ specializationRequirements() }}</span>
        </div>
      </div>
      }
    </div>
    @if (job()?.qualificationDescription) {
    <div class="row mt-4">
      <div class="col-md-12">
        <h5 class="section-title">{{ 'JOB_DETAILS.QUALIFICATIONS' | translate }}</h5>
        <div class="text-area">
          <div [innerHTML]="job()!.qualificationDescription"></div>
        </div>
      </div>
    </div>
    }
  `,
  styles: [`
    .requirement-card {
      background: #fbfbfb;
      border: 1px solid #e6e6e6;
      border-radius: 12px;
      padding: 16px;
      height: 100%;
      .label {
        font-size: 11px;
        color: #999;
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.5px;
      }
      .value {
        font-size: 14px;
        color: #333;
        font-weight: 700;
        display: block;
        padding-left: 26px;
      }
      i {
        font-size: 18px;
        color: #8a1538;
      }
    }
    .section-title {
      font-size: 16px;
      font-weight: 700;
      color: #8a1538;
      margin-bottom: 16px;
    }
  `]
})
export class JobQualificationsComponent {
  private detailsService = inject(JobDetailsService);
  job = this.detailsService.job;

  degreeRequirements = computed(() => {
    if (!this.job()?.degrees?.length) return '';
    const degreeNames = this.job()!.degrees!.map(degree => degree.degree.name);
    return degreeNames.join(', ') || '';
  });

  specializationRequirements = computed(() => {
    if (!this.job()?.jobSpecializations?.length) return '';
    const specs = this.job()!.jobSpecializations!.map(
      (spec) => `${spec.major?.name || ''} - ${spec.subMajor?.name || ''}`
    );
    return specs.filter((s) => s !== ' - ').join(', ') || '';
  });
}
