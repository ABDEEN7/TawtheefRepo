export interface TeamPerformanceRow {
  employeeId: string;
  name: string;
  employeeNumber?: string;
  departmentName?: string;
  jobDescription?: string;
  assignedTasks: number;
  completedTasks: number;
  remainingTasks: number;
  overdueTasks: number;
}

export const employeeAssignmentTranslationKey = (key: string): string =>
  `dashboard.legend.employees.${key.charAt(0).toLowerCase()}${key.slice(1)}`;
