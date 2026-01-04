import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {TargetEntityDto} from '../models/target-entity.dto';
import {TargetEntityFilters} from '../models/target-entity-filters.dto';
import {EndpointsService} from '../../../../../core/http/endpoints.service';

@Injectable({providedIn: 'root'})
export class TargetEntitiesService {
  private http = inject(HttpClient);
  private endpoints = inject(EndpointsService);

  getTargetEntities(filters: TargetEntityFilters) {
    return this.http.get<PaginatedResult<TargetEntityDto>>(this.endpoints.targetEntities.listTargetEntities, filters);
  }

  getTargetEntityDetails(id: string) {
    return this.http.get<TargetEntityDto>(this.endpoints.targetEntities.targetEntityDetails(id));
  }

  createTargetEntity(payload: TargetEntityDto) {
    return this.http.post<TargetEntityDto>(this.endpoints.targetEntities.createTargetEntity, payload);
  }

  updateTargetEntity(id: string, payload: TargetEntityDto) {
    return this.http.put<TargetEntityDto>(this.endpoints.targetEntities.updateTargetEntity(id), payload);
  }

  updateStatus(id: string, isActive: boolean) {
    return this.http.put<void>(this.endpoints.targetEntities.updateStatus(id), {isActive});
  }
}
