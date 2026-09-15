import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Select } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { finalize } from 'rxjs';
import { dropdownOptionsModel } from '../../../../../../shared/models/dropdown-options.model';
import { QuestionBankTypeIds } from '../../models/question-bank-request.models';
import { QuestionBankRequestsService } from '../../services/question-bank-requests.service';

@Component({ selector: 'app-create-question-bank-request-dialog', standalone: true,
  templateUrl: './create-question-bank-request.dialog.component.html',
  styleUrls: ['./create-question-bank-request.dialog.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, Select, TextareaModule] })
export class CreateQuestionBankRequestDialogComponent {
  private readonly fb = inject(FormBuilder); private readonly service = inject(QuestionBankRequestsService);
  private readonly ref = inject(DynamicDialogRef); private readonly config = inject(DynamicDialogConfig);
  private readonly destroyRef = inject(DestroyRef);
  readonly saving = signal(false); readonly submitted = signal(false);
  readonly types = (this.config.data?.questionBankTypes as dropdownOptionsModel[]) ?? [];
  readonly managements = (this.config.data?.managements as dropdownOptionsModel[]) ?? [];
  readonly allJobTitles = (this.config.data?.jobTitles as dropdownOptionsModel[]) ?? [];
  readonly jobTitles = signal(this.allJobTitles);
  readonly form = this.fb.group({ questionBankTypeId: [null as string | null, Validators.required], managementId: [null as string | null], jobTitleId: [null as string | null], reason: ['', Validators.maxLength(2000)] });

  constructor() {
    this.form.controls.questionBankTypeId.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.updateTypeRules());
    this.form.controls.managementId.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(id => {
      this.form.controls.jobTitleId.setValue(null);
      this.jobTitles.set(!id ? [] : this.allJobTitles.filter(x => !x.additionalData?.['managementId'] || x.additionalData['managementId'] === id));
    });
  }
  get specialized(): boolean { return this.form.controls.questionBankTypeId.value === QuestionBankTypeIds.Specialized; }
  optionName(option: dropdownOptionsModel): string { return option.name; }
  submit(): void {
    this.submitted.set(true); if (this.form.invalid || this.saving()) { this.form.markAllAsTouched(); return; }
    const value = this.form.getRawValue(); this.saving.set(true);
    this.service.create({ questionBankTypeId: value.questionBankTypeId!, managementId: value.managementId,
      jobTitleId: value.jobTitleId, stageId: null, reason: value.reason?.trim() || null })
      .pipe(finalize(() => this.saving.set(false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: id => this.ref.close(id) });
  }
  cancel(): void { if (!this.saving()) this.ref.close(); }
  showError(name: keyof typeof this.form.controls): boolean { const c = this.form.controls[name]; return c.invalid && (c.touched || this.submitted()); }
  private updateTypeRules(): void {
    const management = this.form.controls.managementId; const jobTitle = this.form.controls.jobTitleId;
    if (this.specialized) { management.addValidators(Validators.required); jobTitle.addValidators(Validators.required); }
    else { management.clearValidators(); jobTitle.clearValidators(); management.setValue(null, { emitEvent: false }); jobTitle.setValue(null, { emitEvent: false }); this.jobTitles.set([]); }
    management.updateValueAndValidity({ emitEvent: false }); jobTitle.updateValueAndValidity({ emitEvent: false });
  }
}
