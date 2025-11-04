import {AfterViewInit, Component} from '@angular/core';
import { TranslateService} from '@ngx-translate/core';
import AOS from 'aos';
import {routes} from '../../../routes/routes';

@Component({
  selector: 'app-home',
  templateUrl: './index.component.html',
  styleUrl: './index.component.scss',
  standalone: false
})
export class IndexComponent implements AfterViewInit {
  tracks = [
    {
      key: 'schools',
      icon: 'fa-school',
      img: 'https://images.unsplash.com/photo-1588072432836-e10032774350?q=80&w=1200&auto=format&fit=crop'
    },
    {
      key: 'ministry',
      icon: 'fa-landmark',
      img: 'https://images.unsplash.com/photo-1552581234-26160f608093?q=80&w=1200&auto=format&fit=crop'
    }
  ];

  whyItems = [
    { key: 'fair',      icon: 'fa-scale-balanced' },
    { key: 'digital',   icon: 'fa-laptop-code'    },
    { key: 'transparent', icon: 'fa-shield-check' },
    { key: 'support',   icon: 'fa-headset'       }
  ];

  successStats = [
    { value: '95%',  label: 'home.success.placementRate' },
    { value: '48h',  label: 'home.success.avgReviewTime' },
    { value: '120+', label: 'home.success.partners'      }
  ];

  faqItems = [
    { q: 'home.faq.q1', a: 'home.faq.a1' },
    { q: 'home.faq.q2', a: 'home.faq.a2' },
    { q: 'home.faq.q3', a: 'home.faq.a3' },
    { q: 'home.faq.q4', a: 'home.faq.a4' }
  ];
  expandedFaq: number | null = 0;
  toggleFaq(i: number) { this.expandedFaq = this.expandedFaq === i ? null : i; }
  constructor(private translate: TranslateService) {
    translate.addLangs(['ar', 'en']);
    translate.setDefaultLang('ar');
    const saved = localStorage.getItem('lang') || 'ar';
    translate.use(saved);
    document.dir = saved === 'ar' ? 'rtl' : 'ltr';
  }

  ngAfterViewInit(): void {
    AOS.init({ once: true, duration: 600 });
  }

  protected readonly routes = routes;
}
