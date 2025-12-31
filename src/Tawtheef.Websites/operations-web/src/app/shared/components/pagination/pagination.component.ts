import { Component, input, output, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import {TranslatePipe} from '@ngx-translate/core';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule, TranslatePipe, FormsModule],
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss']
})
export class PaginationComponent {
  // Inputs
  currentPage = input.required<number>();
  itemsPerPage = input.required<number>();
  totalItems = input.required<number>();
  showInfo = input(true);
  showControls = input(true);
  translationPrefix = input('app.pagination');

  pageSizeOptions = input<number[]>([10, 20, 50]);

  pageChanged = output<number>();
  pageSizeChanged = output<number>();

  totalPages = computed(() =>
    Math.max(1, Math.ceil(this.totalItems() / this.itemsPerPage()))
  );

  paginationInfo = computed(() => {
    const currentPage = this.currentPage();
    const itemsPerPage = this.itemsPerPage();
    const totalItems = this.totalItems();

    if (totalItems === 0) return { start: 0, end: 0, total: 0 };

    const start = (currentPage - 1) * itemsPerPage + 1;
    const end = Math.min(totalItems, start + itemsPerPage - 1);
    return { start, end, total: totalItems };
  });

  pageNumbers = computed(() => {
    const total = this.totalPages();
    const current = this.currentPage();

    if (total <= 7) {
      return Array.from({ length: total }, (_, i) => i + 1);
    }

    let pages: number[];

    if (current <= 4) {
      pages = [1, 2, 3, 4, 5, -1, total];
    } else if (current >= total - 3) {
      pages = [1, -1, total - 4, total - 3, total - 2, total - 1, total];
    } else {
      pages = [1, -1, current - 1, current, current + 1, -1, total];
    }

    return pages;
  });

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages() && page !== this.currentPage()) {
      this.pageChanged.emit(page);
    }
  }
  changePageSize(size: number) {
    this.pageSizeChanged.emit(size);
  }

  nextPage() {
    const current = this.currentPage();
    const total = this.totalPages();
    if (current < total) {
      this.pageChanged.emit(current + 1);
    }
  }

  previousPage() {
    const current = this.currentPage();
    if (current > 1) {
      this.pageChanged.emit(current - 1);
    }
  }

  isDisabled(direction: 'prev' | 'next'): boolean {
    const current = this.currentPage();
    const total = this.totalPages();
    return direction === 'prev' ? current === 1 : current === total;
  }
}
