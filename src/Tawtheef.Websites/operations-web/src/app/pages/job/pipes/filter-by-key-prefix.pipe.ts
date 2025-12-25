import { Pipe, PipeTransform } from '@angular/core';

interface DetailItem {
  key: string;
  label: string;
  id?: string;
}

@Pipe({
  name: 'filterByKeyPrefix',
  standalone: false
})
export class FilterByKeyPrefixPipe implements PipeTransform {
  transform(items: DetailItem[], prefix: string): DetailItem[] {
    if (!items || !prefix) return items;
    return items.filter(item => item.key.startsWith(prefix));
  }
}