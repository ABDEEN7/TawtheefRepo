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
    },
    {
      id: 'evaluation-affairs',
      title: 'structure.nodes.evaluation-affairs',
      type: 'leadership',
      expanded: false,
    },
    {
      id: 'higher-education-affairs',
      title: 'structure.nodes.higher-education-affairs',
      type: 'leadership',
      expanded: false,
    },
    {
      id: 'shared-services-affairs',
      title: 'structure.nodes.shared-services-affairs',
      type: 'leadership',
      expanded: false,
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
    return this.isNodeExpanded(this.assistantUndersecretaries, 'education-affairs');
  }

  getEducationBranchHeight(): number {
    const educationAffairs = this.assistantUndersecretaries.find(
      (assistant) => assistant.id === 'education-affairs',
    );

    if (!educationAffairs?.expanded) {
      return 0;
    }

    let height = 160;

    educationAffairs.children?.forEach((node) => {
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
