import { TestBed } from '@angular/core/testing';
import { TranslateModule } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { of, throwError } from 'rxjs';

import { NotificationService } from '../../../../../../../core/services/notification.service';
import { AppointmentModel } from '../../models/appointment.model';
import { AppointmentStatus, InterviewType } from '../../models/enums';
import { RescheduleSlotModel } from '../../models/reschedule-slot.model';
import { localToUtcIso } from '../../models/schedule-time';
import { RescheduleDialogComponent, RescheduleDialogData } from './reschedule-dialog';

const slot = (id: string, date: string, start: string, end: string): RescheduleSlotModel => ({
  slotId: id,
  startAt: localToUtcIso(date, start),
  endAt: localToUtcIso(date, end),
  roomId: 'room-1',
  roomNameAr: 'قاعة 1',
  roomNameEn: 'Room 1',
});

const appointment = {
  id: 'appointment-1',
  interviewCommitteeId: 'committee-1',
  interviewType: InterviewType.InPerson,
  roomId: 'room-1',
  roomNameEn: 'Room 1',
  startAt: localToUtcIso('2026-10-05', '09:00'),
  endAt: localToUtcIso('2026-10-05', '09:30'),
  status: AppointmentStatus.Scheduled,
} as AppointmentModel;

describe('RescheduleDialogComponent', () => {
  let dialogRef: jasmine.SpyObj<DynamicDialogRef>;
  let notify: jasmine.SpyObj<NotificationService>;
  let loadSlots: jasmine.Spy;

  function create(slots$ = of<RescheduleSlotModel[]>([])) {
    loadSlots = jasmine.createSpy('loadSlots').and.returnValue(slots$);
    const data: RescheduleDialogData = {
      scheduleId: 'schedule-1',
      appointment,
      candidateName: 'Candidate',
      isRtl: false,
      loadSlots,
    };

    TestBed.configureTestingModule({
      imports: [RescheduleDialogComponent, TranslateModule.forRoot()],
      providers: [
        { provide: DynamicDialogConfig, useValue: { data } },
        { provide: DynamicDialogRef, useValue: dialogRef },
        { provide: NotificationService, useValue: notify },
      ],
    });

    const fixture = TestBed.createComponent(RescheduleDialogComponent);
    fixture.detectChanges();
    return fixture.componentInstance;
  }

  beforeEach(() => {
    dialogRef = jasmine.createSpyObj<DynamicDialogRef>('DynamicDialogRef', ['close']);
    notify = jasmine.createSpyObj<NotificationService>('NotificationService', ['error']);
  });

  it('loads the schedule slots and hands them to the picker with their room', () => {
    const component = create(
      of([slot('a', '2026-10-06', '09:00', '09:30'), slot('b', '2026-10-07', '09:00', '09:30')]),
    );

    expect(loadSlots).toHaveBeenCalledWith('schedule-1', 'appointment-1');
    expect(component.loadState()).toBe('ready');
    expect(component.pickerSlots().map((s) => [s.id, s.location])).toEqual([
      ['a', 'Room 1'],
      ['b', 'Room 1'],
    ]);
    expect(component.current.location).toBe('Room 1');
  });

  it('closes with the chosen slot and the reason', () => {
    const component = create(of([slot('a', '2026-10-06', '09:00', '09:30')]));

    component.selectedSlotId.set('a');
    component.reason = '  Candidate asked for another day  ';
    component.save();

    expect(dialogRef.close).toHaveBeenCalledWith({
      appointmentId: 'appointment-1',
      targetSlotId: 'a',
      reason: 'Candidate asked for another day',
    });
  });

  it('refuses to save without a slot or without a reason', () => {
    const component = create(of([slot('a', '2026-10-06', '09:00', '09:30')]));

    component.reason = 'Reason';
    component.save();
    expect(notify.error).toHaveBeenCalledTimes(1);

    component.selectedSlotId.set('a');
    component.reason = '   ';
    component.save();
    expect(notify.error).toHaveBeenCalledTimes(2);
    expect(dialogRef.close).not.toHaveBeenCalled();
  });

  it('reports a failed load and can retry', () => {
    const component = create(throwError(() => new Error('network')));
    expect(component.loadState()).toBe('error');

    loadSlots.and.returnValue(of([slot('a', '2026-10-06', '09:00', '09:30')]));
    component.load();

    expect(component.loadState()).toBe('ready');
    expect(component.pickerSlots().length).toBe(1);
  });
});
