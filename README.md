# AuthX – Secure Authentication System

## 📌 Description
This project implements a secure authentication system using ASP.NET Core Razor Pages.

Features:
- Register / Login / Logout
- Password hashing
- Rate limiting (IP-based)
- Account lockout after failed logins
- Password reset with secure tokens
- Ticketing system
- Audit logging

---

## ⚙️ Technologies
- ASP.NET Core Razor Pages
- Entity Framework Core
- PostgreSQL
- Bootstrap

---

## 🔐 Security Features

### 1. Password Security
- Passwords are hashed using ASP.NET Identity PasswordHasher
- Strong password policy enforced

### 2. Authentication Protection
- Generic error messages (prevents user enumeration)
- Session cookies secured (HttpOnly, SameSite, Secure)

### 3. Brute Force Protection
- Account locked after 5 failed attempts
- Lock duration: 2 minutes
- IP-based rate limiting

### 4. Password Reset
- Secure random tokens
- Expiration (15 minutes)
- One-time use tokens

### 5. Authorization
- Users can only access their own tickets
- Unauthorized access returns NotFound

### 6. Audit Logging
Actions stored in database:
- LOGIN_SUCCESS / LOGIN_FAILED
- ACCOUNT_LOCKED
- PASSWORD_RESET
- LOGOUT
- TICKET actions

---

## 🚀 How to run

```bash
git clone <repo>
cd AuthX
dotnet ef database update
dotnet run
