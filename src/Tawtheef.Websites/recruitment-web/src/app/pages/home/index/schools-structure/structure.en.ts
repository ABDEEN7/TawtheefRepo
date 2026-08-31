import { OrgChartNode } from './schools-structure.model';

/**
 * Static placeholder mirroring structure.md (English). Replace with an API
 * call returning the same OrgChartNode[] shape once the backend is ready.
 */
const PLACEHOLDER_IMAGE = 'assets/img/home-final/1.png';

function leafNode(key: string, title: string): OrgChartNode {
  return {
    key,
    data: {
      title,
      subtitle: 'Job Title',
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
      subtitle: 'Executive Management',
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
      subtitle: 'Job Category',
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
      subtitle: 'Institution Type',
      image: PLACEHOLDER_IMAGE,
      description: 'we will put a description here for the scjool manager or listt of responsibilites',
      accent,
    },
    children: [
      {
        key: `${key}_p`,
        data: {
          title: 'School Principal',
          subtitle: 'Senior Management',
          image: PLACEHOLDER_IMAGE,
          accent: 'bg-orange-500/10',
        },
        children: principalChildren,
      },
    ],
  };
}

const schools = branchNode('0_0', 'Schools', 'bg-emerald-500/10', [
  deputyNode('0_0_p_0', 'Deputy Principal for Academic Affairs', [
    'Kindergarten',
    'Early Childhood English Language Teacher',
    'Drama Education Teacher',
    'E-Projects Coordinator',
    'Additional Support Teacher',
    'Social Studies Teacher',
    'Mathematics Teacher',
    'Islamic Studies Teacher',
    'Special Education Teacher',
    'Science Teacher',
    'Other Academic Positions',
  ]),
  deputyNode('0_0_p_1', 'Deputy Principal for Administrative & Student Affairs', [
    'Administrative Supervisor Assistant',
    'School Activities Specialist',
    'School Secretary Assistant',
    'Psychologist',
    'IT Technician',
    'Special Education Teacher Assistant',
    'Speech & Language Therapist',
    'Support Staff',
    'Career Guidance Specialist',
    'Social Worker',
    'Other Administrative & Student Affairs Positions',
  ]),
]);

const kindergartens = branchNode('0_1', 'Kindergartens', 'bg-pink-500/10', [
  categoryNode('0_1_p_0', 'Academic Titles', ['Kindergarten Teacher', 'English Language Teacher']),
  categoryNode('0_1_p_1', 'Administrative Titles', [
    'Special Education Teacher Assistant',
    'Support Staff',
    'Social Worker',
    'Student Supervisor',
    'Services Worker',
    'Other Administrative Positions',
  ]),
]);

const specializedSchools = branchNode('0_2', 'Specialized Schools', 'bg-violet-500/10', [
  deputyNode('0_2_p_0', 'Deputy Principal for Technical Affairs', ['Technical Teacher']),
  deputyNode('0_2_p_1', 'Deputy Principal for Academic Affairs', [
    'Life Skills Teacher',
    'E-Projects Coordinator',
    'Social Studies Teacher',
    'Physics Teacher',
    'Chemistry Teacher',
    'Other Academic Teachers',
  ]),
  deputyNode('0_2_p_2', 'Deputy Principal for Administrative & Student Affairs', [
    'Chemistry Lab Technician',
    'Physics Lab Preparator',
    'Psychologist',
    'IT Technician',
    'Social Worker',
    'Student Affairs Coordinator',
  ]),
]);

export const SCHOOLS_STRUCTURE_EN: OrgChartNode[] = [
  {
    key: '0',
    data: {
      title: 'Institutions',
      subtitle: 'Organizational Structure',
      description: 'The general organizational structure of the ministry\'s educational institutions.',
      image: PLACEHOLDER_IMAGE,
      accent: 'bg-blue-500/10',
    },
    children: [schools, kindergartens, specializedSchools],
  },
];
