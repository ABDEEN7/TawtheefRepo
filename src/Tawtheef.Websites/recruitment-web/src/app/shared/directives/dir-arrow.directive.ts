import { Directive, ElementRef, Input, OnInit, OnDestroy, Renderer2 } from '@angular/core';
import { Subscription } from 'rxjs';
import {LanguageService} from '../../core/services/language.service';

@Directive({ selector: '[faDirArrow]' })
export class FaDirArrowDirective implements OnInit, OnDestroy {
  /** "start" | "end" — logical direction */
  @Input('faDirArrow') side: 'start' | 'end' = 'end';

  private sub?: Subscription;

  constructor(
    private el: ElementRef<HTMLElement>,
    private r: Renderer2,
    private lang: LanguageService
  ) {}

  ngOnInit() {
    // apply once + react to future lang changes
    this.apply(this.lang.isRtl);
    this.sub = this.lang.isRtl$.subscribe((isRtl) => this.apply(isRtl));
  }

  ngOnDestroy() { this.sub?.unsubscribe(); }

  private apply(isRtl: boolean) {
    const i = this.el.nativeElement;
    this.r.addClass(i, 'fa-solid');
    this.r.removeClass(i, 'fa-arrow-left');
    this.r.removeClass(i, 'fa-arrow-right');

    const cls =
      (this.side === 'end' && isRtl) || (this.side === 'start' && !isRtl)
        ? 'fa-arrow-left'
        : 'fa-arrow-right';

    this.r.addClass(i, cls);
  }
}
