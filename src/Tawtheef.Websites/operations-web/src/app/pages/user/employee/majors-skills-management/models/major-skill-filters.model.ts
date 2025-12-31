import {PaginatedRequest} from '../../../../../core/models/paginated-request.model';

export interface MajorSkillFiltersModel  extends PaginatedRequest{
  search?: string;
  parentMajorId?: string;
  subMajorId?: string;
  skillTypeId?: string;
  isActive?: boolean | null;
}
