import {Component, signal} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {Toast} from 'primeng/toast';
import {CoreModule} from './core/core.module';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Toast,CoreModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('recruitment-web');
}
