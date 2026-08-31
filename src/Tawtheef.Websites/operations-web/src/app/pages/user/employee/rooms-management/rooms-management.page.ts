import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
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
import { debounceTime, distinctUntilChanged, finalize, forkJoin, Subject } from 'rxjs';
import { NotificationService } from '../../../../core/services/notification.service';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { dropdownOptionsModel } from '../../../../shared/models/dropdown-options.model';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../core/constants/permissions';
import { RoomFilters, RoomListItemDto, RoomLocationDto } from './models/room-list-item.dto';
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
  readonly roomTypeOptions = signal<dropdownOptionsModel[]>([]);
  readonly statusOptions = signal<dropdownOptionsModel[]>([]);
  readonly locationOptions = signal<RoomLocationDto[]>([]);
  protected readonly Permissions = Permissions;

  nameFilter = '';
  selectedLocationId: string | undefined;
  selectedRoomTypeId: string | undefined;
  selectedStatusId: string | undefined;

  ngOnInit(): void {
    this.filterChanges$
      .pipe(debounceTime(500), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.applyFilters());
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(lang => this.currentLang.set(lang));
    this.loadLookups();
  }

  openCreateRoom(): void {
    const ref = this.dialogService.open(RoomDialogComponent, {
      header: this.translate.instant('ROOMS.ADD_ROOM'),
      width: '720px',
      modal: true,
      closable: true,
      dismissableMask: false,
      breakpoints: { '768px': '95vw' },
      data: this.dialogLookupData()
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
      data: { room, ...this.dialogLookupData() }
    });

    ref?.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(id => {
      if (id) this.loadRooms();
    });
  }

  onTextFilterChange(): void {
    this.filterChanges$.next(this.nameFilter);
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

  locationName(location: RoomLocationDto): string {
    return this.currentLang() === 'ar' ? location.nameAr : (location.nameEn || location.nameAr);
  }

  private applyFilters(): void {
    this.filters.update(filters => ({
      ...filters,
      pageNumber: 1,
      search: this.nameFilter.trim() || undefined,
      locationId: this.selectedLocationId,
      roomTypeId: this.selectedRoomTypeId,
      statusId: this.selectedStatusId
    }));
    this.loadRooms();
  }

  private loadLookups(): void {
    forkJoin({
      roomTypes: this.roomsService.getRoomTypes(),
      statuses: this.roomsService.getRoomStatuses(),
      locations: this.roomsService.getLocations()
    }).subscribe({
      next: lookups => {
        this.roomTypeOptions.set(lookups.roomTypes);
        this.statusOptions.set(lookups.statuses);
        this.locationOptions.set(lookups.locations);
        this.loadRooms();
      },
      error: () => this.notification.error(this.translate.instant('ROOMS.LOAD_ERROR'))
    });
  }

  private dialogLookupData(): object {
    return {
      roomTypes: this.roomTypeOptions(),
      statuses: this.statusOptions(),
      locations: this.locationOptions()
    };
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
