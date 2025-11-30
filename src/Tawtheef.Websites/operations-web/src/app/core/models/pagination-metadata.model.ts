export interface PaginationMetadata {
  totalCount: number;
  pageSize: number;
  pageNumber: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}