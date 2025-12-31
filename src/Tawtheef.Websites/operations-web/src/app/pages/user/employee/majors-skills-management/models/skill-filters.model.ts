import {PaginatedRequest} from '../../../../../core/models/paginated-request.model';

export interface SkillFiltersModel  extends PaginatedRequest{
  search?: string;
  skillTypeId?: string;
  pageNumber: number;
  pageSize: number;
}
