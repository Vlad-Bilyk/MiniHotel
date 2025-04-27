import { Component, HostListener, OnInit } from '@angular/core';
import { ServiceDto } from '../../api/models';
import { ServicesService } from '../../api/services';
import { ToastrService } from 'ngx-toastr';
import {
  trigger,
  state,
  style,
  transition,
  animate,
} from '@angular/animations';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  animations: [
    trigger('expandCollapse', [
      state(
        'collapsed',
        style({
          height: '70px',
          overflow: 'hidden',
        })
      ),
      state(
        'expanded',
        style({
          height: '*',
          overflow: 'hidden',
        })
      ),
      transition('collapsed <=> expanded', [animate('300ms ease')]),
    ]),
    trigger('fadeInOnScroll', [
      state(
        'hidden',
        style({
          opacity: 0,
          transform: 'translateY(20px)',
        })
      ),
      state(
        'visible',
        style({
          opacity: 1,
          transform: 'translateY(0)',
        })
      ),
      transition('hidden => visible', [animate('600ms ease-out')]),
    ]),
  ],
})
export class HomeComponent implements OnInit {
  services: ServiceDto[] = [];
  isExpanded: { [key: number]: boolean } = {};
  servicesVisible = false;

  constructor(
    private hotelServices: ServicesService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.loadServices();
  }

  loadServices(): void {
    this.hotelServices.getServices().subscribe({
      next: (data) => {
        this.services = data.filter((s) => s.isAvailable);

        this.services.forEach((service) => {
          this.isExpanded[service.serviceId!] = false;
        });
      },
      error: (err) => {
        console.error(err);
        this.toastr.error('Не вдалося завантажити список послуг');
      },
    });
  }

  toggleExpand(serviceId: number): void {
    this.isExpanded[serviceId] = !this.isExpanded[serviceId];
  }

  @HostListener('window:scroll', [])
  onWindowScroll() {
    const element = document.getElementById('services-section');
    if (element) {
      const rect = element.getBoundingClientRect();
      const windowHeight = (window.innerHeight || document.documentElement.clientHeight);
      if (rect.top <= windowHeight - 100) {
        this.servicesVisible = true;
      }
    }
  }
}
