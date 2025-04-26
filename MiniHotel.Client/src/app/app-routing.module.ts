import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { RegisterComponent } from './auth/register/register.component';
import { LoginComponent } from './auth/login/login.component';
import { authGuard } from './core/guards/auth.guard';
import { RoomsComponent } from './pages/admin/rooms/rooms.component';
import { BookingConfirmationComponent } from './pages/booking-confirmation/booking-confirmation.component';
import { MyBookingsComponent } from './pages/my-bookings/my-bookings.component';
import { RoomTypesComponent } from './pages/admin/room-types/room-types.component';
import { ServicesComponent } from './pages/admin/services/services.component';
import { BookingsComponent } from './pages/admin/bookings/bookings.component';
import { BookingDetailsComponent } from './pages/admin/bookings/booking-details/booking-details.component';
import { UsersComponent } from './pages/admin/users/users.component';
import { roleGuard } from './core/guards/role.guard';
import { UserRole } from './api/models';
import { RoomCategoryCardsComponent } from './pages/room-category-cards/room-category-cards.component';
import { BookingSearchComponent } from './pages/booking-search/booking-search.component';
import { ContactPageComponent } from './pages/contact-page/contact-page.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'auth/register', component: RegisterComponent },
  { path: 'auth/login', component: LoginComponent },
  { path: 'room-categories', component: RoomCategoryCardsComponent },
  { path: 'room-categories/:roomTypeId', component: RoomCategoryCardsComponent },
  { path: 'booking-search', component: BookingSearchComponent },
  { path: 'contacts', component: ContactPageComponent },
  { path: 'booking-confirmation', component: BookingConfirmationComponent, canActivate: [authGuard, roleGuard([UserRole.Client])] },
  { path: 'my-bookings', component: MyBookingsComponent, canActivate: [authGuard, roleGuard([UserRole.Client])] },
  { path: 'admin/room-types', component: RoomTypesComponent, canActivate: [authGuard, roleGuard([UserRole.Manager])] },
  { path: 'admin/rooms', component: RoomsComponent, canActivate: [authGuard, roleGuard([UserRole.Manager])] },
  { path: 'admin/services', component: ServicesComponent, canActivate: [authGuard, roleGuard([UserRole.Manager])] },
  { path: 'admin/bookings', component: BookingsComponent, canActivate: [authGuard, roleGuard([UserRole.Manager, UserRole.Receptionist])] },
  { path: 'admin/booking-details/:id', component: BookingDetailsComponent, canActivate: [authGuard, roleGuard([UserRole.Manager, UserRole.Receptionist])] },
  { path: 'admin/users', component: UsersComponent, canActivate: [authGuard, roleGuard([UserRole.Manager])] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
