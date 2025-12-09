import {Component, OnInit, inject, signal, computed} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { RolesService } from './services/roles.service';
import { Select } from 'primeng/select';
import {RoleDto} from './models/permission.model';
import {PermissionDto} from './models/role.model';
import {PaginationComponent} from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-roles-management',
  standalone: true,
  templateUrl: './roles-management.html',
  styleUrls: ['./roles-management.scss'],
  imports: [CommonModule, FormsModule, TranslatePipe, Select, PaginationComponent]
})
export class RolesManagementComponent implements OnInit {
  private rolesService = inject(RolesService);
  private translate = inject(TranslateService);

  paginationMetadata = this.rolesService.paginationMetadata;

  roles = this.rolesService.roles;
  permissions = signal<PermissionDto[]>([]);
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
    this.rolesService.getPermissions().subscribe (res => this.permissions.set(res));
  }

  openAdd() {
    this.isEditing.set(false);
    this.formModel.set({
      id: '',
      nameAr: '',
      nameEn: '',
      permissions: []
    });
    this.isModalOpen.set(true);
  }

  openEdit(role: RoleDto) {
    this.isEditing.set(true);
    this.formModel.set({ ...role });
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

  delete(role: RoleDto) {
    if (!confirm(this.translate.instant('ROLES.DELETE_CONFIRM'))) return;

    this.rolesService.deleteRole(role.id).subscribe(() => {
      this.loadRoles();
    });
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadRoles();
  }
}
