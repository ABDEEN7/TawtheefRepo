export const MIN_FILTER_YEAR = 1900;
export const MAX_FILTER_YEAR = 9998;

export function parseFilterYear(value: string | null): number | undefined {
  if (!value || !/^\d+$/.test(value)) return undefined;

  const year = Number(value);
  return Number.isInteger(year) && year >= MIN_FILTER_YEAR && year <= MAX_FILTER_YEAR
    ? year
    : undefined;
}
