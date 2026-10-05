import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslateModule } from '@ngx-translate/core';

import { localToUtcIso } from '../../models/schedule-time';
import { PickerSlot, SlotPickerComponent } from './slot-picker';

// Slots are built from local day + time (like schedule-time.spec), so the day grouping holds in any test timezone.
const slot = (id: string, date: string, start: string, end: string, location = 'Room 1'): PickerSlot => ({
  id,
  startAt: localToUtcIso(date, start),
  endAt: localToUtcIso(date, end),
  location,
});

describe('SlotPickerComponent', () => {
  let fixture: ComponentFixture<SlotPickerComponent>;
  let component: SlotPickerComponent;

  function render(slots: PickerSlot[]) {
    fixture.componentRef.setInput('slots', slots);
    fixture.detectChanges();
  }

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [SlotPickerComponent, TranslateModule.forRoot()] });
    fixture = TestBed.createComponent(SlotPickerComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('current', {
      startAt: localToUtcIso('2026-10-05', '09:00'),
      endAt: localToUtcIso('2026-10-05', '09:30'),
      location: 'Room 1',
    });
  });

  it('groups slots by local day in time order and opens on the first day', () => {
    render([
      slot('c', '2026-10-07', '09:00', '09:30'),
      slot('b', '2026-10-06', '10:00', '10:30'),
      slot('a', '2026-10-06', '09:00', '09:30'),
    ]);

    expect(component.days().map((d) => [d.key, d.slots.map((s) => s.id)])).toEqual([
      ['2026-10-06', ['a', 'b']],
      ['2026-10-07', ['c']],
    ]);
    expect(component.selectedDayKey()).toBe('2026-10-06');
    expect(fixture.nativeElement.querySelectorAll('.day-chip').length).toBe(2);
    expect(fixture.nativeElement.querySelectorAll('.slot-chip').length).toBe(2);
  });

  it('selects a slot on click and shows it as the new appointment', () => {
    render([slot('a', '2026-10-06', '09:00', '09:30')]);

    (fixture.nativeElement.querySelector('.slot-chip') as HTMLButtonElement).click();
    fixture.detectChanges();

    expect(component.selectedId()).toBe('a');
    expect(component.selectedSummary()?.location).toBe('Room 1');
    expect(fixture.nativeElement.querySelector('.move-card.next.pending')).toBeNull();
  });

  it('names the room on each slot only when the slots span several locations', () => {
    render([slot('a', '2026-10-06', '09:00', '09:30'), slot('b', '2026-10-06', '10:00', '10:30')]);
    expect(component.days()[0].slots.every((s) => s.location === '')).toBeTrue();

    render([slot('a', '2026-10-06', '09:00', '09:30'), slot('b', '2026-10-06', '10:00', '10:30', 'Room 2')]);
    expect(component.days()[0].slots.map((s) => s.location)).toEqual(['Room 1', 'Room 2']);
  });

  it('shows the empty state when there are no open slots', () => {
    render([]);

    expect(component.days()).toEqual([]);
    expect(component.selectedDayKey()).toBeNull();
    expect(fixture.nativeElement.querySelector('.day-strip')).toBeNull();
    expect(fixture.nativeElement.querySelector('.state-box')).not.toBeNull();
  });
});
