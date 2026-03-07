export const authRoutes = {
  auth: '/auth/',
  get forgotPassword() {
    return this.auth + 'forgot-password';
  },
  get login() {
    return this.auth + 'login';
  },
  get refreshToken() {
    return this.auth + 'refresh-token';
  },
  get register() {
    return this.auth + 'register';
  },
  get setPassword() {
    return this.auth + 'set-password';
  },
  get otp() {
    return this.auth + 'otp';
  },
  get lockScreen() {
    return this.auth + 'lock-screen';
  },
  get verifyAccount() {
    return this.auth + 'verify-account';
  },
  get pendingApproval() {
    return this.auth + 'pending-approval';
  },
}
