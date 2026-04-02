export interface NotificationModel {
  id: string;
  subject?: string;
  content: string;
  body?: string;
  status: string;
  createdDate: string;
  sentAtUtc?: string | null;
  error?: string | null;
  isRead: boolean;
  isDismissed: boolean;
}
