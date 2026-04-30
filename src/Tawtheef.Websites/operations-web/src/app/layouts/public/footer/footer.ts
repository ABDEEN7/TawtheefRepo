import { Component } from '@angular/core';
import {CommonModule} from '@angular/common';
import {TranslatePipe} from '@ngx-translate/core';

@Component({
  selector: 'app-footer',
  imports: [CommonModule, TranslatePipe],
  templateUrl: './footer.html',
  styleUrl: './footer.scss',
})
export class Footer {

  getCurrentYear() {
    return new Date().getFullYear();
  }
}
