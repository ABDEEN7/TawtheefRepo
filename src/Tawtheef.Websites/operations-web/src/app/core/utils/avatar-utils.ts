export class AvatarUtils {
  public static readonly default = 'assets/img/avatar-placeholder.png';
  public static build(userName: string | null): string {
    return AvatarUtils.default;
    // return userName
    //   ? `https://api.dicebear.com/7.x/initials/svg?seed=${encodeURIComponent(userName)}`
    //   : 'https://placehold.co/30';
  }
}
