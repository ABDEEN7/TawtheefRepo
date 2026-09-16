import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { LanguageService } from '../../../../../../core/services/language.service';
import { ExamJobDto } from '../../models/exam-job.dto';
import { ExamLookupsDto } from '../../models/exam-lookups.dto';
import { examForm } from '../helper/exam-wizard.form';

@Component({
  selector: 'app-exam-information-step',
  standalone: true,
  templateUrl: './exam-information-step.component.html',
  styleUrl: './exam-information-step.component.scss',
  imports: [
    ReactiveFormsModule,
    TranslatePipe,
    CheckboxModule,
    InputTextModule,
    SelectModule,
    TextareaModule,
  ],
})
export class ExamInformationStepComponent {
  @Input({ required: true }) form!: ReturnType<typeof examForm>;
  @Input({ required: true }) lookups!: ExamLookupsDto;
  @Input() jobs: ExamJobDto[] = [];
  @Input() isViewMode = false;
  @Output() jobSearch = new EventEmitter<string>();
  @Output() jobSelected = new EventEmitter<string>();

  readonly language = inject(LanguageService);

  jobOptions() {
    return this.jobs.map((job) => ({
      ...job,
      label: job.jobNumber + ' — ' + (this.language.isRtl ? job.nameAr : job.nameEn),
    }));
  }

  onJobSelected(): void {
    this.jobSelected.emit(this.form.controls.jobId.value);
  }
}
