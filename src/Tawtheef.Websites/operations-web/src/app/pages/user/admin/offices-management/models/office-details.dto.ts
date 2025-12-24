import {OfficeDto} from './office.dto';
import {OfficeUserDto} from './office-user.dto';
import {GUID} from '../../../../../shared/types/guid.type';

export interface OfficeDetailsDto extends OfficeDto {
  countryId: GUID;
  users: OfficeUserDto[];
}
