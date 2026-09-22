import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';

import { LazySelectOption } from '../../models/lazy-select-option.model';
import { LazySelectComponent } from './lazy-select';

@Component({
  imports: [LazySelectComponent],
  template: `<app-lazy-select [search]="search" />`,
})
class HostComponent {
  // Every search returns this subject, so a test decides exactly when (and with what) the "server" answers.
  readonly response = new Subject<LazySelectOption[]>();
  readonly search = () => this.response;
}

describe('LazySelectComponent empty-panel message', () => {
  let fixture: ComponentFixture<HostComponent>;
  let host: HostComponent;

  const emptyMessage = () => document.querySelector('.p-select-empty-message')?.textContent?.trim() ?? null;
  const settle = async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
  };
  const openDropdown = async () => {
    (fixture.nativeElement.querySelector('.p-select') as HTMLElement).click();
    await settle();
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HostComponent, TranslateModule.forRoot()],
      // PrimeNG 20's overlay still drives its panel through @angular/animations triggers, so an animations provider is
      // required (without it the overlay throws NG05105). Angular deprecates every provider of that package (20.2, removal
      // planned for v23) and app.config.ts uses the deprecated one too - so the editor hint can't be avoided until PrimeNG
      // moves off it. The synchronous no-op provider is used on purpose: provideAnimationsAsync('noop') loads the renderer
      // lazily, which made the panel assertions race.
      providers: [provideNoopAnimations()],
    }).compileComponents();

    const translate = TestBed.inject(TranslateService);
    translate.setTranslation('en', { 'common.loading': 'Loading', 'common.noResults': 'No results', 'common.search': 'Search' });
    translate.use('en');

    fixture = TestBed.createComponent(HostComponent);
    host = fixture.componentInstance;
    await settle();
  });

  afterEach(() => {
    // The overlay is appended to <body>, so it outlives the fixture unless removed.
    document.querySelectorAll('.p-select-overlay').forEach((el) => el.remove());
  });

  it('says "Loading" - not "No results" - while the first search is still pending', async () => {
    await openDropdown();

    expect(emptyMessage()).toBe('Loading');
  });

  it('says "No results" only once a search has come back empty', async () => {
    await openDropdown();
    host.response.next([]);
    await settle();

    expect(emptyMessage()).toBe('No results');
  });

  it('shows the returned options instead of any empty message', async () => {
    await openDropdown();
    host.response.next([{ value: '1', label: 'Alpha' }]);
    await settle();

    expect(emptyMessage()).toBeNull();
    expect(document.querySelector('.p-select-overlay')?.textContent).toContain('Alpha');
  });

  it('goes back to "Loading" as soon as the user types, before the debounced search has fired', async () => {
    await openDropdown();
    host.response.next([{ value: '1', label: 'Alpha' }]);
    await settle();

    // A term that matches nothing in the old results: PrimeNG filters them away at once and would show its empty message.
    const filter = document.querySelector('.p-select-filter') as HTMLInputElement;
    filter.value = 'zzz';
    filter.dispatchEvent(new Event('input'));
    await settle();

    expect(emptyMessage()).toBe('Loading');
  });
});
