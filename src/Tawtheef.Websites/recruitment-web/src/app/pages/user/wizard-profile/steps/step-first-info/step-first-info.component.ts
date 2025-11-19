import {Component, EventEmitter, inject, Output} from '@angular/core';
import { TranslateService } from "@ngx-translate/core";
import { ProfileLookupsService } from "../../services/profile-lookups.service";
import {DataService} from '../../services/data.service';

@Component({
  selector: 'app-step-first-info',
  templateUrl: './step-first-info.component.html',
  styleUrl: './step-first-info.component.scss',
  standalone: false
})
export class StepFirstInfoComponent {
  @Output() next = new EventEmitter<void>();
  ds = inject(DataService);

  translate = inject(TranslateService);
  lookups = inject(ProfileLookupsService);

  candidateType = '';
  targetEntity  = '';

  cvFile: File | null = null;
  idFile: File | null = null;

  cvError: string | null = null;
  idError: string | null = null;

  constructor() {

  }
  /** Drag & Drop **/
  onDragOver(e: DragEvent) {
    e.preventDefault();
    (e.currentTarget as HTMLElement).classList.add('dragging');
  }
  onDragLeave(e: DragEvent) {
    (e.currentTarget as HTMLElement).classList.remove('dragging');
  }
  onDrop(e: DragEvent, kind: 'cv' | 'id') {
    e.preventDefault();
    (e.currentTarget as HTMLElement).classList.remove('dragging');
    const files = e.dataTransfer?.files;
    if (!files || !files.length) return;
    this.handleFile(files[0], kind);
  }
  onFilePicked(e: Event, kind: 'cv' | 'id') {
    const input = e.target as HTMLInputElement;
    const file  = input.files?.[0];
    if (!file) return;
    this.handleFile(file, kind);
    input.value = ''; // allow picking the same file again if needed
  }
  clearFile(kind: 'cv' | 'id') {
    if (kind === 'cv') { this.cvFile = null; this.cvError = null; }
    else { this.idFile = null; this.idError = null; }
  }

  /** Validation **/
  private handleFile(file: File, kind: 'cv' | 'id') {
    if (kind === 'cv') {
      // CV must be PDF
      if (!/pdf$/i.test(file.type) && !/\.pdf$/i.test(file.name)) {
        this.cvError = this.translate.instant('wizard.prereq.errors.cvPdfOnly');
        this.cvFile = null;
        return;
      }
      if (file.size > 10 * 1024 * 1024) { // 10MB
        this.cvError = this.translate.instant('wizard.prereq.errors.maxSize', { size: 10 });
        this.cvFile = null;
        return;
      }
      this.cvError = null;
      this.cvFile = file;
    } else {
      // ID: image or pdf
      const ok = /pdf$/i.test(file.type) || /^image\//i.test(file.type) || /\.pdf$/i.test(file.name);
      if (!ok) {
        this.idError = this.translate.instant('wizard.prereq.errors.idImageOrPdf');
        this.idFile = null;
        return;
      }
      if (file.size > 10 * 1024 * 1024) {
        this.idError = this.translate.instant('wizard.prereq.errors.maxSize', { size: 10 });
        this.idFile = null;
        return;
      }
      this.idError = null;
      this.idFile = file;
    }
  }
}
