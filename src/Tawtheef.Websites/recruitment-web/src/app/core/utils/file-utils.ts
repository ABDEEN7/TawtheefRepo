import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { DOCUMENT, isPlatformBrowser } from '@angular/common';
import { HttpResponse } from '@angular/common/http';
import { HttpService } from "../http/http.service";
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class FileUtilsService {
  private readonly isBrowser: boolean;

  constructor(
    private http: HttpService,
    @Inject(DOCUMENT) private doc: Document,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }
  private get isAzure(): boolean {
    return environment.storageProvider === 'Azure';
  }
  private isSasUrl(url: string): boolean {
    // Azure SAS غالبًا يحتوي sv + sig
    return /[?&]sv=/.test(url) && /[?&]sig=/.test(url);
  }

  private isSameOrigin(url: string): boolean {
    try {
      const u = new URL(url, window.location.origin);
      return u.origin === window.location.origin;
    } catch {
      return false;
    }
  }

  private isApiSignedDl(url: string): boolean {
    try {
      const u = new URL(url, window.location.origin);
      return u.pathname.toLowerCase().endsWith('/api/resources/dl');
    } catch {
      return url.toLowerCase().includes('/api/resources/dl');
    }
  }

  /**
   * Downloads a file from a Blob/File object.
   */
  async downloadBlob(file: File | Blob, fileName?: string): Promise<void> {
    if (!this.isBrowser) return;
    const name = file instanceof File ? file.name : (fileName ?? 'download');
    this.triggerDownload(file, name);
  }

  /**
   * Previews a File/Blob in a new tab (PDF or Image).
   */
  previewBlob(file: File | Blob): void {
    if (!this.isBrowser) return;

    const type = (file as File).type ?? '';
    if (type.includes('pdf') || type.includes('image')) {
      const url = URL.createObjectURL(file);
      window.open(url, '_blank');
      // Do NOT revoke immediately; some browsers need it alive after open.
      setTimeout(() => URL.revokeObjectURL(url), 30_000);
    }
  }

  /**
   * Previews by URL.
   * - If it’s a public PDF, opens directly.
   * - If it’s an image, opens directly.
   * - If it’s a protected URL (needs JWT), set `forceAuthFetch=true` to fetch as blob (interceptor applies) then open.
   */
  async previewUrl(fileUrl: string, fileName = '', forceAuthFetch = false): Promise<void> {
    if (!this.isBrowser) return;

    const isDl = this.isApiSignedDl(fileUrl);

    // لو Azure و الرابط هو /dl:
    // ❗ لا تفتحيه مباشرة لأنه يحتاج JWT (لن يُرسل في window.open)
    // الأفضل: fetch JSON url (إذا سويتي endpoint JSON)
    if (this.isAzure && isDl) {
      // إذا عندك endpoint JSON:
      const r = await this.http.get<{ url: string }>(fileUrl, undefined, {
        responseType: 'json',
        observe: 'body'
      }).toPromise();
      window.open(r!.url, '_blank');
      return;
    }

    // Local (أو روابط عامة): إذا بدك
    if (!forceAuthFetch && !isDl) {
      window.open(fileUrl, '_blank');
      return;
    }

    // fallback: auth fetch as blob
    const blob = await this.http.get<Blob>(fileUrl, undefined, { responseType: 'blob', observe: 'body' }).toPromise();
    const url = URL.createObjectURL(blob!);
    window.open(url, '_blank');
    setTimeout(() => URL.revokeObjectURL(url), 30_000);
  }

  /**
   * Converts a URL to a File object (uses HttpClient so interceptors/JWT apply).
   */
  async urlToFile(url: string, fileName: string, mimeType: string): Promise<File> {
    const blob = await this.http.get<Blob>(url, undefined, { responseType: 'blob' }).toPromise();
    return new File([blob!], fileName, { type: mimeType });
  }

  /**
   * Returns a FontAwesome icon class based on the file extension.
   */
  getFileIconClass(name: string): string {
    const ext = (name?.split('.').pop() || '').toLowerCase();
    switch (ext) {
      case 'pdf': return 'fa fa-file-pdf text-danger';
      case 'doc':
      case 'docx': return 'fa fa-file-word text-primary';
      case 'xls':
      case 'xlsx': return 'fa fa-file-excel text-success';
      case 'ppt':
      case 'pptx': return 'fa fa-file-powerpoint text-warning';
      case 'png':
      case 'jpg':
      case 'jpeg':
      case 'gif': return 'fa fa-file-image text-secondary';
      case 'zip':
      case 'rar': return 'fa fa-file-zipper';
      case 'mp4':
      case 'mov':
      case 'webm': return 'fa fa-file-video';
      case 'mp3':
      case 'wav': return 'fa fa-file-audio';
      case 'txt': return 'fa fa-file-lines';
      default: return 'fa fa-file';
    }
  }

  // ---- helpers ----
  private triggerDownload(blob: Blob, fileName: string) {
    const url = URL.createObjectURL(blob);
    const anchor = this.doc.createElement('a');
    anchor.href = url;
    if (fileName) anchor.download = fileName;

    this.doc.body.appendChild(anchor);
    anchor.click();
    this.doc.body.removeChild(anchor);

    // Delay revocation to avoid cutting the download early in some browsers
    setTimeout(() => URL.revokeObjectURL(url), 3_000);
  }
}
