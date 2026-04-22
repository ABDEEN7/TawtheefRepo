import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { RolesService } from '../../services/roles.service';
import { RoleDto } from '../../models/permission.model';
import { PermissionDto } from '../../models/role.model';
import { Lang, LanguageService } from '../../../../../../core/services/language.service';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { I18nNamespaceDirective } from '../../../../../../shared/directives/i18n-namespace.directive';

interface RoleDialogData {
  role?: RoleDto;
  permissions?: PermissionDto[];
}

@Component({
  selector: 'app-role-dialog',
  standalone: true,
  templateUrl: './role-dialog.component.html',
  styleUrls: ['./role-dialog.component.scss'],
  imports: [CommonModule, FormsModule, TranslatePipe, I18nNamespaceDirective]
})
export class RoleDialogComponent implements OnInit {
  private rolesService = inject(RolesService);
  private dialogRef = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig<RoleDialogData>);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  private defaultRole: RoleDto = {
    id: '',
    nameAr: '',
    nameEn: '',
    descriptionAr: '',
    descriptionEn: '',
    systemName: '',
    isSystemRole: false,
    permissions: [],
    permissionNames: []
  };

  private roleData = this.config.data?.role;

  formModel = signal<RoleDto>(this.roleData ? { ...this.roleData } : this.defaultRole);
  permissions = signal<PermissionDto[]>(this.config.data?.permissions ?? []);
  permissionSearch = signal('');
  selectedPermissions = signal<string[]>([...(this.roleData?.permissions ?? [])]);
  isSaving = signal(false);
  currentLang = signal<Lang>(this.language.get());
  isEditing = computed(() => !!this.roleData);
  isRtl = computed(() => this.currentLang() === 'ar');

  groupedPermissions = computed(() => {
    const search = this.permissionSearch().trim().toLowerCase();

    // 1. Sort globally first
    const sorted = [...this.permissions()]
      .filter(p => !search || p.name.toLowerCase().includes(search))
      .sort((a, b) => a.order - b.order);

    // 2. Group after sorting
    const groups = new Map<string, PermissionDto[]>();

    for (const perm of sorted) {
      const list = groups.get(perm.module) ?? [];
      list.push(perm);
      groups.set(perm.module, list);
    }

    // 3. Optional: sort modules as well
    return Array.from(groups.entries())
      .sort(([a], [b]) => a.localeCompare(b))
      .map(([module, permissions]) => ({ module, permissions }));
  });

  ngOnInit(): void {
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  updateField<K extends keyof RoleDto>(key: K, value: RoleDto[K]) {
    this.formModel.update(current => ({ ...current, [key]: value }));
  }

  togglePermission(permId: string) {
    const updated = this.selectedPermissions().includes(permId)
      ? this.selectedPermissions().filter(id => id !== permId)
      : [...this.selectedPermissions(), permId];

    this.selectedPermissions.set(updated);
  }

  save() {
    const model = {
      ...this.formModel(),
      permissions: this.selectedPermissions()
    };

    this.isSaving.set(true);

    const request$ = this.isEditing()
      ? this.rolesService.updateRole(model)
      : this.rolesService.addRole(model);

    request$.subscribe({
      next: savedRole => {
        this.isSaving.set(false);
        this.notification.success(this.translate.instant('ROLES.SAVE_SUCCESS'));
        const permissionNames = this.selectedPermissions().map(id => {
          const perm = this.permissions().find(p => p.id === id);
          return perm?.name ?? id;
        });
        this.dialogRef.close({ ...savedRole, permissionNames });
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }

  cancel() {
    this.dialogRef.close(false);
  }
}
