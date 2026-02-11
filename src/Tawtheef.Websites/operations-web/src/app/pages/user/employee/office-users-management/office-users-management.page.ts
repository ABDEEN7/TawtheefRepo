import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {TableModule} from 'primeng/table';
import {ButtonModule} from 'primeng/button';
import {DialogService} from 'primeng/dynamicdialog';
import {PaginationComponent} from '../../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {HasPermissionDirective} from '../../../../shared/directives/has-permission.directive';
import {OfficeUsersService} from './services/office-users.service';
import {OfficeUserDto} from './models/office-user.dto';
import {OfficeUserFilters} from './models/office-user-filters.dto';
import {PaginatedResult} from '../../../../core/models/paginated-result.model';
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';
import {NotificationService} from '../../../../core/services/notification.service';
import {Lang, LanguageService} from '../../../../core/services/language.service';
import {Permissions} from '../../../../core/constants/permissions';
import {OfficeSummaryDto} from './models/office-summary.dto';
import {OfficeUserDialogComponent} from './dialogs/office-user-dialog/office-user-dialog.component';
import {AuthService} from '../../../../core/auth/auth.service';

@Component({
  selector: 'app-office-users-management',
  standalone: true,
  templateUrl: './office-users-management.page.html',
  styleUrls: ['./office-users-management.page.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    PaginationComponent,
    I18nNamespaceDirective,
    HasPermissionDirective
  ],
  providers: [DialogService]
})
export class OfficeUsersManagementPage implements OnInit {
  private officeUsersService = inject(OfficeUsersService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private dialogService = inject(DialogService);
  private auth = inject(AuthService);

  private _users = signal<OfficeUserDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);
  private _officeSummary = signal<OfficeSummaryDto | null>(null);
  private _currentUserId = signal<string | null>(this.auth.getCurrentUser()?.userId ?? null);

  users = this._users.asReadonly();
  paginationMetadata = this._paginationMetadata.asReadonly();
  officeSummary = this._officeSummary.asReadonly();
  currentUserId = this._currentUserId.asReadonly();

  filters = signal<OfficeUserFilters>({
    pageNumber: 1,
    pageSize: 10,
    name: ''
  });

  nameFilter = '';

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);
  officeName = computed(() => {
    const office = this.officeSummary();
    if (!office) return '-';
    return this.isRtl() ? office.nameAr || office.nameEn : office.nameEn || office.nameAr;
  });
  officeCountry = computed(() => {
    const office = this.officeSummary();
    if (!office) return '-';
    return this.isRtl()
      ? office.countryNameAr || office.countryNameEn
      : office.countryNameEn || office.countryNameAr;
  });
  officePhone = computed(() => {
    const office = this.officeSummary();
    if (!office) return '-';
    if (!office.phoneNumber) return '-';
    const code = office.phoneCountryCode ? `${office.phoneCountryCode} ` : '';
    return `${code}${office.phoneNumber}`;
  });

  protected readonly Permissions = Permissions;

  ngOnInit(): void {
    this.loadUsers();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
    this.auth.currentUser$.subscribe(user => this._currentUserId.set(user?.userId ?? null));
  }

  loadUsers(): void {
    this.officeUsersService.getOfficeUsers(this.filters()).subscribe({
      next: (response: PaginatedResult<OfficeUserDto>) => {
        this._users.set(response.items);
        this._paginationMetadata.set(response.metadata);
        this._officeSummary.set(response.additionalData ?? null);

        if (response.metadata) {
          this.filters.update(f => ({
            ...f,
            pageNumber: response.metadata.currentPage,
            pageSize: response.metadata.pageSize
          }));
        }
      }
    });
  }

  onSearchChange(): void {
    this.filters.update(f => ({
      ...f,
      pageNumber: 1,
      name: this.nameFilter
    }));
    this.loadUsers();
  }

  onPageChange(page: number): void {
    this.filters.update(f => ({...f, pageNumber: page}));
    this.loadUsers();
  }

  openAddDialog(): void {
    const ref = this.dialogService.open(OfficeUserDialogComponent, {
      header: this.translate.instant('OFFICE_USERS.ADD_TITLE'),
      styleClass: 'office-user-dialog',
      width: '420px',
      draggable: false,   // ✅ disables dragging
    });

    ref?.onClose.subscribe(result => {
      if (result) {
        this.loadUsers();
      }
    });
  }

  openEditDialog(user: OfficeUserDto): void {
    const ref = this.dialogService.open(OfficeUserDialogComponent, {
      header: this.translate.instant('OFFICE_USERS.EDIT_TITLE'),
      styleClass: 'office-user-dialog',
      width: '420px',
      draggable: false,   // ✅ disables dragging
      data: { user }
    });

    ref?.onClose.subscribe(result => {
      if (result) {
        this.loadUsers();
      }
    });
  }

  toggleBlock(user: OfficeUserDto): void {
    if (this.isCurrentUser(user)) return;
    const desiredState = !user.isBlocked;
    this.officeUsersService.updateBlockStatus(user.id, desiredState).subscribe({
      next: () => {
        this._users.update(users =>
          users.map(item => (item.id === user.id ? {...item, isBlocked: desiredState} : item))
        );
        this.notification.success(
          this.translate.instant(
            desiredState ? 'OFFICE_USERS.BLOCK_SUCCESS' : 'OFFICE_USERS.UNBLOCK_SUCCESS'
          )
        );
      }
    });
  }

  isCurrentUser(user: OfficeUserDto): boolean {
    const currentUserId = this.currentUserId();
    return !!currentUserId && user.id === currentUserId;
  }
}
