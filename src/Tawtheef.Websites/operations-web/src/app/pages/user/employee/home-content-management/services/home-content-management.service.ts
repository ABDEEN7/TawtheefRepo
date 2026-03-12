import {inject, Injectable} from '@angular/core';
import {FAQ, FaqPayload} from '../models/faq.model';
import {HomeSuccessStory, HomeSuccessStoryPayload} from '../models/home-success-story.model';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {HttpService} from '../../../../../core/http/http.service';

@Injectable({providedIn: 'root'})
export class HomeContentManagementService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getSuccessStories() {
    return this.http.get<HomeSuccessStory[]>(this.endpoints.homeContent.successStories);
  }

  getSuccessStory(id: string) {
    return this.http.get<HomeSuccessStory>(this.endpoints.homeContent.successStory(id));
  }

  createSuccessStory(payload: HomeSuccessStoryPayload) {
    const formData = this.buildSuccessStoryFormData(payload);
    return this.http.post<string>(this.endpoints.homeContent.successStories, formData);
  }

  updateSuccessStory(id: string, payload: HomeSuccessStoryPayload) {
    const formData = this.buildSuccessStoryFormData(payload);
    return this.http.put<void>(this.endpoints.homeContent.successStory(id), formData);
  }

  updateSuccessStoryStatus(id: string, isActive: boolean) {
    return this.http.put<void>(this.endpoints.homeContent.successStoryStatus(id), {isActive});
  }

  deleteSuccessStory(id: string) {
    return this.http.delete<void>(this.endpoints.homeContent.successStory(id));
  }

  getFaqs() {
    return this.http.get<FAQ[]>(this.endpoints.homeContent.faqs);
  }

  getFaq(id: string) {
    return this.http.get<FAQ>(this.endpoints.homeContent.faq(id));
  }

  createFaq(payload: FaqPayload) {
    return this.http.post<string>(this.endpoints.homeContent.faqs, payload);
  }

  updateFaq(id: string, payload: FaqPayload) {
    return this.http.put<void>(this.endpoints.homeContent.faq(id), payload);
  }

  updateFaqStatus(id: string, isActive: boolean) {
    return this.http.put<void>(this.endpoints.homeContent.faqStatus(id), {isActive});
  }

  deleteFaq(id: string) {
    return this.http.delete<void>(this.endpoints.homeContent.faq(id));
  }

  private buildSuccessStoryFormData(payload: HomeSuccessStoryPayload): FormData {
    const formData = new FormData();
    const files: File[] = [];

    const imageIndex = payload.imageFile ? files.push(payload.imageFile) - 1 : null;
    files.forEach(file => formData.append('Files', file));

    if (imageIndex !== null) {
      formData.append('ImageFileIndex', imageIndex.toString());
    }

    formData.append('NameAr', payload.nameAr);
    formData.append('NameEn', payload.nameEn);
    formData.append('RoleAr', payload.roleAr);
    formData.append('RoleEn', payload.roleEn);
    formData.append('MetricTitleAr', payload.metricTitleAr);
    formData.append('MetricTitleEn', payload.metricTitleEn);
    formData.append('MetricDescriptionAr', payload.metricDescriptionAr);
    formData.append('MetricDescriptionEn', payload.metricDescriptionEn);
    formData.append('ImageUrl', payload.imageUrl ?? '');
    formData.append('DisplayOrder', payload.displayOrder.toString());
    formData.append('IsActive', payload.isActive.toString());

    return formData;
  }
}
