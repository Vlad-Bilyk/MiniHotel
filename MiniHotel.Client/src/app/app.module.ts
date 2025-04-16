import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './shared/navbar/navbar.component';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';
import { RoomsComponent } from './pages/admin/rooms/rooms.component';
import { BookingSearchComponent } from './pages/booking-search/booking-search.component';
import { RoomCardComponent } from './shared/room-card/room-card.component';
import { BookingConfirmationComponent } from './pages/booking-confirmation/booking-confirmation.component';
import { NightsLabelPipe } from './shared/pipes/labelPipes/nights-label.pipe';
import { RoomLabelPipe } from './shared/pipes/labelPipes/room-label.pipe';
import { MyBookingsComponent } from './pages/my-bookings/my-bookings.component';
import { RoomTypesComponent } from './pages/admin/room-types/room-types.component';
import { RoomTypeDialogComponent } from './pages/admin/room-types/room-type-dialog/room-type-dialog.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { RoomFormDialogComponent } from './pages/admin/rooms/room-form-dialog/room-form-dialog.component';
import { MatSelectModule } from '@angular/material/select';
import { ServicesComponent } from './pages/admin/services/services.component';
import { ServicesFormDialogComponent } from './pages/admin/services/services-form-dialog/services-form-dialog.component'
import { MatTooltipModule } from '@angular/material/tooltip';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    HomeComponent,
    LoginComponent,
    RegisterComponent,
    RoomsComponent,
    BookingSearchComponent,
    RoomCardComponent,
    BookingConfirmationComponent,
    NightsLabelPipe,
    RoomLabelPipe,
    MyBookingsComponent,
    RoomTypesComponent,
    RoomTypeDialogComponent,
    RoomFormDialogComponent,
    ServicesComponent,
    ServicesFormDialogComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatTooltipModule,
  ],
  providers: [provideHttpClient(withInterceptors([jwtInterceptor]))],
  bootstrap: [AppComponent],
})
export class AppModule { }
