import {Component, inject} from '@angular/core';
import {ImageCroppedEvent, ImageCropperComponent, ImageTransform} from "ngx-image-cropper";
import {NgIf} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {TranslatePipe, TranslateService} from "@ngx-translate/core";
import {DynamicDialogRef} from 'primeng/dynamicdialog';

@Component({
  selector: 'app-avatar',
  imports: [
    ImageCropperComponent,
    NgIf,
    ReactiveFormsModule,
    TranslatePipe,
    FormsModule
  ],
  templateUrl: './avatar.modal.html',
  styleUrl: './avatar.modal.scss',
})
export class AvatarModal {
  translate = inject(TranslateService);
  ref = inject(DynamicDialogRef);
  // القصّ
  imageFile: File | null = null;
  private pendingInput?: HTMLInputElement;
  croppedImage: string | null = null;
  avatarError: string | null = null;
  private readonly ALLOWED_MIME = new Set(['image/png', 'image/jpeg']);
  private readonly ALLOWED_EXT  = new Set(['png', 'jpg', 'jpeg']);
  // تحكّم بصري
  zoom = 1;
  rotation = 0;
  transform: ImageTransform = {};

  // قيود
  private readonly MAX_AVATAR_BYTES = 3 * 1024 * 1024; // 3MB
  private readonly TARGET_RATIO = 2 / 3;
  private readonly RATIO_TOLERANCE = 0.12; // ±12%


  onAvatarPicked(e: Event) {
    this.avatarError = null;
    this.croppedImage = null;
    this.imageFile = null;

    const input = e.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    if (!/^image\//i.test(file.type)) {
      this.avatarError = this.translate.instant('wizard.personal.avatar.errors.onlyImage');
      input.value = '';
      return;
    }

    const ext = (file.name.split('.').pop() || '').toLowerCase();
    if (!this.ALLOWED_MIME.has(file.type) || !this.ALLOWED_EXT.has(ext)) {
      this.avatarError = this.translate.instant('wizard.personal.avatar.errors.onlyPngJpg');
      input.value = '';
      return;
    }

    if (file.size > this.MAX_AVATAR_BYTES) {
      this.avatarError = this.translate.instant('wizard.personal.avatar.errors.maxSize', { size: 3 });
      input.value = '';
      return;
    }

    this.imageFile = file;
    this.pendingInput = input;
  }

  onCropImageLoaded() {
    // الآن صار آمن نفضي قيمة الـ input لتسمح باختيار نفس الملف مرة ثانية
    if (this.pendingInput) {
      this.pendingInput.value = '';
      this.pendingInput = undefined;
    }
  }

  onImageCropped(ev: ImageCroppedEvent) {
    const ratio = (ev.width || 0) / (ev.height || 1);
    const diff = Math.abs(ratio - this.TARGET_RATIO);
    if (diff > this.RATIO_TOLERANCE) {
      this.avatarError = this.translate.instant('wizard.personal.avatar.errors.ratio');
      this.croppedImage = null;
      return;
    }
    this.avatarError = null;
    this.croppedImage = ev.objectUrl || null;
  }

  onCropLoadFailed() {
    this.avatarError = this.translate.instant('wizard.personal.avatar.errors.loadFailed');
  }

  applyZoom() {
    this.transform = { ...this.transform, scale: this.zoom };
  }

  applyRotation() {
    this.transform = { ...this.transform, rotate: this.rotation };
  }

  resetCrop() {
    this.zoom = 1;
    this.rotation = 0;
    this.transform = {};
  }

  saveAvatar() {
    if (!this.croppedImage || this.avatarError) return;
    this.ref.close(this.croppedImage);
  }
  close(){
    this.ref.close();
  }
}
