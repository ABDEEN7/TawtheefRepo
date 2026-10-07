import { TestBed } from '@angular/core/testing';
import { TranslateModule } from '@ngx-translate/core';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { QuestionDialogComponent } from './question-dialog.component';
import { QuestionTypes } from '../models/question-bank-assignment.models';
import { QuestionBankAssignmentsService } from '../services/question-bank-assignments.service';

describe('QuestionDialogComponent option bindings', () => {
  const close = jasmine.createSpy('close');

  async function create() {
    TestBed.configureTestingModule({
      imports: [QuestionDialogComponent, TranslateModule.forRoot()],
      providers: [
        { provide: DynamicDialogConfig, useValue: { data: {} } },
        { provide: DynamicDialogRef, useValue: { close } },
        { provide: QuestionBankAssignmentsService, useValue: {} },
        { provide: DialogService, useValue: {} },
      ],
    });
    const fixture = TestBed.createComponent(QuestionDialogComponent);
    const dialog = fixture.componentInstance;
    dialog.form.patchValue({
      questionTypeId: QuestionTypes.multipleChoice,
      difficultyLevelId: dialog.difficulties[0].value,
      questionTextAr: 'سؤال',
      questionTextEn: 'Question',
    });
    fixture.detectChanges();
    await fixture.whenStable();
    return { fixture, dialog };
  }

  beforeEach(() => close.calls.reset());

  it('updates localized form values from real inputs and clears their errors', async () => {
    const { fixture, dialog } = await create();
    dialog.save();
    fixture.detectChanges();
    const inputs = fixture.nativeElement.querySelectorAll('app-rich-content-input input');
    ['أول', 'First', 'ثاني', 'Second'].forEach((text, index) => {
      inputs[index].value = text;
      inputs[index].dispatchEvent(new Event('input'));
      inputs[index].dispatchEvent(new Event('blur'));
    });
    fixture.detectChanges();
    expect(dialog.options.getRawValue()[0].optionTextAr).toBe('أول');
    expect(dialog.options.getRawValue()[0].optionTextEn).toBe('First');
    expect(dialog.optionInvalid(0, 'optionTextAr')).toBeFalse();
    expect(dialog.optionInvalid(0, 'optionTextEn')).toBeFalse();
    dialog.setCorrect(0);
    dialog.save();
    expect(close).toHaveBeenCalled();
  });

  it('keeps radio state exclusive and retains a selected answer on another click', async () => {
    const { fixture, dialog } = await create();
    const click = async (index: number) => {
      fixture.nativeElement.querySelectorAll('p-checkbox input, p-radiobutton input')[index].click();
      fixture.detectChanges();
      await fixture.whenStable();
      fixture.detectChanges();
    };
    await click(0);
    await click(1);
    await click(1);
    expect(dialog.options.getRawValue().map(option => option.isCorrect)).toEqual([false, true]);
    const answers = fixture.nativeElement.querySelectorAll('p-checkbox input, p-radiobutton input');
    expect(answers[0].checked).toBeFalse();
    expect(answers[1].checked).toBeTrue();
  });

  it('keeps inputs attached to surviving groups after deletion and type changes', async () => {
    const { fixture, dialog } = await create();
    dialog.addOption({ optionTextAr: 'ثالث', optionTextEn: 'Third' });
    fixture.detectChanges();
    dialog.removeOption(0);
    fixture.detectChanges();
    const inputs = fixture.nativeElement.querySelectorAll('app-rich-content-input input');
    inputs[2].value = 'معدل';
    inputs[2].dispatchEvent(new Event('input'));
    expect(dialog.options.at(1).value.optionTextAr).toBe('معدل');
    expect(dialog.options.getRawValue().map(option => option.displayOrder)).toEqual([1, 2]);
    dialog.form.controls.questionTypeId.setValue(QuestionTypes.trueFalse);
    fixture.detectChanges();
    dialog.form.controls.questionTypeId.setValue(QuestionTypes.multipleChoice);
    fixture.detectChanges();
    const freshInput = fixture.nativeElement.querySelector('app-rich-content-input input');
    freshInput.value = 'جديد';
    freshInput.dispatchEvent(new Event('input'));
    expect(dialog.options.at(0).value.optionTextAr).toBe('جديد');
  });

  it('supports exclusive selection for true/false questions', async () => {
    const { fixture, dialog } = await create();
    dialog.form.controls.questionTypeId.setValue(QuestionTypes.trueFalse);
    fixture.detectChanges();
    dialog.setCorrect(0);
    dialog.setCorrect(1);
    expect(dialog.options.getRawValue().map(option => option.isCorrect)).toEqual([false, true]);
    dialog.save();
    expect(close).toHaveBeenCalled();
  });

  it('rejects zero or multiple correct answers even when controls are patched directly', async () => {
    const { dialog } = await create();
    dialog.options.controls.forEach(option => option.patchValue({
      optionTextAr: 'خيار', optionTextEn: 'Option',
    }));
    dialog.save();
    expect(close).not.toHaveBeenCalled();
    dialog.options.controls.forEach(option => option.patchValue({ isCorrect: true }));
    expect(dialog.form.invalid).toBeTrue();
    dialog.save();
    expect(close).not.toHaveBeenCalled();
    dialog.setCorrect(1);
    dialog.save();
    expect(close).toHaveBeenCalled();
  });
});
