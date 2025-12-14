export interface PaginationMetadata {
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNext: boolean;
}
