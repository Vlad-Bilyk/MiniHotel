import { Directive, Input, OnDestroy, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { UserRole } from '../../api/models';
import { AuthServiceWrapper } from '../../auth/services/auth.service';
import { Subscription } from 'rxjs';

@Directive({
  selector: '[appHasRole]',
  standalone: false,
})
export class HasRoleDirective implements OnInit, OnDestroy {
  private roles: UserRole[] = [];
  private sub?: Subscription;

  constructor(
    private template: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private authService: AuthServiceWrapper
  ) { }

  @Input() set appHasRole(value: UserRole | UserRole[]) {
    this.roles = Array.isArray(value) ? value : [value];
    this.updateView();
  }

  ngOnInit(): void {
    this.sub = this.authService.userRole$.subscribe(() => {
      this.updateView();
    })
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  private updateView() {
    this.viewContainer.clear();
    if (this.authService.hasAnyRole(this.roles)) {
      this.viewContainer.createEmbeddedView(this.template);
    }
  }
}
