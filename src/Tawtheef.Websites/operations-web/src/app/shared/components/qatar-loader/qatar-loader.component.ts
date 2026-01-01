import {ChangeDetectionStrategy, Component, inject, signal} from '@angular/core';
import {CommonModule} from '@angular/common';
import {LoadingService} from '../../../core/services/loading.service';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {TranslatePipe} from '@ngx-translate/core';

@Component({
  selector: 'app-qatar-loader',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './qatar-loader.component.html',
  styleUrls: ['./qatar-loader.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class QatarLoaderComponent {
  private loadingService = inject(LoadingService);
  loading = signal(false);

  constructor() {
    this.loadingService.loading$
      .pipe(takeUntilDestroyed())
      .subscribe(isLoading => this.loading.set(isLoading));
  }
}
