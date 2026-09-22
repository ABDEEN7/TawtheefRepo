import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpService } from '../../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../../core/http/endpoints.service';
import { PaginatedResult } from '../../../../../../core/models/paginated-result.model';
import { PaginatedRequest } from '../../../../../../core/models/paginated-request.model';
import { GUID } from '../../../../../../shared/types/guid.type';

import { JobResponse } from '../../../job-management/models/job-response-model';
import { JobQueryFilter } from '../../../job-management/models/job-query-filter.model';

import { AppointmentModel } from '../models/appointment.model';
import { InterviewType, ScheduleStatus } from '../models/enums';
import { SchedulePlanPreviewModel } from '../models/plan-preview.model';
import { CreationContextModel, RoomOptionModel, ScheduleListItemModel, ScheduleModel } from '../models/schedule.model';

// The pickers are search-as-you-type: fired on every (debounced) keystroke, so they must not flip the global
// loading overlay each time.
const SKIP_LOADING = { headers: { 'X-Skip-Loading': 'true' } };

// Job search fetches one over-sized page; jobs are narrowed further by typing. Server caps PageSize at 50.
const JOB_SEARCH_PAGE_SIZE = 50;

export interface PeriodInputPayload {
  date: string;
  startTime: string;
  endTime: string;
  roomId?: string | null;
  remoteMeetingUrl?: string | null;
  remoteMeetingInstructions?: string | null;
}

export interface ManualAssignmentPayload {
  slotStartAt: string;
  invitationId: string;
}

export interface CreateSchedulePayload {
  jobId: string;
  interviewType: InterviewType;
  titleAr: string;
  titleEn: string | null;
  durationMinutes: number;
  bufferMinutes: number;
  periods: PeriodInputPayload[];
  manualAssignments: ManualAssignmentPayload[] | null;
}

export interface UpdateSchedulePayload {
  id: string;
  interviewType: InterviewType;
  titleAr: string;
  titleEn: string | null;
  durationMinutes: number;
  bufferMinutes: number;
  periods: PeriodInputPayload[];
  manualAssignments: ManualAssignmentPayload[] | null;
}

export interface PreviewSchedulePayload {
  jobId: string;
  interviewType: InterviewType;
  durationMinutes: number;
  bufferMinutes: number;
  periods: PeriodInputPayload[];
  manualAssignments: ManualAssignmentPayload[] | null;
  excludeScheduleId: string | null;
}

export interface RescheduleAppointmentPayload {
  appointmentId: string;
  newStartAt: string;
  newEndAt: string;
  roomId?: string | null;
  remoteMeetingUrl?: string | null;
  remoteMeetingInstructions?: string | null;
  reason: string;
}

@Injectable({ providedIn: 'root' })
export class InterviewScheduleService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  // ---- Lookups ----
  searchPublishedJobs(statusId: GUID, search: string): Observable<PaginatedResult<JobResponse>> {
    const pagination: PaginatedRequest = {
      pageNumber: 1,
      pageSize: JOB_SEARCH_PAGE_SIZE,
      sortBy: 'createdDate',
      sortDirection: 'desc',
    };
    const filter: JobQueryFilter = { statusId, searchTerm: search || undefined };

    return this.http.post<PaginatedResult<JobResponse>>(
      this.endpoints.job.searchJob,
      { pagination, filter },
      undefined,
      SKIP_LOADING,
    );
  }

  searchRooms(search: string): Observable<RoomOptionModel[]> {
    return this.http.get<RoomOptionModel[]>(this.endpoints.interviewSchedule.lookupRooms, { search: search || undefined }, SKIP_LOADING);
  }

  // ---- Schedules ----
  listSchedules(status?: ScheduleStatus | null): Observable<ScheduleListItemModel[]> {
    return this.http.get<ScheduleListItemModel[]>(this.endpoints.interviewSchedule.list, {
      status: status ?? undefined,
    });
  }

  getSchedule(id: string): Observable<ScheduleModel> {
    return this.http.get<ScheduleModel>(this.endpoints.interviewSchedule.details(id));
  }

  // excludeScheduleId is passed by the edit wizard so the schedule's own candidates still count as eligible.
  getCreationContext(jobId: string, excludeScheduleId?: string): Observable<CreationContextModel> {
    return this.http.get<CreationContextModel>(this.endpoints.interviewSchedule.creationContext, {
      jobId,
      excludeScheduleId: excludeScheduleId ?? undefined,
    });
  }

  previewSlots(payload: PreviewSchedulePayload): Observable<SchedulePlanPreviewModel> {
    return this.http.post<SchedulePlanPreviewModel>(this.endpoints.interviewSchedule.preview, payload, undefined, SKIP_LOADING);
  }

  listAppointments(interviewScheduleId: string): Observable<AppointmentModel[]> {
    return this.http.get<AppointmentModel[]>(this.endpoints.interviewSchedule.appointments, { interviewScheduleId });
  }

  createSchedule(payload: CreateSchedulePayload): Observable<string> {
    return this.http.post<string>(this.endpoints.interviewSchedule.create, payload);
  }

  updateSchedule(payload: UpdateSchedulePayload): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewSchedule.update, payload);
  }

  submitSchedule(id: string): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewSchedule.submit, { id });
  }

  approveSchedule(id: string, decisionNotes: string | null): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewSchedule.approve, { id, decisionNotes });
  }

  returnSchedule(id: string, reason: string): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewSchedule.return, { id, reason });
  }

  cancelSchedule(id: string, reason: string): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewSchedule.cancel, { id, reason });
  }

  // ---- Appointments ----
  rescheduleAppointment(payload: RescheduleAppointmentPayload): Observable<string> {
    return this.http.put<string>(this.endpoints.interviewSchedule.appointmentReschedule, payload);
  }

  sendAppointmentNotification(appointmentId: string): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewSchedule.appointmentSendNotification, { appointmentId });
  }
}
