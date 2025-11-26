export function isFilledScalar(value: unknown): boolean {
  if (value === null || value === undefined) return false;

  if (typeof value === 'string') {
    return value.trim().length > 0;
  }

  // For dropdowns / objects with id or value
  if (typeof value === 'object') {
    const v: any = value;
    if ('id' in v && v.id !== null && v.id !== undefined) return true;
    if ('value' in v && v.value !== null && v.value !== undefined) return true;
  }

  return true;
}
