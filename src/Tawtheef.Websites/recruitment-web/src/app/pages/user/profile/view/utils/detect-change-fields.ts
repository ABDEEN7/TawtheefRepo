export interface FieldChange {
  field: string;
  oldValue: any;
  newValue: any;
}
export function applyFieldChanges<T extends Record<string, any>>(
  value: T | null,
  changes: FieldChange[]
): T | null {
  if (!value || !changes.length) return value;

  const updated: Record<string, any> = { ...value };

  for (const change of changes) {
    if (!change.field) continue;
    const indexedMatch = change.field.match(/^(\w+)\[(\d+)]$/);
    if (indexedMatch) {
      const baseKey = indexedMatch[1];
      const index = Number(indexedMatch[2]);
      const current = Array.isArray(updated[baseKey]) ? [...updated[baseKey]] : [];
      current[index] = change.newValue;
      updated[baseKey] = current;
      continue;
    }

    const current = updated[change.field];
    if (Array.isArray(current)) {
      if (Array.isArray(change.newValue)) {
        updated[change.field] = change.newValue;
      } else if (change.newValue != null) {
        updated[change.field] = [...current, change.newValue];
      } else {
        updated[change.field] = change.newValue;
      }
      continue;
    }

    updated[change.field] = change.newValue;
  }

  return updated as T;
}
export function detectChangedFields(
  oldValueJson: string | null,
  newValueJson: string | null
): FieldChange[] {
  if (!oldValueJson || !newValueJson) {
    return [];
  }

  const oldObj = JSON.parse(oldValueJson);
  const newObj = JSON.parse(newValueJson);

  const changes: FieldChange[] = [];

  const allKeys = new Set([
    ...Object.keys(oldObj),
    ...Object.keys(newObj),
  ]);

  for (const key of allKeys) {
    const oldVal = oldObj[key];
    const newVal = newObj[key];

    if (!isEqual(oldVal, newVal)) {
      changes.push({
        field: key,
        oldValue: oldVal,
        newValue: newVal,
      });
    }
  }

  return changes;
}
function isEqual(a: any, b: any): boolean {
  // Strict equality first
  if (a === b) return true;

  // Handle null / undefined
  if (a == null || b == null) return a === b;

  // Date comparison (ISO strings)
  if (isDateString(a) && isDateString(b)) {
    return new Date(a).getTime() === new Date(b).getTime();
  }

  // Deep object comparison fallback
  return JSON.stringify(a) === JSON.stringify(b);
}

function isDateString(value: any): boolean {
  return typeof value === 'string' && !isNaN(Date.parse(value));
}
