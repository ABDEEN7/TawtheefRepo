import { Injectable, signal } from '@angular/core';
import { Profile } from './profile-list.model';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private readonly profiles = signal<Profile[]>(this.generateProfiles());

  getProfiles() {
    return this.profiles.asReadonly();
  }

  private generateProfiles(): Profile[] {
    const majors = ["تقنية المعلومات", "علوم الحاسب", "هندسة الشبكات", "أمن المعلومات", "نظم المعلومات"];
    const names = ["أحمد محمد المحمود", "سارة علي القحطاني", "محمد سالم النعيمي", "بدر ناصر الكعبي", "نورة عبدالله العمادي", "هديل راشد المهندي", "راشد مبارك المري", "مريم سعيد الأنصاري", "سلمان عبدالعزيز العذبة", "ريم خليل المانع", "يوسف جابر الكواري", "حصة علي ثاني"];
    const statuses = ["بانتظار المراجعة", "قيد الاعتماد", "مكتمل", "يحتاج تصحيح"];
    const entitiesTwo = ["الوزارة", "المدارس"];

    const profiles: Profile[] = [];
    for (let i = 0; i < 48; i++) {
      const id = "C-2025-" + String(i + 1).padStart(4, "0");
      profiles.push({
        id,
        name: names[i % names.length],
        major: majors[i % majors.length],
        entity: entitiesTwo[i % 2],
        sent: new Date(2025, 9, 31 - i).toISOString().slice(0, 10),
        status: statuses[i % statuses.length],
        avatar: "https://picsum.photos/seed/" + encodeURIComponent(id) + "/80/80"
      });
    }
    return profiles;
  }
}
