import { ExamConfigurationDto } from './exam-configuration.dto';

export interface ExamJobSelectionDto {
  hasPendingApprovalExam: boolean;
  approvedExam: ExamConfigurationDto | null;
}
