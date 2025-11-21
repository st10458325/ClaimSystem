# ClaimSystem


# 🌟 **Claim System – Final Project Presentation**

**Developed by:** *Armstrong*
**Course:** *PROG6212*

---

# 🧩 **1. Introduction**

The **Claim System** is a full-stack ASP.NET Core MVC web application built to streamline the monthly claim submission workflow for lecturers, coordinators, and administrators (HR).
The system automates claim submission, reviewing, approvals, role management, file uploads, document storage, and professional PDF report generation.

It is integrated with **ASP.NET Core Identity** for authentication and role-based authorization, ensuring every user only accesses features appropriate to their role.

This system is fully database-backed using **SQL Server (localdb)** with Entity Framework Core.

---

# 👥 **2. User Roles & Capabilities**

The system operates around **three primary roles**, each with specific permissions.

---

## 🔹 **2.1 Lecturer**

Lecturers use the system to:

* Submit monthly work claims
* Attach supporting documents (PDF/DOC/DOCX only)
* View all their pending, approved, and rejected claims
* Download previously submitted documents
* Track status updates from Coordinators and Admin
* Update their claim with hours worked and hourly rate (if applicable)

This role has **no access** to administrative user management or claim approval pages.

---

## 🔹 **2.2 Coordinator**

Coordinators act as the first reviewers:

* View all lecturers’ claims assigned to them
* Approve or Reject claims
* Add feedback or comments
* Track lecturer submissions
* Redirect approved claims to Admin for final processing

They **cannot edit or delete user accounts**, maintaining system security.

---

## 🔹 **2.3 Admin (HR)**

The Admin has full system control:

* Create new **Lecturer**, **Coordinator**, and **Admin** accounts
* Edit or delete user accounts
* View all claims submitted across the institution
* Approve, reject, or mark claims as paid
* Generate **PDF Reports / Invoices**

  * Full claim table
  * Totals and summary
  * Date-range filtering (From / To)
  * Export all claims or filtered subset
* Manage system-wide configuration

Admins are the only users who have access to accounts and system-wide reports.

---

# 📁 **3. Key System Features**

The system includes multiple important functionalities that work together to provide a complete claims management solution.

---

## 📝 **3.1 Claim Submission (Lecturer)**

Lecturers can create a claim by entering:

* Hours Worked
* Hourly Rate (if applicable)
* Claim details
* Supporting document (file upload)

The system validates:

✔ File type (PDF, DOC, DOCX only)
✔ Required fields
✔ Total amount calculation (hours × rate)

All claim records are saved to the database with metadata:

* LecturerId
* SubmittedOn date
* Status
* File path

Lecturers can later **download the uploaded file** from the claim list.

---

## 🔐 **3.2 Authentication & Authorization**

The system uses **ASP.NET Identity** with:

* Login
* Logout
* Password hashing
* Secure cookies
* Role-based routing
* ReturnUrl support (Users are redirected to the page they originally requested after login)

This ensures strict separation of functionality.

---

## 👤 **3.3 User Management (Admin)**

Admin can:

* Create new users (Lecturer, Coordinator, Admin)
* Edit user details
* Delete users
* View user roles in the dashboard
* Redirect users based on roles at login

The system displays each user’s roles properly using a ViewModel (`AdminUserDisplayVM`) to prevent DataReader conflicts.

---

## 🧾 **3.4 Claim Review Workflow**

### Lecturer → Coordinator → Admin

1. Lecturer submits a claim with supporting document
2. Coordinator reviews:

   * Approves
   * Rejects
   * Sends to Admin for final decision
3. Admin performs final review and can:

   * Approve
   * Reject
   * Mark as finalised for HR purposes

All statuses are reflected immediately for the lecturer.

---

# 📤 **3.5 PDF Report / Invoice Generation (Admin)**

This system includes a fully implemented **PDF report generator** for HR.

Features:

✔ Clean, professional A4 layout
✔ Auto totals (Total Hours + Total Amount)
✔ HR summary and branding
✔ Date range filters (From, To, Status)
✔ Downloadable PDF file
✔ Secure generation using QuestPDF
✔ Displays lecturer name, email, hours, rate, status, submission date
✔ Fully formatted table with page numbers

This allows HR to produce monthly invoices directly from the system.

---

# 📦 **3.6 File Upload + Download System**

* Files are uploaded to `/wwwroot/uploads/`
* The file name is saved in the database
* Users can download their own uploaded document
* Admin and Coordinators can download files during review
* Only safe file formats allowed (PDF/DOC/DOCX)

This ensures audit compliance and traceability.

---

# 🔄 **3.7 ReturnUrl-Based Redirection (Improved UX)**

When a user tries to access a protected page:

Example:
`/Claims/CoordinatorIndex`

…but they are not logged in:

1. System redirects them to Login
2. After entering valid credentials
3. System returns them **back to the intended page**, not Home

This makes navigation seamless and professional.

---

# 🚫 **3.8 Custom Access Denied Page**

The application includes a **fully custom 403 Access Denied page** with:

* Glassmorphism design
* Red “403” error code
* Back to home button
* Support contact link
* Responsive layout

This creates a user-friendly experience even when access is restricted.

---

# 🎨 **4. Front-End / Visual Design Features**

Your visual design is modern and impressive. Here is the explanation formatted in presentation style.

---

# 🌈 **4.1 Modern Dashboard Layout**

Instead of simple buttons, each role (Admin, Coordinator, Lecturer) is represented by a **beautiful interactive card** on the welcome page.

* Cards include icons
* Interactive animation
* Clickable areas
* Clean typography
* Professional dashboard appearance

---

# 🧊 **4.2 Glassmorphism UI**

The welcome page and AccessDenied page use:

✔ Semi-transparent containers
✔ Frosted glass blur effects
✔ Soft rounded corners
✔ Drop shadows
✔ Smooth gradients

This creates a modern, premium look consistent with current UI trends.

---

# 🔥 **4.3 Interactive Hover Effects**

Each card:

* Lifts upward slightly
* Slight glow or shadow increases
* Icons animate subtly
* Improves user feedback and engagement

This makes the interface feel “alive” and polished.

---

# 📱 **4.4 Fully Responsive Grid Layout**

The system uses a CSS grid that automatically adjusts:

* 3 cards across on desktop
* 2 cards on medium screens
* 1 card stacked vertically on mobile

This ensures the welcome dashboard works on any screen size.

---

# 🖼 **4.5 Built-in SVG Icons (No External Libraries)**

Each role card includes inline SVG icons:

* Lightning bolt for Admin
* Shield for Coordinator
* Document for Lecturer

These icons are:

* Resolution-independent
* Lightweight
* Zero dependencies
* Perfectly compatible with your theme

---

# 🧭 **5. Routing and Navigation**

The system uses:

* Custom AccountController for authentication
* Role-based dashboard routing
* CoordinatorIndex and LecturerIndex under Claims controller
* Secure redirection using `returnUrl`
* Custom AccessDenied route
* Optional redirects based on user type

---

# 🗄 **6. Database & EF Core Integration**

The data model uses:

### **Identity Tables**

* AspNetUsers
* AspNetRoles
* AspNetUserRoles

### **Custom Tables**

* Claims
* File storage (filename only)
* Navigation property linking lecturer to claims

### Entity Framework Features Used

* Migrations
* LINQ queries
* Eager loading with `.Include()`
* ViewModels for safe data transfer
* Async/await for non-blocking database operations

---

# 🏁 **7. Conclusion**

Your Claim System is now a **fully functional, secure, and professionally designed web application** with:

✔ Authentication & authorization
✔ Role-based workflows
✔ Document uploads/downloads
✔ PDF invoice/report generation
✔ Modern UI/UX
✔ Responsive design
✔ Admin user management
✔ Coordinator approval system
✔ Lecturer submission interface
✔ Custom 403 Access Denied page
✔ Clean dashboard cards using glassmorphism


