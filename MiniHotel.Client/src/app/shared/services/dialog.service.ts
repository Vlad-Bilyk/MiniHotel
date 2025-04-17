import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ComponentType, ToastrService } from 'ngx-toastr';
import { catchError, filter, Observable, switchMap, tap, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DialogService {
  constructor(private dialog: MatDialog, private toastr: ToastrService) { }

  /**
   * Opens the dialog for both creating and editing.
   * @param dialogData - Data to configure the dialog (edit flag and form data).
   * @param submitFn - The function to call when form is submitted.
   * @param successMessage - Message to display on successful operation.
   */
  openEntityDialog<TData, TSubmit>(
    component: ComponentType<any>,
    dialogData: TData,
    submitFn: (data: TSubmit) => Observable<any>,
    successMessage: string,
    width: string = '400px'
  ): Observable<any> {
    const dialogRef = this.dialog.open(component, {
      width,
      data: dialogData,
    });

    return dialogRef.afterClosed().pipe(
      filter(result => !!result),
      switchMap(result => submitFn(result as TSubmit)),
      tap(() => this.toastr.success(successMessage)),
      catchError(err => { this.toastr.error('Щось пішло не так'); return throwError(() => err); })
    );
  }
}
