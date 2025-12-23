import { Directive, DestroyRef, Input, OnInit, inject, OnChanges, SimpleChanges } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { I18nFeatureLoader } from '../loaders/i18n-feature.loader';

@Directive({
  selector: '[i18nNamespace]',
  standalone: true
})
export class I18nNamespaceDirective implements OnInit, OnChanges {
  @Input('i18nNamespace') ns!: string | string[];

  private loader = inject(I18nFeatureLoader);
  private translate = inject(TranslateService);
  private destroyRef = inject(DestroyRef);
  private namespaces: string[] = [];

  ngOnInit(): void {
    this.processNamespaces();
    this.setupLanguageChangeSubscription();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['ns']) {
      this.processNamespaces();
    }
  }

  private processNamespaces(): void {
    if (!this.ns) return;
    this.namespaces = Array.isArray(this.ns) ? this.ns : [this.ns];
    this.namespaces.forEach(namespace => {
      if (namespace) { // Ensure namespace is not empty
        this.loader.ensureLoaded(namespace);
      }
    });
  }

  private setupLanguageChangeSubscription(): void {
    this.translate.onLangChange
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(e => {
        this.namespaces.forEach(namespace => {
          if (namespace) { // Ensure namespace is not empty
            this.loader.ensureLoaded(namespace, e.lang);
          }
        });
      });
  }
}
