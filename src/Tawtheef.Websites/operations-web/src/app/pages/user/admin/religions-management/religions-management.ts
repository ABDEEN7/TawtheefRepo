import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ToggleSwitchModule} from 'primeng/toggleswitch';
import {Tooltip} from 'primeng/tooltip';
import {TableModule} from 'primeng/table';
import {Lang, LanguageService} from '../../../../core/services/language.service';
import {PaginationComponent} from '../../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {NotificationService} from '../../../../core/services/notification.service';
import {ReligionsService} from './services/religions.service';
import {ReligionDto} from './models/religion.dto';
import {ReligionFilters} from './models/religion-filters.dto';
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';
import {PaginatedResult} from '../../../../core/models/paginated-result.model';
import {ReligionModalComponent} from './components/religion-modal/religion-modal.component';
import {finalize} from 'rxjs/operators';

@Component({
  selector: 'app-religions-management',
  standalone: true,
  templateUrl: './religions-management.html',
  styleUrls: ['./religions-management.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    PaginationComponent,
    I18nNamespaceDirective,
    ToggleSwitchModule,
    Tooltip,
    TableModule,
    ReligionModalComponent
  ]
})
export class ReligionsManagement implements OnInit {
  private religionsService = inject(ReligionsService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  private _religions = signal<ReligionDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  public religions = this._religions.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  filters = signal<ReligionFilters>({
    pageNumber: 1,
    pageSize: 10,
    search: ''
  });

  searchTerm = '';
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  isModalOpen = signal(false);
  modalMode = signal<'create' | 'edit'>('create');
  isModalLoading = signal(false);
  editingReligion = signal<ReligionDto | null>(null);

  ngOnInit(): void {
    this.loadReligions();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadReligions() {
    this.religionsService.getReligions(this.filters()).subscribe({
      next: (response: PaginatedResult<ReligionDto>) => {
        this._religions.set(response.items || []);
        this._paginationMetadata.set(response.metadata);

        if (response.metadata) {
          this.filters.update(f => ({
            ...f,
            pageNumber: response.metadata.currentPage,
            pageSize: response.metadata.pageSize
          }));
        }
      }
    });
  }

  onSearchChange() {
    this.filters.update(f => ({...f, pageNumber: 1, search: this.searchTerm}));
    this.loadReligions();
  }

  onPageChange(page: number) {
    this.filters.update(f => ({...f, pageNumber: page}));
    this.loadReligions();
  }

  onPageSizeChange(size: number) {
    this.filters.update(f => ({
      ...f,
      pageNumber: 1,
      pageSize: size
    }));
    this.loadReligions();
  }

  openAdd() {
    this.modalMode.set('create');
    this.editingReligion.set(null);
    this.isModalLoading.set(false);
    this.isModalOpen.set(true);
  }

  openEdit(religion: ReligionDto) {
    if (!religion.id) {
      return;
    }
    this.modalMode.set('edit');
    this.fetchReligionDetails(religion.id);
  }

  private fetchReligionDetails(id: string) {
    this.isModalLoading.set(true);
    this.religionsService
      .getReligionDetails(id)
      .pipe(finalize(() => this.isModalLoading.set(false)))
      .subscribe({
        next: religion => {
          this.editingReligion.set(religion);
          this.isModalOpen.set(true);
        }
      });
  }

  toggleStatus(religion: ReligionDto) {
    if (!religion.id) {
      return;
    }
    const desiredState = !religion.isActive;
    this.religionsService.updateStatus(religion.id, desiredState).subscribe({
      next: () => {
        this._religions.update(items =>
          items.map(l => (l.id === religion.id ? {...l, isActive: desiredState} : l))
        );
        this.notification.success(
          this.translate.instant(desiredState ? 'RELIGIONS.ACTIVATE_SUCCESS' : 'RELIGIONS.DEACTIVATE_SUCCESS')
        );
      }
    });
  }

  createReligion(payload: ReligionDto) {
    this.religionsService.createReligion(payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('RELIGIONS.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadReligions();
      }
    });
  }

  updateReligion(payload: { id: string; payload: ReligionDto }) {
    this.religionsService.updateReligion(payload.id, payload.payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('RELIGIONS.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadReligions();
      }
    });
  }

  closeModal() {
    this.isModalOpen.set(false);
    this.modalMode.set('create');
    this.editingReligion.set(null);
    this.isModalLoading.set(false);
  }
}
