import { Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { JobTabReviewNoteResponse } from '../../models/job-tab-review-note-response';
import { FileUtilsService } from '../../../../core/utils/file-utils';

@Component({
  selector: 'app-review-note',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  providers: [FileUtilsService],
  templateUrl: './review-note.component.html',
  styleUrls: ['./review-note.component.scss']
})
export class ReviewNoteComponent {
  @Input({ required: true }) note: JobTabReviewNoteResponse | null = null;

  protected fileUtils = inject(FileUtilsService);

  preview(file: any): void {
    this.fileUtils.previewUrl(file.url).then(() => {});
  }

  download(file: any): void {
    this.fileUtils.downloadUrl(file.url, file.fileName).then(() => {});
  }

  fileIcon(name: string): string {
    return this.fileUtils.getFileIconClass(name);
  }
}
