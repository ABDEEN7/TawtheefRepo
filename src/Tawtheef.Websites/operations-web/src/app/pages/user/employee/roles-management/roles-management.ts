import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {CommonModule} from '@angular/common';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {RolesService} from './services/roles.service';
import {ConfirmDialog} from 'primeng/confirmdialog';
import {ConfirmationService} from 'primeng/api';
import {RoleDto} from './models/permission.model';
import {PermissionDto} from './models/role.model';
import {Tooltip} from 'primeng/tooltip';
import {PaginationComponent} from '../../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {Lang, LanguageService} from '../../../../core/services/language.service';
import {NotificationService} from '../../../../core/services/notification.service';
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';
import {PaginatedResult} from '../../../../core/models/paginated-result.model';
import {DialogService} from 'primeng/dynamicdialog';
import {RoleDialogComponent} from './dialogs/role-dialog/role-dialog.component';
import {TableModule} from 'primeng/table';

@Component({
  selector: 'app-roles-management',
  standalone: true,
  templateUrl: './roles-management.html',
  styleUrls: ['./roles-management.scss'],
  imports: [
    CommonModule,
    TranslatePipe,
    PaginationComponent,
    ConfirmDialog,
    I18nNamespaceDirective,
    Tooltip,
    TableModule
  ],
  providers: [ConfirmationService, DialogService]
})
export class RolesManagement implements OnInit {
  private rolesService = inject(RolesService);
  private translate = inject(TranslateService);
  private confirmationService = inject(ConfirmationService);
  private language = inject(LanguageService);
  private notification = inject(NotificationService);
  private dialogService = inject(DialogService);

  // Component now manages its own state
  private _roles = signal<RoleDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  public roles = this._roles.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  permissions = signal<PermissionDto[]>([]);
  currentPage = signal(1);
  itemsPerPage = signal(10);
  itemsPerPageOptions = [10, 20, 50];
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  ngOnInit(): void {
    this.loadRoles();
    this.loadPermissions();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  pagedRoles = computed(() => {
    return this.roles();
  });

  loadRoles() {
    this.rolesService.getRoles({
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage()
    }).subscribe({
      next: (response: PaginatedResult<RoleDto>) => {
        this._roles.set(response.items || []);
        this._paginationMetadata.set(response.metadata);

        if (response.metadata) {
          this.currentPage.set(response.metadata.currentPage);
          this.itemsPerPage.set(response.metadata.pageSize);
        }
      }
    });
  }

  loadPermissions() {
    this.rolesService.getPermissions().subscribe({
      next: res => this.permissions.set(res)
    });
  }

  openAdd() {
    this.openRoleDialog();
  }

  openEdit(role: RoleDto) {
    this.openRoleDialog(role);
  }

  confirmDelete(role: RoleDto) {
    this.confirmationService.confirm({
      message: this.translate.instant('ROLES.DELETE_CONFIRM'),
      header: this.translate.instant('ROLES_DELETE'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('ROLES_DELETE'),
      rejectLabel: this.translate.instant('ROLES_CANCEL'),
      acceptButtonStyleClass: 'btn btn-danger',
      rejectButtonStyleClass: 'btn btn-outline-secondary',
      defaultFocus: 'reject',
      accept: () => {
        this.rolesService.deleteRole(role.id).subscribe({
          next: () => {
            this.notification.success(this.translate.instant('ROLES.DELETE_SUCCESS'));

            // Remove role from local state
            this._roles.update(roles => roles.filter(r => r.id !== role.id));

            // Update pagination metadata
            this._paginationMetadata.update(metadata => {
              if (!metadata) return metadata;
              return {
                ...metadata,
                totalCount: Math.max(0, metadata.totalCount - 1),
                totalPages: Math.ceil(Math.max(0, metadata.totalCount - 1) / metadata.pageSize)
              };
            });
          }
        });
      }
    });
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadRoles();
  }

  onPageSizeChange(size: number) {
    this.itemsPerPage.set(size);
    this.currentPage.set(1);
    this.loadRoles();
  }

  localizedName(role: RoleDto) {
    return this.currentLang() === 'ar'
      ? role.nameAr || role.nameEn
      : role.nameEn || role.nameAr;
  }

  localizedDescription(role: RoleDto) {
    return this.currentLang() === 'ar'
      ? role.descriptionAr || role.descriptionEn
      : role.descriptionEn || role.descriptionAr;
  }

  private openRoleDialog(role?: RoleDto) {
    const ref = this.dialogService.open(RoleDialogComponent, {
      header: this.translate.instant(role ? 'ROLES.EDIT_ROLE' : 'ROLES.ADD_ROLE'),
      width: '820px',
      draggable: false,   // ✅ disables dragging
      data: {
        role,
        permissions: this.permissions()
      },
      styleClass: 'role-dialog'
    });

    ref?.onClose.subscribe((result: RoleDto | boolean) => {
      if (!result || typeof result === 'boolean') {
        return;
      }

      if (role) {
        this._roles.update(roles => roles.map(r => r.id === result.id ? result : r));
      } else {
        this.loadRoles();
      }
    });
  }
}
