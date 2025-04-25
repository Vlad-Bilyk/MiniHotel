import { Component, OnInit, ViewChild } from '@angular/core';
import { ServiceDto, ServiceUpsertDto } from '../../../api/models';
import { ServicesService } from '../../../api/services';
import { DialogService } from '../../../shared/services/dialog.service';
import { ToastrService } from 'ngx-toastr';
import { ServiceFormData, ServicesFormDialogComponent } from './services-form-dialog/services-form-dialog.component';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

@Component({
  selector: 'app-services',
  standalone: false,
  templateUrl: './services.component.html',
  styleUrl: './services.component.scss'
})
export class ServicesComponent implements OnInit {
  displayedColumns = ['name', 'description', 'price', 'status', 'actions'];

  dataSource = new MatTableDataSource<ServiceDto>([]);
  loading = false;
  searchValue = "";

  @ViewChild(MatPaginator) set MatPaginator(p: MatPaginator) {
    this.dataSource.paginator = p;
  }

  constructor(
    private hotelService: ServicesService,
    private dialogService: DialogService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.loadServices();
  }

  loadServices(): void {
    this.loading = true;
    this.hotelService.getServices().subscribe({
      next: (data) => {
        this.dataSource.data = data;
        this.loading = false;
      },
      error: (err) => {
        this.toastr.error('Сталася помилка під час завантаження');
        console.error(err);
        this.loading = false;
      },
    });
  }

  applyFilter(value: string): void {
    this.dataSource.filter = value.trim().toLowerCase();
  }

  toggleServiceStatus(service: ServiceDto): void {
    const action = service.isAvailable ? 'деактивувати' : 'активувати';

    if (!confirm(`Ви впевнені, що хочете ${action} цю послугу?`)) return;

    const request$ = service.isAvailable
      ? this.hotelService.deactivate({ id: service.serviceId! })
      : this.hotelService.reactivate({ id: service.serviceId! });

    request$.subscribe({
      next: () => {
        this.toastr.success(`Послугу успішно ${action}о`);
        this.loadServices();
      },
      error: (err) => {
        this.toastr.error(`Сталася помилка під час спроби ${action} послугу`);
        console.error(err);
      },
    });
  }

  editService(service: ServiceDto): void {
    this.dialogService.openEntityDialog<ServiceFormData, ServiceUpsertDto>(
      ServicesFormDialogComponent,
      {
        isEdit: true,
        formData: {
          name: service.name!,
          description: service.description,
          price: service.price,
          isAvailable: service.isAvailable!
        }
      },
      (data) => this.hotelService.updateService({ id: service.serviceId!, body: data }),
      'Послугу оновлено успішно',
      '500px'
    ).subscribe(() => this.loadServices());
  }

  createService(): void {
    this.dialogService.openEntityDialog<ServiceFormData, ServiceUpsertDto>(
      ServicesFormDialogComponent,
      {
        isEdit: false,
        formData: undefined
      },
      (data) => this.hotelService.createService({ body: data }),
      'Послугу створено успішно',
      '500px'
    ).subscribe(() => this.loadServices());
  }
}
