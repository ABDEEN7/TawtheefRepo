import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { Select } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { finalize } from 'rxjs';
import { EmployeeLookup } from '../../models/question-bank-request.models';
import { QuestionBankRequestsService } from '../../services/question-bank-requests.service';

@Component({
  selector: 'app-assign-question-bank-employees-dialog', standalone: true,
  templateUrl: './assign-question-bank-employees.dialog.component.html',
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, ButtonModule, InputNumberModule, Select, TextareaModule]
})
export class AssignQuestionBankEmployeesDialogComponent {
  private readonly fb = inject(FormBuilder); private readonly service = inject(QuestionBankRequestsService);
  private readonly ref = inject(DynamicDialogRef); private readonly config = inject(DynamicDialogConfig);
  private readonly destroyRef = inject(DestroyRef); readonly saving = signal(false); readonly submitted = signal(false);
  readonly requestId = this.config.data?.requestId as string; readonly employees = (this.config.data?.employees as EmployeeLookup[]) ?? [];
  readonly form = this.fb.group({ assignments: this.fb.array([this.row()]) });
  get assignments(): FormArray { return this.form.controls.assignments; }
  row() { return this.fb.group({ employeeId: [null as string | null, Validators.required], minimumQuestionCount: [null as number | null, [Validators.required, Validators.min(1)]], notes: ['', Validators.maxLength(1000)] }); }
  add(): void { this.assignments.push(this.row()); }
  remove(index: number): void { if (this.assignments.length > 1) this.assignments.removeAt(index); }
  employeeName(x: EmployeeLookup): string { return x.nameEn || x.nameAr || '-'; }
  invalid(index: number, field: string): boolean { const c = this.assignments.at(index).get(field); return !!c?.invalid && (!!c.touched || this.submitted()); }
  duplicate(index: number): boolean { const id = this.assignments.at(index).get('employeeId')?.value; return !!id && this.assignments.controls.some((x, i) => i !== index && x.get('employeeId')?.value === id); }
  submit(): void {
    this.submitted.set(true); this.form.markAllAsTouched();
    if (this.form.invalid || this.assignments.controls.some((_, i) => this.duplicate(i)) || this.saving()) return;
    this.saving.set(true);
    const assignments = this.assignments.getRawValue().map(x => ({ employeeId: x.employeeId!, minimumQuestionCount: x.minimumQuestionCount!, notes: x.notes?.trim() || null }));
    this.service.assign(this.requestId, { requestId: this.requestId, assignments })
      .pipe(finalize(() => this.saving.set(false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: () => this.ref.close(true) });
  }
  cancel(): void { if (!this.saving()) this.ref.close(false); }
}
