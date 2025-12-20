import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TooltipModule } from 'primeng/tooltip';
import {TranslatePipe} from '@ngx-translate/core';

export type StepUiStatus = 'idle' | 'progress' | 'done' | 'bad';

export interface ProfileApprovalStepperSection {
  section: number;
  labelKey: string;
  icon?: string;
  uiStatus: StepUiStatus;

  /**
   * Optional: can be precomputed by parent; if omitted we compute based on mode/current/previous.
   */
  disabled?: boolean;

  /**
   * Tooltip key OR resolved tooltip string.
   * If you pass keys, you can pipe translate in template, or pass resolved strings from parent.
   */
  tooltip?: string;
}

export type StepperMode = 'jump' | 'lock';

@Component({
  selector: 'app-profile-approval-stepper',
  standalone: true,
  imports: [CommonModule, TooltipModule, TranslatePipe],
  templateUrl: './profile-approval-stepper.component.html',
  styleUrls: ['./profile-approval-stepper.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileApprovalStepperComponent {
  @Input({ required: true }) sections: ProfileApprovalStepperSection[] = [];
  @Input({ required: true }) activeSection: number | null = null;
  trackBySection = (_: number, s: ProfileApprovalStepperSection) => s.section;

  /**
   * jump: allow jump to any section
   * lock: lock steps until previous is decided (done/bad), while still allowing current and previous
   */
  @Input() mode: StepperMode = 'jump';

  /**
   * In lock mode: allow opening a step even if previous is not decided (soft lock).
   * default: false (strict lock)
   */
  @Input() softLock = false;

  /**
   * Optional: show numbers beside icon
   */
  @Input() showIndex = false;

  @Output() sectionChange = new EventEmitter<number>();

  onStepClick(s: ProfileApprovalStepperSection, index: number): void {
    if (this.isStepDisabled(s, index)) return;
    this.sectionChange.emit(s.section);
  }

  isStepActive(s: ProfileApprovalStepperSection): boolean {
    return this.activeSection === s.section;
  }

  isStepDisabled(s: ProfileApprovalStepperSection, index: number): boolean {
    if (s.disabled === true) return true;

    if (this.mode === 'jump') return false;

    // lock mode
    // allow clicking active step always
    if (this.isStepActive(s)) return false;

    // allow previous steps
    const activeIdx = this.sections.findIndex(x => x.section === this.activeSection);
    if (activeIdx >= 0 && index <= activeIdx) return false;

    // for next steps: require all previous steps decided (done/bad) unless softLock=true
    if (this.softLock) return false;

    for (let i = 0; i < index; i++) {
      const prev = this.sections[i];
      if (prev.uiStatus !== 'done' && prev.uiStatus !== 'bad') return true;
    }
    return false;
  }

  /**
   * Tooltip text (English shown as requested). You can localize later if needed.
   */
  tooltipText(s: ProfileApprovalStepperSection): string {
    if (s.tooltip) return s.tooltip;

    switch (s.uiStatus) {
      case 'done': return 'Saved as Approved';
      case 'bad': return 'Saved as Needs correction';
      case 'progress': return 'Current section';
      default: return 'Not decided';
    }
  }

  stepClass(s: ProfileApprovalStepperSection, index: number): Record<string, boolean> {
    return {
      'step': true,
      'is-active': this.isStepActive(s),
      'is-disabled': this.isStepDisabled(s, index),
      'is-done': s.uiStatus === 'done',
      'is-bad': s.uiStatus === 'bad',
      'is-progress': s.uiStatus === 'progress',
      'is-idle': s.uiStatus === 'idle',
    };
  }
}
