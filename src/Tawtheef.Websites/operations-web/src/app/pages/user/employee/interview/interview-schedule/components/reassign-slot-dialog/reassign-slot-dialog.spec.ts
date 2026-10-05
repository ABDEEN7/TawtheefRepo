import { TestBed } from '@angular/core/testing';
import { TranslateModule } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { NotificationService } from '../../../../../../../core/services/notification.service';
import { SlotPreviewModel } from '../../models/plan-preview.model';
import { localToUtcIso } from '../../models/schedule-time';
import { ReassignSlotDialogComponent, ReassignSlotDialogData } from './reassign-slot-dialog';

const preview = (start: string, invitationId: string | null, roomNameEn = 'Room 1'): SlotPreviewModel => ({
  slot: {
    startAt: localToUtcIso('2026-10-06', start),
    endAt: localToUtcIso('2026-10-06', start.replace(':00', ':30')),
    roomId: 'room-' + roomNameEn,
    roomNameEn,
  },
  invitationId,
});

describe('ReassignSlotDialogComponent', () => {
  let dialogRef: jasmine.SpyObj<DynamicDialogRef>;
  let notify: jasmine.SpyObj<NotificationService>;

  function create(slots: SlotPreviewModel[]) {
    const data: ReassignSlotDialogData = {
      candidateInvitationId: 'me',
      candidateName: 'Candidate',
      slots,
      isRtl: false,
      localized: (ar, en) => en || ar || '',
    };

    TestBed.configureTestingModule({
      imports: [ReassignSlotDialogComponent, TranslateModule.forRoot()],
      providers: [
        { provide: DynamicDialogConfig, useValue: { data } },
        { provide: DynamicDialogRef, useValue: dialogRef },
        { provide: NotificationService, useValue: notify },
      ],
    });

    const fixture = TestBed.createComponent(ReassignSlotDialogComponent);
    fixture.detectChanges();
    return fixture.componentInstance;
  }

  beforeEach(() => {
    dialogRef = jasmine.createSpyObj<DynamicDialogRef>('DynamicDialogRef', ['close']);
    notify = jasmine.createSpyObj<NotificationService>('NotificationService', ['error']);
  });

  it("offers only the empty slots and shows the candidate's own slot as the current one", () => {
    const own = preview('09:00', 'me');
    const component = create([own, preview('10:00', 'someone-else'), preview('11:00', null, 'Room 2')]);

    expect(component.current.startAt).toBe(own.slot.startAt);
    expect(component.openSlots.map((s) => [s.id, s.location])).toEqual([
      [localToUtcIso('2026-10-06', '11:00'), 'Room 2'],
    ]);
    expect(component.selectedSlotStartAt()).toBeNull();
  });

  it('closes with the chosen slot start, and refuses to save without one', () => {
    const component = create([preview('09:00', 'me'), preview('11:00', null)]);

    component.save();
    expect(notify.error).toHaveBeenCalled();
    expect(dialogRef.close).not.toHaveBeenCalled();

    component.selectedSlotStartAt.set(component.openSlots[0].id);
    component.save();
    expect(dialogRef.close).toHaveBeenCalledWith(localToUtcIso('2026-10-06', '11:00'));
  });
});
