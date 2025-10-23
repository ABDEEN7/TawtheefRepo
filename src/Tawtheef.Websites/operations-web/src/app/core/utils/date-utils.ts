/**
 * Date formatting utilities for dd-MM-yyyy format
 */
export function formatDateToYYYYMMDD(date?: Date): string | null {
  if(!date) return null;
  const year = date.getFullYear();
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const day = date.getDate().toString().padStart(2, '0');

  return `${year}-${month}-${day}`;
}

/**
 * Formats a Date object to dd-MM-yyyy format
 * @param date - Date object to format
 * @returns Formatted date string (dd-MM-yyyy)
 */
export function formatDateToDDMMYYYY(date: Date): string {
  const day = date.getDate().toString().padStart(2, '0');
  const month = (date.getMonth() + 1).toString().padStart(2, '0'); // Months are 0-based
  const year = date.getFullYear();

  return `${day}-${month}-${year}`;
}

/**
 * Formats a date string to dd-MM-yyyy format
 * @param dateString - Date string to parse and format
 * @returns Formatted date string (dd-MM-yyyy)
 * @throws Error if the input string is not a valid date
 */
export function formatDateStringToDDMMYYYY(dateString: string): string {
  const date = new Date(dateString);
  if (isNaN(date.getTime())) {
    throw new Error('Invalid date string');
  }
  return formatDateToDDMMYYYY(date);
}

/**
 * Alternative implementation using Intl.DateTimeFormat
 * @param date - Date object to format
 * @returns Formatted date string (dd-MM-yyyy)
 */
export function formatDateWithIntl(date: Date): string {
  return new Intl.DateTimeFormat('en-GB', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  }).format(date)
    .replace(/\//g, '-'); // Replace slashes with dashes
}
