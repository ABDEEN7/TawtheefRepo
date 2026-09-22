import { Component, DestroyRef, Input, OnChanges, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslatePipe } from '@ngx-translate/core';
import { TableModule } from 'primeng/table';
import { LanguageService } from '../../../../../../core/services/language.service';
import { TestSlotSessionDto } from '../../../test-slots-management/models/test-slot-details.dto';
import { TestSlotsService } from '../../../test-slots-management/services/test-slots.service';
@Component({ selector: 'app-test-slot-sessions', standalone: true, templateUrl: './test-slot-sessions.component.html', imports: [TableModule, TranslatePipe] })
export class TestSlotSessionsComponent implements OnChanges {
  @Input({ required: true }) testSlotId!: string;
  private readonly service = inject(TestSlotsService); private readonly language = inject(LanguageService); private readonly destroyRef = inject(DestroyRef);
  readonly sessions = signal<TestSlotSessionDto[]>([]);
  ngOnChanges(): void { if (this.testSlotId) this.service.sessions(this.testSlotId, this.language.get()).pipe(takeUntilDestroyed(this.destroyRef)).subscribe(sessions => this.sessions.set(sessions)); }
}
