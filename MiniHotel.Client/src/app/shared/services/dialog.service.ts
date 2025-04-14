import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ComponentType, ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';

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

    return new Observable((observer) => {
      dialogRef.afterClosed().subscribe((result: TSubmit | undefined) => {
        if (!result) {
          observer.complete();
          return;
        }

        submitFn(result).subscribe({
          next: (response) => {
            this.toastr.success(successMessage);
            observer.next(response);
            observer.complete();
          },
          error: (err) => {
            this.toastr.error('Щось пішло не так');
            console.error('[DialogService Error]', err);
            observer.error(err);
          },
        });
      });
    })
  }
}
