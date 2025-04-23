import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { UserDto, UserRole, UserUpdateDto } from '../../../api/models';
import { UsersService } from '../../../api/services';
import { MatPaginator } from '@angular/material/paginator';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { AuthServiceWrapper } from '../../../auth/services/auth.service';

@Component({
  selector: 'app-users',
  standalone: false,
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss',
})
export class UsersComponent implements OnInit {
  displayedColumns = ['fullName', 'email', 'phoneNumber', 'role', 'actions'];
  dataSource = new MatTableDataSource<UserDto>([]);
  loading = true;
  searchValue = '';
  selectedRole = '';
  roleOptions = Object.values(UserRole);

  @ViewChild(MatPaginator) set MatPaginator(p: MatPaginator) {
    this.dataSource.paginator = p;
  }

  constructor(
    private usersService: UsersService,
    private authService: AuthServiceWrapper,
    private toastr: ToastrService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.dataSource.filterPredicate = (user: UserDto, filter: string) => {
      // Divide the filter into two parts: role | term
      const [role, term] = filter.split('|');

      const matchesRole = !role || user.role === role;

      const search = term?.toLowerCase().trim() ?? '';
      const matchesTerm =
        !search ||
        [
          user.firstName,
          user.lastName,
          user.email,
          user.phoneNumber,
          user.role,
        ].some((f) => f?.toLowerCase().includes(search));

      return matchesRole && matchesTerm;
    };

    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.usersService.getUsers().subscribe({
      next: (users) => {
        this.dataSource.data = users;
        this.loading = false;
      },
      error: (err) => {
        console.log(err);
        this.toastr.error('Не вдалося завантажити коористувачів');
        this.loading = false;
      },
    });
  }

  goToRegister(): void {
    const isManager = this.authService.hasRole(UserRole.Manager);
    this.router.navigate(['/auth/register'], { queryParams: { isManager } })
  }

  // TODO: implement this dialog
  openEditDialog(user: UserUpdateDto) { }

  applyGlobalFilter(value: string) {
    this.searchValue = value;
    this.updateFilter();
  }

  applyRoleFilter(role: string) {
    this.selectedRole = role;
    this.updateFilter();
  }

  private updateFilter() {
    const composite = `${this.selectedRole}|${this.searchValue}`;
    this.dataSource.filter = composite;
  }
}
