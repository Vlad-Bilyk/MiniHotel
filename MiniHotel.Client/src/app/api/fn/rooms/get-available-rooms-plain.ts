/* tslint:disable */
/* eslint-disable */
/* Code generated by ng-openapi-gen DO NOT EDIT. */

import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { RoomDto } from '../../models/room-dto';

export interface GetAvailableRooms$Plain$Params {

/**
 * The start date of the desired booking period.
 */
  startDate: string;

/**
 * The end date of the desired booking period.
 */
  endDate: string;

/**
 * Optional. Booking ID to ignore when checking room availability (used for editing existing bookings).
 */
  ignoreBookingId?: number;
}

export function getAvailableRooms$Plain(http: HttpClient, rootUrl: string, params: GetAvailableRooms$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<RoomDto>>> {
  const rb = new RequestBuilder(rootUrl, getAvailableRooms$Plain.PATH, 'get');
  if (params) {
    rb.path('startDate', params.startDate, {});
    rb.path('endDate', params.endDate, {});
    rb.query('ignoreBookingId', params.ignoreBookingId, {});
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<RoomDto>>;
    })
  );
}

getAvailableRooms$Plain.PATH = '/api/Rooms/available/{startDate}/{endDate}';
