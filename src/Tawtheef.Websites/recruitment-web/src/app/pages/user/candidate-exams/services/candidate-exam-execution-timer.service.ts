import { computed, Injectable, OnDestroy, signal } from '@angular/core';
import { CANDIDATE_EXAM_TIMER_DISPLAY_INTERVAL_MS } from '../constants/candidate-exam-execution-timer.constants';

@Injectable()
export class CandidateExamExecutionTimerService implements OnDestroy {
  readonly remainingSeconds = signal(0);
  readonly formattedRemainingTime = computed(() =>
    this.formatRemainingTime(this.remainingSeconds()),
  );

  private serverTimeAtSync = 0;
  private deadlineAtSync = 0;
  private performanceAtSync = 0;
  private displayInterval: ReturnType<typeof setInterval> | null = null;
  private onDeadlineReached: (() => void) | null = null;

  synchronize(serverTimeUtc: string, effectiveDeadlineAt: string): boolean | null {
    const serverTime = Date.parse(serverTimeUtc);
    const deadline = Date.parse(effectiveDeadlineAt);
    if (!Number.isFinite(serverTime) || !Number.isFinite(deadline)) return null;

    this.serverTimeAtSync = serverTime;
    this.deadlineAtSync = deadline;
    this.performanceAtSync = performance.now();
    return deadline <= serverTime;
  }

  start(onDeadlineReached: () => void): void {
    this.onDeadlineReached = onDeadlineReached;
    this.updateRemainingTime();

    if (!this.displayInterval) {
      this.displayInterval = setInterval(
        () => this.updateRemainingTime(),
        CANDIDATE_EXAM_TIMER_DISPLAY_INTERVAL_MS,
      );
    }
  }

  resetRemainingTime(): void {
    this.remainingSeconds.set(0);
  }

  stop(): void {
    if (this.displayInterval) clearInterval(this.displayInterval);
    this.displayInterval = null;
  }

  ngOnDestroy(): void {
    this.stop();
  }

  private updateRemainingTime(): void {
    if (!this.deadlineAtSync) return;
    const estimatedServerNow =
      this.serverTimeAtSync + (performance.now() - this.performanceAtSync);
    const seconds = Math.max(0, Math.ceil((this.deadlineAtSync - estimatedServerNow) / 1000));
    this.remainingSeconds.set(seconds);
    if (seconds === 0) {
      this.onDeadlineReached?.();
    }
  }

  private formatRemainingTime(totalSeconds: number): string {
    const hours = Math.floor(totalSeconds / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const seconds = totalSeconds % 60;
    const pairs = [minutes, seconds].map((value) => value.toString().padStart(2, '0'));
    return hours > 0
      ? `${hours.toString().padStart(2, '0')}:${pairs.join(':')}`
      : pairs.join(':');
  }
}
