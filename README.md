# **Hosted ASP.NET Core 7 Blazor WebAssembly PWA Application with Offline Support**

The application is a School Management Academics Web App with ***3 SPAs (Single Page Applications)*** in ***One Hosted Blazor PWA*** and can be explore at the following URLs with different login credentials as given:

1. [Staff Login](https://schoolwebapps.com.ng/) - Email: admin@schoolwebapps.com.ng and Password: 1234567890 
2. [Computer-Base Test for Students - CBT](https://schoolwebapps.com.ng/cbt) - PINs: TA37WQ, YZK4LF, DK738R, RDG8TH and Password: honey
3. [Result Checker (Checking/Downloading Students Rsults Online by Parents)](https://schoolwebapps.com.ng/resultchecker) - PINs: MST7R, CVHF6X, 0FB6KM, RJ0D3

The Offline Mode is only available for some pages of the application. Though it can be applied to all the pages in the application. The pages that have Offline Mode supports are
* Mark entr


### [This is the deployed application on a Windows Hosting](https://schoolwebapps.com.ng/)   ###
This application was built using ASP.NET Core 7 Hosted Blazor PWA with Offline Support using the following components
 
Client Side
* MudBlazor
* BlazorIndexedDB
* Blazored.LocalStorage and Blazored.SessionStorage
* SweetAlert2
* EPPlus
* Blazored.FluentValidation
* MathJaxBlazor
* System.Security.Claims

Server Side
* Custom thentication using Json Web Token (JWT)
* Dapper
* iTextSharp.LGPLv2.Core
* Microsoft.AspNetCore.Authentication.JwtBearer and System.IdentityModel.Tokens.Jwt
