export interface LookupItemModel {
  id: string;
  nameAr: string;
  nameEn?: string | null;
}

export interface TemplateLookupsModel {
  organizationScopes: LookupItemModel[];
  jobTitles: LookupItemModel[];
  departments: LookupItemModel[];
}
