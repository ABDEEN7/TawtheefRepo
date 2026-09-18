import { DatePipe } from '@angular/common';
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
import { LanguageService } from '../../../../../../../core/services/language.service';
import { PaginationMetadata } from '../../../../../../../core/models/pagination-metadata.model';
import { PaginationComponent } from '../../../../../../../shared/components/pagination/pagination.component';
import { RoomListItemDto } from '../../../../rooms-management/models/room-list-item.dto';
import { UserDto } from '../../../../users-management/models/user.dto';
import { UserFilters } from '../../../../users-management/models/user-filters.dto';
import { TestSlotStaffMemberDto } from '../../../models/test-slot-staff-member.dto';
import { TestSlotsService } from '../../../services/test-slots.service';
import { testSlotCreateForm } from '../../helper/test-slot-create.form';
import { TeamAssignmentState } from '../../models/team-assignment-state.model';

@Component({
  selector: 'app-team-assignment',
  standalone: true,
  templateUrl: './team-assignment.component.html',
  styleUrl: './team-assignment.component.scss',
  imports: [DatePipe, FormsModule, TranslatePipe, CheckboxModule, IconFieldModule, InputIconModule, InputTextModule, SelectModule, TableModule, PaginationComponent],
})
export class TeamAssignmentComponent implements OnInit {
  private readonly testSlotsService = inject(TestSlotsService);
  private readonly destroyRef = inject(DestroyRef);
  readonly language = inject(LanguageService);
  private readonly searchChanges$ = new Subject<string>();

  @Input({ required: true }) form!: ReturnType<typeof testSlotCreateForm>;
  @Input() rooms: RoomListItemDto[] = [];
  @Input({ required: true }) state!: TeamAssignmentState;
  @Input() isViewMode = false;
  @Input() showRoomHeadError = false;
  @Output() stateChange = new EventEmitter<TeamAssignmentState>();

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
      if (this.isViewMode) return;
      this.filters.update(filters => ({ ...filters, search: search.trim() || null, pageNumber: 1 }));
      this.loadUsers();
    });
    if (!this.isViewMode) this.loadUsers();
  }

  get room(): RoomListItemDto | undefined { return this.rooms.find(room => room.id === this.form.controls.roomId.value); }
  get roomName(): string { return (this.language.isRtl ? this.room?.nameAr : this.room?.nameEn) ?? '—'; }
  get roomCapacity(): number | string { return this.room?.capacity ?? '—'; }
  get roomHeadOptions(): UserDto[] {
    const users = this.users();
    const roomHead = this.state.roomHead;
    if (this.roomHeadOptionsUsers === users && this.roomHeadOptionsRoomHead === roomHead) {
      return this.cachedRoomHeadOptions;
    }

    this.roomHeadOptionsUsers = users;
    this.roomHeadOptionsRoomHead = roomHead;
    this.cachedRoomHeadOptions = roomHead ? this.mergeUsers([roomHead], users) : users;
    return this.cachedRoomHeadOptions;
  }
  get selectedCount(): number { return this.state.selectedStaff.length; }
  get allVisibleSelected(): boolean {
    const visibleUsers = this.filteredUsers();
    return visibleUsers.length > 0 && visibleUsers.every(user => this.isSelected(user.id));
  }
  isSelected(userId: string): boolean { return this.state.selectedStaff.some(user => user.id === userId); }
  onSearchChange(search: string): void { if (!this.isViewMode) this.searchChanges$.next(search); }
  onRoomHeadFilter(search: string): void { this.onSearchChange(search); }
  onDepartmentChange(department: string | null): void { this.department = department ?? ''; }
  get roomHeadUserId(): string | null { return this.state.roomHead?.id ?? null; }

  setRoomHead(userId: string | null): void {
    if (this.isViewMode) return;
    const user = userId ? this.roomHeadOptions.find(option => option.id === userId) ?? null : null;
    this.emit({
      ...this.state,
      roomHead: user,
      selectedStaff: user
        ? this.state.selectedStaff.filter(staff => staff.id !== user.id)
        : this.state.selectedStaff,
    });
  }
  departmentName(user: TestSlotStaffMemberDto): string { return user.departmentName || '—'; }
  jobTitle(user: TestSlotStaffMemberDto): string { return user.jobTitle || '—'; }
  filteredUsers(): TestSlotStaffMemberDto[] {
    const users = this.users();
    const selectedStaff = this.state.selectedStaff;
    const roomHeadUserId = this.roomHeadUserId;
    if (
      this.filteredUsersSource === users &&
      this.filteredUsersSelectedStaff === selectedStaff &&
      this.filteredUsersRoomHeadId === roomHeadUserId &&
      this.filteredUsersDepartment === this.department
    ) {
      return this.cachedFilteredUsers;
    }

    const usersById = new Map(users.map(user => [user.id, user]));
    selectedStaff.forEach(user => {
      if (!usersById.has(user.id)) {
        usersById.set(user.id, { ...user, jobTitle: null, departmentName: null });
      }
    });
    this.filteredUsersSource = users;
    this.filteredUsersSelectedStaff = selectedStaff;
    this.filteredUsersRoomHeadId = roomHeadUserId;
    this.filteredUsersDepartment = this.department;
    this.cachedFilteredUsers = [...usersById.values()].filter(
      user => user.id !== roomHeadUserId && (!this.department || this.departmentName(user) === this.department),
    );
    return this.cachedFilteredUsers;
  }

  toggleUser(user: UserDto, selected: boolean): void {
    if (this.isViewMode) return;
    const selectedStaff = selected ? this.mergeUsers(this.state.selectedStaff, [user]) : this.state.selectedStaff.filter(staff => staff.id !== user.id);
    this.emit({ ...this.state, selectedStaff });
  }
  toggleVisible(selected: boolean): void {
    if (this.isViewMode) return;
    const visible = this.filteredUsers();
    const visibleIds = new Set(visible.map(user => user.id));
    const selectedStaff = selected ? this.mergeUsers(this.state.selectedStaff, visible) : this.state.selectedStaff.filter(user => !visibleIds.has(user.id));
    this.emit({ ...this.state, selectedStaff });
  }
  clearSelection(): void { if (!this.isViewMode) this.emit({ ...this.state, selectedStaff: [] }); }
  onPageChange(pageNumber: number): void {
    if (this.isViewMode) return;
    this.filters.update(filters => ({ ...filters, pageNumber }));
    this.loadUsers();
  }
  onPageSizeChange(pageSize: number): void {
    if (this.isViewMode) return;
    this.filters.update(filters => ({ ...filters, pageSize, pageNumber: 1 }));
    this.loadUsers();
  }

  private loadUsers(): void {
    if (this.isViewMode) return;
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
  private emit(state: TeamAssignmentState): void { this.stateChange.emit({ ...state, selectedStaff: this.mergeUsers([], state.selectedStaff) }); }
  private mergeUsers(current: UserDto[], additions: UserDto[]): UserDto[] {
    const users = new Map(current.map(user => [user.id, user])); additions.forEach(user => users.set(user.id, user)); return [...users.values()];
  }
}
