import { DashboardMetricTrend } from './dashboard-overview.model';

export type QuickActionKey = 'createJob' | 'sendInvitation' | 'approveProfiles' | 'manageUsers';

export interface QuickAction {
  key: QuickActionKey;
  labelKey: string;
  icon: string;
  route: string;
  permission: string | string[];
  requireAll?: boolean;
}

export interface IndicatorNavigation {
  route: string;
  queryParams?: Record<string, unknown>;
  requiredPermission: string | string[];
}

export interface MainIndicator {
  labelKey: string;
  value: number;
  icon: string;
  color: string;
  navigation?: IndicatorNavigation;
  trend?: DashboardMetricTrend;
}
