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
  /**
   * PrimeNG renders a node's children with `visibility: hidden` while
   * collapsed instead of removing them from the DOM, so a collapsed subtree
   * still reserves its full expanded footprint. To make the page's width
   * and height actually track what's expanded, collapsed nodes get an
   * empty `children` array here, and their real children (cached below)
   * are swapped back in on expand.
   */
  private readonly childrenByKey = new Map<string, OrgChartNode[]>();

  /**
   * Built once — the tree's shape doesn't vary by language, only the
   * displayed text does (title/description are translation keys, resolved
   * reactively in the template via `| translate`), so there's no need to
   * rebuild on language change like this used to.
   */
  organizationData: OrgChartNode[] = this.buildTree(SCHOOLS_STRUCTURE);

  selectedNode: OrgChartNode | null = null;

  /**
   * Desktop keeps the PrimeNG chart; below the Ministry chart's own
   * breakpoint we swap to a plain recursive accordion instead (PrimeNG's
   * org chart is a <table>-based horizontal layout and can't reflow into
   * a vertical list via CSS alone).
   */
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

  /**
   * Mobile accordion's own toggle — mirrors onNodeExpand/onNodeCollapse
   * above so `node.children`/`node.expanded` stay consistent with the
   * PrimeNG chart's expectations if the viewport crosses the breakpoint
   * mid-session (both views share the same organizationData objects).
   */
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

  /**
   * Only one level-3+ ("below the branches") path stays open anywhere in
   * the tree at a time. Runs on every expand, including a branch itself
   * (e.g. expanding "Kindergartens" also collapses "School Principal" left
   * open under "Schools") — collapseOtherDeepNodes only ever touches
   * level 3+ nodes, so branches (level 1-2) themselves are never collapsed
   * by this, only stale deeper content under *other* branches.
   */
  private focusExpandedNode(node: OrgChartNode): void {
        if ((node.data?.level ?? 0) <= 2 || !node.key) {
      return;
    }

    this.collapseOtherDeepNodes(this.organizationData, node.key);
    this.scrollNodeIntoView(node.key);
    this.highlightNode(node.key);
  }

  /**
   * Briefly marks the just-expanded node as "focused" (see .is-focused in
   * the stylesheet — a short pulse ring) so it's visually obvious which
   * node the auto-scroll/pan just centered on.
   */
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

  /**
   * Brings the just-expanded node into view — smoothly on mobile (plain
   * page scroll), or by panning .organization-container on desktop
   * (same eased-scroll approach as pages/home/structure's
   * animateMapToElement). Waits two animation frames so Angular/PrimeNG
   * have actually painted the newly revealed children before measuring.
   */
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

  /**
   * Drag-to-pan for the desktop chart, ported from pages/home/structure's
   * .structure-map. No mobile check needed here (unlike that page): this
   * only ever binds to .organization-container, which is already inside
   * an `@if (!isMobile)` branch.
   */
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
