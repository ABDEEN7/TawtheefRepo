import { AfterViewInit, Component, computed, inject, OnInit, signal } from '@angular/core';
import { finalize } from 'rxjs/operators';
import AOS from 'aos';
import { routes } from '../../../routes/routes';
import { CarouselResponsiveOptions } from 'primeng/carousel';
import { HomeContentService } from '../services/home-content.service';
import { FAQ, HomeSuccessStory } from '../models/home-content.model';
import { Lang, LanguageService } from '../../../core/services/language.service';
import { AvatarUtils } from '../../../core/utils/avatar-utils';


@Component({
  selector: 'app-home',
  templateUrl: './index.component.html',
  styleUrl: './index.component.scss',
  standalone: false
})

export class IndexComponent implements OnInit, AfterViewInit {
  private homeContentService = inject(HomeContentService);
  private languageService = inject(LanguageService);
  defaultAvatar = AvatarUtils.default;
  pageLoading = signal(true);

  protected readonly routes = routes;
  activeTab: string = 'schools';

  tracks = [
    { key: 'schools', },
    { key: 'ministry', },

  ];

  setActiveTab(tabKey: string) {
    this.activeTab = tabKey;
  }
  whyItems = [
    { key: 'fair', icon: 'fa-award' },
    { key: 'digital', icon: 'fa-laptop-code' },
    { key: 'transparent', icon: 'fa-scale-balanced' },
    { key: 'support', icon: 'fa-headset' }
  ];
  responsiveOptions: CarouselResponsiveOptions[] = [

    {
      breakpoint: '1024px',
      numVisible: 2,
      numScroll: 1
    },
    {
      breakpoint: '768px',
      numVisible: 1,
      numScroll: 1
    }
  ];
  private _successStories = signal<HomeSuccessStory[]>([]);
  private _faqItems = signal<FAQ[]>([]);

  currentLang = signal<Lang>(this.languageService.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  successStoriesView = computed(() =>
    this._successStories().map(story => ({
      id: story.id,
      profile: this.isRtl() ? story.nameAr : story.nameEn,
      desc: this.isRtl() ? story.roleAr : story.roleEn,
      value: this.isRtl() ? story.metricTitleAr : story.metricTitleEn,
      label: this.isRtl() ? story.metricDescriptionAr : story.metricDescriptionEn,
      img: story.imageUrl
    }))
  );

  faqItemsView = computed(() =>
    this._faqItems().map(item => ({
      id: item.id,
      question: this.isRtl() ? item.questionAr : item.questionEn,
      answer: this.isRtl() ? item.answerAr : item.answerEn
    }))
  );

  expandedFaq: number | null = 0;

  toggleFaq(i: number) { this.expandedFaq = this.expandedFaq === i ? null : i; }

  ngOnInit(): void {
    this.languageService.current$.subscribe(lang => this.currentLang.set(lang));
    this.loadHomeContent();
  }

  loadHomeContent() {
    this.homeContentService.getHomeContent()
      .pipe(finalize(() => this.pageLoading.set(false)))
      .subscribe({
        next: response => {
          this._successStories.set(response.successStories ?? []);
          this._faqItems.set(response.faqs ?? []);
          this.expandedFaq = (response.faqs ?? []).length > 0 ? 0 : null;
        }
      });
  }

  ngAfterViewInit(): void {
    AOS.init({ once: true, duration: 600 });
  }

}
