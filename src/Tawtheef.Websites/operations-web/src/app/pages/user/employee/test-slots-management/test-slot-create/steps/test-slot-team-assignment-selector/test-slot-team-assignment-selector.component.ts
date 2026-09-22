import { Component, DestroyRef, EventEmitter, inject, Input, OnInit, Output, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CheckboxModule } from 'primeng/checkbox';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { PaginationMetadata } from '../../../../../../../core/models/pagination-metadata.model';
import { PaginationComponent } from '../../../../../../../shared/components/pagination/pagination.component';
import { UserDto } from '../../../../users-management/models/user.dto';
import { UserFilters } from '../../../../users-management/models/user-filters.dto';
import { TestSlotStaffMemberDto } from '../../../models/test-slot-staff-member.dto';
import { TestSlotsService } from '../../../services/test-slots.service';

export interface TestSlotTeamAssignmentSelection {
  roomHead: UserDto | null;
  selectedStaff: UserDto[];
}

@Component({
  selector: 'app-test-slot-team-assignment-selector',
  standalone: true,
  templateUrl: './test-slot-team-assignment-selector.component.html',
  styleUrl: './test-slot-team-assignment-selector.component.scss',
  imports: [FormsModule, TranslatePipe, CheckboxModule, IconFieldModule, InputIconModule, InputTextModule, SelectModule, TableModule, PaginationComponent],
})
export class TestSlotTeamAssignmentSelectorComponent implements OnInit {
  private readonly testSlotsService = inject(TestSlotsService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly searchChanges$ = new Subject<string>();
  private hasExternalStaff = false;

  @Input() selectedRoomHead: UserDto | null = null;
  @Input() selectedTeamMembers: UserDto[] = [];
  @Input() isViewMode = false;
  @Input() showRoomHeadError = false;
  @Input()
  set availableStaff(staff: TestSlotStaffMemberDto[] | null | undefined) {
    this.hasExternalStaff = staff != null;
    if (staff != null) {
      this.users.set(staff);
      this.metadata.set(null);
      this.departments.set([...new Set(staff.map(user => user.departmentName).filter((name): name is string => !!name))]);
    }
  }
  @Output() assignmentChange = new EventEmitter<TestSlotTeamAssignmentSelection>();

  readonly users = signal<TestSlotStaffMemberDto[]>([]);
  readonly metadata = signal<PaginationMetadata | null>(null);
  readonly loading = signal(false);
  readonly filters = signal<UserFilters>({ pageNumber: 1, pageSize: 10, search: null, isBlocked: false });
  department = '';
  readonly departments = signal<string[]>([]);
  private roomHeadOptionsUsers: TestSlotStaffMemberDto[] | null = null;
  private roomHeadOptionsRoomHead: UserDto | null | undefined;
  private cachedRoomHeadOptions: UserDto[] = [];
  private filteredUsersSource: TestSlotStaffMemberDto[] | null = null;
  private filteredUsersSelectedStaff: UserDto[] | null = null;
  private filteredUsersRoomHeadId: string | null | undefined;
  private filteredUsersDepartment: string | null = null;
  private cachedFilteredUsers: TestSlotStaffMemberDto[] = [];

  ngOnInit(): void {
    this.searchChanges$.pipe(debounceTime(350), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef)).subscribe(search => {
      if (this.isViewMode || this.hasExternalStaff) return;
      this.filters.update(filters => ({ ...filters, search: search.trim() || null, pageNumber: 1 }));
      this.loadUsers();
    });
    if (!this.isViewMode && !this.hasExternalStaff) this.loadUsers();
  }

  get roomHeadOptions(): UserDto[] {
    const users = this.users();
    if (this.roomHeadOptionsUsers === users && this.roomHeadOptionsRoomHead === this.selectedRoomHead) return this.cachedRoomHeadOptions;
    this.roomHeadOptionsUsers = users;
    this.roomHeadOptionsRoomHead = this.selectedRoomHead;
    this.cachedRoomHeadOptions = this.selectedRoomHead ? this.mergeUsers([this.selectedRoomHead], users) : users;
    return this.cachedRoomHeadOptions;
  }
  get selectedCount(): number { return this.selectedTeamMembers.length; }
  get allVisibleSelected(): boolean {
    const visibleUsers = this.filteredUsers();
    return visibleUsers.length > 0 && visibleUsers.every(user => this.isSelected(user.id));
  }
  get roomHeadUserId(): string | null { return this.selectedRoomHead?.id ?? null; }
  isSelected(userId: string): boolean { return this.selectedTeamMembers.some(user => user.id === userId); }
  onSearchChange(search: string): void { if (!this.isViewMode && !this.hasExternalStaff) this.searchChanges$.next(search); }
  onRoomHeadFilter(search: string): void { this.onSearchChange(search); }
  onDepartmentChange(department: string | null): void { this.department = department ?? ''; }

  setRoomHead(userId: string | null): void {
    if (this.isViewMode) return;
    const user = userId ? this.roomHeadOptions.find(option => option.id === userId) ?? null : null;
    this.emitAssignment(user, user ? this.selectedTeamMembers.filter(staff => staff.id !== user.id) : this.selectedTeamMembers);
  }
  departmentName(user: TestSlotStaffMemberDto): string { return user.departmentName || '—'; }
  jobTitle(user: TestSlotStaffMemberDto): string { return user.jobTitle || '—'; }
  filteredUsers(): TestSlotStaffMemberDto[] {
    const users = this.users();
    if (this.filteredUsersSource === users && this.filteredUsersSelectedStaff === this.selectedTeamMembers && this.filteredUsersRoomHeadId === this.roomHeadUserId && this.filteredUsersDepartment === this.department) return this.cachedFilteredUsers;
    const usersById = new Map(users.map(user => [user.id, user]));
    this.selectedTeamMembers.forEach(user => {
      if (!usersById.has(user.id)) usersById.set(user.id, { ...user, jobTitle: null, departmentName: null });
    });
    this.filteredUsersSource = users;
    this.filteredUsersSelectedStaff = this.selectedTeamMembers;
    this.filteredUsersRoomHeadId = this.roomHeadUserId;
    this.filteredUsersDepartment = this.department;
    this.cachedFilteredUsers = [...usersById.values()].filter(user => user.id !== this.roomHeadUserId && (!this.department || this.departmentName(user) === this.department));
    return this.cachedFilteredUsers;
  }
  toggleUser(user: UserDto, selected: boolean): void {
    if (!this.isViewMode) {
      this.emitAssignment(
        this.selectedRoomHead,
        selected
          ? this.mergeUsers(this.selectedTeamMembers, [user])
          : this.selectedTeamMembers.filter(staff => staff.id !== user.id),
      );
    }
  }
  toggleVisible(selected: boolean): void {
    if (this.isViewMode) return;
    const visible = this.filteredUsers();
    const visibleIds = new Set(visible.map(user => user.id));
    this.emitAssignment(
      this.selectedRoomHead,
      selected
        ? this.mergeUsers(this.selectedTeamMembers, visible)
        : this.selectedTeamMembers.filter(user => !visibleIds.has(user.id)),
    );
  }
  clearSelection(): void {
    if (!this.isViewMode) this.emitAssignment(this.selectedRoomHead, []);
  }
  onPageChange(pageNumber: number): void {
    if (this.isViewMode || this.hasExternalStaff) return;
    this.filters.update(filters => ({ ...filters, pageNumber }));
    this.loadUsers();
  }
  onPageSizeChange(pageSize: number): void {
    if (this.isViewMode || this.hasExternalStaff) return;
    this.filters.update(filters => ({ ...filters, pageSize, pageNumber: 1 }));
    this.loadUsers();
  }

  private loadUsers(): void {
    this.loading.set(true);
    this.testSlotsService.getStaffMembers(this.filters()).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: response => {
        this.users.set(response.items);
        this.metadata.set(response.metadata);
        this.departments.update(existing => [...new Set([...existing, ...response.items.map(user => user.departmentName).filter((name): name is string => !!name)])]);
        this.loading.set(false);
      },
      error: () => { this.users.set([]); this.loading.set(false); },
    });
  }
  private mergeUsers(current: UserDto[], additions: UserDto[]): UserDto[] {
    const users = new Map(current.map(user => [user.id, user]));
    additions.forEach(user => users.set(user.id, user));
    return [...users.values()];
  }

  private emitAssignment(roomHead: UserDto | null, selectedStaff: UserDto[]): void {
    this.assignmentChange.emit({ roomHead, selectedStaff: this.mergeUsers([], selectedStaff) });
  }
}
