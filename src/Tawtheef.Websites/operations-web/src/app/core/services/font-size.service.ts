import { Injectable, Inject, PLATFORM_ID, inject } from '@angular/core';
import { DOCUMENT, isPlatformBrowser } from '@angular/common';
import { BehaviorSubject } from 'rxjs';
import { APP_FONT_SIZE_KEY } from '../constants/website-storage.const';

@Injectable({ providedIn: 'root' })
export class FontSizeService {
  private readonly defaultScale = 1;
  private readonly minScale = 0.7;
  private readonly maxScale = 1.4;
  private readonly step = 0.1;

  private scaleSubject = new BehaviorSubject<number>(this.defaultScale);
  public scale$ = this.scaleSubject.asObservable();

  constructor(
    @Inject(DOCUMENT) private document: Document,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    // Initial initialization
    this.init();
  }

  private init(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const stored = localStorage.getItem(APP_FONT_SIZE_KEY);
    const initialScale = stored ? parseFloat(stored) : this.defaultScale;
    this.applyScale(initialScale, false);
  }

  /**
   * Public initialize method for app initializer if needed
   */
  public async setup(): Promise<void> {
    this.init();
  }

  private applyScale(scale: number, persist = true): void {
    if (!isPlatformBrowser(this.platformId)) return;

    // Round to prevent floating point issues (e.g., 1.100000000001)
    const safeScale = parseFloat(Math.min(Math.max(scale, this.minScale), this.maxScale).toFixed(1));

    this.scaleSubject.next(safeScale);

    if (persist) {
      localStorage.setItem(APP_FONT_SIZE_KEY, safeScale.toString());
    }

    // Apply to html element to scale everything using rem
    const html = this.document.documentElement;
    html.style.fontSize = `${safeScale * 100}%`;

    // Also set a CSS variable for custom scaling where rem isn't enough
    html.style.setProperty('--app-font-scale', safeScale.toString());
  }

  public increase(): void {
    this.applyScale(this.scaleSubject.value + this.step);
  }

  public decrease(): void {
    this.applyScale(this.scaleSubject.value - this.step);
  }

  public reset(): void {
    this.applyScale(this.defaultScale);
  }

  public get currentScale(): number {
    return this.scaleSubject.value;
  }
}
