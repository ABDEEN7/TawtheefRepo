import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { AvatarModule } from 'primeng/avatar';
import { ProgressBarModule } from 'primeng/progressbar';
import { JobsModule } from "../../../../job-management/jobs.module";

@Component({
  selector: 'app-assign-files',
  imports: [CommonModule,
    FormsModule,
    TranslateModule,
    ButtonModule,
    InputNumberModule,
    AvatarModule, ProgressBarModule, JobsModule],
  templateUrl: './assign-files.html',
  styleUrl: './assign-files.scss',
})
export class AssignFiles {
  private ref = inject(DynamicDialogRef);
  private cfg = inject(DynamicDialogConfig);
  
 cancel(): void {
    this.ref.close(null);
  }
}
