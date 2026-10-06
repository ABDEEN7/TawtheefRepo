import { TestSlotTeamAssignmentSelection } from '../../../../test-slot-create/models/test-slot-team-assignment-selection.model';

export interface TestSlotAssignmentDialogData {
  testSlotId?: string;
  localMode?: boolean;
  initialAssignment?: TestSlotTeamAssignmentSelection;
}
