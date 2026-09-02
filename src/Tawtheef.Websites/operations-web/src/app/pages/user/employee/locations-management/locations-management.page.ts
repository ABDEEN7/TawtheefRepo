import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DialogService } from 'primeng/dynamicdialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { Ripple } from 'primeng/ripple';
import { Tooltip } from 'primeng/tooltip';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { debounceTime, distinctUntilChanged, finalize, Subject } from 'rxjs';
import { Permissions } from '../../../../core/constants/permissions';
import { NotificationService } from '../../../../core/services/notification.service';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { PageFiltersComponent } from '../../../../shared/components/page-filters/page-filters.component';
import { LocationDto, LocationFilters } from './models/location.dto';
import { LocationsService } from './services/locations.service';
import { LocationDialogComponent } from './dialogs/location-dialog/location-dialog.component';

@Component({
  selector: 'app-locations-management',
  standalone: true,
  templateUrl: './locations-management.page.html',
  styleUrls: ['./locations-management.page.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    I18nNamespaceDirective,
    PaginationComponent,
    PageFiltersComponent,
    HasPermissionDirective,
    TableModule,
    ButtonModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    Ripple,
    Tooltip,
    ConfirmDialog
  ],
  providers: [DialogService, ConfirmationService]
})
export class LocationsManagementPage implements OnInit {
  private readonly dialogService = inject(DialogService);
  private readonly locationsService = inject(LocationsService);
  private readonly notification = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly searchChanges$ = new Subject<string>();

  readonly locations = signal<LocationDto[]>([]);
  readonly loading = signal(false);
  readonly deletingLocationId = signal<string | null>(null);
  readonly totalItems = signal(0);
  readonly filters = signal<LocationFilters>({ pageNumber: 1, pageSize: 10 });
  protected readonly Permissions = Permissions;
  search = '';

  ngOnInit(): void {
    this.searchChanges$
      .pipe(debounceTime(500), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.applySearch());
    this.loadLocations();
  }

  openCreateLocation(): void {
    const ref = this.dialogService.open(LocationDialogComponent, {
      header: this.translate.instant('LOCATIONS.ADD_LOCATION'),
      width: '720px',
      modal: true,
      closable: true,
      dismissableMask: false,
      breakpoints: { '768px': '95vw' }
    });

    ref?.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(result => {
      if (result) this.loadLocations();
    });
  }

  openEditLocation(location: LocationDto): void {
    const ref = this.dialogService.open(LocationDialogComponent, {
      header: this.translate.instant('LOCATIONS.EDIT_LOCATION'),
      width: '720px',
      modal: true,
      closable: true,
      dismissableMask: false,
      breakpoints: { '768px': '95vw' },
      data: { location }
    });

    ref?.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(result => {
      if (result) this.loadLocations();
    });
  }

  openLocationLink(locationLink: string, event: MouseEvent): void {
    event.preventDefault();

    const externalWindow = window.open(locationLink, '_blank', 'noopener,noreferrer');
    if (externalWindow) externalWindow.opener = null;
  }

  confirmDeleteLocation(location: LocationDto): void {
    this.confirmationService.confirm({
      message: this.translate.instant('LOCATIONS.DELETE_CONFIRM'),
      header: this.translate.instant('LOCATIONS.DELETE_HEADER'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('common.delete'),
      rejectLabel: this.translate.instant('common.cancel'),
      acceptButtonStyleClass: 'btn btn-danger',
      rejectButtonStyleClass: 'btn btn-outline-secondary',
      defaultFocus: 'reject',
      accept: () => this.deleteLocation(location.id)
    });
  }

  onSearchChange(): void {
    this.searchChanges$.next(this.search);
  }

  onPageChange(pageNumber: number): void {
    this.filters.update(filters => ({ ...filters, pageNumber }));
    this.loadLocations();
  }

  onPageSizeChange(pageSize: number): void {
    this.filters.update(filters => ({ ...filters, pageNumber: 1, pageSize }));
    this.loadLocations();
  }

  private applySearch(): void {
    this.filters.update(filters => ({
      ...filters,
      pageNumber: 1,
      search: this.search.trim() || undefined
    }));
    this.loadLocations();
  }

  private deleteLocation(id: string): void {
    this.deletingLocationId.set(id);
    this.locationsService
      .delete(id)
      .pipe(finalize(() => this.deletingLocationId.set(null)))
      .subscribe({
        next: () => {
          this.notification.success(this.translate.instant('LOCATIONS.DELETE_SUCCESS'));
          this.loadLocations();
        }
      });
  }

  private loadLocations(): void {
    this.loading.set(true);
    this.locationsService
      .list(this.filters())
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: response => {
          this.locations.set(response.items ?? []);
          this.totalItems.set(response.metadata?.totalCount ?? 0);
        },
        error: () => this.notification.error(this.translate.instant('LOCATIONS.LOAD_ERROR'))
      });
  }
}
