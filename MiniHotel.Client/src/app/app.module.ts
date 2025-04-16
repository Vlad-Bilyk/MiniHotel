import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './shared/navbar/navbar.component';
import { HomeComponent } from './pages/home/home.component';

import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';

import { BookingSearchComponent } from './pages/booking-search/booking-search.component';
import { BookingConfirmationComponent } from './pages/booking-confirmation/booking-confirmation.component';
import { MyBookingsComponent } from './pages/my-bookings/my-bookings.component';

import { RoomsComponent } from './pages/admin/rooms/rooms.component';
import { RoomCardComponent } from './shared/room-card/room-card.component';
import { RoomFormDialogComponent } from './pages/admin/rooms/room-form-dialog/room-form-dialog.component';

import { RoomTypesComponent } from './pages/admin/room-types/room-types.component';
import { RoomTypeDialogComponent } from './pages/admin/room-types/room-type-dialog/room-type-dialog.component';

import { ServicesComponent } from './pages/admin/services/services.component';
import { ServicesFormDialogComponent } from './pages/admin/services/services-form-dialog/services-form-dialog.component';

import { BookingsComponent } from './pages/admin/bookings/bookings.component';
import { BookingsOfflineDialogComponent } from './pages/admin/bookings/bookings-offline-dialog/bookings-offline-dialog.component';

import { NightsLabelPipe } from './shared/pipes/labelPipes/nights-label.pipe';
import { RoomLabelPipe } from './shared/pipes/labelPipes/room-label.pipe';

import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';
import { ToastrModule } from 'ngx-toastr';

/* Angular Material modules */
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';

const MATERIAL_MODULES = [
  MatButtonModule,
  MatChipsModule,
  MatDatepickerModule,
  MatDialogModule,
  MatFormFieldModule,
  MatInputModule,
  MatNativeDateModule,
  MatPaginatorModule,
  MatSelectModule,
  MatSortModule,
  MatTableModule,
  MatTooltipModule,
];

@NgModule({
  declarations: [
    // Components and pipes
    AppComponent,
    NavbarComponent,
    HomeComponent,

    LoginComponent,
    RegisterComponent,

    BookingSearchComponent,
    BookingConfirmationComponent,
    MyBookingsComponent,

    RoomsComponent,
    RoomCardComponent,
    RoomFormDialogComponent,

    RoomTypesComponent,
    RoomTypeDialogComponent,

    ServicesComponent,
    ServicesFormDialogComponent,

    BookingsComponent,
    BookingsOfflineDialogComponent,

    NightsLabelPipe,
    RoomLabelPipe,
  ],
  imports: [
    // Angular core
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,

    // 3rd‑party
    ToastrModule.forRoot(),

    // Material
    ...MATERIAL_MODULES,
  ],
  providers: [provideHttpClient(withInterceptors([jwtInterceptor]))],
  bootstrap: [AppComponent],
})
export class AppModule { }
