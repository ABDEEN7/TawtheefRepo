export interface NotificationModel {
  id: string;
  subject?: string;
  body?: string;
  status: string;
  createdDate: string;
  sentAtUtc?: string | null;
  error?: string | null;
}
