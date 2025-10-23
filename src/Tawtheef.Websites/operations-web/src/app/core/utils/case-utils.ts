export class CaseUtils {
  public static toPascalCase(input: string): string {
    return input
      .toLowerCase()
      .replace(/(?:^|\s|_|-)\w/g, match => match.toUpperCase())
      .replace(/[\s_-]/g, ''); // remove spaces, underscores, hyphens
  }

  public static toCamelCase(input: string): string {
    return input
      .toLowerCase()
      .replace(/(?:^|\s|_|-)(\w)/g, (_, c) => c ? c.toUpperCase() : '')
      .replace(/[\s_-]/g, ''); // remove spaces, underscores, hyphens
  }

  public static toKebabCase(input: string): string {
    return input
      .toLowerCase()
      .replace(/(?:^|\s|_)(\w)/g, (_, c) => c ? '-' + c.toLowerCase() : '')
      .replace(/[\s_]/g, ''); // remove spaces and underscores
  }

  public static toSnakeCase(input: string): string {
    return input
      .toLowerCase()
      .replace(/(?:^|\s|_)(\w)/g, (_, c) => c ? '_' + c.toLowerCase() : '')
      .replace(/[\s-]/g, ''); // remove spaces and hyphens
  }

  public static toTitleCase(input: string): string {
    return input
      .toLowerCase()
      .replace(/\b\w/g, match => match.toUpperCase())
      .replace(/[\s_-]/g, ''); // remove spaces, underscores, hyphens
  }

  public static toSentenceCase(input: string): string {
    if (!input) return input;
    return input
      .toLowerCase()
      .replace(/(^\w|\.\s*\w)/g, match => match.toUpperCase())
      .replace(/[\s_-]/g, ''); // remove spaces, underscores, hyphens
  }

  toLowerCase(input: string): string {
    return input.toLowerCase();
  }

  toUpperCase(input: string): string {
    return input.toUpperCase();
  }

  public static toConstantCase(input: string): string {
    return input
      .toUpperCase()
      .replace(/(?:^|\s|_)(\w)/g, (_, c) => c ? '_' + c : '')
      .replace(/[\s-]/g, ''); // remove spaces and hyphens
  }

  public static toDotCase(input: string): string {
    return input
      .toLowerCase()
      .replace(/(?:^|\s|_)(\w)/g, (_, c) => c ? '.' + c.toLowerCase() : '')
      .replace(/[\s_-]/g, ''); // remove spaces, underscores, hyphens
  }
}
