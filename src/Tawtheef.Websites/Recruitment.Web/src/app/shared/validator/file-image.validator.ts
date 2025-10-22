import {AbstractControl} from "@angular/forms";

export function imageValidator(required: boolean = true) {
  return (control: AbstractControl) => {
    const file = control.value;
    if (!file) {
      return required ? { required: true } : null;
    }

    if (typeof file === 'string') {
      return null;
    }

    const validTypes = ['image/jpeg', 'image/png'];
    if (file.type && !validTypes.includes(file.type)) {
      return { fileType: true };
    }

    if (file.size && file.size > 2 * 1024 * 1024) {
      return { fileSize: true };
    }

    return null;
  };
}
