import {PaginatedRequest} from '../../../../../core/models/paginated-request.model';

export interface SkillFiltersModel  extends PaginatedRequest{
  search?: string;
  skillTypeId?: string;
  isGeneral?: boolean;
  pageNumber: number;
  pageSize: number;
}
