import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-job-points-main-elements',
  standalone: false,
  templateUrl: './job-points-main-elements.component.html',
  styleUrls: ['./job-points-main-elements.component.scss'],
})
export class JobPointsMainElementsComponent {
  @Input() form!: FormGroup;
  @Input() keys: { key: string; initialValue: number }[] = [];
  @Input() systemMaxPoints = 0;
  @Input() isReadOnlyMode = false;

  @Output() next = new EventEmitter<void>();

  get maxTotal(): number {
    return this.keys.reduce((sum, k) => sum + k.initialValue, 0);
  }

  isTotalValid(): boolean {
    const total = this.form.get('total')?.value || 0;
    return total === this.systemMaxPoints;
  }
}
