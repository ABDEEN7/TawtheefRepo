import {Directive, ElementRef, Input, OnInit} from '@angular/core';
import {TranslateService} from "@ngx-translate/core";

@Directive({
  selector: '[localized]',
  standalone: true
})
export class LocalizedDirective implements OnInit {
  @Input() localizedParams?: Record<string, any>;

  constructor(
    private el: ElementRef,
    private translate: TranslateService
  ) {
  }

  ngOnInit() {
    const text = this.el.nativeElement.textContent.trim();
    if (!text) return;
    this.translate.get(text, this.localizedParams).subscribe((translation) => {
      if (this.el.nativeElement.tagName === 'INPUT' || this.el.nativeElement.tagName === 'MAT-SELECT') {
        this.el.nativeElement.placeholder = translation;
      } else {
        this.el.nativeElement.textContent = translation;
      }
    });
  }
}
