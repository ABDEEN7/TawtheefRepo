import { Directive, DestroyRef, Input, OnInit, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {I18nFeatureLoader} from '../loaders/i18n-feature.loader';

@Directive({
  selector: '[i18nNamespace]',
  standalone: true
})
export class I18nNamespaceDirective implements OnInit {
  @Input('i18nNamespace') ns!: string;

  private loader = inject(I18nFeatureLoader);
  private translate = inject(TranslateService);
  private destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    // Initial load
    this.loader.ensureLoaded(this.ns);

    // Reload on language change
    this.translate.onLangChange
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(e => this.loader.ensureLoaded(this.ns, e.lang));
  }
}
