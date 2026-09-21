export enum CalculationMethod {
  AverageOfEvaluators = 1,
  WeightedByRole = 2,
  ChairmanDecides = 3,
}

export enum TemplateVersionStatus {
  Draft = 1,
  PendingApproval = 2,
  Returned = 3,
  Approved = 4,
  Superseded = 5,
  Cancelled = 6,
}

export const CALCULATION_METHOD_LABELS: Record<CalculationMethod, string> = {
  [CalculationMethod.AverageOfEvaluators]: 'INTERVIEW_TEMPLATES.CALCULATION_METHOD.AVERAGE_OF_EVALUATORS',
  [CalculationMethod.WeightedByRole]: 'INTERVIEW_TEMPLATES.CALCULATION_METHOD.WEIGHTED_BY_ROLE',
  [CalculationMethod.ChairmanDecides]: 'INTERVIEW_TEMPLATES.CALCULATION_METHOD.CHAIRMAN_DECIDES',
};

export const TEMPLATE_VERSION_STATUS_LABELS: Record<TemplateVersionStatus, string> = {
  [TemplateVersionStatus.Draft]: 'INTERVIEW_TEMPLATES.STATUS.DRAFT',
  [TemplateVersionStatus.PendingApproval]: 'INTERVIEW_TEMPLATES.STATUS.PENDING_APPROVAL',
  [TemplateVersionStatus.Returned]: 'INTERVIEW_TEMPLATES.STATUS.RETURNED',
  [TemplateVersionStatus.Approved]: 'INTERVIEW_TEMPLATES.STATUS.APPROVED',
  [TemplateVersionStatus.Superseded]: 'INTERVIEW_TEMPLATES.STATUS.SUPERSEDED',
  [TemplateVersionStatus.Cancelled]: 'INTERVIEW_TEMPLATES.STATUS.CANCELLED',
};

export const TEMPLATE_VERSION_STATUS_PILL: Record<TemplateVersionStatus, 'neutral' | 'warning' | 'danger' | 'success' | 'info'> = {
  [TemplateVersionStatus.Draft]: 'neutral',
  [TemplateVersionStatus.PendingApproval]: 'warning',
  [TemplateVersionStatus.Returned]: 'danger',
  [TemplateVersionStatus.Approved]: 'success',
  [TemplateVersionStatus.Superseded]: 'info',
  [TemplateVersionStatus.Cancelled]: 'danger',
};
