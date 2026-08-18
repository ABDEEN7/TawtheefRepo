import { Component, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { OrganizationChartModule } from 'primeng/organizationchart';
import {
  OrganizationChartNodeExpandEvent,
  OrganizationChartNodeCollapseEvent,
} from 'primeng/types/organizationchart';
import { TooltipModule } from 'primeng/tooltip';
import { DialogModule } from 'primeng/dialog';
import { LanguageService, Lang } from '../../../../core/services/language.service';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { OrgChartNode } from './schools-structure.model';
import { SCHOOLS_STRUCTURE_AR } from './structure.ar';
import { SCHOOLS_STRUCTURE_EN } from './structure.en';
 
/**
 * Keys expanded by default so the page starts compact: only the root
 * ("Institutions") is open, revealing the Schools/Kindergartens/Specialized
 * Schools branches. Everything deeper waits for a click.
 */
const DEFAULT_EXPANDED_KEYS = new Set<string>(['0']);
 
@Component({
  selector: 'app-schools-structure',
  standalone: true,
  imports: [
    CommonModule,
    OrganizationChartModule,
    TooltipModule,
    DialogModule,
    TranslatePipe,
    I18nNamespaceDirective,
  ],
  templateUrl: './schools-structure.html',
  styleUrl: './schools-structure.scss',
})
export class SchoolsStructure {
  private readonly language = inject(LanguageService);
 
  /**
   * PrimeNG renders a node's children with `visibility: hidden` while
   * collapsed instead of removing them from the DOM, so a collapsed subtree
   * still reserves its full expanded footprint. To make the page's width
   * and height actually track what's expanded, collapsed nodes get an
   * empty `children` array here, and their real children (cached below)
   * are swapped back in on expand.
   */
  private readonly childrenByKey = new Map<string, OrgChartNode[]>();
 
  organizationData: OrgChartNode[] = this.buildTree(this.dataFor(this.language.get()));
 
  selectedNode: OrgChartNode | null = null;
 
  constructor() {
    this.language.current$.pipe(takeUntilDestroyed()).subscribe((lang) => {
      this.organizationData = this.buildTree(this.dataFor(lang));
      this.selectedNode = null;
    });
  }
 
  private dataFor(lang: Lang): OrgChartNode[] {
    return lang === 'ar' ? SCHOOLS_STRUCTURE_AR : SCHOOLS_STRUCTURE_EN;
  }
 
  private buildTree(source: OrgChartNode[]): OrgChartNode[] {
    this.childrenByKey.clear();
    return source.map((node) => this.buildNode(node, 1));
  }
 
  private buildNode(node: OrgChartNode, level: number): OrgChartNode {
    const hasChildren = !!node.children?.length;
    const children = hasChildren ? node.children!.map((child) => this.buildNode(child, level + 1)) : [];
 
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
 
    if (node.data?.description) {
      this.selectedNode = node;
    }
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
  }
 
  onNodeCollapse(event: OrganizationChartNodeCollapseEvent): void {
    event.node.children = [];
  }
}
 
 