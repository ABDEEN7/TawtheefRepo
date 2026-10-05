import {
  Component,
  DestroyRef,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  inject,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslatePipe } from '@ngx-translate/core';
import { TableModule } from 'primeng/table';
import { Tooltip } from 'primeng/tooltip';
import { Permissions } from '../../../../../../core/constants/permissions';
import { HasPermissionDirective } from '../../../../../../shared/directives/has-permission.directive';
import { LanguageService } from '../../../../../../core/services/language.service';
import { TestSlotSessionDto } from '../../../test-slots-management/models/test-slot-details.dto';
import { TestSlotsService } from '../../../test-slots-management/services/test-slots.service';

@Component({
  selector: 'app-test-slot-sessions',
  standalone: true,
  templateUrl: './test-slot-sessions.component.html',
  imports: [TableModule, Tooltip, TranslatePipe, HasPermissionDirective],
})
export class TestSlotSessionsComponent implements OnChanges {
  readonly permissions = Permissions;
  @Input({ required: true }) testSlotId!: string;
  @Input() sendingNotifications = false;
  @Output() viewSession = new EventEmitter<string>();
@Output() sendSessionNotifications =
  new EventEmitter<Pick<TestSlotSessionDto, 'id' | 'sessionNo'>>();

  private readonly service = inject(TestSlotsService);
  private readonly language = inject(LanguageService);
  private readonly destroyRef = inject(DestroyRef);

  readonly sessions = signal<TestSlotSessionDto[]>([]);

  ngOnChanges(): void {
    if (!this.testSlotId) return;

    this.service
      .sessions(this.testSlotId, this.language.get())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((sessions) => this.sessions.set(sessions));
      console.log(this.permissions.TestSlots.SendCandidateNotification);
      
  }

  genderLabel(filter: TestSlotSessionDto['genderFilter']): string {
    return filter === 'Male'
      ? 'TEST_SLOT_DETAILS.MALE'
      : filter === 'Female'
        ? 'TEST_SLOT_DETAILS.FEMALE'
        : 'TEST_SLOT_DETAILS.ALL';
  }

  nationalityLabel(filter: TestSlotSessionDto['nationalityFilter']): string {
    return filter === 'Qatari'
      ? 'TEST_SLOT_DETAILS.QATARI'
      : filter === 'NonQatari'
        ? 'TEST_SLOT_DETAILS.NON_QATARI'
        : 'TEST_SLOT_DETAILS.ALL';
  }
}
