import {Component, signal} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {TranslateModule} from '@ngx-translate/core';
import {Toast} from 'primeng/toast';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TranslateModule, Toast],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('recruitment-web');
}
