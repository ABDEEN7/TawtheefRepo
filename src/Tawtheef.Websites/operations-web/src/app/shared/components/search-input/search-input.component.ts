import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-search-input',
  standalone: true,
  imports: [FormsModule, InputTextModule],
  templateUrl: './search-input.component.html',
  styleUrl: './search-input.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SearchInputComponent {
  readonly value = input('');
  readonly placeholder = input('');
  readonly ariaLabel = input.required<string>();
  readonly disabled = input(false);

  readonly valueChange = output<string>();
}
