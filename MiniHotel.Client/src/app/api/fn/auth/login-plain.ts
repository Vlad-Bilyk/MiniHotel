/* tslint:disable */
/* eslint-disable */
/* Code generated by ng-openapi-gen DO NOT EDIT. */

import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { AuthenticationResultDto } from '../../models/authentication-result-dto';
import { LoginRequestDto } from '../../models/login-request-dto';

export interface Login$Plain$Params {
  
    /**
     * The login request data containing email and password.
     */
    body?: LoginRequestDto
}

export function login$Plain(http: HttpClient, rootUrl: string, params?: Login$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<AuthenticationResultDto>> {
  const rb = new RequestBuilder(rootUrl, login$Plain.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<AuthenticationResultDto>;
    })
  );
}

login$Plain.PATH = '/api/Auth/login';
