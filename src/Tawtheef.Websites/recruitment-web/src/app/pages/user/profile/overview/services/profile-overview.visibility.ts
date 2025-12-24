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

export function createProfileOverviewVisibility(p: ProfileStatusDto) {
  return computed(() => {
    // If you only have IDs in ProfileStatusDto, map to "state-like" shape or adapt the helpers.
    // Example assumes p has candidateTypeId/provider etc. Adjust as needed.
    const s = {
      candidateType: { id: p?.candidateTypeId },
      provider: p?.provider
    } as any;

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
