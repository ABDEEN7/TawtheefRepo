import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ToggleSwitchModule} from 'primeng/toggleswitch';
import {Tooltip} from 'primeng/tooltip';
import {DialogService} from 'primeng/dynamicdialog';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {NotificationService} from '../../../../core/services/notification.service';
import {Lang, LanguageService} from '../../../../core/services/language.service';
import {DialogHelperService} from '../../../../core/services/dialog-helper.service';
import {HomeContentManagementService} from './services/home-content-management.service';
import {HomeSuccessStory, HomeSuccessStoryPayload} from './models/home-success-story.model';
import {FAQ, FaqPayload} from './models/faq.model';
import {HomeSuccessStoryDialogComponent} from './success-story-dialog/home-success-story-dialog.component';
import {HomeFaqDialogComponent} from './faq-dialog/home-faq-dialog.component';

interface SuccessStoryDialogResult {
  id: string | null;
  payload: HomeSuccessStoryPayload;
}

interface FaqDialogResult {
  id: string | null;
  payload: FaqPayload;
}

@Component({
  selector: 'app-home-content-management',
  standalone: true,
  templateUrl: './home-content-management.component.html',
  styleUrls: ['./home-content-management.component.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    ToggleSwitchModule,
    Tooltip,
    I18nNamespaceDirective
  ],
  providers: [DialogService]
})
export class HomeContentManagementComponent implements OnInit {
  private dialogService = inject(DialogService);
  private homeContentService = inject(HomeContentManagementService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private dialogHelper = inject(DialogHelperService);

  private _successStories = signal<HomeSuccessStory[]>([]);
  private _faqs = signal<FAQ[]>([]);

  successStories = this._successStories.asReadonly();
  faqs = this._faqs.asReadonly();

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  ngOnInit(): void {
    this.loadSuccessStories();
    this.loadFaqs();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadSuccessStories() {
    this.homeContentService.getSuccessStories().subscribe({
      next: items => this._successStories.set([...items].sort(this.sortByOrder))
    });
  }

  loadFaqs() {
    this.homeContentService.getFaqs().subscribe({
      next: items => this._faqs.set([...items].sort(this.sortByOrder))
    });
  }

  openAddSuccessStory() {
    const ref = this.dialogService.open(HomeSuccessStoryDialogComponent, {
      header: this.translate.instant('HOME_CONTENT.ADD_SUCCESS_STORY'),
      width: '720px',
      contentStyle: { 'border-radius': '12px' },
      data: { mode: 'create' }
    });

    ref?.onClose.subscribe((result?: SuccessStoryDialogResult) => {
      if (!result?.payload) return;
      this.createSuccessStory(result.payload);
    });
  }

  openEditSuccessStory(story: HomeSuccessStory) {
    const ref = this.dialogService.open(HomeSuccessStoryDialogComponent, {
      header: this.translate.instant('HOME_CONTENT.EDIT_SUCCESS_STORY'),
      width: '720px',
      contentStyle: { 'border-radius': '12px' },
      data: { mode: 'edit', story }
    });

    ref?.onClose.subscribe((result?: SuccessStoryDialogResult) => {
      if (!result?.payload || !story.id) return;
      this.updateSuccessStory(story.id, result.payload);
    });
  }

  createSuccessStory(payload: HomeSuccessStoryPayload) {
    this.homeContentService.createSuccessStory(payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('HOME_CONTENT.SAVE_SUCCESS'));
        this.loadSuccessStories();
      }
    });
  }

  updateSuccessStory(id: string, payload: HomeSuccessStoryPayload) {
    this.homeContentService.updateSuccessStory(id, payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('HOME_CONTENT.SAVE_SUCCESS'));
        this.loadSuccessStories();
      }
    });
  }

  toggleSuccessStoryStatus(story: HomeSuccessStory) {
    const updated = !story.isActive;
    this.homeContentService.updateSuccessStoryStatus(story.id, updated).subscribe({
      next: () => {
        this._successStories.update(items =>
          items.map(item => (item.id === story.id ? {...item, isActive: updated} : item))
        );
        this.notification.success(this.translate.instant('HOME_CONTENT.STATUS_UPDATED'));
      }
    });
  }

  confirmDeleteSuccessStory(story: HomeSuccessStory) {
    const dialogRef = this.dialogHelper.openConfirmDialog({
      type: 'delete',
      title: 'HOME_CONTENT.DELETE_SUCCESS_STORY_TITLE',
      description: 'HOME_CONTENT.DELETE_SUCCESS_STORY_DESC',
    });

    dialogRef?.onClose.subscribe(result => {
      if (!result) return;
      this.homeContentService.deleteSuccessStory(story.id).subscribe({
        next: () => {
          this.notification.success(this.translate.instant('HOME_CONTENT.DELETE_SUCCESS'));
          this._successStories.update(items => items.filter(item => item.id !== story.id));
        }
      });
    });
  }

  openAddFaq() {
    const ref = this.dialogService.open(HomeFaqDialogComponent, {
      header: this.translate.instant('HOME_CONTENT.ADD_FAQ'),
      width: '640px',
      contentStyle: { 'border-radius': '12px' },
      data: { mode: 'create' }
    });

    ref?.onClose.subscribe((result?: FaqDialogResult) => {
      if (!result?.payload) return;
      this.createFaq(result.payload);
    });
  }

  openEditFaq(faq: FAQ) {
    const ref = this.dialogService.open(HomeFaqDialogComponent, {
      header: this.translate.instant('HOME_CONTENT.EDIT_FAQ'),
      width: '640px',
      contentStyle: { 'border-radius': '12px' },
      data: { mode: 'edit', faq }
    });

    ref?.onClose.subscribe((result?: FaqDialogResult) => {
      if (!result?.payload || !faq.id) return;
      this.updateFaq(faq.id, result.payload);
    });
  }

  createFaq(payload: FaqPayload) {
    this.homeContentService.createFaq(payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('HOME_CONTENT.SAVE_SUCCESS'));
        this.loadFaqs();
      }
    });
  }

  updateFaq(id: string, payload: FaqPayload) {
    this.homeContentService.updateFaq(id, payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('HOME_CONTENT.SAVE_SUCCESS'));
        this.loadFaqs();
      }
    });
  }

  toggleFaqStatus(faq: FAQ) {
    const updated = !faq.isActive;
    this.homeContentService.updateFaqStatus(faq.id, updated).subscribe({
      next: () => {
        this._faqs.update(items =>
          items.map(item => (item.id === faq.id ? {...item, isActive: updated} : item))
        );
        this.notification.success(this.translate.instant('HOME_CONTENT.STATUS_UPDATED'));
      }
    });
  }

  confirmDeleteFaq(faq: FAQ) {
    const dialogRef = this.dialogHelper.openConfirmDialog({
      type: 'delete',
      title: 'HOME_CONTENT.DELETE_FAQ_TITLE',
      description: 'HOME_CONTENT.DELETE_FAQ_DESC'
    });

    dialogRef?.onClose.subscribe(result => {
      if (!result) return;
      this.homeContentService.deleteFaq(faq.id).subscribe({
        next: () => {
          this.notification.success(this.translate.instant('HOME_CONTENT.DELETE_SUCCESS'));
          this._faqs.update(items => items.filter(item => item.id !== faq.id));
        }
      });
    });
  }

  private sortByOrder(a: { displayOrder: number }, b: { displayOrder: number }) {
    return a.displayOrder - b.displayOrder;
  }
}
