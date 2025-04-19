import { Component } from '@angular/core';
import { AuthServiceWrapper } from '../../auth/services/auth.service';
import { Router } from '@angular/router';
import { UserRole } from '../../api/models';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  public UserRole = UserRole;

  constructor(
    public authService: AuthServiceWrapper,
    private router: Router) { }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['auth/login']);
  }
}
