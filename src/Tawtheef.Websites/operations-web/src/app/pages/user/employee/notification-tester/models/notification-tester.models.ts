export interface TemplateParameter {
  name: string;
  type: string;
  isEnum: boolean;
  enumValues: string[] | null;
  defaultValue: string;
}

export interface TemplateMetadata {
  templateKey: string;
  subjectAr: string;
  subjectEn: string;
  supportedChannels: string[];
  parameters: TemplateParameter[];
}

export interface SendTestRequest {
  templateKey: string;
  channel: string;
  toAddress?: string;
  phoneNumber?: string;
  userId?: string;
  language: string;
  parameters: Record<string, string>;
}

export interface SendTestResponse {
  success: boolean;
  notificationId: string;
  message: string;
}

export interface PreviewRequest {
  templateKey: string;
  language: string;
  parameters: Record<string, string>;
}

export interface PreviewResponse {
  subject: string;
  html: string;
  plainText: string;
}

export type NotificationChannel = 'Email' | 'Sms' | 'InApp';
