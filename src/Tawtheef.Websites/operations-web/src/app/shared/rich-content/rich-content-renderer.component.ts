import {AfterViewChecked, Component, ElementRef, inject, Input} from '@angular/core';
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
    const formulas = this.element.nativeElement.querySelectorAll(
      '.ql-formula:not([data-rendered])'
    ) as NodeListOf<HTMLElement>;

    formulas.forEach((formula: HTMLElement) => {
      try {
        katex.render(
          formula.dataset['value'] ?? '',
          formula,
          { throwOnError: false }
        );

        formula.dataset['rendered'] = 'true';
      } catch {
        // Malformed expressions remain visible as their source value
      }
    });
  }
}
