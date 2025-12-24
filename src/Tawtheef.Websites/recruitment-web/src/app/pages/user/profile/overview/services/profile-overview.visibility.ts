import { computed } from '@angular/core';
import {
  candidateTypeFromState,
  candidateTypeIsResident,
  candidateTypeNeedsBirthCertificate,
  candidateTypeNeedsMarriageCertificate,
  candidateTypeNeedsSponsor
} from '../../wizard-profile/state/profile-step-validity.signal';
import {ProfileStatusDto} from '../../../../../core/models/auth/auth-response.model';
import {MaritalStatus} from '../../../../../core/enums/lookups.enum';
import {ProfileState} from '../../wizard-profile/models/profile-state.model';

export function createProfileOverviewVisibility(p: ProfileStatusDto) {
  return computed(() => {
    const s = {
      candidateType: p?.candidateType,
      provider: p?.provider
    } as ProfileState;

    const type = candidateTypeFromState(s);
    const isResident = candidateTypeIsResident(type, s.provider);

    return {
      type,
      isResident,
      needsSponsor: candidateTypeNeedsSponsor(type),
      needsBirth: candidateTypeNeedsBirthCertificate(type),
      needsMarriage: candidateTypeNeedsMarriageCertificate(type),

      // Derived flags (optional)
      showOffice: !isResident,
      showQidExpiry: isResident,
      showNumberOfChild: p.maritalStatus?.backendName != MaritalStatus.Single,
      showNationalAddress: isResident,
      showForeignAddress: !isResident,
      showSponsorSection: candidateTypeNeedsSponsor(type)
    };
  });
}
