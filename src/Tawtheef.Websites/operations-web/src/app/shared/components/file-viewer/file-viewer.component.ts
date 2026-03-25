import { Component, EventEmitter, Input, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Pipe({
  name: 'safeUrl',
  standalone: true
})
export class SafeUrlPipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) {}
  transform(url: string | null): SafeResourceUrl {
    return url ? this.sanitizer.bypassSecurityTrustResourceUrl(url) : '';
  }
}

@Component({
  selector: 'app-file-viewer',
  standalone: true,
  imports: [CommonModule, TranslateModule, ButtonModule, TooltipModule, SafeUrlPipe, ProgressSpinnerModule],
  templateUrl: './file-viewer.component.html',
  styleUrl: './file-viewer.component.scss'
})
export class FileViewerComponent {
  @Input() url: string | null = null;
  @Input() fileName: string = '';
  @Input() loading: boolean = false;
  @Input() mimeType: string = '';
  @Output() close = new EventEmitter<void>();

  zoomLevel = signal(100);

  get isImage(): boolean {
    if (this.mimeType?.startsWith('image/')) return true;
    if (!this.fileName) return false;
    const ext = this.fileName.split('.').pop()?.toLowerCase();
    return ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'].includes(ext || '');
  }

  get isPdf(): boolean {
    if (this.mimeType === 'application/pdf') return true;
    if (!this.fileName) return false;
    const ext = this.fileName.split('.').pop()?.toLowerCase();
    return ext === 'pdf';
  }

  get isText(): boolean {
    if (this.mimeType?.startsWith('text/')) return true;
    if (!this.fileName) return false;
    const ext = this.fileName.split('.').pop()?.toLowerCase();
    return ext === 'txt';
  }

  zoomIn() {
    this.zoomLevel.update(v => Math.min(v + 20, 300));
  }

  zoomOut() {
    this.zoomLevel.update(v => Math.max(v - 20, 20));
  }

  resetZoom() {
    this.zoomLevel.set(100);
  }

  onClose() {
    this.close.emit();
  }
  
  download() {
    if (!this.url) return;
    const link = document.createElement('a');
    link.href = this.url;
    link.download = this.fileName || 'file';
    link.click();
  }
}
