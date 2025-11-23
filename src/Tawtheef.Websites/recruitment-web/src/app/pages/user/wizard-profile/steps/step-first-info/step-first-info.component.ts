import {Component, EventEmitter, inject, Output} from '@angular/core';
import { TranslateService } from "@ngx-translate/core";
import { ProfileLookupsService } from "../../services/profile-lookups.service";
import {DataService} from '../../services/data.service';
import {CandidateType} from '../../../../../core/enums/lookups.enum';

@Component({
  selector: 'app-step-first-info',
  templateUrl: './step-first-info.component.html',
  styleUrl: './step-first-info.component.scss',
  standalone: false
})
export class StepFirstInfoComponent {
  @Output() next = new EventEmitter<void>();
  ds = inject(DataService);

  translate = inject(TranslateService);
  lookups = inject(ProfileLookupsService);
  get isNeedBirthCertificate(){
    if(!this.ds.state().candidateType) return false;
    return [CandidateType.SonOfQatariMother].includes(
      this.ds.state().candidateType?.backendName as CandidateType
    )
  }
  get isNeedMarriageCertificate(){
    if(!this.ds.state().candidateType) return false;
    return [CandidateType.WifeOfQatari].includes(
      this.ds.state().candidateType?.backendName as CandidateType
    )
  }
}
