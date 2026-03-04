import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { LanguageService } from './core/services/language.service';
import { Toast } from 'primeng/toast';
import { ProgressBarComponent } from './shared/components/progress-bar/progress-bar.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Toast, ProgressBarComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('recruitment-web');
}
