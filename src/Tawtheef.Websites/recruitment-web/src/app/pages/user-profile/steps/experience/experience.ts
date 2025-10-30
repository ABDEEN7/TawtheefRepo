import { Component } from '@angular/core';

interface ExperienceModel {
  org: string;
  title: string;
  from: string;
  to: string;
}
@Component({
  selector: 'app-experience',
  standalone: false,
  templateUrl: './experience.html',
  styleUrl: './experience.scss',
})
export class Experience {
  experiences: ExperienceModel[] = [
    { org: 'وزارة التعليم', title: 'معلم', from: '2018', to: '2023' }
  ];

  addExperience(): void {
    this.experiences.push({ org: '', title: '', from: '', to: '' });
  }

  removeExperience(index: number): void {
    this.experiences.splice(index, 1);
  }
}
