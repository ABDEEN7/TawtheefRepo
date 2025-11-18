// services/points-config.service.ts
import { Injectable } from '@angular/core';
import { PointsConfig } from '../models/points-config.model';
import { PointsGroup } from '../types/points-group.type';

@Injectable({
  providedIn: 'root'
})
export class PointsConfigService {
  readonly groups: PointsGroup[] = ['degree', 'exp', 'langs', 'skills', 'avail', 'geo'];
  
  readonly rubricLabels: Record<PointsGroup, string> = {
    degree: 'lbl_degree',
    exp: 'lbl_experience',
    langs: 'lbl_languages',
    skills: 'lbl_skills',
    avail: 'lbl_availability',
    geo: 'lbl_geography'
  };

  getDefaultPointsConfig(): PointsConfig {
    return {
      totals: { degree: 250, exp: 200, langs: 150, skills: 150, avail: 100, geo: 150 },
      rubrics: {
        degree: {
          phd: 70,
          masters: 60,
          bachelor: 50,
          diploma: 35,
          high_school: 20,
          preparatory: 10,
          elementary: 5
        },
        exp: {
          '10+': 60,
          '7-9': 50,
          '4-6': 40,
          '1-3': 30,
          '0-1': 20
        },
        langs: {
          two_languages: 60,
          one_strong_language: 45,
          medium_language: 30,
          basic: 15
        },
        skills: {
          '>=80%': 60,
          '60-79%': 45,
          '40-59%': 30,
          '<40%': 15
        },
        avail: {
          immediate_inside_qatar: 40,
          within_month: 30,
          outside_country: 20,
          not_available: 10
        },
        geo: {
          policy_match: 70,
          priority: 50,
          general: 30
        }
      }
    };
  }

  getGroupLabels(): PointsGroup[] {
    return [...this.groups];
  }

  getRubricLabel(group: PointsGroup): string {
    return this.rubricLabels[group];
  }

  getRubricOptions(config: PointsConfig, group: PointsGroup): string[] {
    return Object.keys(config.rubrics[group]);
  }

  getRubricValue(config: PointsConfig, group: PointsGroup, option: string): number {
    return config.rubrics[group][option];
  }

  getTotalValue(config: PointsConfig, group: PointsGroup): number {
    return config.totals[group];
  }
}