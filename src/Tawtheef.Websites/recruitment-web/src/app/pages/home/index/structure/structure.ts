import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { SharedModule } from '../../../../shared/shared.module';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';

export type StructureNodeType = 'leadership' | 'department' | 'special' | 'section';

export interface StructureNode {
  id: string;
  title: string;
  type: StructureNodeType;
  expanded?: boolean;
  description?: string;
  responsibilities?: string[];
  children?: StructureNode[];
}

@Component({
  selector: 'app-structure',
  standalone: true,
  imports: [CommonModule, SharedModule, I18nNamespaceDirective],
  templateUrl: './structure.html',
  styleUrls: ['./structure.scss'],
})
export class Structure implements AfterViewInit {
  @ViewChild('ministerNode')
  ministerNode!: ElementRef<HTMLElement>;

  isDragging = false;
  selectedNode: StructureNode | null = null;

  private dragStartX = 0;
  private dragStartY = 0;
  private startingScrollLeft = 0;
  private startingScrollTop = 0;
  private scrollAnimationFrame?: number;

  minister: StructureNode = {
    id: 'minister',
    title: 'structure.nodes.minister',
    type: 'leadership',
    expanded: false,
  };

  ministerRightDepartments: StructureNode[] = [
    {
      id: 'minister-office',
      title: 'structure.nodes.minister-office',
      type: 'department',
    },
    {
      id: 'technical-office',
      title: 'structure.nodes.technical-office',
      type: 'department',
    },
    {
      id: 'internal-audit',
      title: 'structure.nodes.internal-audit',
      type: 'department',
    },
  ];

  ministerLeftDepartments: StructureNode[] = [
    {
      id: 'planning',
      title: 'structure.nodes.planning',
      type: 'department',
      expanded: false,
      children: [
        {
          id: 'strategic-planning',
          title: 'structure.nodes.strategic-planning',
          type: 'section',
          description: 'يختص قسم التخطيط الإستراتيجي بما يلي:',
          responsibilities: [
            'إعداد الخطة الإستراتيجية العامة للوزارة، بالتنسيق مع الجهات المختصة، والوحدات الإدارية المعنية، واتخاذ إجراءات اعتمادها.',
            'التنسيق مع الوحدات الإدارية المختصة بالوزارة، لإعداد الخطط التنفيذية والمشاريع والبرامج، واتخاذ إجراءات اعتمادها.',
            'العمل على تحقيق أهداف الوزارة، وتقديم المقترحات بشأنها، بالتنسيق مع الوحدات الإدارية المعنية.',
            'متابعة تنفيذ الخطط المتعلقة بأنشطة الوزارة وتقييم نتائجها، ووضع تقارير دورية بشأنها.',
            'دراسة المشاكل والصعوبات التي تواجه تنفيذ الخطة الإستراتيجية والخطط التنفيذية، واقتراح الحلول المناسبة بشأنها.',
            'إدارة وتجهيز خطط بديلة تضمن استمرارية تشغيل الأعمال بالوزارة في حالات الطوارئ والأزمات، بالتنسيق مع الوحدات الإدارية المختلفة كل فيما يخصه.',
          ],
        },
        {
          id: 'educational-policies-research',
          title: 'structure.nodes.educational-policies-research',
          type: 'section',
        },
        {
          id: 'quality-development-innovation',
          title: 'structure.nodes.quality-development-innovation',
          type: 'section',
        },
        {
          id: 'statistical-studies',
          title: 'structure.nodes.statistical-studies',
          type: 'section',
        },
      ],
    },
    {
      id: 'legal-affairs',
      title: 'structure.nodes.legal-affairs',
      type: 'department',
      expanded: false,
      children: [
        {
          id: 'legal-studies',
          title: 'structure.nodes.legal-studies',
          type: 'section',
        },
        {
          id: 'investigations-cases',
          title: 'structure.nodes.investigations-cases',
          type: 'section',
        },
      ],
    },
  ];

  undersecretary: StructureNode = {
    id: 'undersecretary',
    title: 'structure.nodes.undersecretary',
    type: 'leadership',
    expanded: false,
  };

  undersecretaryRightDepartments: StructureNode[] = [
    {
      id: 'undersecretary-office',
      title: 'structure.nodes.undersecretary-office',
      type: 'department',
    },
    {
      id: 'training-center',
      title: 'structure.nodes.training-center',
      type: 'department',
      expanded: false,
      children: [
        {
          id: 'training-program-planning',
          title: 'structure.nodes.training-program-planning',
          type: 'section',
        },
        {
          id: 'educational-training',
          title: 'structure.nodes.educational-training',
          type: 'section',
        },
        {
          id: 'administrative-training',
          title: 'structure.nodes.administrative-training',
          type: 'section',
        },
        {
          id: 'professional-licenses',
          title: 'structure.nodes.professional-licenses',
          type: 'section',
        },
        {
          id: 'training-systems',
          title: 'structure.nodes.training-systems',
          type: 'section',
        },
      ],
    },
    {
      id: 'international-cooperation',
      title: 'structure.nodes.international-cooperation',
      type: 'department',
    },
  ];

  undersecretaryLeftDepartments: StructureNode[] = [
    {
      id: 'public-relations',
      title: 'structure.nodes.public-relations',
      type: 'department',
      expanded: false,
      children: [
        {
          id: 'communication',
          title: 'structure.nodes.communication',
          type: 'section',
        },
        {
          id: 'public-relations-section',
          title: 'structure.nodes.public-relations-section',
          type: 'section',
        },
        {
          id: 'customer-service',
          title: 'structure.nodes.customer-service',
          type: 'section',
        },
        {
          id: 'content-translation',
          title: 'structure.nodes.content-translation',
          type: 'section',
        },
      ],
    },
    {
      id: 'information-systems',
      title: 'structure.nodes.information-systems',
      type: 'department',
      expanded: false,
      children: [
        {
          id: 'information-security',
          title: 'structure.nodes.information-security',
          type: 'section',
        },
        {
          id: 'project-management',
          title: 'structure.nodes.project-management',
          type: 'section',
        },
        {
          id: 'applications',
          title: 'structure.nodes.applications',
          type: 'section',
        },
        {
          id: 'technology-infrastructure',
          title: 'structure.nodes.technology-infrastructure',
          type: 'section',
        },
        {
          id: 'it-policies-quality',
          title: 'structure.nodes.it-policies-quality',
          type: 'section',
        },
        {
          id: 'technology-equipment-support',
          title: 'structure.nodes.technology-equipment-support',
          type: 'section',
        },
      ],
    },
  ];

  assistantUndersecretaries: StructureNode[] = [
    {
      id: 'education-affairs',
      title: 'structure.nodes.education-affairs',
      type: 'leadership',
      expanded: false,
      children: [
        {
          id: 'assistant-undersecretary-office',
          title: 'structure.nodes.assistant-undersecretary-office',
          type: 'special',
        },
        {
          id: 'vocational-technical-education',
          title: 'structure.nodes.vocational-technical-education',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'vocational-education-development',
              title: 'structure.nodes.vocational-education-development',
              type: 'section',
            },
            {
              id: 'vocational-education-supervision',
              title: 'structure.nodes.vocational-education-supervision',
              type: 'section',
            },
          ],
        },
        {
          id: 'digital-education',
          title: 'structure.nodes.digital-education',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'digital-education-development',
              title: 'structure.nodes.digital-education-development',
              type: 'section',
            },
            {
              id: 'digital-education-support',
              title: 'structure.nodes.digital-education-support',
              type: 'section',
            },
          ],
        },
        {
          id: 'curriculum-learning-resources',
          title: 'structure.nodes.curriculum-learning-resources',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'curriculum',
              title: 'structure.nodes.curriculum',
              type: 'section',
            },
            {
              id: 'learning-resources',
              title: 'structure.nodes.learning-resources',
              type: 'section',
            },
            {
              id: 'research-talent-innovation',
              title: 'structure.nodes.research-talent-innovation',
              type: 'section',
            },
            {
              id: 'school-laboratories',
              title: 'structure.nodes.school-laboratories',
              type: 'section',
            },
          ],
        },
        {
          id: 'educational-guidance',
          title: 'structure.nodes.educational-guidance',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'islamic-education',
              title: 'structure.nodes.islamic-education',
              type: 'section',
            },
            {
              id: 'arabic-language',
              title: 'structure.nodes.arabic-language',
              type: 'section',
            },
            {
              id: 'mathematics',
              title: 'structure.nodes.mathematics',
              type: 'section',
            },
            {
              id: 'science',
              title: 'structure.nodes.science',
              type: 'section',
            },
            {
              id: 'english-language',
              title: 'structure.nodes.english-language',
              type: 'section',
            },
            {
              id: 'social-sciences',
              title: 'structure.nodes.social-sciences',
              type: 'section',
            },
            {
              id: 'information-technology',
              title: 'structure.nodes.information-technology',
              type: 'section',
            },
            {
              id: 'physical-education',
              title: 'structure.nodes.physical-education',
              type: 'section',
            },
            {
              id: 'arts-theater',
              title: 'structure.nodes.arts-theater',
              type: 'section',
            },
          ],
        },
        {
          id: 'early-education',
          title: 'structure.nodes.early-education',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'kindergarten-stage',
              title: 'structure.nodes.kindergarten-stage',
              type: 'section',
            },
            {
              id: 'foundation-stage',
              title: 'structure.nodes.foundation-stage',
              type: 'section',
            },
          ],
        },
        {
          id: 'special-inclusive-education',
          title: 'structure.nodes.special-inclusive-education',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'special-schools-inclusion',
              title: 'structure.nodes.special-schools-inclusion',
              type: 'section',
            },
            {
              id: 'student-support-center',
              title: 'structure.nodes.student-support-center',
              type: 'section',
            },
          ],
        },
        {
          id: 'schools-students-affairs',
          title: 'structure.nodes.schools-students-affairs',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'student-admission-registration',
              title: 'structure.nodes.student-admission-registration',
              type: 'section',
            },
            {
              id: 'academic-guidance',
              title: 'structure.nodes.academic-guidance',
              type: 'section',
            },
            {
              id: 'student-protection-care',
              title: 'structure.nodes.student-protection-care',
              type: 'section',
            },
            {
              id: 'programs-activities',
              title: 'structure.nodes.programs-activities',
              type: 'section',
            },
            {
              id: 'school-management-support',
              title: 'structure.nodes.school-management-support',
              type: 'section',
            },
            {
              id: 'continuing-education',
              title: 'structure.nodes.continuing-education',
              type: 'section',
            },
          ],
        },
      ],
    },
    {
      id: 'private-education-affairs',
      title: 'structure.nodes.private-education-affairs',
      type: 'leadership',
      expanded: false,
      children: [
        {
          id: 'private-education-office',
          title: 'structure.nodes.assistant-undersecretary-office',
          type: 'special',
        },
        {
          id: 'daycare-centers',
          title: 'structure.nodes.daycare-centers',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'daycare-licensing',
              title: 'structure.nodes.daycare-licensing',
              type: 'section',
            },
            {
              id: 'daycare-supervision-control',
              title: 'structure.nodes.daycare-supervision-control',
              type: 'section',
            },
          ],
        },
        {
          id: 'private-school-licensing',
          title: 'structure.nodes.private-school-licensing',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'licensing-tuition-fees',
              title: 'structure.nodes.licensing-tuition-fees',
              type: 'section',
            },
            {
              id: 'private-school-registration',
              title: 'structure.nodes.private-school-registration',
              type: 'section',
            },
          ],
        },
        {
          id: 'private-schools-kindergartens',
          title: 'structure.nodes.private-schools-kindergartens',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'private-admin-supervision',
              title: 'structure.nodes.private-admin-supervision',
              type: 'section',
            },
            {
              id: 'private-academic-supervision',
              title: 'structure.nodes.private-academic-supervision',
              type: 'section',
            },
          ],
        },
        {
          id: 'educational-service-centers',
          title: 'structure.nodes.educational-service-centers',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'educational-centers-licensing',
              title: 'structure.nodes.educational-centers-licensing',
              type: 'section',
            },
            {
              id: 'educational-centers-supervision',
              title: 'structure.nodes.educational-centers-supervision',
              type: 'section',
            },
          ],
        },
      ],
    },
    {
      id: 'evaluation-affairs',
      title: 'structure.nodes.evaluation-affairs',
      type: 'leadership',
      expanded: false,
      children: [
        {
          id: 'evaluation-office',
          title: 'structure.nodes.assistant-undersecretary-office',
          type: 'special',
        },
        {
          id: 'student-assessment',
          title: 'structure.nodes.student-assessment',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'examination-affairs',
              title: 'structure.nodes.examination-affairs',
              type: 'section',
            },
            {
              id: 'international-examinations',
              title: 'structure.nodes.international-examinations',
              type: 'section',
            },
            {
              id: 'literary-subjects-assessment',
              title: 'structure.nodes.literary-subjects-assessment',
              type: 'section',
            },
            {
              id: 'scientific-subjects-assessment',
              title: 'structure.nodes.scientific-subjects-assessment',
              type: 'section',
            },
          ],
        },
        {
          id: 'school-assessment',
          title: 'structure.nodes.school-assessment',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'daycare-kindergarten-assessment',
              title: 'structure.nodes.daycare-kindergarten-assessment',
              type: 'section',
            },
            {
              id: 'government-schools-assessment',
              title: 'structure.nodes.government-schools-assessment',
              type: 'section',
            },
            {
              id: 'private-schools-assessment',
              title: 'structure.nodes.private-schools-assessment',
              type: 'section',
            },
          ],
        },
        {
          id: 'student-information-center',
          title: 'structure.nodes.student-information-center',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'examination-support',
              title: 'structure.nodes.examination-support',
              type: 'section',
            },
            {
              id: 'certificate-equivalence-authentication',
              title: 'structure.nodes.certificate-equivalence-authentication',
              type: 'section',
            },
          ],
        },
      ],
    },
    {
      id: 'higher-education-affairs',
      title: 'structure.nodes.higher-education-affairs',
      type: 'leadership',
      expanded: false,
      children: [
        {
          id: 'higher-education-office',
          title: 'structure.nodes.assistant-undersecretary-office',
          type: 'special',
        },
        {
          id: 'higher-education-institutions-affairs',
          title: 'structure.nodes.higher-education-institutions-affairs',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'higher-education-licensing',
              title: 'structure.nodes.higher-education-licensing',
              type: 'section',
            },
            {
              id: 'higher-education-supervision',
              title: 'structure.nodes.higher-education-supervision',
              type: 'section',
            },
          ],
        },
        {
          id: 'scholarships',
          title: 'structure.nodes.scholarships',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'scholarships-admission-guidance',
              title: 'structure.nodes.scholarships-admission-guidance',
              type: 'section',
            },
            {
              id: 'domestic-scholarships',
              title: 'structure.nodes.domestic-scholarships',
              type: 'section',
            },
            {
              id: 'external-scholarships',
              title: 'structure.nodes.external-scholarships',
              type: 'section',
            },
            {
              id: 'scholarships-admin-financial-support',
              title: 'structure.nodes.scholarships-admin-financial-support',
              type: 'section',
            },
          ],
        },
        {
          id: 'university-degree-equivalence',
          title: 'structure.nodes.university-degree-equivalence',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'degree-equivalence',
              title: 'structure.nodes.degree-equivalence',
              type: 'section',
            },
            {
              id: 'degree-authentication',
              title: 'structure.nodes.degree-authentication',
              type: 'section',
            },
          ],
        },
      ],
    },
    {
      id: 'shared-services-affairs',
      title: 'structure.nodes.shared-services-affairs',
      type: 'leadership',
      expanded: false,
      children: [
        {
          id: 'shared-services-office',
          title: 'structure.nodes.assistant-undersecretary-office',
          type: 'special',
        },
        {
          id: 'human-resources',
          title: 'structure.nodes.human-resources',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'hr-planning',
              title: 'structure.nodes.hr-planning',
              type: 'section',
            },
            {
              id: 'staff-affairs',
              title: 'structure.nodes.staff-affairs',
              type: 'section',
            },
            {
              id: 'teachers-admin-technical-affairs',
              title: 'structure.nodes.teachers-admin-technical-affairs',
              type: 'section',
            },
            {
              id: 'salaries-wages',
              title: 'structure.nodes.salaries-wages',
              type: 'section',
            },
            {
              id: 'recruitment-employment',
              title: 'structure.nodes.recruitment-employment',
              type: 'section',
            },
            {
              id: 'hr-services',
              title: 'structure.nodes.hr-services',
              type: 'section',
            },
          ],
        },
        {
          id: 'financial-affairs',
          title: 'structure.nodes.financial-affairs',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'financial-accounting',
              title: 'structure.nodes.financial-accounting',
              type: 'section',
            },
            {
              id: 'budget',
              title: 'structure.nodes.budget',
              type: 'section',
            },
            {
              id: 'treasury',
              title: 'structure.nodes.treasury',
              type: 'section',
            },
          ],
        },
        {
          id: 'procurement-tenders',
          title: 'structure.nodes.procurement-tenders',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'procurement',
              title: 'structure.nodes.procurement',
              type: 'section',
            },
            {
              id: 'tenders-bidding-contracts',
              title: 'structure.nodes.tenders-bidding-contracts',
              type: 'section',
            },
          ],
        },
        {
          id: 'general-services',
          title: 'structure.nodes.general-services',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'admin-services',
              title: 'structure.nodes.admin-services',
              type: 'section',
            },
            {
              id: 'building-services',
              title: 'structure.nodes.building-services',
              type: 'section',
            },
            {
              id: 'waste-management',
              title: 'structure.nodes.waste-management',
              type: 'section',
            },
            {
              id: 'warehouses',
              title: 'structure.nodes.warehouses',
              type: 'section',
            },
          ],
        },
        {
          id: 'health-safety',
          title: 'structure.nodes.health-safety',
          type: 'department',
          expanded: false,
          children: [
            {
              id: 'security-safety',
              title: 'structure.nodes.security-safety',
              type: 'section',
            },
            {
              id: 'health-nutrition',
              title: 'structure.nodes.health-nutrition',
              type: 'section',
            },
          ],
        },
      ],
    },
  ];

  ngAfterViewInit(): void {
    requestAnimationFrame(() => {
      if (!window.matchMedia('(max-width: 767.98px)').matches) {
        this.ministerNode.nativeElement.scrollIntoView({
          behavior: 'auto',
          block: 'nearest',
          inline: 'center',
        });
      }
    });
  }

  toggleMinister(event: MouseEvent): void {
    event.stopPropagation();

    const shouldOpen = !this.minister.expanded;

    this.closeAllMaroonNodes();

    this.minister.expanded = shouldOpen;

    if (this.minister.expanded) {
      this.centerOpenedContent(event.currentTarget as HTMLElement, '.minister-branches');
    }
  }

  toggleUndersecretary(event: MouseEvent): void {
    event.stopPropagation();

    const shouldOpen = !this.undersecretary.expanded;

    this.closeAllMaroonNodes();

    this.undersecretary.expanded = shouldOpen;

    if (this.undersecretary.expanded) {
      this.centerOpenedContent(event.currentTarget as HTMLElement, '.undersecretary-branches');
    }
  }

  toggleNode(node: StructureNode, event: MouseEvent): void {
    event.stopPropagation();

    if (!node.children?.length) {
      return;
    }

    const willExpand = !node.expanded;

    if (node.type === 'leadership') {
      this.closeAllMaroonNodes();
      node.expanded = willExpand;
    } else if (willExpand && node.type === 'department') {
      this.closeOtherBlueNodes(node);
      node.expanded = true;
    } else {
      node.expanded = willExpand;
    }

    if (node.expanded) {
      this.centerOpenedNode(event.currentTarget as HTMLElement);
    } else {
      this.closeNodes(node.children);
    }
  }

  hasExpandedMinisterLeftDepartment(): boolean {
    return this.ministerLeftDepartments.some((node) => node.expanded);
  }

  isPlanningExpanded(): boolean {
    return this.isNodeExpanded(this.ministerLeftDepartments, 'planning');
  }

  hasExpandedUndersecretaryDepartment(): boolean {
    return [...this.undersecretaryLeftDepartments, ...this.undersecretaryRightDepartments].some(
      (node) => node.expanded,
    );
  }

  isPublicRelationsExpanded(): boolean {
    return this.isNodeExpanded(this.undersecretaryLeftDepartments, 'public-relations');
  }

  isEducationAffairsExpanded(): boolean {
    return this.assistantUndersecretaries.some((assistant) => assistant.expanded);
  }

  getEducationBranchHeight(): number {
    const expandedAssistant = this.assistantUndersecretaries.find(
      (assistant) => assistant.expanded,
    );

    if (!expandedAssistant?.children?.length) {
      return 0;
    }

    let height = 160;

    expandedAssistant.children.forEach((node) => {
      height += 96;

      if (node.expanded && node.children?.length) {
        height += 44 + node.children.length * 68;
      }
    });

    return height;
  }

  showNodeInformation(node: StructureNode, event: MouseEvent): void {
    event.stopPropagation();
    this.selectedNode = node;
  }

  closeNodeInformation(): void {
    this.selectedNode = null;
  }

  stopModalClick(event: MouseEvent): void {
    event.stopPropagation();
  }

  @HostListener('document:keydown.escape')
  closeModalWithEscape(): void {
    this.closeNodeInformation();
  }

  startDragging(event: PointerEvent): void {
    if (window.matchMedia('(max-width: 767.98px)').matches) {
      return;
    }

    const target = event.target as HTMLElement;

    if (target.closest('button')) {
      return;
    }

    const map = event.currentTarget as HTMLElement;

    if (this.scrollAnimationFrame !== undefined) {
      cancelAnimationFrame(this.scrollAnimationFrame);

      this.scrollAnimationFrame = undefined;
    }

    this.isDragging = true;
    this.dragStartX = event.clientX;
    this.dragStartY = event.clientY;
    this.startingScrollLeft = map.scrollLeft;
    this.startingScrollTop = map.scrollTop;

    map.setPointerCapture(event.pointerId);
  }

  dragMap(event: PointerEvent): void {
    if (!this.isDragging) {
      return;
    }

    const map = event.currentTarget as HTMLElement;

    const movementX = event.clientX - this.dragStartX;

    const movementY = event.clientY - this.dragStartY;

    map.scrollLeft = this.startingScrollLeft - movementX;

    map.scrollTop = this.startingScrollTop - movementY;
  }

  stopDragging(event: PointerEvent): void {
    if (!this.isDragging) {
      return;
    }

    const map = event.currentTarget as HTMLElement;

    this.isDragging = false;

    if (map.hasPointerCapture(event.pointerId)) {
      map.releasePointerCapture(event.pointerId);
    }
  }

  trackByNodeId(index: number, node: StructureNode): string {
    return node.id;
  }

  private isNodeExpanded(nodes: StructureNode[], id: string): boolean {
    return nodes.some((node) => node.id === id && node.expanded);
  }

  private closeNodes(nodes: StructureNode[]): void {
    nodes.forEach((node) => {
      node.expanded = false;

      if (node.children?.length) {
        this.closeNodes(node.children);
      }
    });
  }

  private closeOtherBlueNodes(openedNode: StructureNode): void {
    const treeRoots: StructureNode[] = [
      ...this.ministerLeftDepartments,
      ...this.ministerRightDepartments,
      ...this.undersecretaryLeftDepartments,
      ...this.undersecretaryRightDepartments,
      ...this.assistantUndersecretaries,
    ];

    const closeOtherDepartments = (nodes: StructureNode[]): void => {
      nodes.forEach((node) => {
        if (node !== openedNode && node.type === 'department') {
          node.expanded = false;

          if (node.children?.length) {
            this.closeNodes(node.children);
          }
        }

        if (node.children?.length) {
          closeOtherDepartments(node.children);
        }
      });
    };

    closeOtherDepartments(treeRoots);
  }

  private closeAllMaroonNodes(): void {
    this.minister.expanded = false;
    this.undersecretary.expanded = false;

    this.closeNodes(this.ministerLeftDepartments);
    this.closeNodes(this.ministerRightDepartments);

    this.closeNodes(this.undersecretaryLeftDepartments);

    this.closeNodes(this.undersecretaryRightDepartments);

    this.assistantUndersecretaries.forEach((assistant) => {
      assistant.expanded = false;

      if (assistant.children?.length) {
        this.closeNodes(assistant.children);
      }
    });
  }

  private centerOpenedContent(trigger: HTMLElement, selector: string): void {
    requestAnimationFrame(() => {
      requestAnimationFrame(() => {
        const tree = trigger.closest('.structure-tree');

        const openedContent = tree?.querySelector<HTMLElement>(selector);

        if (openedContent) {
          this.animateMapToElement(trigger, openedContent);
        }
      });
    });
  }

  private centerOpenedNode(trigger: HTMLElement): void {
    requestAnimationFrame(() => {
      requestAnimationFrame(() => {
        const openedBranch = trigger.closest<HTMLElement>(
          '.department-node-wrapper, ' + '.assistant-branch, ' + '.education-department-group',
        );

        const openedContent = openedBranch?.querySelector<HTMLElement>(
          '.department-children, ' + '.education-affairs-tree, ' + '.education-children',
        );

        const target = openedContent ?? openedBranch;

        if (target) {
          this.animateMapToElement(trigger, target);
        }
      });
    });
  }

  private animateMapToElement(trigger: HTMLElement, target: HTMLElement): void {
    if (window.matchMedia('(max-width: 767.98px)').matches) {
      target.scrollIntoView({
        behavior: 'smooth',
        block: 'center',
        inline: 'nearest',
      });

      return;
    }

    const map = trigger.closest<HTMLElement>('.structure-map');

    if (!map) {
      return;
    }

    if (this.scrollAnimationFrame !== undefined) {
      cancelAnimationFrame(this.scrollAnimationFrame);
    }

    const mapRect = map.getBoundingClientRect();

    const targetRect = target.getBoundingClientRect();

    const startLeft = map.scrollLeft;
    const startTop = map.scrollTop;

    const requestedLeft =
      startLeft + targetRect.left - mapRect.left - (map.clientWidth - targetRect.width) / 2;

    const requestedTop =
      startTop + targetRect.top - mapRect.top - (map.clientHeight - targetRect.height) / 2;

    const targetLeft = requestedLeft;

    const targetTop = Math.max(0, Math.min(requestedTop, map.scrollHeight - map.clientHeight));

    const distanceLeft = targetLeft - startLeft;

    const distanceTop = targetTop - startTop;

    const duration = 950;
    const startTime = performance.now();

    const easeInOutCubic = (progress: number): number =>
      progress < 0.5 ? 4 * progress * progress * progress : 1 - Math.pow(-2 * progress + 2, 3) / 2;

    const animate = (currentTime: number): void => {
      const progress = Math.min((currentTime - startTime) / duration, 1);

      const easedProgress = easeInOutCubic(progress);

      map.scrollLeft = startLeft + distanceLeft * easedProgress;

      map.scrollTop = startTop + distanceTop * easedProgress;

      if (progress < 1) {
        this.scrollAnimationFrame = requestAnimationFrame(animate);
      } else {
        this.scrollAnimationFrame = undefined;
      }
    };

    this.scrollAnimationFrame = requestAnimationFrame(animate);
  }
}
