import { TestBed } from '@angular/core/testing';
import { NavigationEnd, provideRouter, Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { of, Subject } from 'rxjs';
import { LanguageService } from '../../../core/services/language.service';
import { AuthService } from '../../../core/auth/auth.service';
import { Permissions } from '../../../core/constants/permissions';
import { NavigationAuditService } from '../../../core/services/navigation-audit.service';
import { routes } from '../../../routes/routes';
import { isMenuGroup, Sidebar } from '../../admin/sidebar/sidebar.models';
import { SidebarComponent } from './sidebar.component';

describe('Employee sidebar groups', () => {
  let permitted: Set<string>;

  beforeEach(() => {
    permitted = new Set([Permissions.QuestionBankAssignments.Manage]);
    TestBed.configureTestingModule({
      imports: [SidebarComponent, TranslateModule.forRoot()],
      providers: [
        provideRouter([]),
        { provide: LanguageService, useValue: { isRtl: false, isRtl$: of(false) } },
        { provide: AuthService, useValue: {
          hasPermission: (permission: string | string[]) =>
            (Array.isArray(permission) ? permission : [permission]).some(p => permitted.has(p)),
        } },
        { provide: NavigationAuditService, useValue: { logSidebarNavigation: () => {} } },
      ],
    });
  });

  for (const language of ['en', 'ar']) {
    it(`shows only the permitted child and opens its parent on a deep link in ${language}`, async () => {
      TestBed.overrideProvider(LanguageService, { useValue: {
        isRtl: language === 'ar', isRtl$: of(language === 'ar'),
      } });
      const translate = TestBed.inject(TranslateService);
      translate.setTranslation(language, {
        'internal.sidebar.groups.question-banks-group': language === 'ar' ? 'بنوك الأسئلة' : 'Question Banks',
        'internal.sidebar.question-bank-assignments': language === 'ar' ? 'مهامي' : 'My Assignments',
      });
      translate.use(language);
      const fixture = TestBed.createComponent(SidebarComponent);
      const sidebar = fixture.componentInstance;
      fixture.nativeElement.dir = language === 'ar' ? 'rtl' : 'ltr';
      fixture.detectChanges();
      sidebar.highlightActive(`${routes.portal.questionBankAssignmentQuestions('assignment')}?tab=questions#item`);
      fixture.detectChanges();
      await fixture.whenStable();
      const groups = sidebar.filteredMenuItems.filter(isMenuGroup);
      expect(groups.length).toBe(1);
      expect(groups[0].children.map(child => child.key)).toEqual(['question-bank-assignments']);
      const button = fixture.nativeElement.querySelector('.group-toggle') as HTMLButtonElement;
      expect(button.getAttribute('aria-expanded')).toBe('true');
      const links = fixture.nativeElement.querySelectorAll('.group-children a');
      expect(links.length).toBe(1);
      expect(links[0].classList.contains('active')).toBeTrue();
      expect(links[0].getAttribute('href')).toBe(routes.portal.questionBankAssignments);
      expect(button.textContent).toContain(language === 'ar' ? 'بنوك الأسئلة' : 'Question Banks');
      const childStyle = getComputedStyle(fixture.nativeElement.querySelector('.group-children'));
      expect(childStyle.direction).toBe(language === 'ar' ? 'rtl' : 'ltr');
      expect(childStyle.paddingInlineStart).toBe('12px');
      const expectedWidth = innerWidth <= 480 ? '180px' : innerWidth <= 768 ? '200px' : '240px';
      expect(getComputedStyle(fixture.nativeElement.querySelector('aside')).width).toBe(expectedWidth);

      button.click();
      fixture.detectChanges();
      expect(fixture.nativeElement.querySelector('.group-children').hidden).toBeTrue();
      sidebar.highlightActive(routes.portal.questionBankAssignmentQuestions('other'));
      fixture.detectChanges();
      expect(fixture.nativeElement.querySelector('.group-children').hidden).toBeFalse();
      sidebar.toggle();
      fixture.detectChanges();
      expect(fixture.nativeElement.querySelector('.group-children').hidden).toBeTrue();
      button.click();
      fixture.detectChanges();
      expect(sidebar.isCollapsed).toBeFalse();
      expect(fixture.nativeElement.querySelector('.group-children').hidden).toBeFalse();
    });
  }

  it('hides empty groups and searches permitted child labels', () => {
    const sidebar = TestBed.runInInjectionContext(() => new SidebarComponent(TestBed.inject(Router)));
    permitted.clear();
    expect(sidebar.filteredMenuItems).toEqual([]);
    permitted.add(Permissions.QuestionBankAssignments.Manage);
    sidebar.searchTerm = 'question-bank-assignments';
    const groups = sidebar.filteredMenuItems.filter(isMenuGroup);
    expect(groups.length).toBe(1);
    expect(sidebar.groupExpanded(groups[0])).toBeTrue();
    sidebar.searchTerm = 'question-bank-requests';
    expect(sidebar.filteredMenuItems).toEqual([]);
  });

  it('highlights an active route independently of the current search and respects route boundaries', () => {
    const sidebar = TestBed.runInInjectionContext(() => new SidebarComponent(TestBed.inject(Router)));
    sidebar.searchTerm = 'unrelated search';
    sidebar.highlightActive(routes.portal.questionBankAssignmentQuestions('assignment'));
    expect(sidebar.activeItem).toBe('question-bank-assignments');
    sidebar.highlightActive(`${routes.portal.questionBankAssignments}-other`);
    expect(sidebar.activeItem).toBe('');
  });

  it('updates active child and expands the group when navigation completes', () => {
    const events = new Subject<NavigationEnd>();
    const sidebar = TestBed.runInInjectionContext(() => new SidebarComponent({ url: '/', events } as unknown as Router));
    sidebar.ngOnInit();
    events.next(new NavigationEnd(1, '/', routes.portal.questionBankAssignmentQuestions('assignment')));
    expect(sidebar.activeItem).toBe('question-bank-assignments');
    expect(sidebar.expandedGroups.has('question-banks-group')).toBeTrue();
  });

  it('keeps every menu leaf unique and places all existing question-bank pages in one group', () => {
    const leaves = Sidebar.groupedMenuItems.flatMap(item => isMenuGroup(item) ? item.children : [item]);
    expect(new Set(leaves.map(item => item.key)).size).toBe(leaves.length);
    expect(leaves.length).toBe(43);
    const group = Sidebar.groupedMenuItems.find(item => item.key === 'question-banks-group');
    expect(group && isMenuGroup(group) ? group.children.map(child => child.key) : []).toEqual([
      'question-banks', 'question-bank-requests', 'question-bank-assignments',
    ]);
  });
});
