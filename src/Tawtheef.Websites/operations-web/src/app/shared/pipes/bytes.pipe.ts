import {Pipe, PipeTransform} from '@angular/core';

@Pipe({ name: 'bytes' })
export class BytesPipe implements PipeTransform {
  transform(value?: number, digits = 1): string {
    if (value == null || isNaN(value)) return '';
    const units = ['B','KB','MB','GB','TB'];
    let i = 0, n = value;
    while (n >= 1024 && i < units.length - 1) { n /= 1024; i++; }
    return n.toFixed(digits) + ' ' + units[i];
  }
}
