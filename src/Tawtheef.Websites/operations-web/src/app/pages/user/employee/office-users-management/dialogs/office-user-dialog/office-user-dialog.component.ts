import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';
import {ButtonModule} from 'primeng/button';
import {InputTextModule} from 'primeng/inputtext';
import {OfficeUsersService} from '../../services/office-users.service';
import {OfficeUserDto} from '../../models/office-user.dto';
import {OfficeUserUpsertDto} from '../../models/office-user-upsert.dto';
import {NotificationService} from '../../../../../../core/services/notification.service';
import {I18nNamespaceDirective} from '../../../../../../shared/directives/i18n-namespace.directive';
import {Lang, LanguageService} from '../../../../../../core/services/language.service';
import {AuthService} from '../../../../../../core/auth/auth.service';

@Component({
  selector: 'app-office-user-dialog',
  standalone: true,
  templateUrl: './office-user-dialog.component.html',
  styleUrls: ['./office-user-dialog.component.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    ButtonModule,
    InputTextModule,
    I18nNamespaceDirective
  ]
})
export class OfficeUserDialogComponent implements OnInit {
  private officeUsersService = inject(OfficeUsersService);
  private dialogRef = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private auth = inject(AuthService);

  user = signal<OfficeUserDto | null>(null);
  currentUserId = signal<string | null>(this.auth.getCurrentUser()?.userId ?? null);
  nameAr = '';
  nameEn = '';
  email = '';
  isSaving = signal(false);
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  isEditingSelf = computed(() => {
    const currentUserId = this.currentUserId();
    const selectedUser = this.user();
    return !!currentUserId && !!selectedUser && selectedUser.id === currentUserId;
  });

  ngOnInit(): void {
    const user = this.config.data?.user as OfficeUserDto | undefined;
    if (user) {
      this.user.set(user);
      this.nameAr = user.fullNameAr;
      this.nameEn = user.fullNameEn;
      this.email = user.email;
    }

    this.language.current$.subscribe(lang => this.currentLang.set(lang));
    this.auth.currentUser$.subscribe(currentUser =>
      this.currentUserId.set(currentUser?.userId ?? null)
    );
  }

  save(): void {
    if (!this.isFormValid()) return;

    const payload: OfficeUserUpsertDto = {
      nameAr: this.nameAr.trim(),
      nameEn: this.nameEn.trim(),
      email: this.email.trim()
    };

    this.isSaving.set(true);

    const request$ = this.user()
      ? this.officeUsersService.updateOfficeUser(this.user()!.id, payload)
      : this.officeUsersService.createOfficeUser(payload);

    request$.subscribe({
      next: () => {
        const successKey = this.user()
          ? 'OFFICE_USERS.UPDATE_SUCCESS'
          : 'OFFICE_USERS.CREATE_SUCCESS';
        this.notification.success(this.translate.instant(successKey));
        this.dialogRef.close(true);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  isFormValid(): boolean {
    return !!this.nameAr.trim() && !!this.nameEn.trim() && !!this.email.trim();
  }
}
