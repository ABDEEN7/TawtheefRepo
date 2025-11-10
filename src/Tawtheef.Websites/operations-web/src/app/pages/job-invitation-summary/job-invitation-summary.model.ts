export interface Job {
  id: number;
  title: string;
  entity: string;
  type: 'academic' | 'administrative' | 'labor';
  status: 'open' | 'closed';
}

export interface Invite {
  jobId: number;
  profileId: number;
  status: 'new' | 'viewed' | 'declined' | 'applied';
  sentAt: string;
}

export interface Application {
  jobId: number;
  profileId: number;
  appliedAt: string;
  status: string;
}

export interface JobSummary {
  job: Job;
  totalInv: number;
  applied: number;
  declined: number;
  unseen: number;
}

export interface FilterState {
  type: string;
  entity: string;
  status: string;
}
