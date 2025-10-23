export interface UnverifiedEmailDataModel {
  email: string;
  canResend: boolean;
  resendCooldown: number;
}
