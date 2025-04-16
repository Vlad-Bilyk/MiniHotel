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

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'booking-confirmation', component: BookingConfirmationComponent },
  { path: 'my-bookings', component: MyBookingsComponent, canActivate: [authGuard] },
  { path: 'room-types', component: RoomTypesComponent, canActivate: [authGuard] },
  { path: 'rooms', component: RoomsComponent, canActivate: [authGuard] },
  { path: 'services', component: ServicesComponent, canActivate: [authGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
