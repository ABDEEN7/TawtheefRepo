import { Component, OnInit, inject, OnDestroy } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subscription } from 'rxjs';
import { InAppNotificationService } from '../../../core/services/in-app-notification.service';
import { NotificationModel } from '../../../shared/models/notification.model';
import { RelativeTimePipe } from '../../../shared/pipes/relative-time.pipe';
import { NotificationDetailsDialogComponent } from '../../../shared/components/notification-details-dialog/notification-details-dialog.component';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, TranslateModule, RelativeTimePipe],
  templateUrl: './notifications.page.html',
  styleUrl: './notifications.page.scss'
})
export class NotificationsPage implements OnInit, OnDestroy {
  private notificationsApi = inject(InAppNotificationService);
  private translate = inject(TranslateService);
  private modalService = inject(NgbModal);

  notifications: NotificationModel[] = [];
  selectedIds = new Set<string>();
  isLoading = false;
  unreadOnly = false;
  hasMore = true;
  limit = 20;

  private subscriptions = new Subscription();

  ngOnInit() {
    this.subscriptions.add(
      this.notificationsApi.notifications$.subscribe(list => {
        this.notifications = list.map(n => ({
          ...n,
          content: this.getPreview(n.body ?? '')
        }));
      })
    );
    this.subscriptions.add(
      this.notificationsApi.isLoading$.subscribe(state => (this.isLoading = state))
    );
    this.refresh();
  }
  private getPreview(html: string, maxLength: number = 120): string {
    const text = this.htmlToText(html)
      .replace(/\s+/g, ' ')
      .trim();

    return text.length > maxLength
      ? text.substring(0, maxLength) + '...'
      : text;
  }

  private htmlToText(html: string): string {
    const doc = new DOMParser().parseFromString(html, 'text/html');
    return doc.body.textContent || '';
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  refresh() {
    this.hasMore = true;
    this.notificationsApi.refresh(this.limit, true, this.unreadOnly).subscribe(res => {
      this.hasMore = res.length >= this.limit;
    });
  }

  loadMore() {
    if (this.isLoading || !this.hasMore) return;

    const lastNotif = this.notifications[this.notifications.length - 1];
    if (!lastNotif) return;

    this.notificationsApi.refresh(this.limit, true, this.unreadOnly, lastNotif.createdDate, true).subscribe(res => {
      this.hasMore = res.length >= this.limit;
    });
  }

  openDetails(notification: NotificationModel) {
    if (!notification.isRead) {
      this.markAsRead(notification.id);
    }
    const modalRef = this.modalService.open(NotificationDetailsDialogComponent, {
      size: 'lg',
      centered: true,
      backdrop: 'static'
    });
    modalRef.componentInstance.notification = notification;
  }

  toggleUnreadFilter() {
    this.unreadOnly = !this.unreadOnly;
    this.refresh();
  }

  toggleSelection(id: string) {
    if (this.selectedIds.has(id)) {
      this.selectedIds.delete(id);
    } else {
      this.selectedIds.add(id);
    }
  }

  toggleAll(event: any) {
    if (event.target.checked) {
      this.notifications.forEach(n => this.selectedIds.add(n.id));
    } else {
      this.selectedIds.clear();
    }
  }

  isAllSelected(): boolean {
    return this.notifications.length > 0 && this.selectedIds.size === this.notifications.length;
  }

  bulkMarkAsRead() {
    if (this.selectedIds.size === 0) return;
    this.notificationsApi.markManyAsRead(Array.from(this.selectedIds))
      .subscribe(() => {
        this.selectedIds.clear();
        this.refresh();
      });
  }

  bulkDismiss() {
    if (this.selectedIds.size === 0) return;
    this.notificationsApi.dismissMany(Array.from(this.selectedIds))
      .subscribe(() => {
        this.selectedIds.clear();
        this.refresh();
      });
  }

  markAsRead(id: string) {
    this.notificationsApi.markAsRead(id).subscribe(() => this.refresh());
  }

  dismiss(id: string) {
    this.notificationsApi.dismiss(id).subscribe(() => this.refresh());
  }

  notificationTrackBy(index: number, n: NotificationModel) {
    return n.id;
  }
}
