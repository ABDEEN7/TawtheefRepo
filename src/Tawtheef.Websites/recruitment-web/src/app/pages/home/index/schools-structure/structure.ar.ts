import { OrgChartNode } from './schools-structure.model';

/**
 * Static placeholder mirroring structure.md. Replace with an API call
 * returning the same OrgChartNode[] shape once the backend is ready.
 */
const PLACEHOLDER_IMAGE = 'assets/img/home-final/1.png';

function leafNode(key: string, title: string): OrgChartNode {
  return {
    key,
    data: {
      title,
      subtitle: 'مسمى وظيفي',
      image: PLACEHOLDER_IMAGE,
      accent: 'bg-cyan-500/10',
    },
  };
}

function leafNodes(parentKey: string, titles: string[]): OrgChartNode[] {
  return titles.map((title, i) => leafNode(`${parentKey}_${i}`, title));
}

function deputyNode(key: string, title: string, positions: string[]): OrgChartNode {
  return {
    key,
    data: {
      title,
      subtitle: 'الإدارة التنفيذية',
      image: PLACEHOLDER_IMAGE,
      accent: 'bg-sky-500/10',
    },
    children: leafNodes(key, positions),
  };
}

function categoryNode(key: string, title: string, positions: string[]): OrgChartNode {
  return {
    key,
    data: {
      title,
      subtitle: 'فئة وظيفية',
      image: PLACEHOLDER_IMAGE,
      accent: 'bg-sky-500/10',
    },
    children: leafNodes(key, positions),
  };
}

function branchNode(key: string, title: string, accent: string, principalChildren: OrgChartNode[]): OrgChartNode {
  return {
    key,
    data: {
      title,
      subtitle: 'نوع مؤسسة',
      image: PLACEHOLDER_IMAGE,
      accent,
    },
    children: [
      {
        key: `${key}_p`,
        data: {
          title: 'مدير المدرسة',
          subtitle: 'الإدارة العليا',
          image: PLACEHOLDER_IMAGE,
          description: 'هذا وصف وظيفي او يمكن ان نوضع المهام التي يقوم بها مدير المدرسة او اي معلومات اخرى.',
          accent: 'bg-orange-500/10',
        },
        children: principalChildren,
      },
    ],
  };
}

const schools = branchNode('0_0', 'المدارس', 'bg-emerald-500/10', [
  deputyNode('0_0_p_0', 'نائب المدير للشؤون الأكاديمية', [
    'رياض أطفال',
    'معلم طفلة اللغة الإنجليزية',
    'معلم تربية مسرحية',
    'منسق المشاريع الإلكترونية',
    'معلم دعم إضافي',
    'معلم علوم اجتماعية',
    'معلم رياضيات',
    'معلم علوم شرعية',
    'معلم تربية خاصة',
    'معلم علوم',
    'وظائف أكاديمية أخرى',
  ]),
  deputyNode('0_0_p_1', 'نائب المدير للشؤون الإدارية وشؤون الطلاب', [
    'مساعد مشرف إداري',
    'أخصائي أنشطة مدرسية',
    'مساعد سكرتير المدرسة',
    'أخصائي نفسي',
    'فني تقنية معلومات',
    'مساعد معلم تربية خاصة',
    'أخصائي علاج نطق ولغة',
    'مرافق الدعم',
    'أخصائي إعلام وظيفي',
    'أخصائي اجتماعي',
    'وظائف إدارية ووظائف شؤون طلاب أخرى',
  ]),
]);

const kindergartens = branchNode('0_1', 'رياض الأطفال', 'bg-pink-500/10', [
  categoryNode('0_1_p_0', 'المسميات الأكاديمية', ['معلم رياض أطفال', 'معلم لغة إنجليزية']),
  categoryNode('0_1_p_1', 'المسميات الإدارية', [
    'مساعد معلم تربية خاصة',
    'مرافق الدعم',
    'أخصائي اجتماعي',
    'ملاحظ طلبة',
    'عامل خدمات',
    'وظائف إدارية أخرى',
  ]),
]);

const specializedSchools = branchNode('0_2', 'المدارس التخصصية', 'bg-violet-500/10', [
  deputyNode('0_2_p_0', 'نائب المدير للشؤون الفنية', ['معلم تقني']),
  deputyNode('0_2_p_1', 'نائب المدير للشؤون الأكاديمية', [
    'معلم المهارات الحياتية',
    'منسق المشاريع الإلكترونية',
    'معلم علوم اجتماعية',
    'معلم فيزياء',
    'معلم كيمياء',
    'معلمين أكاديميين آخرين',
  ]),
  deputyNode('0_2_p_2', 'نائب المدير للشؤون الإدارية وشؤون الطلاب', [
    'فني معمل الكيمياء',
    'محضر مختبر فيزياء',
    'أخصائي نفسي',
    'فني تقنية معلومات',
    'أخصائي اجتماعي',
    'منسق شؤون الطلاب',
  ]),
]);

export const SCHOOLS_STRUCTURE_AR: OrgChartNode[] = [
  {
    key: '0',
    data: {
      title: 'المؤسسات',
      subtitle: 'الهيكل التنظيمي',
      description: 'الهيكل التنظيمي العام لمؤسسات الوزارة التعليمية.',
      image: PLACEHOLDER_IMAGE,
      accent: 'bg-blue-500/10',
    },
    children: [schools, kindergartens, specializedSchools],
  },
];
