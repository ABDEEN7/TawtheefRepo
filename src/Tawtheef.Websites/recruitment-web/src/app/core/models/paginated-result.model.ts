import { PaginationMetadata } from "./pagination-metadata.model";

export interface PaginatedResult<T> {
  items: T[];
  metadata: PaginationMetadata;
  additionalData?: any;
}
