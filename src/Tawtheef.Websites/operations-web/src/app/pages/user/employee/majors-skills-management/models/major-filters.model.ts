import {PaginatedRequest} from '../../../../../core/models/paginated-request.model';

export interface MajorFiltersModel extends PaginatedRequest{
  search?: string;
  parentMajorId?: string;
}
