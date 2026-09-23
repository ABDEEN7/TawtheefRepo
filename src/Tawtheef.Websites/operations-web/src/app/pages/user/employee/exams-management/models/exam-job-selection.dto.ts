import { ExamConfigurationDto } from './exam-configuration.dto';

export interface ExamJobSelectionDto {
  approvedExam: ExamConfigurationDto | null;
}
