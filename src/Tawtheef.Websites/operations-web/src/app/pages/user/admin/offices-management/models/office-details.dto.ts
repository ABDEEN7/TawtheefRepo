import {OfficeDto} from './office.dto';
import {OfficeUserDto} from './office-user.dto';

export interface OfficeDetailsDto extends OfficeDto {
  users: OfficeUserDto[];
}
