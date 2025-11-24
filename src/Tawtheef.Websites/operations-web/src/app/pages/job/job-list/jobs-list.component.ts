import { Component, computed, inject, OnInit, signal, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { DynamicDialogRef, DialogService } from 'primeng/dynamicdialog';
import {JobService} from '../services/job.service';
import {Job} from '../models/job.model';
import {PointsConfigModalComponent} from '../modals/points-config-modal/points-config-modal.component';
import { JobLookupService } from '../services/job-lookup.service';

@Component({
  selector: 'app-job-list',
  standalone : false,
  templateUrl: './jobs-list.component.html',
  styleUrl: './jobs-list.component.scss',
})
export class JobListComponent implements OnInit {
  private jobService = inject(JobService);
  private router = inject(Router);
  private dialogService = inject(DialogService);
  lookupsService = inject(JobLookupService)

  jobs = this.jobService.jobs;
  currentPage = signal(1);
  itemsPerPage = 10;

  searchQuery = signal<string>('');
  filterType = signal<string>('');
  filterStatus = signal<string>('');

  filteredJobs = computed(() => {
    const q = this.searchQuery().trim().toLowerCase();
    const fType = this.filterType();
    const fStatus = this.filterStatus();

    return this.jobs().filter((j) => {
      if (fType && j.jobCategoryId !== fType) return false;
      if (fStatus && j.status !== fStatus) return false;
      if (q) {
        const hay = [j.title, j.requestingDepartmentId].join(' ').toLowerCase();
        if (!hay.includes(q)) return false;
      }
      return true;
    });
  });


  pagedJobs = computed(() => {
    const start = (this.currentPage() - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.filteredJobs().slice(start, end);
  });

  ngOnInit() {
    this.jobService.loadJobs();
    this.lookupsService.loadAll();
  }

  onFilterChange() {
    this.currentPage.set(1);
  }

  clearFilters() {
    this.searchQuery.set('');
    this.filterType.set('');
    this.filterStatus.set('');
    this.currentPage.set(1);
  }

  editJob(job: Job) {
    this.router.navigate([`jobs/edit/${job.id}`]).then();
  }

  openPointsModal(job: Job) {
    const ref: DynamicDialogRef | null = this.dialogService.open(PointsConfigModalComponent, {
      data: { jobId: job.id, jobTitle: job.title },
      width: '80%',
    });

    ref?.onClose.subscribe((saved: boolean) => {
      if (saved) {
      }
    });
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
  }
}
