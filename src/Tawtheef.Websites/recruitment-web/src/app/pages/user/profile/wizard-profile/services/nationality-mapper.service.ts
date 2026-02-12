import {inject, Injectable} from "@angular/core";
import {ProfileLookupsService} from "./profile-lookups.service";
import {dropdownOptionsModel, DropdownOptionVM} from '../../../../../shared/models/dropdown-options.model';

@Injectable({providedIn: 'root'})
export class NationalityMapperService {
  private lookups = inject(ProfileLookupsService);

  /**
   * raw: can be either +962777123456 or 0777123456
   * defaultRegion: used when number doesn't start with + (e.g. JO, SA, AE...)
   */
  toNationalityObject(raw?: string | null | undefined): DropdownOptionVM | null {
    if (!raw) return null;
    return this.lookups.nationalities().find(x => x.additionalData?.["code"] == raw || x.id == raw) ?? null;
  }
}
