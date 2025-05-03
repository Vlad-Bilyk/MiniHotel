# 🏨 MiniHotel – Web Application for Small Hotel Automation

**MiniHotel** is a full-stack web application developed as a diploma project to automate key business processes of a small hotel, such as room booking, service management, invoicing, and online payments.

**Authorization:**

Manager: login - manager@hotel.com, 

Receptionist - receptionist@hotel.com

Client - maria.ivanova@example.com

Password - Password1!

---

## 📌 Features

### 🧑‍💼 For Clients:
- Search available rooms by dates
- View room types and descriptions
- Make online bookings
- Pay securely via **LiqPay**
- View and cancel active bookings
- Review final invoices with ordered services

### 🛠️ For Staff (Receptionists & Managers):
- Manage room types and rooms (CRUD)
- Manage additional services (CRUD)
- Manage all bookings (view, edit offline bookings, cancel)
- Add extra services during guest stay
- View full invoice details and payments
--

## 🧰 Technologies Used

### 🔙 Backend (ASP.NET Core Web API)
- Clean Architecture with layered structure
- Entity Framework Core + PostgreSQL
- ASP.NET Core Identity (JWT Authentication)
- Role-based authorization (`Client`, `Receptionist`)
- AutoMapper for DTO mapping
- Integration with LiqPay API (online payments)
- Global exception handling + Toastr error messages
- Swagger (for API testing and admin fallback)

### 🌐 Frontend (Angular)
- Angular 17 with standalone components
- Angular Material for modern UI
- Client-side form validation
- JWT Authentication + Guards
- Responsive layout (for desktop)
- Toast notifications via ngx-toastr

---
