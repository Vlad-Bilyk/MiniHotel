/* tslint:disable */
/* eslint-disable */
/* Code generated by ng-openapi-gen DO NOT EDIT. */

import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { BookingDto } from '../../models/booking-dto';

export interface GetBookingById$Params {

/**
 * The unique identifier of the booking.
 */
  id: number;
}

export function getBookingById(http: HttpClient, rootUrl: string, params: GetBookingById$Params, context?: HttpContext): Observable<StrictHttpResponse<BookingDto>> {
  const rb = new RequestBuilder(rootUrl, getBookingById.PATH, 'get');
  if (params) {
    rb.path('id', params.id, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<BookingDto>;
    })
  );
}

getBookingById.PATH = '/api/Bookings/{id}';
