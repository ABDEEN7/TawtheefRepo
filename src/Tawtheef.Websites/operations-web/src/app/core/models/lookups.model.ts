import { GUID } from "../../shared/types/guid.type";

export interface Lookups {
  id: GUID;
  backendName: string;
  name: string;
  description?: string;
}