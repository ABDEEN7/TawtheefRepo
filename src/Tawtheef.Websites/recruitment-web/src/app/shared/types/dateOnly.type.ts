export type DateOnly = string & { readonly __brand: "DateOnly" };
export function toDateOnly(value: string): DateOnly {
  return value as DateOnly;
}
export function dateToDateOnly(date: Date): DateOnly {
  const year = date.getFullYear();
  const month = date.getMonth();
  const day = date.getDate();
  const selectedDateObject = new Date(Date.UTC(year, month, day));

  const dateString = selectedDateObject.toISOString().slice(0, 10);
  return toDateOnly(dateString);
}
