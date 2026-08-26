import { TreeNode } from 'primeng/api';

export interface OrgChartNodeData {
  title: string;
  description?: string;
  /**
   * 1-indexed depth in the tree, computed by StructureSchools.buildNode.
   * Drives the per-level card color (see .organization-card--level-*).
   */
  level?: number;
}

export type OrgChartNode = TreeNode<OrgChartNodeData>;
