/**
 * One entry of a server-searched dropdown. `data` carries whatever the caller needs back on selection
 * (the full job context, the picked user, ...) so it never has to look the record up a second time.
 */
export interface LazySelectOption<T = unknown> {
  value: string;
  label: string;
  hint?: string | null;
  /**
   * Text PrimeNG's built-in filter matches against. The server already narrowed the list by name/e-mail in either
   * language, so this must include all of those or the client-side filter would hide valid server results.
   */
  searchText?: string;
  data?: T;
}
