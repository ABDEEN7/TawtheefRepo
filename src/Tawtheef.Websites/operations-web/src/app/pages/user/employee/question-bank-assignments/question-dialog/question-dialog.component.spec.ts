import { TestBed } from '@angular/core/testing';
import { TranslateModule } from '@ngx-translate/core';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { QuestionDialogComponent } from './question-dialog.component';
import { QuestionTypes } from '../models/question-bank-assignment.models';
import { QuestionBankAssignmentsService } from '../services/question-bank-assignments.service';
import { of, throwError } from 'rxjs';

describe('QuestionDialogComponent option bindings', () => {
  const close = jasmine.createSpy('close');

  async function create(data = {}, service = {}) {
    TestBed.configureTestingModule({
      imports: [QuestionDialogComponent, TranslateModule.forRoot()],
      providers: [
        { provide: DynamicDialogConfig, useValue: { data } },
        { provide: DynamicDialogRef, useValue: { close } },
        { provide: QuestionBankAssignmentsService, useValue: service },
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

  function selection(file: File) {
    const input = { files: [file], value: 'selected-image' };
    return { input, event: { target: input } as unknown as Event };
  }

  function fillValidOptions(dialog: QuestionDialogComponent): void {
    dialog.options.controls.forEach(option => option.patchValue({
      optionTextAr: 'خيار', optionTextEn: 'Option',
    }));
    dialog.setCorrect(0);
  }

  it('rejects invalid files but allows valid text/options to save without attaching them', async () => {
    const { fixture, dialog } = await create();
    fillValidOptions(dialog);
    const before = dialog.form.getRawValue();
    const files = [
      new File(['fake'], 'image.gif', { type: 'image/jpeg' }),
      new File(['fake'], 'image.svg', { type: 'image/png' }),
      new File(['fake'], 'image.pdf', { type: 'image/png' }),
      new File(['fake'], 'image', { type: 'image/png' }),
      new File(['fake'], 'image.png', { type: 'application/pdf' }),
      new File([], 'image.png', { type: 'image/png' }),
      new File([new Uint8Array(1_000_001)], 'image.png', { type: 'image/png' }),
      new File(['renamed invalid image'], 'image.png', { type: 'image/png' }),
    ];
    for (const file of files) {
      const { input, event } = selection(file);
      await dialog.chooseImage(event);
      fixture.detectChanges();
      expect(fixture.nativeElement.textContent).toContain('QUESTION_ASSIGNMENTS.INVALID_IMAGE');
      expect(fixture.nativeElement.querySelector('button[type="submit"]').disabled).toBeFalse();
      close.calls.reset();
      dialog.save();
      expect(dialog.imageError).toBeTrue();
      expect(dialog.imageFile).toBeUndefined();
      expect(input.value).toBe('');
      expect(dialog.form.getRawValue()).toEqual(before);
      expect(close).toHaveBeenCalledOnceWith({ ...before, imageFile: undefined });
    }
  });

  it('preserves an existing image after an invalid replacement and saves its resource', async () => {
    const resourceId = 'existing-resource';
    const { fixture, dialog } = await create(
      { question: { resourceId, imageUrl: 'existing-image' } },
      { image: () => of(new Blob(['existing image'], { type: 'image/png' })) },
    );
    fillValidOptions(dialog);
    // The existing resource alone must still be usable for an image-only question.
    dialog.form.patchValue({ questionTextAr: '', questionTextEn: '' });
    const before = dialog.form.getRawValue();
    const preview = dialog.imagePreview;
    const { input, event } = selection(new File(['fake'], 'replacement.png', { type: 'image/png' }));
    await dialog.chooseImage(event);
    fixture.detectChanges();
    expect(dialog.imageError).toBeTrue();
    expect(fixture.nativeElement.textContent).toContain('QUESTION_ASSIGNMENTS.INVALID_IMAGE');
    expect(fixture.nativeElement.querySelector('button[type="submit"]').disabled).toBeFalse();
    expect(dialog.imagePreview).toBe(preview);
    expect(dialog.imageFile).toBeUndefined();
    expect(dialog.form.controls.resourceId.value).toBe(resourceId);
    expect(input.value).toBe('');
    dialog.save();
    expect(close).toHaveBeenCalledOnceWith({ ...before, imageFile: undefined });
    expect(dialog.imageError).toBeTrue();
  });

  it('still rejects an image-only question with no valid resource or file', async () => {
    const { fixture, dialog } = await create();
    fillValidOptions(dialog);
    dialog.form.patchValue({ questionTextAr: '', questionTextEn: '' });
    await dialog.chooseImage(selection(new File(['fake'], 'image.png', { type: 'image/png' })).event);
    fixture.detectChanges();
    expect(dialog.imageError).toBeTrue();
    expect(dialog.imageFile).toBeUndefined();
    expect(dialog.form.controls.resourceId.value).toBeNull();
    expect(fixture.nativeElement.querySelector('button[type="submit"]').disabled).toBeFalse();
    dialog.save();
    expect(close).not.toHaveBeenCalled();
    expect(dialog.submitted).toBeTrue();
    expect(dialog.questionContentInvalid()).toBeTrue();
  });

  it('allows text edits to save when the existing image preview cannot be downloaded', async () => {
    const resourceId = 'existing-resource';
    const { fixture, dialog } = await create(
      { question: { resourceId, imageUrl: 'existing-image' } },
      { image: () => throwError(() => new Error('Preview unavailable')) },
    );
    fillValidOptions(dialog);
    fixture.detectChanges();
    expect(dialog.imageError).toBeTrue();
    expect(fixture.nativeElement.querySelector('button[type="submit"]').disabled).toBeFalse();
    dialog.save();
    expect(close).toHaveBeenCalledOnceWith({ ...dialog.form.getRawValue(), imageFile: undefined });
    expect(dialog.form.controls.resourceId.value).toBe(resourceId);
  });

  it('checks magic bytes against the extension and preserves a previous valid image on failure', async () => {
    const { dialog } = await create();
    const valid = new File([new Uint8Array([0xff, 0xd8, 0xff])], 'image.JPG', { type: 'image/jpeg' });
    await dialog.chooseImage(selection(valid).event);
    const preview = dialog.imagePreview;
    const resourceId = 'existing-resource';
    dialog.form.controls.resourceId.setValue(resourceId);
    for (const file of [
      new File(['renamed text'], 'image.png', { type: 'image/png' }),
      new File([new Uint8Array([0xff, 0xd8, 0xff])], 'image.png', { type: 'image/png' }),
      new File(['RIFFfakeWEB'], 'image.webp', { type: 'image/webp' }),
    ]) {
      await dialog.chooseImage(selection(file).event);
      expect(dialog.imageError).toBeTrue();
      expect(dialog.imageFile).toBe(valid);
      expect(dialog.imagePreview).toBe(preview);
      expect(dialog.form.controls.resourceId.value).toBe(resourceId);
    }
  });

  it('accepts JPG, JPEG, PNG, and WebP signatures, including uppercase extensions and the size limit', async () => {
    const { dialog } = await create();
    const png = new Uint8Array(1_000_000);
    png.set([0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a]);
    const files = [
      new File([new Uint8Array([0xff, 0xd8, 0xff])], 'image.JPG', { type: 'image/jpeg' }),
      new File([new Uint8Array([0xff, 0xd8, 0xff])], 'image.JPEG', { type: 'image/jpeg' }),
      new File([png], 'image.PNG', { type: 'image/png' }),
      new File(['RIFF0000WEBP'], 'image.WeBp', { type: 'image/webp' }),
    ];
    for (const file of files) {
      await dialog.chooseImage(selection(file).event);
      expect(dialog.imageError).toBeFalse();
      expect(dialog.imageValidating).toBeFalse();
      expect(dialog.imageFile).toBe(file);
      expect(dialog.imagePreview).toMatch(/^blob:/);
    }
  });

  it('blocks save while reading a signature and ignores an image removed during validation', async () => {
    const { fixture, dialog } = await create();
    dialog.form.controls.questionTypeId.setValue(QuestionTypes.trueFalse);
    dialog.setCorrect(0);
    let finishRead!: (buffer: ArrayBuffer) => void;
    const file = new File(['pending'], 'image.jpg', { type: 'image/jpeg' });
    spyOn(file, 'slice').and.returnValue({
      arrayBuffer: () => new Promise<ArrayBuffer>(resolve => finishRead = resolve),
    } as Blob);
    const pending = dialog.chooseImage(selection(file).event);
    fixture.detectChanges();
    expect(dialog.imageValidating).toBeTrue();
    expect(fixture.nativeElement.querySelector('button[type="submit"]').disabled).toBeTrue();
    dialog.save();
    expect(close).not.toHaveBeenCalled();
    dialog.removeImage();
    finishRead(new Uint8Array([0xff, 0xd8, 0xff]).buffer);
    await pending;
    expect(dialog.imageFile).toBeUndefined();
    expect(dialog.imagePreview).toBeUndefined();
    expect(dialog.imageValidating).toBeFalse();
    dialog.save();
    expect(close).toHaveBeenCalled();
  });

  it('ignores a stale file read after a newer invalid selection', async () => {
    const { dialog } = await create();
    let finishRead!: (buffer: ArrayBuffer) => void;
    const file = new File(['pending'], 'image.jpg', { type: 'image/jpeg' });
    spyOn(file, 'slice').and.returnValue({
      arrayBuffer: () => new Promise<ArrayBuffer>(resolve => finishRead = resolve),
    } as Blob);
    const pending = dialog.chooseImage(selection(file).event);
    await dialog.chooseImage(selection(new File(['bad'], 'bad.svg', { type: 'image/svg+xml' })).event);
    finishRead(new Uint8Array([0xff, 0xd8, 0xff]).buffer);
    await pending;
    expect(dialog.imageError).toBeTrue();
    expect(dialog.imageFile).toBeUndefined();
  });

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
