import { TreeNode } from 'primeng/api';

export interface OrgChartNodeData {
  title: string;
  description?: string;
  level?: number;
}

export type OrgChartNode = TreeNode<OrgChartNodeData>;
