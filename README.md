# HIT-Project
### Project Overview

To keep it short and simple, the HIT-Project is meant to provide the HIT Students with a new and improved way to 
practice data entry of new birth records. Features include adding patients, adding patient children, adding a birth record, and eventually
printing of said record form.

### Database

Database configuration is handled in the web.config and should automatically connect to remote version of the database.

### Structure

<dl>
  <dt>Controllers/</dt>
  <dd>
    This contains all controllers for backend management of database information. This is the only place that will handle interactions       with the database.
  </dd>


  <dt>Views/</dt>
  <dd>
    Contains all of the front end pages that the client will see.
  </dd>

  <dt>Model/</dt>
  <dd>
    This contains the .edmx file imported from the remote database. This will need to be updated if the schema is changed.
  </dd>

  <dt>Content/</dt>
  <dd>
    Houses all of the css, we will use the provided Site.css to implement any css changes.
  </dd>

  <dt>App_Start/</dt>
  <dd>
    Contains the config files.
  </dd>

  <dt>Scripts/</dt>
  <dd>
    This has all of the Javascript files used in the project. Any files added must be added to the BundleConfig.cs in the App_Start         directory.
  </dd>

</dl>

### Dependancies

**Microsoft Visual Studio**

The HIT-Project is build on ASP.NET and will require the use of Microsoft Visual Studio to successfully participate in development and testing. 
Microsoft Visual Studio 2015 is recommended.

**Microsoft SQL Management Studio**

While the database does live online, and the connection information is in the project, Microsoft SQL Management Studio is suggested 
for any database management outside of the application.
