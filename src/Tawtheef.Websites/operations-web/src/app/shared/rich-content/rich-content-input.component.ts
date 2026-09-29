import { Component, forwardRef, inject, Input } from '@angular/core';
import { FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DialogService } from 'primeng/dynamicdialog';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { isRichContent } from './rich-content.utils';
import { RichContentEditorDialogComponent } from './rich-content-editor.dialog.component';
import { RichContentRendererComponent } from './rich-content-renderer.component';

@Component({
  selector: 'app-rich-content-input',
  standalone: true,
  imports: [
    FormsModule,
    TranslatePipe,
    ButtonModule,
    InputTextModule,
    TooltipModule,
    RichContentRendererComponent,
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RichContentInputComponent),
      multi: true,
    },
  ],
  template: `
    <div class="d-flex gap-1 align-items-stretch">
      @if (rich) {
        <button type="button" class="form-control text-start overflow-hidden" (click)="open()">
          <app-rich-content-renderer [content]="value" [direction]="direction" />
        </button>
      } @else {
        <input
          pInputText
          class="w-100"
          [(ngModel)]="value"
          (ngModelChange)="changed($event)"
          (blur)="touched()"
          [placeholder]="placeholder"
          [attr.dir]="direction"
        />
      }
      <p-button
        type="button"
        icon="pi pi-pencil"
        [text]="true"
        [pTooltip]="'QUESTION_ASSIGNMENTS.ADVANCED_EDITOR' | translate"
        (onClick)="open()"
      />
    </div>
  `,
})
export class RichContentInputComponent {
  @Input() direction: 'rtl' | 'ltr' = 'ltr';
  @Input() placeholder = '';
  private readonly dialogs = inject(DialogService);
  private readonly translate = inject(TranslateService);
  value = '';
  private onChange = (_value: string) => {};
  touched = () => {};
  get rich(): boolean {
    return isRichContent(this.value);
  }
  writeValue(value: string | null): void {
    this.value = value ?? '';
  }
  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: () => void): void {
    this.touched = fn;
  }
  changed(value: string): void {
    this.onChange(value);
  }
  open(): void {
    const previous = this.value;
    this.dialogs
      .open(RichContentEditorDialogComponent, {
        header: this.translate.instant('QUESTION_ASSIGNMENTS.ADVANCED_EDITOR'),
        width: 'min(760px, 95vw)',
        data: { content: previous, direction: this.direction },
      })!
      .onClose.subscribe((result) => {
        if (typeof result === 'string') {
          this.value = result;
          this.onChange(result);
          this.touched();
        }
      });
  }
}
