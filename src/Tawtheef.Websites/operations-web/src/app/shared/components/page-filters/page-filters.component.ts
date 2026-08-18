import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-page-filters',
  standalone: true,
  imports: [ButtonModule],
  templateUrl: './page-filters.component.html',
  styleUrl: './page-filters.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PageFiltersComponent {
  advancedLabel = input.required<string>();
  clearLabel = input.required<string>();
  advancedExpanded = input(false);
  activeAdvancedCount = input(0);
  showClear = input(true);
  showSecondary = input(true);

  advancedToggled = output<void>();
  filtersCleared = output<void>();
}
