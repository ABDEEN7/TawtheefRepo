import { GUID } from "../../../shared/types/guid.type";

export interface NationalityPreferenceRow {
  nationalityId: GUID | null;
  percentage: number | null;
}