import { Component, DestroyRef, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Toast } from 'primeng/toast';
import { ProgressBarComponent } from './shared/components/progress-bar/progress-bar.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Toast, ProgressBarComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('operations-web');
  private readonly destroyRef = inject(DestroyRef);

  constructor() {
    this.setupInputValidation();
  }

  private setupInputValidation(): void {
    const onInput = (event: Event) => {
      const target = event.target;
      if (!(target instanceof HTMLInputElement || target instanceof HTMLTextAreaElement)) {
        return;
      }

      const initialValue = target.value;
      const sanitizedValue = this.sanitizeValue(target, initialValue);

      if (sanitizedValue !== initialValue) {
        target.value = sanitizedValue;
        target.dispatchEvent(new Event('input', { bubbles: true }));
      }
    };

    document.addEventListener('input', onInput, true);
    this.destroyRef.onDestroy(() => document.removeEventListener('input', onInput, true));
  }

  private sanitizeValue(control: HTMLInputElement | HTMLTextAreaElement, value: string): string {
    if (control instanceof HTMLTextAreaElement) {
      return value.replace(/[^\p{L}\p{N}\s\-',.،]/gu, '').slice(0, 500);
    }

    if (control.type === 'email') {
      return value.replace(/\s/g, '');
    }

    if (control.type === 'text' || !control.type) {
      return value.replace(/[^\p{L}\p{N}\s\-@._]/gu, '');
    }

    return value;
  }
}
