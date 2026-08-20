import { Component, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { OrganizationChartModule } from 'primeng/organizationchart';
import {
  OrganizationChartNodeExpandEvent,
  OrganizationChartNodeCollapseEvent,
} from 'primeng/types/organizationchart';
import { TooltipModule } from 'primeng/tooltip';
import { DialogModule } from 'primeng/dialog';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { OrgChartNode } from './structure-schools.model';
import { SCHOOLS_STRUCTURE } from './structure-schools.data';

/**
 * Keys expanded by default so the page starts compact: only the root
 * ("Institutions") is open, revealing the Schools/Kindergartens/Specialized
 * Schools branches. Everything deeper waits for a click.
 */
const DEFAULT_EXPANDED_KEYS = new Set<string>(['0']);

/**
 * Same breakpoint pages/home/structure (the Ministry chart) uses to swap
 * into its vertical accordion.
 */
const MOBILE_BREAKPOINT_PX = 767.98;

@Component({
  selector: 'app-structure-schools',
  standalone: true,
  imports: [
    CommonModule,
    OrganizationChartModule,
    TooltipModule,
    DialogModule,
    TranslatePipe,
    I18nNamespaceDirective,
  ],
  templateUrl: './structure-schools.html',
  styleUrl: './structure-schools.scss',
})
export class StructureSchools {

  private readonly childrenByKey = new Map<string, OrgChartNode[]>();
  organizationData: OrgChartNode[] = this.buildTree(SCHOOLS_STRUCTURE);

  selectedNode: OrgChartNode | null = null;
  isMobile = this.matchesMobile();

  @HostListener('window:resize')
  onWindowResize(): void {
    this.isMobile = this.matchesMobile();
  }

  private buildTree(source: OrgChartNode[]): OrgChartNode[] {
    this.childrenByKey.clear();
    return source.map((node) => this.buildNode(node, 1));
  }

  private buildNode(node: OrgChartNode, level: number): OrgChartNode {
    const hasChildren = !!node.children?.length;
    const children = hasChildren
      ? node.children!.map((child) => this.buildNode(child, level + 1))
      : [];

    if (hasChildren && node.key) {
      this.childrenByKey.set(node.key, children);
    }

    const expanded = hasChildren && !!node.key && DEFAULT_EXPANDED_KEYS.has(node.key);

    return {
      ...node,
      data: node.data ? { ...node.data, level } : node.data,
      leaf: !hasChildren,
      expanded,
      children: expanded ? children : [],
    };
  }

  openNodeDetails(node: OrgChartNode, event: Event): void {
    event.stopPropagation(); // this function for the node click event, so we don't want the click to bubble up and trigger the expand/collapse behavior

    this.selectedNode = node;
  }

  onNodeUnselect(): void {
    this.selectedNode = null;
  }

  onDialogVisibleChange(visible: boolean): void {
    if (!visible) {
      this.onNodeUnselect();
    }
  }

  onNodeExpand(event: OrganizationChartNodeExpandEvent): void {
    const key = event.node.key;
    event.node.children = (key && this.childrenByKey.get(key)) || [];

    this.focusExpandedNode(event.node as OrgChartNode);
  }

  onNodeCollapse(event: OrganizationChartNodeCollapseEvent): void {
    event.node.children = [];
  }

  toggleMobileNode(node: OrgChartNode, event: Event): void {
    event.stopPropagation();

    if (!node.key || !this.childrenByKey.has(node.key)) {
      return;
    }

    node.expanded = !node.expanded;
    node.children = node.expanded ? (this.childrenByKey.get(node.key) ?? []) : [];

    if (node.expanded) {
      this.focusExpandedNode(node);
    }
  }

  private focusExpandedNode(node: OrgChartNode): void {
        if ((node.data?.level ?? 0) <= 2 || !node.key) {
      return;
    }

    this.collapseOtherDeepNodes(this.organizationData, node.key);
    this.scrollNodeIntoView(node.key);
    this.highlightNode(node.key);
  }
  focusedNodeKey: string | null = null;

  private focusHighlightTimeout?: ReturnType<typeof setTimeout>;

  private highlightNode(key: string): void {
    this.focusedNodeKey = key;

    clearTimeout(this.focusHighlightTimeout);

    this.focusHighlightTimeout = setTimeout(() => {
      this.focusedNodeKey = null;
    }, 1400);
  }

  private collapseOtherDeepNodes(nodes: OrgChartNode[], keepKey: string): void {
    nodes.forEach((node) => {
      const key = node.key ?? '';
      const isRelated =
        key === keepKey || keepKey.startsWith(`${key}_`) || key.startsWith(`${keepKey}_`);

      if ((node.data?.level ?? 0) > 2 && node.expanded && !isRelated) {
        node.expanded = false;
        node.children = [];
      }

      if (node.children?.length) {
        this.collapseOtherDeepNodes(node.children, keepKey);
      }
    });
  }
  private scrollNodeIntoView(key: string): void {
    requestAnimationFrame(() => {
      requestAnimationFrame(() => {
        const element = document.querySelector<HTMLElement>(`[data-node-key="${key}"]`);

        if (!element) {
          return;
        }

        if (this.isMobile) {
          element.scrollIntoView({ behavior: 'smooth', block: 'center' });
          return;
        }

        const map = element.closest<HTMLElement>('.organization-container');

        if (map) {
          this.animatePanToElement(map, element);
        }
      });
    });
  }

  private animatePanToElement(map: HTMLElement, target: HTMLElement): void {
    const mapRect = map.getBoundingClientRect();
    const targetRect = target.getBoundingClientRect();

    const startLeft = map.scrollLeft;
    const startTop = map.scrollTop;

    const requestedLeft =
      startLeft + targetRect.left - mapRect.left - (map.clientWidth - targetRect.width) / 2;
    const requestedTop =
      startTop + targetRect.top - mapRect.top - (map.clientHeight - targetRect.height) / 2;

    const targetLeft = Math.max(0, Math.min(requestedLeft, map.scrollWidth - map.clientWidth));
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
        requestAnimationFrame(animate);
      }
    };

    requestAnimationFrame(animate);
  }

  private matchesMobile(): boolean {
    return window.matchMedia(`(max-width: ${MOBILE_BREAKPOINT_PX}px)`).matches;
  }
  isDragging = false;

  private dragStartX = 0;
  private dragStartY = 0;
  private startingScrollLeft = 0;
  private startingScrollTop = 0;

  startDragging(event: PointerEvent): void {
    const target = event.target as HTMLElement;

    if (target.closest('a, button, .node-info-icon')) {
      return;
    }

    const map = event.currentTarget as HTMLElement;

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

    map.scrollLeft = this.startingScrollLeft - (event.clientX - this.dragStartX);
    map.scrollTop = this.startingScrollTop - (event.clientY - this.dragStartY);
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
}
