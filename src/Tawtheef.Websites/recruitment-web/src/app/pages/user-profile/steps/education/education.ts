import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Degree {
  degree: string;
  major: string;
  university: string;
  year: string;
}
@Component({
  selector: 'app-education',
  standalone: false,
  templateUrl: './education.html',
  styleUrl: './education.scss',
})
export class Education {
  degrees: Degree[] = [
    { degree: 'بكالوريوس', major: 'علوم', university: 'قطر', year: '2020' }
  ];

  addDegree(): void {
    this.degrees.push({ degree: '', major: '', university: '', year: '' });
  }

  removeDegree(index: number): void {
    this.degrees.splice(index, 1);
  }
}
