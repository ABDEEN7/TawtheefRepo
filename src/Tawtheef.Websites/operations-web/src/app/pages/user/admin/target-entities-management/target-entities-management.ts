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
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';
import {PaginatedResult} from '../../../../core/models/paginated-result.model';
import {TargetEntitiesService} from './services/target-entities.service';
import {TargetEntityDto} from './models/target-entity.dto';
import {TargetEntityFilters} from './models/target-entity-filters.dto';
import {TargetEntityModalComponent} from './components/target-entity-modal/target-entity-modal.component';
import {finalize} from 'rxjs/operators';

@Component({
  selector: 'app-target-entities-management',
  standalone: true,
  templateUrl: './target-entities-management.html',
  styleUrls: ['./target-entities-management.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    PaginationComponent,
    I18nNamespaceDirective,
    ToggleSwitchModule,
    Tooltip,
    TableModule,
    TargetEntityModalComponent
  ]
})
export class TargetEntitiesManagement implements OnInit {
  private targetEntitiesService = inject(TargetEntitiesService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  private _targetEntities = signal<TargetEntityDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  public targetEntities = this._targetEntities.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  filters = signal<TargetEntityFilters>({
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
  editingTargetEntity = signal<TargetEntityDto | null>(null);

  ngOnInit(): void {
    this.loadTargetEntities();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadTargetEntities() {
    this.targetEntitiesService.getTargetEntities(this.filters()).subscribe({
      next: (response: PaginatedResult<TargetEntityDto>) => {
        this._targetEntities.set(response.items || []);
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
    this.loadTargetEntities();
  }

  onPageChange(page: number) {
    this.filters.update(f => ({...f, pageNumber: page}));
    this.loadTargetEntities();
  }

  onPageSizeChange(size: number) {
    this.filters.update(f => ({
      ...f,
      pageNumber: 1,
      pageSize: size
    }));
    this.loadTargetEntities();
  }

  openAdd() {
    this.modalMode.set('create');
    this.editingTargetEntity.set(null);
    this.isModalLoading.set(false);
    this.isModalOpen.set(true);
  }

  openEdit(targetEntity: TargetEntityDto) {
    if (!targetEntity.id) {
      return;
    }
    this.modalMode.set('edit');
    this.fetchTargetEntityDetails(targetEntity.id);
  }

  private fetchTargetEntityDetails(id: string) {
    this.isModalLoading.set(true);
    this.targetEntitiesService
      .getTargetEntityDetails(id)
      .pipe(finalize(() => this.isModalLoading.set(false)))
      .subscribe({
        next: entity => {
          this.editingTargetEntity.set(entity);
          this.isModalOpen.set(true);
        }
      });
  }

  toggleStatus(targetEntity: TargetEntityDto) {
    if (!targetEntity.id) {
      return;
    }
    const desiredState = !targetEntity.isActive;
    this.targetEntitiesService.updateStatus(targetEntity.id, desiredState).subscribe({
      next: () => {
        this._targetEntities.update(items =>
          items.map(t => (t.id === targetEntity.id ? {...t, isActive: desiredState} : t))
        );
        this.notification.success(
          this.translate.instant(desiredState ? 'TARGET_ENTITIES.ACTIVATE_SUCCESS' : 'TARGET_ENTITIES.DEACTIVATE_SUCCESS')
        );
      }
    });
  }

  createTargetEntity(payload: TargetEntityDto) {
    this.targetEntitiesService.createTargetEntity(payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('TARGET_ENTITIES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadTargetEntities();
      }
    });
  }

  updateTargetEntity(payload: { id: string; payload: TargetEntityDto }) {
    this.targetEntitiesService.updateTargetEntity(payload.id, payload.payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('TARGET_ENTITIES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadTargetEntities();
      }
    });
  }

  closeModal() {
    this.isModalOpen.set(false);
    this.modalMode.set('create');
    this.editingTargetEntity.set(null);
    this.isModalLoading.set(false);
  }
}
