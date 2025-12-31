export interface KawaderUploadError {
  rowNumber: number;
  value: string;
  reason: string;
}

export interface KawaderUploadResult {
  importedCount: number;
  processedRows: number;
  errors: KawaderUploadError[];
  success: boolean;
}
