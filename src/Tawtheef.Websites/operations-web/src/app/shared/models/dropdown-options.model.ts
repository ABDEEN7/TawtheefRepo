export interface dropdownOptionsModel {
  id: string;
  backendName: string;
  name: string;
  description: string;
  additionalData?: {
    code?: number;
    isoCode?: string;
    codeAlpha?: string;
  };
}
