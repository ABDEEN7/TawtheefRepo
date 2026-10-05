import { ChangeDetectionStrategy, Component, computed, effect, input, model, output } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';

import { utcIsoToLocalParts } from '../../models/schedule-time';

// One pickable slot. startAt/endAt are UTC ISO instants; location is already localized by the caller
// (room name, or "Online" for a remote slot). Every slot seats exactly one candidate.
export interface PickerSlot {
  id: string;
  startAt: string;
  endAt: string;
  location: string;
}

// The candidate's current slot, shown as the "before" card.
export type PickerCurrentSlot = Omit<PickerSlot, 'id'>;

export type SlotPickerStatus = 'loading' | 'ready' | 'error';

// A before/after card. Kept in parts (not one string) so the template can isolate the Latin time range
// with <bdi dir="ltr"> - joined into Arabic text, "4:20 PM - 4:50 PM" gets reordered by the bidi algorithm.
interface SlotSummary {
  day: string;
  time: string;
  location: string;
}

interface SlotView {
  id: string;
  timeRange: string;
  location: string;
}

// One calendar day (in the viewer's timezone). The day's remaining capacity is simply its slot count.
interface DayView {
  key: string;
  weekday: string;
  dayOfMonth: string;
  month: string;
  fullLabel: string;
  slots: SlotView[];
}

// Day strip + time grid + before/after cards, shared by the reschedule dialog (approved schedule) and the
// wizard's reassign dialog (draft distribution) so both pick a slot the same way. Purely presentational:
// the caller supplies the open slots and reads the chosen id back through [(selectedId)].
@Component({
  selector: 'app-slot-picker',
  templateUrl: './slot-picker.html',
  styleUrl: './slot-picker.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [NgTemplateOutlet, TranslatePipe],
})
export class SlotPickerComponent {
  slots = input.required<PickerSlot[]>();
  current = input.required<PickerCurrentSlot>();
  isRtl = input(false);
  // For callers that fetch their slots (the reschedule dialog); the wizard already has them, so 'ready'.
  status = input<SlotPickerStatus>('ready');
  retry = output<void>();
  selectedId = model<string | null>(null);

  selectedDayKey = model<string | null>(null);

  // Arabic month/day names, but the Latin digits the rest of the app uses.
  private dateLocale = computed(() => (this.isRtl() ? 'ar-u-nu-latn' : 'en-US'));

  days = computed<DayView[]>(() => {
    const slots = this.slots();
    // Only name the room when the open slots span more than one location - otherwise it's noise.
    const showLocation = new Set(slots.map((s) => s.location)).size > 1;

    const byDay = new Map<string, DayView>();
    for (const slot of [...slots].sort((a, b) => a.startAt.localeCompare(b.startAt))) {
      const key = utcIsoToLocalParts(slot.startAt).date;
      let day = byDay.get(key);
      if (!day) {
        const start = new Date(slot.startAt);
        day = {
          key,
          weekday: this.formatDate(start, { weekday: 'short' }),
          dayOfMonth: this.formatDate(start, { day: 'numeric' }),
          month: this.formatDate(start, { month: 'short' }),
          fullLabel: this.formatDate(start, { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' }),
          slots: [],
        };
        byDay.set(key, day);
      }
      day.slots.push({
        id: slot.id,
        timeRange: this.timeRange(slot.startAt, slot.endAt),
        location: showLocation ? slot.location : '',
      });
    }
    return [...byDay.values()];
  });

  selectedDay = computed(() => this.days().find((d) => d.key === this.selectedDayKey()) ?? null);

  currentSummary = computed<SlotSummary>(() => this.summary(this.current()));

  selectedSummary = computed<SlotSummary | null>(() => {
    const slot = this.slots().find((s) => s.id === this.selectedId());
    return slot ? this.summary(slot) : null;
  });

  constructor() {
    // Open on the first day whenever the slot list (re)loads and no valid day is selected.
    effect(() => {
      const days = this.days();
      const key = this.selectedDayKey();
      if (!days.some((d) => d.key === key)) this.selectedDayKey.set(days[0]?.key ?? null);
    });
  }

  selectDay(key: string) {
    this.selectedDayKey.set(key);
  }

  selectSlot(id: string) {
    this.selectedId.set(id);
  }

  private summary(slot: PickerCurrentSlot): SlotSummary {
    return {
      day: this.formatDate(new Date(slot.startAt), {
        weekday: 'long',
        day: 'numeric',
        month: 'long',
        year: 'numeric',
      }),
      time: this.timeRange(slot.startAt, slot.endAt),
      location: slot.location,
    };
  }

  // Same 12-hour "h:mm AM - h:mm AM" shape the schedule tables use (date pipe 'h:mm a').
  private timeRange(startAt: string, endAt: string): string {
    const time = (iso: string) =>
      new Date(iso).toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit' });
    return `${time(startAt)} - ${time(endAt)}`;
  }

  private formatDate(date: Date, options: Intl.DateTimeFormatOptions): string {
    return new Intl.DateTimeFormat(this.dateLocale(), options).format(date);
  }
}
