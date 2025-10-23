/**
 * String comparison utilities with case-insensitive and partial matching options
 */
export class StringUtils {
  /**
   * Case-insensitive string equality check
   * @param str1 First string to compare
   * @param str2 Second string to compare
   * @returns true if strings are equal ignoring case
   */
  static equalsIgnoreCase(str1: string, str2: string): boolean {
    return str1.localeCompare(str2, undefined, { sensitivity: 'accent' }) === 0;
  }

  /**
   * Check if string contains substring (case-insensitive)
   * @param source The string to search in
   * @param search The substring to search for
   * @returns true if source contains search string (case-insensitive)
   */
  static containsIgnoreCase(source: string, search: string): boolean {
    return source.toLowerCase().includes(search.toLowerCase());
  }

  /**
   * Check if string starts with prefix (case-insensitive)
   * @param str The string to check
   * @param prefix The prefix to look for
   * @returns true if string starts with prefix (case-insensitive)
   */
  static startsWithIgnoreCase(str: string, prefix: string): boolean {
    return str.toLowerCase().startsWith(prefix.toLowerCase());
  }

  /**
   * Check if string ends with suffix (case-insensitive)
   * @param str The string to check
   * @param suffix The suffix to look for
   * @returns true if string ends with suffix (case-insensitive)
   */
  static endsWithIgnoreCase(str: string, suffix: string): boolean {
    return str.toLowerCase().endsWith(suffix.toLowerCase());
  }

  /**
   * Advanced string comparison with multiple options
   * @param str1 First string to compare
   * @param str2 Second string to compare
   * @param options Comparison options
   * @returns true if strings match according to options
   */
  static compare(
    str1: string,
    str2: string,
    options: {
      ignoreCase?: boolean;
      partial?: boolean;
      trim?: boolean;
      startsWith?: boolean;
      endsWith?: boolean;
    } = {}
  ): boolean {
    let s1 = str1;
    let s2 = str2;

    if (options.trim) {
      s1 = s1.trim();
      s2 = s2.trim();
    }

    if (options.ignoreCase) {
      s1 = s1.toLowerCase();
      s2 = s2.toLowerCase();
    }

    if (options.partial) {
      return s1.includes(s2);
    }

    if (options.startsWith) {
      return s1.startsWith(s2);
    }

    if (options.endsWith) {
      return s1.endsWith(s2);
    }

    return s1 === s2;
  }

  /**
   * Normalize strings before comparison (trim + lowercase)
   * @param str String to normalize
   * @returns Normalized string
   */
  static normalize(str: string): string {
    return str.trim().toLowerCase();
  }

  /**
   * Compare strings after normalization
   * @param str1 First string
   * @param str2 Second string
   * @returns true if normalized strings are equal
   */
  static normalizedEquals(str1: string, str2: string): boolean {
    return this.normalize(str1) === this.normalize(str2);
  }

  /**
   * Check if any string in an array matches the search string
   * @param strings Array of strings to search
   * @param search String to search for
   * @param options Comparison options
   * @returns true if any string in array matches
   */
  static anyMatch(
    strings: string[],
    search: string,
    options: {
      ignoreCase?: boolean;
      partial?: boolean;
    } = {}
  ): boolean {
    return strings.some(str => this.compare(str, search, options));
  }
}
