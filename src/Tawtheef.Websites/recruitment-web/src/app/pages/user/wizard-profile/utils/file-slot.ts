import { UploadedFileRef } from '../models/profile-state.model';

export interface FileSlot {
  local: File | null;
  remote: UploadedFileRef | null | undefined;
}

export function createFileSlot(remote?: UploadedFileRef | null): FileSlot {
  return {
    local: null,
    remote: remote ?? null,
  };
}

export function setLocalFile(slot: FileSlot, file: File | null): void {
  slot.local = file;
  if (file) {
    slot.remote = null;
  }
}

export function updateRemote(slot: FileSlot, remote: UploadedFileRef | null | undefined): void {
  slot.remote = remote ?? null;
}

export function fileSlotSignature(slot: FileSlot) {
  return {
    localName: slot.local?.name ?? null,
    resourceId: slot.remote?.resourceId ?? null,
    resourceName: slot.remote?.resourceName ?? null,
  };
}

export function fileToUpload(slot: FileSlot): File | null {
  return slot.local;
}

export function canPreviewFile(slot: FileSlot): boolean {
  return !!slot.local || !!slot.remote;
}

export function previewFileFromSlot(slot: FileSlot): File | null {
  return slot.local ?? slot.remote?.file ?? null;
}

export function previewUrlFromSlot(slot: FileSlot): string | null {
  return slot.remote?.url ?? null;
}

export function displayedFileName(slot: FileSlot, fallback?: string | null): string {
  return slot.local?.name ?? slot.remote?.resourceName ?? fallback ?? '';
}
