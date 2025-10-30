import { Component } from '@angular/core';

interface Language {
  name: string;
  level: string;
}

@Component({
  selector: 'app-skills',
  standalone: false,
  templateUrl: './skills.html',
  styleUrl: './skills.scss',
})
export class Skills {
  skills: string[] = [];
  newSkill = '';
  languages: Language[] = [];
  newLang: Language = { name: '', level: '' };

  addSkill(): void {
    if (this.newSkill.trim()) {
      this.skills.push(this.newSkill.trim());
      this.newSkill = '';
    }
  }

  addLanguage(): void {
    if (this.newLang.name.trim() && this.newLang.level) {
      this.languages.push({ ...this.newLang });
      this.newLang = { name: '', level: '' };
    }
  }
}
