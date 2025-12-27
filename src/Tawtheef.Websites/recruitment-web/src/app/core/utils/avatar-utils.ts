export class AvatarUtils {
  public static readonly default = 'https://placehold.co/30';
public static build(userName: string | null): string {
    return userName
      ? `https://api.dicebear.com/7.x/initials/svg?seed=${encodeURIComponent(userName)}`
      : 'https://placehold.co/30';
  }
}
