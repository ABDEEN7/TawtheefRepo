import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { MessageService } from 'primeng/api';
import { Toast } from 'primeng/toast';
import { Tooltip } from 'primeng/tooltip';
import { NotificationTesterService } from './services/notification-tester.service';
import {
  TemplateMetadata,
  TemplateParameter,
  NotificationChannel,
  SendTestRequest,
  PreviewResponse
} from './models/notification-tester.models';
import { environment } from '../../../../../environments/environment';
import { Lang, LanguageService } from '../../../../core/services/language.service';

interface SentLog {
  templateKey: string;
  channel: string;
  toAddress: string;
  time: Date;
  status: 'sent' | 'error';
  notificationId?: string;
  error?: string;
}

@Component({
  selector: 'app-notification-tester',
  standalone: true,
  templateUrl: './notification-tester.page.html',
  styleUrls: ['./notification-tester.page.scss'],
  imports: [
    CommonModule,
    FormsModule,
    Toast,
    Tooltip,
    TranslatePipe
  ],
  providers: [MessageService]
})
export class NotificationTesterPage implements OnInit {
  private testerService = inject(NotificationTesterService);
  private messageService = inject(MessageService);
  private sanitizer = inject(DomSanitizer);
  private languageService = inject(LanguageService);
  private translate = inject(TranslateService);

  // State
  templates = signal<TemplateMetadata[]>([]);
  loading = signal(false);
  sending = signal(false);
  previewing = signal(false);
  showPreview = signal(false);

  // Selections
  selectedChannel = signal<NotificationChannel>('Email');
  selectedTemplateKey = signal<string>('');
  templateSearch = signal('');
  language = signal<string>('ar');

  // Recipient fields
  toEmail = signal('');
  toPhone = signal('');
  toUserId = signal('');

  // Parameters
  paramValues = signal<Record<string, string>>({});

  // Preview
  previewData = signal<PreviewResponse | null>(null);
  previewHtml = signal<SafeHtml>('');

  // Sent log
  sentLog = signal<SentLog[]>([]);

  // Lang
  currentLang = signal<Lang>(this.languageService.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  // Computed
  selectedTemplate = computed(() => {
    const key = this.selectedTemplateKey();
    return this.templates().find(t => t.templateKey === key) ?? null;
  });

  filteredTemplates = computed(() => {
    const search = this.templateSearch().toLowerCase();
    const channel = this.selectedChannel();
    let filtered = this.templates();

    // Filter by channel support
    filtered = filtered.filter(t => t.supportedChannels.includes(channel));

    if (search) {
      filtered = filtered.filter(t =>
        t.templateKey.toLowerCase().includes(search) ||
        t.subjectAr.includes(search) ||
        t.subjectEn.toLowerCase().includes(search)
      );
    }
    return filtered;
  });

  channelTemplateCount = computed(() => {
    const all = this.templates();
    return {
      Email: all.filter(t => t.supportedChannels.includes('Email')).length,
      Sms: all.filter(t => t.supportedChannels.includes('Sms')).length,
      InApp: all.filter(t => t.supportedChannels.includes('InApp')).length
    };
  });

  channels: { key: NotificationChannel; label: string; icon: string; color: string }[] = [
    { key: 'Email', label: 'Email', icon: 'hgi-mail-02', color: '#6366f1' },
    { key: 'Sms', label: 'SMS', icon: 'hgi-smart-phone-01', color: '#10b981' },
    { key: 'InApp', label: 'In-App', icon: 'hgi-notification-03', color: '#f59e0b' }
  ];

  ngOnInit(): void {
    this.languageService.current$.subscribe(lang => this.currentLang.set(lang));
    this.loadTemplates();
  }

  loadTemplates(): void {
    this.loading.set(true);
    this.testerService.getTemplates().subscribe({
      next: (data) => {
        this.templates.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('common.error'),
          detail: this.translate.instant('NOTIFICATION_TESTER.ERROR_LOAD_TEMPLATES')
        });
      }
    });
  }

  selectChannel(channel: NotificationChannel): void {
    this.selectedChannel.set(channel);
    // Reset template selection if current template doesn't support new channel
    const current = this.selectedTemplate();
    if (current && !current.supportedChannels.includes(channel)) {
      this.selectedTemplateKey.set('');
      this.paramValues.set({});
    }
    this.showPreview.set(false);
    this.previewData.set(null);
  }

  selectTemplate(key: string): void {
    this.selectedTemplateKey.set(key);
    this.showPreview.set(false);
    this.previewData.set(null);

    // Initialize parameter values with defaults
    const template = this.templates().find(t => t.templateKey === key);
    if (template) {
      const defaults: Record<string, string> = {};
      template.parameters.forEach(p => {
        defaults[p.name] = p.defaultValue;
      });
      this.paramValues.set(defaults);
    }
  }

  updateParam(name: string, value: string): void {
    this.paramValues.update(current => ({ ...current, [name]: value }));
  }

  onPreview(): void {
    const template = this.selectedTemplate();
    if (!template) return;

    this.previewing.set(true);
    this.testerService.preview({
      templateKey: template.templateKey,
      language: this.language(),
      parameters: this.paramValues()
    }).subscribe({
      next: (data) => {
        this.previewData.set(data);
        this.previewHtml.set(this.sanitizer.bypassSecurityTrustHtml(data.html));
        this.showPreview.set(true);
        this.previewing.set(false);
      },
      error: (err) => {
        this.previewing.set(false);
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('NOTIFICATION_TESTER.PREVIEW_FAILED'),
          detail: err?.error?.error || this.translate.instant('NOTIFICATION_TESTER.ERROR_LOAD_TEMPLATES')
        });
      }
    });
  }

  onSend(): void {
    const template = this.selectedTemplate();
    if (!template) return;

    const channel = this.selectedChannel();

    // Validate recipient
    if (channel === 'Email' && !this.toEmail()) {
      this.messageService.add({ severity: 'warn', summary: this.translate.instant('NOTIFICATION_TESTER.VALIDATION'), detail: this.translate.instant('NOTIFICATION_TESTER.VALIDATION_EMAIL') });
      return;
    }
    if (channel === 'Sms' && !this.toPhone()) {
      this.messageService.add({ severity: 'warn', summary: this.translate.instant('NOTIFICATION_TESTER.VALIDATION'), detail: this.translate.instant('NOTIFICATION_TESTER.VALIDATION_PHONE') });
      return;
    }
    if (channel === 'InApp' && !this.toUserId()) {
      this.messageService.add({ severity: 'warn', summary: this.translate.instant('NOTIFICATION_TESTER.VALIDATION'), detail: this.translate.instant('NOTIFICATION_TESTER.VALIDATION_USER_ID') });
      return;
    }

    const request: SendTestRequest = {
      templateKey: template.templateKey,
      channel: channel,
      language: this.language(),
      parameters: this.paramValues()
    };

    if (channel === 'Email' && this.toEmail()) request.toAddress = this.toEmail();
    if (channel === 'Sms' && this.toPhone()) request.phoneNumber = this.toPhone();
    if (channel === 'InApp' && this.toUserId()) request.userId = this.toUserId();

    this.sending.set(true);
    this.testerService.sendTest(request).subscribe({
      next: (res) => {
        this.sending.set(false);
        this.messageService.add({
          severity: 'success',
          summary: this.translate.instant('NOTIFICATION_TESTER.NOTIFICATION_SENT'),
          detail: res.message,
          life: 5000
        });
        this.sentLog.update(log => [{
          templateKey: template.templateKey,
          channel: channel,
          toAddress: channel === 'Email' ? this.toEmail() : channel === 'Sms' ? this.toPhone() : this.toUserId(),
          time: new Date(),
          status: 'sent' as const,
          notificationId: res.notificationId
        }, ...log].slice(0, 10));
      },
      error: (err) => {
        this.sending.set(false);
        const errorMsg = err?.error?.error || 'Failed to send test notification';
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('NOTIFICATION_TESTER.SEND_FAILED'),
          detail: errorMsg
        });
        this.sentLog.update(log => [{
          templateKey: template.templateKey,
          channel: channel,
          toAddress: channel === 'Email' ? this.toEmail() : channel === 'Sms' ? this.toPhone() : this.toUserId(),
          time: new Date(),
          status: 'error' as const,
          error: errorMsg
        }, ...log].slice(0, 10));
      }
    });
  }

  fillAllTestValues(): void {
    const template = this.selectedTemplate();
    if (!template) return;
    const vals: Record<string, string> = {};
    template.parameters.forEach(p => {
      vals[p.name] = p.defaultValue;
    });
    this.paramValues.set(vals);
  }

  getChannelIcon(channel: string): string {
    return this.channels.find(c => c.key === channel)?.icon || 'hgi-notification-03';
  }

  isProduction(): boolean {
    return environment.production;
  }
}
