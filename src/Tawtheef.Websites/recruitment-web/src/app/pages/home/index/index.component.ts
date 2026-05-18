import { AfterViewInit, Component } from '@angular/core';
import AOS from 'aos';

type HomeValue = {
  key: string;
  icon: string;
};

type ContactItem = {
  key: string;
  icon: string;
};

@Component({
  selector: 'app-home',
  templateUrl: './index.component.html',
  styleUrl: './index.component.scss',
  standalone: false
})
export class IndexComponent implements AfterViewInit {
  values: HomeValue[] = [
    { key: 'environment', icon: 'hgi-award-02' },
    { key: 'development', icon: 'hgi-chart-increase' },
    { key: 'fairness', icon: 'hgi-justice-scale-01' },
    { key: 'digital', icon: 'hgi-laptop-programming' },
  ];

  contacts: ContactItem[] = [
    { key: 'hotline', icon: 'hgi-rounded hgi-calling-02' },
    { key: 'email', icon: 'hgi-mail-01' }
  ];

  ngAfterViewInit(): void {
    AOS.init({ once: true, duration: 600 });
  }
}
