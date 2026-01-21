import {AfterViewInit, Component} from '@angular/core';
import AOS from 'aos';
import {routes} from '../../../routes/routes';

@Component({
  selector: 'app-home',
  templateUrl: './index.component.html',
  styleUrl: './index.component.scss',
  standalone: false
})

export class IndexComponent implements AfterViewInit {
  protected readonly routes = routes;
  activeTab: string = 'schools';

  tracks = [
    {key: 'schools', },
    { key: 'ministry', },
    
  ];

  setActiveTab(tabKey: string) {
    this.activeTab = tabKey;
  }
  whyItems = [
    { key: 'fair',      icon: 'hgi-star-award-02' },
    { key: 'digital',   icon: 'fa-laptop-code'    },
    { key: 'transparent', icon: 'hgi-justice-scale-01' },
    { key: 'support',   icon: 'hgi-laptop-programming'       }
  ];
  successStats = [
    { profile: 'home.success.profile1', desc: 'home.success.desc1', value: 'home.success.label1', label: 'home.success.placementRate',  img: 'https://images.unsplash.com/photo-1552581234-26160f608093?q=80&w=1200&auto=format&fit=crop'},
    { profile: 'home.success.profile2', desc: 'home.success.desc2', value: 'home.success.label2',  label: 'home.success.avgReviewTime', img: 'https://images.unsplash.com/photo-1552581234-26160f608093?q=80&w=1200&auto=format&fit=crop' },
     { profile: 'home.success.profile3', desc: 'home.success.desc3', value: 'home.success.label3', label: 'home.success.partners', img: 'https://images.unsplash.com/photo-1552581234-26160f608093?q=80&w=1200&auto=format&fit=crop'      }
  ];
//   reviews = [
//   {
//     name: 'John Doe',
//     position: 'CEO, ABC Company',
//     avatar: 'assets/avatars/john.jpg',
//     message: 'Great service and excellent support!'
//   },
//   {
//     name: 'Sara Khan',
//     position: 'HR Manager, XYZ Ltd',
//     avatar: 'assets/avatars/sara.jpg',
//     message: 'Very professional team and timely delivery.'
//   },
//   {
//     name: 'Ali Ahmed',
//     position: 'Project Manager, QRS',
//     avatar: 'assets/avatars/ali.jpg',
//     message: 'Highly recommended for all UI/UX needs.'
//   }
// ];

  faqItems = [
    { q: 'home.faq.q1', a: 'home.faq.a1' },
    { q: 'home.faq.q2', a: 'home.faq.a2' },
    { q: 'home.faq.q3', a: 'home.faq.a3' },
    { q: 'home.faq.q4', a: 'home.faq.a4' }
  ];
  expandedFaq: number | null = 0;

  toggleFaq(i: number) { this.expandedFaq = this.expandedFaq === i ? null : i; }

  ngAfterViewInit(): void {
    AOS.init({ once: true, duration: 600 });
  }
}
