export interface dropdownOptionsModel {
  id: string;
  backendName: string;
  name: string;
  description: string;
  additionalData?: {
    nameAr?: string;
    nameEn?: string;
    code?: number;
    isoCode?: string;
    codeAlpha?: string;
    [key: string]: unknown;
  };
}
