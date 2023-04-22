# **Hosted ASP.NET Core 7 Blazor WebAssembly PWA Application with Offline Support using C#**
## Overview ##
The application is a School Management Academics Web App with ***3 SPAs (Single Page Applications)*** in ***One Hosted Blazor PWA*** with Authentication and Authorization using Dapper for the Data Access Layer with SQL Server as the Backend. This was developed using ASP.NET Core 6/7 and C#. The completed project in this repository is in ASP.NET Core 7 while the deployed applications is in ASP.NET Core 6. The project development started with ASP.NET Core 6 and with the release of ASP.NET Core 7 last November 2022, the application was re-developed using ASP.NET Core 7. The Windows Hosting provider did not have provision for ASP.NET Core 7 when the application was about to be deploy as a result the project was Built, Published and Deployed in ASP.NET Core 6. Please, see the instructions on how to explore the deployed application below. 

Being Hosted Blazor PWA, it has the Client Side and Server Side. The ASP.NET Core 7 project in this repository was designed and developed with the following web components and packages:

### **Client Side** ###
* **MudBlazor** - All Forms, Tables, Editable Table like Data Grid, Button etc. are MudBlazor Components
* **BlazorIndexedDB** - This is used for the Web App Offline Support. The package along with a C# Method to run a JavaScript  function that determines when the app is Online or Offline using MudBlazor WiFi Icon.
* **Blazored.LocalStorage and Blazored.SessionStorage** - Used to store session data like User Session, Claims data, Role ID etc. All session data are encrypted before they are stored and decryot when they are to be used.
* **SweetAlert2** - Used to display beautiful alert during form validation, data submission etc.
* **EPPlus** - Used for Exporting records to Excel along with JavaScript  Functions for SaveAs, Download. Example usage  is Exporting Students/Staff List to Excel etc.
* **Blazored.FluentValidation** - This is used for Form Validation
* **MathJaxBlazor** - This is used in the Custom Equation Editor I built in the app to Generate Mathematical Equations. It is also used in displaying Mathematical equation for the CBT Exams module of the web app.
* **System.Security.Claims** - Used in the CustomAuthStateProvider Class for user authentication. The CustomAuthStateProvider Class contains the asynchronous methods used to Authenticate user in the Staff Login Form, CBT Login Form and Result Checker PINs confirmation form along with Authorization which is used to Hide or Show Menu Items bases on the authenticated user role using AuthorizeView & Authorized components of ASP.NET Core AuthenticationStateProvider.

### **Server Side** ###
* **Custom Authentication using Json Web Token (JWT)** - This is the server side Authentication providing authentication service for Staff Login, CBT Login and Result Checker PINs authentication.
* **Microsoft.AspNetCore.Authentication.JwtBearer and System.IdentityModel.Tokens.Jwt** - Along with the JWT, this contains types that enable support for JWT bearer based authentication which performs authentication by extracting and validating a JWT token from the Authorization request header. The  System.IdentityModel.Tokens.Jwt provide support for creating, serializing and validating JSON Web Tokens. 
* **Dapper** - Dapper is used in this application for Data Access Layer between the Client Side and the Database. I created an Interface Class that handles Generic Dapper Repository along with Unit of Work. This is used to separate and handle each Database Table CRUD operation. A ServiceRegistration Class with a static void method called AddDataAccess was created to collate all the Data Access Repository classes using IServiceCollection with an AddTransient service. The AddDataAccess method is then added to the Build Service in Program.cs file to provide Dependency Injection in the Controllers.
* **iTextSharp.LGPLv2.Core** - This is used for the Reporting Service in the Web App. All the different types of Student Results are created in ***PDF format*** using this package and these were achieved by creating different Controllor Methods for the results. 

## Application Offline Support ##
The Offline Mode is only available for some pages of the application. Though it can be applied to all the pages in the application. The pages that have Offline Mode supports can be access under the following menus:
- Staff Login Page and Academic Year/Academic Session Selection Page
* ACADEMICS - Mark Entry
  - Cognitive Mark Entry
  - Psychomotor Mark Entry
  - CheckPoint and IGCSE Mark Entry
* ACDEMICS - Comments Entry
  - Mid-Term Comments
  - End of Term Comments
  
## Instructions on How to Explore the Deployed Web App ##
This Web Application can be explore at the following URLs with sample information given below:

1. [Staff Login](https://schoolwebapps.com.ng/) - Email: admin@schoolwebapps.com.ng and Password: 1234567890 
2. [Computer-Base Test for Students (CBT)](https://schoolwebapps.com.ng/cbt) - PINs: TA37WQ, YZK4LF, DK738R, RDG8TH and Password: honey
3. [Result Checker (Checking/Downloading Students Rsults Online by Parents)](https://schoolwebapps.com.ng/resultchecker) - PINs: MST7R, CVHF6X, 0FB6KM, RJ0D3

The following should be noted about the sample information given above to explore the Web App: 
- 2022/2023 - First Term Academic Session has been used to generate the above sample information for the CBT and Result Checker for Students in Year 10 SAPHIRE and Year 12 SADIUS.
- Academics Data are only available for the Academic Year 2020/2021, 2021/2022 and 2022/2023 even though Academic Years 2013 to 2023 were listed for selection when the Staff login to the Web App. This is because there was significant change in the database used for the Desktop version of this Web App which was developed prior to the deployment in 2013. The data in the Database are life data from the school using my Desktop Windows Form Application since 2013 to date. The Desktop Windows Form Application consists of Administration, Financial and Academics Modules with Crystal Reports.
- The above sample will be override under the following situations:
  - When a New Academic Results are processed for a selected Academic Session with selected option for a selected Class. Academics Results are generated using the ***Academic Results*** submenu under the ***ACADEMICS*** main menu. Once the results are generated, use the ***GO TO RESULT PAGE*** Icon to export the results to PDF for Parents Result Checker by clicking on the ***EXPORT RESULTS FOR PARENTS*** Icon. With this ACTION, the above PINs for Result Checker becomes Invalid.
  - When New PINs are generated for CBT and Result Checker, the above PINs becomes Invalid. New PINS can be generated for CBT and Result Checker by using the submenu ***Student*** under the submenu ***Manager Users*** which is under the main menu ***SETTINGS***. The ***Parent PIN*** column is for Result Checker while the ***Student PIN*** column is for CBT.
  - After successful login to the CBT, the ***Examination Instructions*** page opened with a list of Subjects where the student can select a CBT Exam Subject to be taken. Once the CBT exam starts and the ***Submit*** button is selected to end the exam, another page is opened displaying the Student CBT Scores. Click on the ***Close*** button to return back to the ***Examination Instructions*** page but this time around the completed CBT Exam Subject will have been removed from the list of subjects. To return the removed subject(s) back to the list, the administrator will have to use the ***CBT Exams*** submenu under the main menu ***ACADEMICS***. On the CBT Exams page, there are 5 toolbar Icon buttons with Tool Tip to show what the button is for. Click on the 4th Icon button with the Tool Tip ***Lock Exams Taken***. Selecting a School and then a Class with option to a select a student from the selected class will list all the subjects that were removed from the list of subjects on the CBT ***Examination Instructions*** page. On the CBT Exams page, selecteing a row will present an option to Check(Yes) or Uncheck(No). Yes remove the subject while No returns the subject. After Check or Uncheck , selecte the Mark Icon to the right to update your selection.

## Up Coming Projects ##
The Hosted Blazor PWA project in this repository was design and developed to test ASP.NET Core, Hosted Blazor Application, MudBlazor and BlazorIndexedDB to achieve the above sucessfull result.

The next project is the Web Application version of my Financial Desktop Windows Form Application which will be design and developed using ASP.NET Core 7 or possibly ASP.NET Core 8 Preview. It will demonstrate the following:
- Software Design Pattern - Factory Method Theory
- Test Driven Development - TDD
- Onion Architecture with the approach to split the architecture into 4 layers
  - Domain Layer
  - Service Layer
  - Infrastructure Layer
  - Presentation Layer
- I will start with any one of the folllwoing. Though, I am still going to develop the Web App using the two approach below.
  - Hosted Blazor WebAssembly
  - Angular & ASP.NET Core
  - EF Core and/Or Dapper


