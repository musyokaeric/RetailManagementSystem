# Retail Management System
A retail management system that runs a cash register, handles inventory, and manages the entire store. The purpose of this readme file will be adding the technologies/tools that I used to create this project  in the order of newest to oldest

## 01 - Added WebAPI project to solution
ASP.NET Web Application (.NET Framework with individual user account for authentication)

**Postman**
- Register an account without hardcoding in the project (api/Account/Register)

```
RAW (JSON) 
{    
    "Email": "eric@rms.com",
    "Password": "Password@123",
    "ConfirmPassword": "Password@123"
}
```

- Get access token (/token)

```
BODY 
grant_type : password
usename : eric@rms.com
password : Password@123
```

- Get data using access token (/api/Values)

```
HEADERS 
Authorization : "Bearer + {access token}"
```