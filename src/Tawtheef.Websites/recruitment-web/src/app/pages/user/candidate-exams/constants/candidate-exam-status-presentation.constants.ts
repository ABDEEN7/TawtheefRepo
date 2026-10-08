import { CandidateExamEntryState } from '../models/candidate-exam.model';
import { CandidateExamStatusPresentation } from '../models/candidate-exam-status-presentation.model';

export const CANDIDATE_EXAM_ENTRY_STATE_PRESENTATION: Record<
  CandidateExamEntryState,
  CandidateExamStatusPresentation
> = {
  Assigned: { labelKey: 'candidate_exams.states.assigned', tone: 'neutral' },
  NotEligible: {
    labelKey: 'candidate_exams.states.notEligible',
    tone: 'neutral',
  },
  WaitingForSession: {
    labelKey: 'candidate_exams.states.waitingForSession',
    tone: 'info',
  },
  WaitingForAttendance: {
    labelKey: 'candidate_exams.states.waitingForAttendance',
    tone: 'warning',
  },
  EntryWindowClosed: {
    labelKey: 'candidate_exams.states.entryWindowClosed',
    tone: 'neutral',
  },
  WaitingForIdentityVerification: {
    labelKey: 'candidate_exams.states.waitingForIdentityVerification',
    tone: 'warning',
  },
  WaitingForAuthorization: {
    labelKey: 'candidate_exams.states.waitingForAuthorization',
    tone: 'info',
  },
  ReadyToEnter: { labelKey: 'candidate_exams.states.readyToEnter', tone: 'success' },
  Started: { labelKey: 'candidate_exams.states.inProgress', tone: 'info' },
  NoShow: { labelKey: 'candidate_exams.states.noShow', tone: 'danger' },
  Completed: { labelKey: 'candidate_exams.states.completed', tone: 'success' },
  Rescheduled: { labelKey: 'candidate_exams.states.rescheduled', tone: 'warning' },
  Cancelled: { labelKey: 'candidate_exams.states.cancelled', tone: 'danger' },
  Closed: { labelKey: 'candidate_exams.states.closed', tone: 'neutral' },
};
