import { CommonModule } from '@angular/common';
import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { ButtonModule } from 'primeng/button';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { Ripple } from 'primeng/ripple';
import { Select } from 'primeng/select';
import { TagModule } from 'primeng/tag';
import { TableModule } from 'primeng/table';
import { Tooltip } from 'primeng/tooltip';
import { debounceTime, distinctUntilChanged, finalize, Subject } from 'rxjs';
import { NotificationService } from '../../../../core/services/notification.service';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import {
  ROOM_STATUS_OPTIONS,
  ROOM_TYPE_OPTIONS,
  RoomStatus,
  RoomType
} from '../../../../core/enums/lookups.enum';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../core/constants/permissions';
import { RoomFilters, RoomListItemDto } from './models/room-list-item.dto';
import { RoomsService } from './services/rooms.service';
import { RoomDialogComponent } from './dialogs/room-dialog/room-dialog.component';

@Component({
  selector: 'app-rooms-management',
  standalone: true,
  templateUrl: './rooms-management.page.html',
  styleUrls: ['./rooms-management.page.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    I18nNamespaceDirective,
    HasPermissionDirective,
    PaginationComponent,
    Select,
    TableModule,
    ButtonModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    TagModule,
    Ripple,
    Tooltip
  ],
  providers: [DialogService]
})
export class RoomsManagementPage implements OnInit {
  private readonly dialogService = inject(DialogService);
  private readonly roomsService = inject(RoomsService);
  private readonly notification = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  private readonly language = inject(LanguageService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly filterChanges$ = new Subject<string>();

  readonly rooms = signal<RoomListItemDto[]>([]);
  readonly loading = signal(false);
  readonly currentLang = signal<Lang>(this.language.get());
  readonly totalItems = signal(0);
  readonly filters = signal<RoomFilters>({ pageNumber: 1, pageSize: 10 });
  readonly roomTypeOptions = ROOM_TYPE_OPTIONS;
  readonly statusOptions = ROOM_STATUS_OPTIONS;
  protected readonly Permissions = Permissions;
  readonly hasRooms = computed(() => this.rooms().length > 0);

  nameFilter = '';
  locationFilter = '';
  selectedRoomType: RoomType | undefined;
  selectedStatus: RoomStatus | undefined;

  ngOnInit(): void {
    this.filterChanges$
      .pipe(debounceTime(500), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.applyFilters());
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(lang => this.currentLang.set(lang));
    this.loadRooms();
  }

  openCreateRoom(): void {
    const ref = this.dialogService.open(RoomDialogComponent, {
      header: this.translate.instant('ROOMS.ADD_ROOM'),
      width: '720px',
      modal: true,
      closable: true,
      dismissableMask: false,
      breakpoints: { '768px': '95vw' }
    });

    ref?.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(id => {
      if (id) this.loadRooms();
    });
  }

  openEditRoom(room: RoomListItemDto): void {
    const ref = this.dialogService.open(RoomDialogComponent, {
      header: this.translate.instant('ROOMS.EDIT_ROOM'),
      width: '720px',
      modal: true,
      closable: true,
      dismissableMask: false,
      breakpoints: { '768px': '95vw' },
      data: { room }
    });

    ref?.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(id => {
      if (id) this.loadRooms();
    });
  }

  onTextFilterChange(): void {
    this.filterChanges$.next(`${this.nameFilter}|${this.locationFilter}`);
  }

  onSelectFilterChange(): void {
    this.applyFilters();
  }

  onPageChange(pageNumber: number): void {
    this.filters.update(filters => ({ ...filters, pageNumber }));
    this.loadRooms();
  }

  onPageSizeChange(pageSize: number): void {
    this.filters.update(filters => ({ ...filters, pageNumber: 1, pageSize }));
    this.loadRooms();
  }

  roomTypeLabel(value: RoomType): string {
    return this.roomTypeOptions.find(option => option.value === value)?.labelKey ?? '';
  }

  statusLabel(value: RoomStatus): string {
    return this.statusOptions.find(option => option.value === value)?.labelKey ?? '';
  }

  private applyFilters(): void {
    this.filters.update(filters => ({
      ...filters,
      pageNumber: 1,
      search: this.nameFilter.trim() || undefined,
      location: this.locationFilter.trim() || undefined,
      roomType: this.selectedRoomType,
      status: this.selectedStatus
    }));
    this.loadRooms();
  }

  private loadRooms(): void {
    this.loading.set(true);
    this.roomsService.list(this.filters())
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: response => {
          this.rooms.set(response.items ?? []);
          this.totalItems.set(response.metadata?.totalCount ?? 0);
        },
        error: () => this.notification.error(this.translate.instant('ROOMS.LOAD_ERROR'))
      });
  }
}
