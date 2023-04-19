# **Hosted ASP.NET Core 7 Blazor WebAssembly PWA Application with Offline Support using C#**

The application is a School Management Academics Web App with ***3 SPAs (Single Page Applications)*** in ***One Hosted Blazor PWA*** and can be explore at the following URLs with different login credentials as given:

1. [Staff Login](https://schoolwebapps.com.ng/) - Email: admin@schoolwebapps.com.ng and Password: 1234567890 
2. [Computer-Base Test for Students (CBT)](https://schoolwebapps.com.ng/cbt) - PINs: TA37WQ, YZK4LF, DK738R, RDG8TH and Password: honey
3. [Result Checker (Checking/Downloading Students Rsults Online by Parents)](https://schoolwebapps.com.ng/resultchecker) - PINs: MST7R, CVHF6X, 0FB6KM, RJ0D3

The following should be noted about the sample information given above to explore the Web App: 
- 2022/2023 - First Term Academic Session has been used to generate the above sample information for the CBT and Result Checker for Students in Year 10 SAPHIRE and Year 12 SADIUS.
- Academics Data are only available for the Academic Year 2020/2021, 2021/2022 and 2022/2023 even though Academic Years 2013 to 2023 were listed for selection when the Staff login to the Web App. This is because there was significant change in the database used for the Desktop version of this Web App which was developed prior to the deployment in 2013. The data in the Database are life data from the school using my Desktop Windows Form Application since 2013 to date. The Desktop Windows Form Application consists of Administration, Financial and Academics Modules with Crystal Reports.
- The above sample will be override under the following situations:
  - When a New Academic Results are processed for a selected Academic Session with selected option for a selected Class. Academics Results are generated using the ***Academic Results*** submenu under the ***ACADEMICS*** main menu. Once the results are generated, use the ***GO TO RESULT PAGE*** Icon to export the results to PDF for Parents Result Checker by clicking on the ***EXPORT RESULTS FOR PARENTS*** Icon. With this ACTION, the above PINs for Result Checker becomes Invalid.
  - When New PINs are generated for CBT and Result Checker. New PINS can be generated for CBT and Result Checker by using the submenu ***Student*** under the submenu ***Manager Users*** which is under the main menu ***SETTINGS***. The ***Parent PIN*** column is for Result Checker while the ***Student PIN*** column is for CBT

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
