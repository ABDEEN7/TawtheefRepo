import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  inject
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LoadingService } from '../../../core/services/loading.service';

@Component({
  selector: 'app-progress-bar',
  templateUrl: './progress-bar.component.html',
  styleUrls: ['./progress-bar.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
})
export class ProgressBarComponent {
  private readonly loading = inject(LoadingService);
  private readonly cdr = inject(ChangeDetectorRef);

  progressValue = 0;
  visible = true;
  private timer?: number;

  constructor() {
    this.loading.loading$
      .pipe(takeUntilDestroyed())
      .subscribe(isLoading => {
        isLoading ? this.start() : this.finish();
      });
  }

  private start() {
    this.clearTimer();
    this.visible = true;
    this.progressValue = 8;
    this.cdr.markForCheck();

    this.timer = window.setInterval(() => {
      const delta = Math.max(1, Math.round((100 - this.progressValue) * 0.08));
      this.progressValue = Math.min(this.progressValue + delta, 95);
      this.cdr.markForCheck();
    }, 180);
  }

  private finish() {
    this.clearTimer();
    this.progressValue = 100;
    this.cdr.markForCheck();

    setTimeout(() => {
      // this.visible = false;
      this.progressValue = 30;
      this.cdr.markForCheck();
    }, 250);
  }

  private clearTimer() {
    if (this.timer) {
      clearInterval(this.timer);
      this.timer = undefined;
    }
  }
}
