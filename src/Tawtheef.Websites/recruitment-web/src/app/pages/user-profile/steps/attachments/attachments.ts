import { Component } from '@angular/core';

@Component({
  selector: 'app-attachments',
  standalone: false,
  templateUrl: './attachments.html',
  styleUrl: './attachments.scss',
})
export class Attachments {
  files: File[] = [];

  onFileSelect(event: Event): void {
    const target = event.target as HTMLInputElement;
    if (target.files) {
      this.files.push(...Array.from(target.files));
    }
  }
}
