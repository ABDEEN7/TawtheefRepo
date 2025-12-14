import {Component, OnInit, inject, signal, computed} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { RolesService } from './services/roles.service';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import {RoleDto} from './models/permission.model';
import {PermissionDto} from './models/role.model';
import {PaginationComponent} from '../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';
import {Lang, LanguageService} from '../../../core/services/language.service';
import {NotificationService} from '../../../core/services/notification.service';
import {Tooltip} from 'primeng/tooltip';

@Component({
  selector: 'app-roles-management',
  standalone: true,
  templateUrl: './roles-management.html',
  styleUrls: ['./roles-management.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    PaginationComponent,
    ConfirmDialog,
    I18nNamespaceDirective,
    Tooltip
  ],
  providers: [ConfirmationService]
})
export class RolesManagement implements OnInit {
  private rolesService = inject(RolesService);
  private translate = inject(TranslateService);
  private confirmationService = inject(ConfirmationService);
  private language = inject(LanguageService);
  private notification = inject(NotificationService);

  paginationMetadata = this.rolesService.paginationMetadata;

  roles = this.rolesService.roles;
  permissions = signal<PermissionDto[]>([]);
  permissionSearch = signal('');
  filteredPermissions = computed(() => {
    const term = this.permissionSearch().trim().toLowerCase();

    if (!term) return this.permissions();

    return this.permissions().filter(perm => perm.name.toLowerCase().includes(term));
  });
  currentPage = signal(1);
  itemsPerPage = signal(3);
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  isModalOpen = signal(false);
  isEditing = signal(false);
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  formModel = signal<RoleDto>({
    id: '',
    nameAr: '',
    nameEn: '',
    descriptionAr: '',
    descriptionEn: '',
    permissions: []
  });

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
      next: () => {
        const metadata = this.paginationMetadata();
        if (metadata) {
          this.currentPage.set(metadata.currentPage);
          this.itemsPerPage.set(metadata.pageSize);
        }
      },
      error: () => this.notification.error(this.translate.instant('ROLES.LOAD_FAILED'))
    });
  }

  loadPermissions() {
    this.rolesService.getPermissions().subscribe({
      next: res => this.permissions.set(res),
      error: () => this.notification.error(this.translate.instant('ROLES.LOAD_FAILED'))
    });
  }

  openAdd() {
    this.isEditing.set(false);
    this.formModel.set({
      id: '',
      nameAr: '',
      nameEn: '',
      descriptionAr: '',
      descriptionEn: '',
      permissions: []
    });
    this.permissionSearch.set('');
    this.isModalOpen.set(true);
  }

  openEdit(role: RoleDto) {
    this.isEditing.set(true);
    this.formModel.set({ ...role });
    this.permissionSearch.set('');
    this.isModalOpen.set(true);
  }

  togglePermission(permId: string) {
    const model = this.formModel();
    const exists = model.permissions.includes(permId);

    model.permissions = exists
      ? model.permissions.filter(x => x !== permId)
      : [...model.permissions, permId];

    this.formModel.set({ ...model });
  }

  save() {
    const model = this.formModel();

    const request$ = this.isEditing()
      ? this.rolesService.updateRole(model)
      : this.rolesService.addRole(model);

    request$.subscribe({
      next: () => {
        this.notification.success(this.translate.instant('ROLES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadRoles();
      },
      error: () => {
        this.notification.error(this.translate.instant('ROLES.SAVE_FAILED'));
      }
    });
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
            this.loadRoles();
          },
          error: () => {
            this.notification.error(this.translate.instant('ROLES.DELETE_FAILED'));
          }
        });
      }
    });
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
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
}
