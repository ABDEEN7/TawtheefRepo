import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { RichContentEditorComponent } from './rich-content-editor.component';

@Component({
  standalone: true,
  imports: [FormsModule, TranslatePipe, ButtonModule, RichContentEditorComponent],
  template: `
    <app-rich-content-editor [(ngModel)]="content" [direction]="direction" height="230px" />
    <div class="d-flex justify-content-end gap-2 mt-3">
      <p-button [text]="true" severity="secondary" [label]="'QUESTION_ASSIGNMENTS.CANCEL' | translate" (onClick)="ref.close()" />
      <p-button [label]="'QUESTION_ASSIGNMENTS.APPLY' | translate" (onClick)="ref.close(content)" />
    </div>
  `,
})
export class RichContentEditorDialogComponent {
  readonly ref = inject(DynamicDialogRef);
  private readonly config = inject(DynamicDialogConfig);
  content = this.config.data?.content ?? '';
  direction: 'rtl' | 'ltr' = this.config.data?.direction ?? 'ltr';
}
