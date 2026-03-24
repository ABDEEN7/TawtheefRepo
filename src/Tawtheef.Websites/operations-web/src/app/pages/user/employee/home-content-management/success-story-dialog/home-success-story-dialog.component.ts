import {CommonModule} from '@angular/common';
import {Component, computed, ElementRef, inject, OnDestroy, OnInit, signal, ViewChild} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';
import {I18nNamespaceDirective} from '../../../../../shared/directives/i18n-namespace.directive';
import {Lang, LanguageService} from '../../../../../core/services/language.service';
import {HomeSuccessStory, HomeSuccessStoryPayload} from '../models/home-success-story.model';
import {FileUtilsService} from '../../../../../core/utils/file-utils';

interface SuccessStoryDialogData {
  mode: 'create' | 'edit';
  story?: HomeSuccessStory;
}

@Component({
  selector: 'app-home-success-story-dialog',
  standalone: true,
  templateUrl: './home-success-story-dialog.component.html',
  styleUrls: ['./home-success-story-dialog.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective]
})
export class HomeSuccessStoryDialogComponent implements OnInit, OnDestroy {
  private static readonly allowedImageMimeTypes = new Set([
    'image/jpeg',
    'image/png',
    'image/webp',
    'image/bmp'
  ]);

  private fb = inject(FormBuilder);
  private dialogRef = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig<SuccessStoryDialogData>);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private fileUtils = inject(FileUtilsService);

  story = this.config.data?.story;
  mode: 'create' | 'edit' = this.config.data?.mode ?? 'create';
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  submitted = false;
  imageFile: File | null = null;
  imagePreview = signal<string | null>(null);
  imageError = signal<string | null>(null);
  imageName = signal<string | null>(null);
  existingImageCleared = signal(false);
  imageDisplayName = computed(() => this.imageName() || this.getExistingImageName());
  private imageObjectUrl: string | null = null;

  @ViewChild('imageInput') imageInput?: ElementRef<HTMLInputElement>;

  form = this.fb.nonNullable.group({
    nameAr: ['', Validators.required],
    nameEn: ['', Validators.required],
    roleAr: ['', Validators.required],
    roleEn: ['', Validators.required],
    metricTitleAr: ['', Validators.required],
    metricTitleEn: ['', Validators.required],
    metricDescriptionAr: ['', Validators.required],
    metricDescriptionEn: ['', Validators.required],
    imageUrl: [''],
    displayOrder: [0, Validators.min(0)],
    isActive: [true]
  });

  ngOnInit(): void {
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
    if (this.story) {
      this.form.patchValue({
        nameAr: this.story.nameAr,
        nameEn: this.story.nameEn,
        roleAr: this.story.roleAr,
        roleEn: this.story.roleEn,
        metricTitleAr: this.story.metricTitleAr,
        metricTitleEn: this.story.metricTitleEn,
        metricDescriptionAr: this.story.metricDescriptionAr,
        metricDescriptionEn: this.story.metricDescriptionEn,
        imageUrl: this.story.imageUrl,
        displayOrder: this.story.displayOrder,
        isActive: this.story.isActive
      });
      this.imageName.set(this.getExistingImageName());
      this.setPreview(this.getExistingImagePreviewUrl());
    }
  }

  ngOnDestroy(): void {
    this.revokeObjectUrl();
  }

  submit() {
    this.submitted = true;
    const imageUrl = this.form.controls.imageUrl.value?.trim() ?? '';
    if (this.form.invalid || this.imageError()) {
      this.form.markAllAsTouched();
      return;
    }

    const payload: HomeSuccessStoryPayload = {
      ...this.form.getRawValue(),
      imageUrl,
      imageFile: this.imageFile
    };
    this.dialogRef.close({id: this.story?.id ?? null, payload});
  }

  cancel() {
    this.dialogRef.close(false);
  }

  async onImageSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    const existingImageUrl = this.getExistingImagePreviewUrl();

    if (file && !(await this.isAllowedStaticImage(file))) {
      this.imageError.set(this.translate.instant('HOME_CONTENT.INVALID_IMAGE_TYPE'));
      this.setImageFile(null);
      this.imageName.set(this.getExistingImageName());
      this.setPreview(existingImageUrl);
      input.value = '';
      return;
    }

    this.imageError.set(null);
    this.setImageFile(file);

    if (file) {
      this.existingImageCleared.set(false);
      this.imageName.set(file.name);
      this.setPreview(URL.createObjectURL(file), true);
    } else {
      this.imageName.set(this.getExistingImageName());
      this.setPreview(existingImageUrl);
    }
  }

  async previewImage(ev: MouseEvent) {
    ev.stopPropagation();
    ev.preventDefault();

    if (this.imageFile) {
      this.fileUtils.previewBlob(this.imageFile);
      return;
    }

    const preview = this.imagePreview();
    if (preview) {
      await this.fileUtils.previewUrl(preview, '', false);
    }
  }

  clearImage(ev: MouseEvent) {
    ev.stopPropagation();
    ev.preventDefault();

    this.setImageFile(null);
    this.imageError.set(null);
    this.imageName.set(null);
    this.existingImageCleared.set(true);
    this.setPreview(null);
    this.form.patchValue({imageUrl: ''});

    if (this.imageInput?.nativeElement) {
      this.imageInput.nativeElement.value = '';
    }
  }

  isEditMode() {
    return this.mode === 'edit';
  }

  requiredError() {
    return this.translate.instant('HOME_CONTENT.FIELD_REQUIRED');
  }

  private setImageFile(file: File | null) {
    this.imageFile = file;
  }

  private setPreview(url: string | null, isObjectUrl = false) {
    this.revokeObjectUrl();
    this.imagePreview.set(url);
    this.imageObjectUrl = isObjectUrl ? url : null;
  }

  private revokeObjectUrl() {
    if (this.imageObjectUrl) {
      URL.revokeObjectURL(this.imageObjectUrl);
      this.imageObjectUrl = null;
    }
  }

  private getExistingImagePreviewUrl(): string | null {
    if (!this.story || this.existingImageCleared()) {
      return null;
    }

    if (this.story.imagePreviewUrl) {
      return this.story.imagePreviewUrl;
    }

    if (this.story.imageUrl.startsWith('http')) {
      return this.story.imageUrl;
    }

    return null;
  }

  private getExistingImageName(): string | null {
    if (this.existingImageCleared()) {
      return null;
    }

    const raw = this.story?.imageUrl ?? null;
    if (!raw) return null;

    if (raw.startsWith('http')) {
      try {
        const u = new URL(raw);
        const last = u.pathname.split('/').filter(Boolean).pop();
        return last ?? null;
      } catch {
        return raw.split('/').pop() ?? null;
      }
    }

    return `${this.translate.instant('HOME_CONTENT.IMAGE')} (${raw.slice(0, 8)}...)`;
  }

  private async isAllowedStaticImage(file: File): Promise<boolean> {
    if (!HomeSuccessStoryDialogComponent.allowedImageMimeTypes.has(file.type)) {
      return false;
    }

    const bytes = new Uint8Array(await file.arrayBuffer());
    const isJpeg = this.hasSignature(bytes, [0xff, 0xd8, 0xff]);
    if (isJpeg) {
      return true;
    }

    if (this.isPng(bytes)) {
      return !this.isAnimatedPng(bytes);
    }

    if (this.isWebp(bytes)) {
      return !this.isAnimatedWebp(bytes);
    }

    return this.hasSignature(bytes, [0x42, 0x4d]);
  }

  private hasSignature(bytes: Uint8Array, signature: number[]): boolean {
    if (bytes.length < signature.length) {
      return false;
    }

    return signature.every((value, index) => bytes[index] === value);
  }

  private isPng(bytes: Uint8Array): boolean {
    return this.hasSignature(bytes, [0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a]);
  }

  private isAnimatedPng(bytes: Uint8Array): boolean {
    let offset = 8;
    while (offset + 8 <= bytes.length) {
      const chunkLength = this.readUInt32(bytes, offset);
      const chunkType = this.readAscii(bytes, offset + 4, 4);

      if (chunkType === 'acTL') {
        return true;
      }

      offset += chunkLength + 12;
    }

    return false;
  }

  private isWebp(bytes: Uint8Array): boolean {
    return this.readAscii(bytes, 0, 4) === 'RIFF' && this.readAscii(bytes, 8, 4) === 'WEBP';
  }

  private isAnimatedWebp(bytes: Uint8Array): boolean {
    let offset = 12;
    while (offset + 8 <= bytes.length) {
      const chunkType = this.readAscii(bytes, offset, 4);
      const chunkLength = this.readUInt32(bytes, offset + 4);

      if (chunkType === 'ANIM') {
        return true;
      }

      if (chunkType === 'VP8X' && offset + 16 <= bytes.length) {
        const flags = bytes[offset + 8];
        if ((flags & 0x02) !== 0) {
          return true;
        }
      }

      const paddedLength = chunkLength + (chunkLength % 2);
      offset += 8 + paddedLength;
    }

    return false;
  }

  private readUInt32(bytes: Uint8Array, offset: number): number {
    if (offset + 4 > bytes.length) {
      return 0;
    }

    return ((bytes[offset] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3]) >>> 0;
  }

  private readAscii(bytes: Uint8Array, offset: number, length: number): string {
    if (offset + length > bytes.length) {
      return '';
    }

    return String.fromCharCode(...bytes.slice(offset, offset + length));
  }
}
