export class ObjectExtensions {
  static isEmpty(obj: Record<string, any>): boolean {
    return Object.keys(obj).length === 0 && obj.constructor === Object;
  }

  static isNotEmpty(obj: Record<string, any>): boolean {
    return !this.isEmpty(obj);
  }

  static isNullOrUndefined(obj: any): boolean {
    return obj === null || obj === undefined;
  }

  static ignoredNull(filters: any): any {
    return Object.fromEntries(
      Object.entries(filters).filter(([_, value]) => value !== '' && value !== null)
    );
  }
}
