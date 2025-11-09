import { Component, EventEmitter, Output, inject } from '@angular/core';
import { DataService } from '../../services/data.service';

@Component({
  selector: 'app-step-contact',
  templateUrl: './step-contact.component.html',
  styleUrl: './step-contact.component.scss',
  standalone: false,
})
export class StepContactComponent {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();
  ds = inject(DataService);

  countries = [
    { name:'قطر', code:'+974', iso: 'qa' }, { name:'السعودية', code:'+966', iso: 'sa' }, { name:'الإمارات', code:'+971', iso: 'ae' },
    { name:'البحرين', code:'+973', iso: 'br' }, { name:'الكويت', code:'+965', iso: 'kw' }, { name:'الأردن', code:'+962', iso: 'jo' },
    { name:'مصر', code:'+20', iso: 'eg' }, { name:'تونس', code:'+216', iso: 'ta' }, { name:'المغرب', code:'+212', iso: 'mg' }
  ];
}
