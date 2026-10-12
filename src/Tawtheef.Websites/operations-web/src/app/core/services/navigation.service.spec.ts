import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { PermissionService } from '../auth/permission.service';
import { Permissions } from '../constants/permissions';
import { routes } from '../../routes/routes';
import { NavigationService } from './navigation.service';

describe('NavigationService grouped menus', () => {
  it('redirects a user with only one child permission to that child page', () => {
    TestBed.configureTestingModule({ providers: [
      { provide: Router, useValue: {} },
      { provide: PermissionService, useValue: {
        hasPermission: (permission: string | string[]) => permission === Permissions.QuestionBankAssignments.Manage,
      } },
    ] });
    expect(TestBed.inject(NavigationService).getRoleBasedRoute('employee')).toEqual([
      routes.portal.questionBankAssignments,
    ]);
  });
});
