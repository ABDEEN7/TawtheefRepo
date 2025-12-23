import { Routes } from '@angular/router';

export const profileEditShellRoutes: Routes = [
  { path: 'prerequisites', loadComponent: () =>  import('./profile-edit-shell/profile-edit-shell.page').then(m => m.ProfileEditShellPage), data: { section: 'prerequisites' } },
  { path: 'personal',       loadComponent: () => import('./profile-edit-shell/profile-edit-shell.page').then(m => m.ProfileEditShellPage), data: { section: 'personal' } },
  { path: 'contact',        loadComponent: () => import('./profile-edit-shell/profile-edit-shell.page').then(m => m.ProfileEditShellPage), data: { section: 'contact' } },
  { path: 'qualifications', loadComponent: () => import('./profile-edit-shell/profile-edit-shell.page').then(m => m.ProfileEditShellPage), data: { section: 'qualifications' } },
  { path: 'experience',     loadComponent: () => import('./profile-edit-shell/profile-edit-shell.page').then(m => m.ProfileEditShellPage), data: { section: 'experience' } },
  { path: 'training',       loadComponent: () => import('./profile-edit-shell/profile-edit-shell.page').then(m => m.ProfileEditShellPage), data: { section: 'training' } },
  { path: 'achievements',   loadComponent: () => import('./profile-edit-shell/profile-edit-shell.page').then(m => m.ProfileEditShellPage), data: { section: 'achievements' } },
  { path: 'skills',         loadComponent: () => import('./profile-edit-shell/profile-edit-shell.page').then(m => m.ProfileEditShellPage), data: { section: 'skills' } },
  { path: 'languages',      loadComponent: () => import('./profile-edit-shell/profile-edit-shell.page').then(m => m.ProfileEditShellPage), data: { section: 'languages' } },
  { path: 'attachments',    loadComponent: () => import('./profile-edit-shell/profile-edit-shell.page').then(m => m.ProfileEditShellPage), data: { section: 'attachments' } },
];
