import {Component} from '@angular/core';
import {DynamicDialogRef, DynamicDialogConfig} from 'primeng/dynamicdialog';
import {ScrollerOptions} from 'primeng/api';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {TranslatePipe} from '@ngx-translate/core';
import {Select} from 'primeng/select';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-residents-modal',
  standalone: true,
  imports: [
    I18nNamespaceDirective,
    TranslatePipe,
    Select,
    FormsModule
  ],
  templateUrl: './residents-modal.component.html'
})
export class ResidentsModalComponent {
  nationalities = ['مصر', 'الأردن', 'تونس', 'المغرب', 'فلسطين', 'السودان', 'لبنان', 'سوريا', 'الهند', 'باكستان'];
  breakdown: { nat: string; pct: number }[] = [];
  nationality = '';
  percent = 0;
  target = 0;
  lazyLoading = false
  loadLazyTimeout = 0
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {
    // Receive data from the parent
    if (config.data) {
      this.target = config.data.target || 0;
      this.breakdown = config.data.breakdown ? [...config.data.breakdown] : [];
    }
  }

  nationalitieOptions: ScrollerOptions = {
    delay: 250,
    showLoader: true,
    lazy: true,
    onLazyLoad: this.loadNationalitiesLazy.bind(this)
  };

  nationalitiesOptions = this.nationalities.map(d => ({label: d, value: d}));

  loadNationalitiesLazy(event: any) {
    this.lazyLoading = true;
    this.loadLazyTimeout = setTimeout(() => {
      const {first, last} = event;
      const items = [...this.nationalities];
      for (let i = first; i < last; i++) {
        items[i] = this.nationalities[i];
      }
      this.nationalities = items;
      this.lazyLoading = false;
    }, Math.random() * 1000 + 250);
  }

  add() {
    if (!this.nationality || this.percent <= 0) return;
    this.breakdown.push({nat: this.nationality, pct: this.percent});
    this.nationality = '';
    this.percent = 0;
  }

  remove(i: number) {
    this.breakdown.splice(i, 1);
  }

  get total() {
    return this.breakdown.reduce((a, b) => a + b.pct, 0);
  }

  save() {
    if (this.total !== this.target) {
      //alert(`يجب أن يساوي المجموع ${this.target}%`);
      return;
    }
    this.ref.close(this.breakdown);
  }

  close() {
    this.ref.close(null);
  }
}
