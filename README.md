# EasyRent — Apartment Renting Platform

A Service Oriented Architecture (SOA) final project — a web platform where landlords publish
apartment listings and tenants search, book, and pay for rentals.

**University:** South East European University — SOA, Spring 25-26
**Team:** Edin Snopche · Mesut Izeti · Besjana Sulejmani

---

## Tech Stack
- **Backend:** .NET 8.0, ASP.NET Core Web API, EF Core, SQL Server
- **Auth:** ASP.NET Core Identity + JWT (roles: Admin, Landlord, Tenant)
- **Front-end:** Angular 17+ with Angular Material
- **Testing:** xUnit, Moq, FluentAssertions
- **Cloud:** Azure App Service + Azure SQL
- **CI/CD:** GitHub Actions

## Repository Structure
```
/                         → solution + docs
├── EasyRent.API            ASP.NET Core Web API
├── EasyRent.Application     Services, DTOs, interfaces
├── EasyRent.Domain          Entities, enums
├── EasyRent.Infrastructure  EF Core, repositories, migrations
├── EasyRent.Tests           Unit tests
└── easyrent-client          Angular front-end
```

## Getting Started (for team members)

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) and Angular CLI (`npm i -g @angular/cli`)
- SQL Server (LocalDB / Express) or Docker

### Clone
```bash
git clone <REPO_URL>
cd SOA_FInal_Project
```

### Run the API
```bash
dotnet restore
dotnet ef database update --project EasyRent.Infrastructure --startup-project EasyRent.API
dotnet run --project EasyRent.API
```
Swagger UI: `https://localhost:<port>/swagger`

### Run the front-end
```bash
cd easyrent-client
npm install
ng serve
```
App: `http://localhost:4200`

## Branching
`main` (protected) ← `dev` ← `feature/*`. Open a Pull Request for review before merging.

## Documentation
See [IMPLEMENTATION_PLAN.md](IMPLEMENTATION_PLAN.md) for the full architecture, work split, and grading map.
