import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, catchError, of, map, forkJoin } from 'rxjs';
import { InviteDetails } from '../models/invite-details.model';
import { Invite } from '../models/invite.model';
import { Profile } from '../models/profile.model';
import { Application } from '../models/application.model';
import {JobService} from '../../job/services/job.service';
import {Job} from '../../job/models/job.model';
import { GUID } from '../../../shared/types/guid.type';

@Injectable({
  providedIn: 'root'
})
export class StaffInvitesService {
  private http = inject(HttpClient);
  private jobService = inject(JobService);

  private _invites = signal<InviteDetails[]>([]);
  invites = this._invites.asReadonly();

  getJobDetails(jobId: GUID): Observable<Job> {
    return this.jobService.loadJob(jobId);
  }

 getInvitesForJob(jobId: GUID): Observable<InviteDetails[]> {
    return forkJoin({
      invites: this.http.get<Invite[]>(`/api/invites?jobId=${jobId}`),
      profiles: this.http.get<Profile[]>('/api/profiles'),
      applications: this.http.get<Application[]>(`/api/applications?jobId=${jobId}`)
    }).pipe(
      map(({ invites, profiles, applications }) => {
        // Create a map for quick lookups
        const profileMap = new Map(profiles.map(p => [p.id, p]));
        const applicationMap = new Map(
          applications.map(a => [`${a.jobId}-${a.profileId}`, a])
        );

        // Combine the data
        return invites.map(invite => {
          const profile = profileMap.get(invite.profileId);
          const application = applicationMap.get(`${invite.jobId}-${invite.profileId}`);

          if (!profile) {
            console.warn(`Profile not found for invite ${invite.id}, profileId: ${invite.profileId}`);
          }

          return {
            ...invite,
            profile: profile || {
              id: invite.profileId,
              name: 'Unknown',
              nationality: 'Unknown',
              phone: 'N/A'
            },
            application
          } as InviteDetails;
        });
      }),
      tap(inviteDetails => {
        this._invites.set(inviteDetails);
      }),
      catchError(error => {
        console.error('Error loading invites:', error);
        this._invites.set([]);
        return of([]);
      })
    );
  }


   updateInviteStatus(inviteId: GUID, status: string): Observable<InviteDetails | null> {
    return this.http.patch<Invite>(`/api/invites/${inviteId}`, { status }).pipe(
      map(updatedInvite => {
        const invites = this._invites();
        const existingInvite = invites.find(i => i.id === inviteId);

        if (!existingInvite) {
          console.warn(`Invite ${inviteId} not found in local state`);
          return null;
        }

        // Merge the update with existing invite details
        const updatedInviteDetails: InviteDetails = {
          ...existingInvite,
          ...updatedInvite
        };

        // Update the signal
        const updatedInvites = invites.map(invite =>
          invite.id === inviteId ? updatedInviteDetails : invite
        );
        this._invites.set(updatedInvites);

        return updatedInviteDetails;
      }),
      catchError(error => {
        console.error('Error updating invite status:', error);
        return of(null);
      })
    );
  }

  reset(): void {
    this._invites.set([]);
  }
}
