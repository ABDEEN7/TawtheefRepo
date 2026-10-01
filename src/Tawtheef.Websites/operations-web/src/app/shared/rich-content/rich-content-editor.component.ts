import { Component, forwardRef, Input } from '@angular/core';
import { FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { EditorModule } from 'primeng/editor';
import katex from 'katex';

@Component({
  selector: 'app-rich-content-editor',
  standalone: true,
  imports: [FormsModule, EditorModule],
  providers: [{ provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => RichContentEditorComponent), multi: true }],
  template: `
    <p-editor
      [(ngModel)]="value"
      (ngModelChange)="changed($event)"
      (onTextChange)="touched()"
      [readonly]="readonly"
      [placeholder]="placeholder"
      [style]="{ height: height }"
      [attr.dir]="direction"
    >
      <ng-template #header>
        <span class="ql-formats"><button type="button" class="ql-bold"></button><button type="button" class="ql-italic"></button><button type="button" class="ql-underline"></button></span>
        <span class="ql-formats"><button type="button" class="ql-script" value="sub"></button><button type="button" class="ql-script" value="super"></button></span>
        <span class="ql-formats"><button type="button" class="ql-list" value="ordered"></button><button type="button" class="ql-list" value="bullet"></button></span>
        <span class="ql-formats"><select class="ql-align"><option selected></option><option value="center"></option><option value="right"></option><option value="justify"></option></select></span>
        <span class="ql-formats"><button type="button" class="ql-formula"></button></span>
      </ng-template>
    </p-editor>
  `,
})
export class RichContentEditorComponent {
  @Input() direction: 'rtl' | 'ltr' = 'ltr';
  @Input() placeholder = '';
  @Input() readonly = false;
  @Input() height = '150px';
  value = '';
  private onChange = (_value: string) => {};
  touched = () => {};

  constructor() { (globalThis as unknown as { katex: typeof katex }).katex = katex; }
  writeValue(value: string | null): void { this.value = value ?? ''; }
  registerOnChange(fn: (value: string) => void): void { this.onChange = fn; }
  registerOnTouched(fn: () => void): void { this.touched = fn; }
  setDisabledState(disabled: boolean): void { this.readonly = disabled; }
  changed(value: string): void { this.onChange(value); }
}
