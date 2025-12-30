import {
  Directive,
  Input,
  TemplateRef,
  ViewContainerRef
} from '@angular/core';
import {AuthService} from '../../core/auth/auth.service';

interface HasPermissionContext {
  $implicit: boolean;
  hasPermission: boolean;
}

@Directive({
  selector: '[hasPermission]'
})
export class HasPermissionDirective {
  private permissions: string[] = [];
  private requireAll = false;
  private isViewVisible = false;

  constructor(
    private templateRef: TemplateRef<HasPermissionContext>,
    private viewContainer: ViewContainerRef,
    private authService: AuthService
  ) {}

  /**
   * Main input
   * *hasPermission="'jobs.manage'"
   * *hasPermission="['jobs.manage','users.manage']"
   * *hasPermission="['jobs.manage','users.manage']; hasPermissionRequireAll: true">
   */
  @Input()
  set hasPermission(value: string | string[] | null) {
    if (!value) return;
    this.permissions = Array.isArray(value) ? value : [value];
    this.updateView();
  }

  /**
   * Optional:
   * *hasPermission="['jobs.manage','users.manage']; requireAll: true"
   */
  @Input('hasPermissionRequireAll')
  set hasPermissionRequireAll(value: boolean) {
    this.requireAll = value;
    this.updateView();
  }

  private updateView(): void {
    const has = this.authService.hasPermission(this.permissions, this.requireAll);

    if (has && !this.isViewVisible) {
      this.viewContainer.clear();
      this.viewContainer.createEmbeddedView(this.templateRef, {
        $implicit: true,
        hasPermission: true
      });
      this.isViewVisible = true;
    } else if (!has && this.isViewVisible) {
      this.viewContainer.clear();
      this.isViewVisible = false;
    }
  }
}
