import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { LanguageService } from '../../../../core/services/language.service';
import { portalRoutes } from '../../../../routes/portal-routes';
import { TestSlotStatusIds } from './models/test-slot-status.ids';
import { TestSlotsService } from './services/test-slots.service';

export const testSlotEditStatusGuard: CanActivateFn = (route) => {
  const testSlotId = route.paramMap.get('testSlotId');
  if (!testSlotId) return true;

  const service = inject(TestSlotsService);
  const language = inject(LanguageService);
  const router = inject(Router);

  return service.details(testSlotId, language.get()).pipe(
    map((testSlot) =>
      testSlot.status.id === TestSlotStatusIds.ready
        ? true
        : router.parseUrl(portalRoutes.viewTestSlot(testSlotId)),
    ),
    catchError(() => of(true)),
  );
};
