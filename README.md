# Retail Management System
A retail management system that runs a cash register, handles inventory, and manages the entire store. The purpose of this readme file will be adding the technologies/tools that I used to create this project.

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

## 02 - Configuring Swagger for WebAPI
Swagger allows us to document and test our API’s. You can get swagger by installing the Swashbuckle package in NPM

- Testing swagger url: https://url/swagger

- **App_Start/SwaggerConfig.cs**: Swagger UI configuration

```
// To rename the WebAPI Title, edit the following line
c.SingleApiVersion("v1", "Retail Management Systems API");

// For proper indentation, uncomment the following:
c.PrettyPrint();

// To revoke access to swagger (some teams prefer to do this when the application is in production), comment the following line:
[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]
```

- Use Swagger in ASP.NET WebAPI with token-based authentication (Similar to Postman token authentication, but easy to use) - https://stackoverflow.com/questions/51117655/how-to-use-swagger-in-asp-net-webapi-2-0-with-token-based-authentication