import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './shared/navbar/navbar.component';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import {
  provideHttpClient,
  withInterceptors,
} from '@angular/common/http';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';
import { RoomsComponent } from './pages/rooms/rooms.component';
import { BookingSearchComponent } from './pages/booking-search/booking-search.component';
import { RoomCardComponent } from './shared/room-card/room-card.component';
import { BookingConfirmationComponent } from './pages/booking-confirmation/booking-confirmation.component';
import { NightsLabelPipe } from './shared/pipes/labelPipes/nights-label.pipe';
import { RoomLabelPipe } from './shared/pipes/labelPipes/room-label.pipe';

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
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    ReactiveFormsModule,
  ],
  providers: [provideHttpClient(withInterceptors([jwtInterceptor]))],
  bootstrap: [AppComponent],
})
export class AppModule { }
