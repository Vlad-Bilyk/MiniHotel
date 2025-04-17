import { Component, OnInit } from '@angular/core';
import { ServiceDto, ServiceUpsertDto } from '../../../api/models';
import { ServicesService } from '../../../api/services';
import { DialogService } from '../../../shared/services/dialog.service';
import { ToastrService } from 'ngx-toastr';
import { ServiceFormData, ServicesFormDialogComponent } from './services-form-dialog/services-form-dialog.component';

@Component({
  selector: 'app-services',
  standalone: false,
  templateUrl: './services.component.html',
  styleUrl: './services.component.css'
})
export class ServicesComponent implements OnInit {
  services: ServiceDto[] = [];
  isLoading = false;

  constructor(
    private hotelService: ServicesService,
    private dialogService: DialogService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.fetchServices();
  }

  fetchServices(): void {
    this.isLoading = true;
    this.hotelService.getServices().subscribe({
      next: (data) => {
        this.services = data.filter(s => s.serviceId !== 1);
        this.isLoading = false;
      },
      error: (err) => {
        this.toastr.error('Сталася помилка під час завантаження');
        console.error(err);
      },
    });
  }

  deleteService(id: number): void {
    if (!confirm('Ви впевнені, що хочете видалити цей номер')) return;

    this.hotelService.deleteService({ id }).subscribe({
      next: () => {
        this.toastr.success('Послугу видалено');
        this.fetchServices();
      },
      error: (err) => {
        this.toastr.error('Сталася помилка під час видалення');
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
    ).subscribe(() => this.fetchServices());
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
    ).subscribe(() => this.fetchServices());
  }
}
