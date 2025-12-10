import {Component, OnInit, inject, signal, computed} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { RolesService } from './services/roles.service';
import { Select } from 'primeng/select';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import {RoleDto} from './models/permission.model';
import {PermissionDto} from './models/role.model';
import {PaginationComponent} from '../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';

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
    I18nNamespaceDirective
  ],
  providers: [ConfirmationService]
})
export class RolesManagement implements OnInit {
  private rolesService = inject(RolesService);
  private translate = inject(TranslateService);
  private confirmationService = inject(ConfirmationService);

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

  formModel = signal<RoleDto>({
    id: '',
    nameAr: '',
    nameEn: '',
    permissions: []
  });

  ngOnInit(): void {
    this.loadRoles();
    this.loadPermissions();
  }

  pagedRoles = computed(() => {
    return this.roles();
  });

  loadRoles() {
    this.rolesService.getRoles();
  }

  loadPermissions() {
    this.rolesService.getPermissions().subscribe(res => this.permissions.set(res));
  }

  openAdd() {
    this.isEditing.set(false);
    this.formModel.set({
      id: '',
      nameAr: '',
      nameEn: '',
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

    if (this.isEditing()) {
      this.rolesService.updateRole(model).subscribe(() => {
        this.isModalOpen.set(false);
        this.loadRoles();
      });
    } else {
      this.rolesService.addRole(model).subscribe(() => {
        this.isModalOpen.set(false);
        this.loadRoles();
      });
    }
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
        this.rolesService.deleteRole(role.id).subscribe(() => {
          this.loadRoles();
        });
      }
    });
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadRoles();
  }
}
