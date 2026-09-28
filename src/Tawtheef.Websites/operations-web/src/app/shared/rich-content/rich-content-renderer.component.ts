import { AfterViewChecked, Component, ElementRef, Input, inject } from '@angular/core';
import katex from 'katex';

@Component({
  selector: 'app-rich-content-renderer',
  standalone: true,
  template: `<div class="rich-content" [attr.dir]="direction" [innerHTML]="content"></div>`,
  styles: `.rich-content :first-child{margin-top:0}.rich-content :last-child{margin-bottom:0}`,
})
export class RichContentRendererComponent implements AfterViewChecked {
  private readonly element = inject(ElementRef<HTMLElement>);
  @Input() content = '';
  @Input() direction: 'rtl' | 'ltr' = 'ltr';
  ngAfterViewChecked(): void {
    this.element.nativeElement.querySelectorAll<HTMLElement>('.ql-formula:not([data-rendered])').forEach((formula) => {
      try {
        katex.render(formula.dataset['value'] ?? '', formula, { throwOnError: false });
        formula.dataset['rendered'] = 'true';
      } catch { /* malformed expressions remain visible as their source value */ }
    });
  }
}
