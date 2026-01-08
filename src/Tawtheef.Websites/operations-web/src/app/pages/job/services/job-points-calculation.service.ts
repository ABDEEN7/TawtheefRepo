import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Injectable({ providedIn: 'root' })
export class JobPointsCalculationService {
  public readonly sections = [
    'applicantCategory',
    'education',
    'experience',
    'training',
    'skills',
    'languages',
    'certificates',
  ];

  sumCategory(group: FormGroup | null): number {
    if (!group) return 0;

    const value = group.getRawValue();
    return Object.keys(value)
      .filter((k) => !['pointsPerYear', 'maxYears', 'total', 'max'].includes(k))
      .reduce((sum, k) => sum + (value[k] || 0), 0);
  }

  sumLanguagesCategory(languagesGroup: FormGroup | null): number {
    if (!languagesGroup) return 0;

    let total = 0;

    ['speaking', 'reading', 'conversation'].forEach((ability) => {
      const abilityGroup = languagesGroup.get(ability) as FormGroup;
      if (abilityGroup) {
        const abilityMax = abilityGroup.get('max')?.value || 0;
        const levelsSum = this.sumCategory(abilityGroup);

        total += Math.min(levelsSum, abilityMax);
      }
    });

    const nativeValue = languagesGroup.get('native')?.value || 0;
    total += nativeValue;

    return total;
  }

  sumAbilityLevels(abilityGroup: FormGroup | null): number {
    if (!abilityGroup) return 0;
    return this.sumCategory(abilityGroup);
  }

  isAbilityValid(abilityGroup: FormGroup | null): boolean {
    if (!abilityGroup) return true;

    const max = abilityGroup.get('max')?.value || 0;
    const levelsSum = this.sumAbilityLevels(abilityGroup);

    return levelsSum <= max;
  }

  experienceTotal(expGroup: FormGroup | null): number {
    if (!expGroup) return 0;
    const v = expGroup.getRawValue();
    return (v.pointsPerYear || 0) * (v.maxYears || 0);
  }

  isSectionValid(sectionKey: string, mainForm: FormGroup, detailsForm: FormGroup): boolean {
    const mainValue = mainForm.get(sectionKey)?.value || 0;

    if (sectionKey === 'experience') {
      return this.experienceTotal(detailsForm.get('experience') as FormGroup) <= mainValue;    }

    if (sectionKey === 'languages') {
      return this.sumLanguagesCategory(detailsForm.get('languages') as FormGroup) <= mainValue;
    }

    return this.sumCategory(detailsForm.get(sectionKey) as FormGroup) <= mainValue;
    }

  areAllCategoriesValid(mainForm: FormGroup, detailsForm: FormGroup, sections: string[]): boolean {
    return sections.every((section) => {
      if (section === 'experience') {
        const expTotal = this.experienceTotal(detailsForm.get('experience') as FormGroup);
        const mainValue = mainForm.get(section)?.value || 0;
        return expTotal <= mainValue;
      }

      if (section === 'languages') {
        return this.validateLanguagesSection(mainForm, detailsForm);
      }

      return this.validateDetailSection(section, mainForm, detailsForm);
    });
  }

  validateDetailSection(sectionKey: string, mainForm: FormGroup, detailsForm: FormGroup): boolean {
    const mainValue = mainForm.get(sectionKey)?.value || 0;
    const detailSum = this.sumCategory(detailsForm.get(sectionKey) as FormGroup);
    return detailSum <= mainValue;
  }

  validateLanguagesSection(mainForm: FormGroup, detailsForm: FormGroup): boolean {
    const mainLanguagesValue = mainForm.get('languages')?.value || 0;
    const languagesGroup = detailsForm.get('languages') as FormGroup;

    if (!languagesGroup) return true;

    const totalSum = this.sumLanguagesCategory(languagesGroup);
    if (totalSum > mainLanguagesValue) return false;

    const abilities = ['speaking', 'reading', 'conversation'];
    return abilities.every((ability) => {
      const abilityGroup = languagesGroup.get(ability) as FormGroup;
      return this.isAbilityValid(abilityGroup);
    });
  }
}
