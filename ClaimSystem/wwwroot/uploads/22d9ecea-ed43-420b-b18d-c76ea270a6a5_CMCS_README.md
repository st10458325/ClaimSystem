# CMCS (Contract Monthly Claim System) - README

This README explains how to set up the project locally, connect to SQL Server (SSMS 21), run Entity Framework Core migrations, seed roles/users, and run the ASP.NET Core MVC application. It also includes useful sample SQL queries you can run in SSMS for inspection and troubleshooting.

---

## Prerequisites

- .NET SDK (6.0, 7.0 or compatible with project)
- SQL Server (local or remote) and SQL Server Management Studio 21 (SSMS 21)
- Visual Studio Code (or Visual Studio)
- Git
- Optional: dotnet-ef tool (`dotnet tool install --global dotnet-ef`)

---

## 1. Connection string (appsettings.json)

Edit `appsettings.json` and set `DefaultConnection`:

**Windows / Local default (Windows Authentication):**
```
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\SQLEXPRESS;Database=CMCS;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**SQL Server Authentication example:**
```
"DefaultConnection": {
  "DefaultConnection": "Server=MY_SQL_SERVER;Database=CMCS;User Id=sa;Password=YourStrong@Pass;TrustServerCertificate=True;"
}
```

> Replace server name and credentials as required. If using a named instance (e.g., `SQLEXPRESS`), include it as shown.

---

## 2. Create the database (optional)
You can let EF Core create the database via migrations, or create it manually in SSMS.

**Manual (SSMS) — create database:**
```sql
CREATE DATABASE CMCS;
GO
```

If you created the DB manually, ensure the connection string points to it and that the SQL login/user has appropriate permissions.

---

## 3. Restore NuGet packages & install tools

From the project root:

```bash
dotnet restore
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package ClosedXML
```

(ClosedXML is for Excel export; optional for exports)

---

## 4. Update Program.cs - ensure services registered

Make sure your `Program.cs` registers the DbContext and Identity, for example:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
```

---

## 5. Run EF Core migrations (create DB schema)

From the project root:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

- `add InitialCreate` generates migration files under `Migrations/`.
- `database update` applies migrations and creates the required tables (Identity + Claims).

If you get *No project was found* error: ensure you run the commands in the project's `.csproj` directory, or add `--project <path-to-csproj>`.

---

## 6. Seed roles & admin user

If you included `DbInitializer.SeedRolesAndAdmin`, ensure you call it during startup (before `app.Run()`):

```csharp
await DbInitializer.SeedRolesAndAdmin(app.Services);
```

Then run the app; the roles `Lecturer`, `Coordinator`, `Manager`, `HR`, and `Admin` will be created plus an admin user:

- Email: `admin@cmcs.local`
- Password: `Admin123!` (change after first login)

---

## 7. Running the application

From project root:

```bash
dotnet run
```

Open the site at: `https://localhost:5001` (or the port shown in console).

- Register or create users in SSMS / admin portal
- Assign roles (Lecturer, Coordinator, Manager, HR) using UserManager or manually via DB (not recommended)

---

## 8. Common SSMS Queries (inspect data)

**List claims:**
```sql
USE CMCS;
SELECT Id, LecturerId, HoursWorked, HourlyRate, SubmittedAt, Status, DocumentPath
FROM Claims
ORDER BY SubmittedAt DESC;
```

**View AspNetUsers and roles (identity tables):**
```sql
USE CMCS;

/* Users */
SELECT Id, UserName, Email
FROM AspNetUsers;

/* Roles */
SELECT Id, [Name]
FROM AspNetRoles;

/* UserRoles join */
SELECT ur.UserId, u.UserName, ur.RoleId, r.Name
FROM AspNetUserRoles ur
JOIN AspNetUsers u ON u.Id = ur.UserId
JOIN AspNetRoles r ON r.Id = ur.RoleId;
```

**Add a role to a user (not recommended manually unless you know GUIDs)**
```sql
-- Get user id and role id first
DECLARE @userId nvarchar(450) = 'USER_ID';
DECLARE @roleId nvarchar(450) = 'ROLE_ID';

INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES (@userId, @roleId);
```

**Fix migration 'No project found' errors**
- Ensure you're in same folder as `.csproj` or use `--project` option:
```bash
dotnet ef database update --project ./ClaimsSystemPrototype.csproj
```

---

## 9. File uploads

- Files are saved to `wwwroot/Uploads` (ensure folder exists and the app has write permission).
- `DocumentPath` stores the relative path (`/Uploads/<guid>_file.ext`)
- To view file in browser: `https://localhost:5001/Uploads/<file>`

---

## 10. Exports (CSV, Excel, PDF)

- CSV: returns `text/csv` file
- Excel: uses ClosedXML; ensure package installed
- PDF: placeholder included in code — install a proper PDF library for polished PDF (e.g., `DinkToPdf`, `PdfSharpCore`, or a commercial library)

---

## 11. Unit tests

- Create a test project (`xUnit`) and use `Microsoft.EntityFrameworkCore.InMemory` for in-memory DbContext
- Typical tests: Claim submission auto-approval, Approve/Reject flows, file upload validation logic

---

## 12. Troubleshooting tips

- **Cannot connect to SQL Server**: check SQL Server service, TCP/IP enabled in SQL Server Configuration Manager, firewall rules, and that server name is correct.
- **EF commands fail**: ensure `dotnet-ef` installed and you run commands from project folder.
- **Permissions issues saving files**: check folder write permissions and antivirus blockers.
- **Port conflicts**: `dotnet run` will show port; specify `ASPNETCORE_URLS` env var if needed.

---

## 13. Recommended Git commits (examples)
- `feat: add Claim model and DbContext registration`
- `feat: scaffold ClaimsController and Create view`
- `feat: implement file upload and save to wwwroot/Uploads`
- `feat: implement Coordinator/Manager Review view and approve/reject`
- `test: add basic unit test scaffolding`
- `feat: add auto-approve rule on claim submission`
- `feat: add HR export CSV and Excel (ClosedXML)`
- `chore: add Identity role seeding and ApplicationUser`
- `feat: add validation and client-side validation scripts`
- `docs: add README and PPT slide content`

---

## 14. Next steps / Helpful commands

**Open SSMS and verify tables**
- After `dotnet ef database update`, open SSMS → Connect to server → Expand Databases → CMCS → Tables.

**If you prefer manual SQL schema generation**
- Generate SQL script from EF migration:
```bash
dotnet ef migrations script -o prepare_db.sql
```
- Run `prepare_db.sql` in SSMS.

---

If you want, I can:
- generate a `prepare_db.sql` script from the migrations (requires running `dotnet ef migrations script` locally),
- create a `.pptx` file with the provided slide text,
- or create a sample seed script to add test lecturers and claims.

Tell me which and I will generate/download the files for you.
