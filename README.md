# AuthX – Secure Authentication System

## Description

This project implements a secure web authentication system using ASP.NET Core Razor Pages. It includes user authentication, password reset functionality, a ticketing system, and audit logging. The application is designed with security best practices in mind and demonstrates protection against common web vulnerabilities.

---

## Technologies Used

- ASP.NET Core Razor Pages
- Entity Framework Core
- PostgreSQL
- Bootstrap
- .NET 7/8

---

## Features

### Authentication
- User registration
- Login and logout
- Password hashing using ASP.NET Identity PasswordHasher
- Secure session management using cookies

### Security Mechanisms
- Generic error messages to prevent user enumeration
- Rate limiting based on IP address
- Account lockout after multiple failed login attempts (5 attempts, 2 minutes lock)
- Secure cookies (HttpOnly, SameSite, Secure)

### Password Reset
- Secure token generation
- Token expiration (15 minutes)
- One-time use tokens
- Token validation on reset

### Ticketing System
- Users can create and view tickets
- Each user can only access their own tickets
- Protection against unauthorized access (IDOR)

### Audit Logging
The system logs important actions in the database:
- Login success and failure
- Account lock events
- Password reset requests and completions
- Logout actions
- Ticket creation and access attempts

---

## Project Structure

- Pages/
  - Authentication pages (Login, Register, Logout, ForgotPassword, ResetPassword)
  - Dashboard
  - Tickets (Index, Create, Details)
  - AuditLogs
- Models/
  - User
  - Ticket
  - PasswordResetToken
  - AuditLog
- Services/
  - PasswordService
  - AuditService
- Data/
  - AppDbContext

---

###Videoclip

Link videoclip https://www.youtube.com/watch?v=23dXRp1gDuU

## Database

The application uses PostgreSQL with the following main tables:
- Users
- Tickets
- AuditLogs
- PasswordResetTokens

---

## How to Run the Project

### 1. Clone the repository

```bash
git clone <repository_url>
cd AuthX
```

### 2. Apply migrations

```bash
dotnet ef database update
```

```md
Make sure PostgreSQL is running before starting the application.
```

### 3. Run the application

```bash
dotnet run
```




