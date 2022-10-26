## Web API Template with EF Core for .Net 6
  

[![license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/Milvasoft/Milvasoft/blob/master/LICENSE)  [![NuGet](https://img.shields.io/nuget/v/Milvasoft.Templates.Web.Ef)](https://www.nuget.org/packages/Milvasoft.Helpers/)   [![NuGet](https://img.shields.io/nuget/dt/Milvasoft.Templates.Web.Ef)](https://www.nuget.org/packages/Milvasoft.Templates.Web.Ef/) 


### Create a "Ready to go" project with [Milvasoft.Helpers](https://github.com/Milvasoft/Milvasoft)
### This template contains integration with PostgreSQL, but could be change to any RDMS you prefer.

***

# How to install project template via CLI?


- Install the latest [.NET Core SDK](https://dot.net) and [Visual Studio 2019](https://visualstudio.microsoft.com/tr/thank-you-downloading-visual-studio/?sku=Community&rel=16)
- Run `dotnet new --install Milvasoft.Templates.Web.Ef` to install the project template.
- Run `dotnet new milvawebef --help` to see how to select the feature of the project.
- Run `dotnet new milvawebef --name` "MyProject" along with any other custom options to create a project from the template.


# How to use project template in VS?


- Install the latest [.NET Core SDK](https://dot.net) and [Visual Studio 2019](https://visualstudio.microsoft.com/tr/thank-you-downloading-visual-studio/?sku=Community&rel=16)
- Run `dotnet new --install Milvasoft.Templates.Web.Ef` to install the project template.
- Close all Visual Studio instances.
- Open Visual Studio and click "Create New Project". Search for Milva, then select the template and click next.
- Then follow the steps same as creating a project.



> <b>Note :</b> In case you would like to select "Place solution and project in the same directory" box, you will then need add project to the solution with "Add New Existing Project" option like down below;

![Capture](https://user-images.githubusercontent.com/32344242/135268651-227dc8ed-24a1-4e02-bb53-e1af9edd0c36.PNG)

<br>

># Project Structure

### **Layers;**

- **Localization :** Contains localization key-value pairs.
- **Entity :** Contains database related models. Usually it is preferred that the entity layer is not created as a separate project, but it is a folder within the data layer instead. However, we prefer this structure intentionally, as creating the Entity as a layer makes it easier to implement the business logics using with the Assembly class.
- **Data :** Data access layer.
- **API :** This is the executable part of the project. All the business logic happens here. Endpoints and project-specific business logics for the client are written here. Project configurations, files that will/will not be served to the Client are also defined at this layer.

<br>

># File Structure

<br>

## **Localization Layer**

There is a "Resources" folder to hold the localization key-value pairs, language by language.

- Each file name has the language code at the end. According to these language codes, we do it in order to use the built-in Localization structure in .Net Core. The Resx file without the language extension code is the default language file. This let's us getting rid of all the magic strings using auto generated code.

- SharedResource.cs is a dummy class for access localization key-value pairs.

<br>

## **Entity Layer**

- Classes in root folder represents the database tables,
- The non-virtual properties of these classes represent columns,
- Virtual properties of these classes represent relationships.
- Identity folder contains user related classes.

<br>

## **Data Layer**

- Database connections and configurations made with custom DbContext.
- Concrete folder contains RepositoryBase class for Generic Repository Design Pattern.
- Abstract folder contains RepositoryBase interface for Generic Repository Design Pattern.

<br>

## **API Layer**

Under normal circumstances, the Services folder and a few more folders are created in Core named layer. We consider the layer we call API as the Business layer in other companies' projects. Since project dependencies do not change, we use it this way as managing them as folders in the same layer increases efficiency rather than creating separate layers for them.

### **Folders;**
- **wwwroot :** Contains public static files.
- **AppStartup :** Contains application startup configuration.
- **Controllers :** Contains endpoints which are public to client. No business logic here. All the controllers are similar.
- **DTOS :** Contains Data Transfer Objects available to the client. It provides abstraction of entity models, that is, database structure, from the client.
- **Middlewares :** It contains custom middlewares running on the project.
- **Migrations :** It includes auto-generated migrations and Data Seed components.
- **Services :** The complete business logic of the project is here. This is the most complicated piece. Endpoints call these service methods, and the service methods provide the necessary business logic and perform the related operation(such as CRUD operations). 
- **StaticFiles :** Contains private static files which could be the html templates to be sent as an e-mail.


### **Files;**
- **FodyWeavers.xml :** It is the configuration file of the Fody library that we use to avoid writing ConfigureAwait(false) at the end of asynchronous methods across the project.
- **ProjectName.API.xml :** It is an XML file which is automatically created as a result of the summary we have written. We create Open Api documentation from this file with the Swashbuckle library. If you change a summary, make sure you rebuild the project so it re-generates this file.

<br>


## **Project Dependencies**

<br>

![ApiLayerFileStructure](https://user-images.githubusercontent.com/32344242/142745035-cede3747-ab37-4bf4-b0e2-08c7e50048e2.png)

<br>

### **Projects Build Order**

- Entity
- Localization
- Data
- API
