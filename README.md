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

## 03 - Created the SQL Database Project
Created and published the SQL database project. When publishing, set the database name and connection, and save the profile in the PublushLocation folder.

## 04 - Created the WPF Project with MVVM setup 
Created a new WPF (.NET framework) project and changed the Assembly name in the Properties setting which will serve as the .exe name when the app is published.

- Created MVVM files and folders and deleted the **MainPage.xaml** file.

- Installed Caliburn.Micro package from NPM and created the Boostrapper.cs file to configure ShellView.xaml as the launch file.

**Bootstrapper.cs**

```
public class Bootstrapper : BootstrapperBase
{
    public Bootstrapper()
    {
        Initialize();
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
        //DisplayRootViewFor<ShellViewModel>();
        DisplayRootViewForAsync<ShellViewModel>();
    }
}
```

- In **App.xaml**, delete the **StartupUri** property and add the following to the **Application.Resources** tag

```
<ResourceDictionary>
    <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary>
            <local:Bootstrapper x:Key="Bootstrapper" />
        </ResourceDictionary>
    </ResourceDictionary.MergedDictionaries>
</ResourceDictionary>
```

## 05 - Dependency Injecion in WPF 
Additional code in the **Bootstrapper.cs** file that includes the Simple Container DI system.

## 06 - Register SQL Database Tables 
Created tables in the **RMSDta** SQL Project and published them to the local database by launching **RMSData.publish.xml** file located in the **PublishLocations** folder.

## 07 - Create and Wipre up WPF Login Form to API
**ShellView.xaml** code used to launch **LoginView.xaml** WPF Window

```
<DockPanel>
    <Menu DockPanel.Dock="Top" FontSize="15">
        <MenuItem Header="_File">
        </MenuItem>

        <MenuItem Header="_Account">
                <MenuItem x:Name="LoginScreen" Header="_Login" />
        </MenuItem>
    </Menu>

    <Grid>
        <ContentControl x:Name="ActiveItem" Margin="5" />
    </Grid>
</DockPanel>

```
**ShellViewModel.cs**
```
using Caliburn.Micro;

namespace RMSDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private LoginViewModel _loginViewModel;
        public ShellViewModel(LoginViewModel loginViewModel)
        {
            _loginViewModel = loginViewModel;
            ActivateItemAsync(_loginViewModel);
        }
    }
}
```
Caliburn.Micro support for PasswordBox. The PasswordBox does not bind well with MVVM (making it less secure), but this approach is to make sure that the password is never stored in clear text


https://stackoverflow.com/questions/30631522/caliburn-micro-support-for-passwordbox


Wiring up the login form to the API (authenticate against the database)
- Installed **Microsoft.AspNet.WebAPI.Client (Newtonsoft.json)** package
- Created **APIHelper.cs** helper class and **AuthenticatedUser.cs** model class
- Updated the solutions property to run the WPF and API simultaneously and updated the **App.config** file
```
<appSettings>
	<add key="api" value="https://localhost:44341/" />
</appSettings>
```
- Updated the LoginViewModel class
```
private IAPIHelper _apiHelper;

...

public LoginViewModel(IAPIHelper aPIHelper)
{
    _apiHelper = aPIHelper;
}

...

public async Task LogIn()
{
    try
    {
        var result = await _apiHelper.Authenticate(UserName, Password);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
```
- Updated the Bootstrapper.cs file
```
_container
    .Singleton<IWindowManager, WindowManager>()
    .Singleton<IEventAggregator, EventAggregator>()
    .Singleton<IAPIHelper, APIHelper>();
```

## 08 - Login Form Error Handling 
A new label added to display an error when attempting to log in.
- Updated the label's visibility binding properties
```
Visibility="{Binding IsErrorVisible, Converter={StaticResource BooleanToVisibilityConverter}, FallbackValue=Collapsed}"
```
- Updated the **App.xaml** file
```
<BooleanToVisibilityConverter x:Key="BooleanToVisibilityConverter" />
```
- Updated the **LoginViewModel** class
```
public bool IsErrorVisible
{
    get 
    {
        bool output = false;
        if (ErrorMessage?.Length > 0)
        {
            output = true;
        }
        return output;
    }
}

private string _errorMessage;

public string ErrorMessage
{
    get { return _errorMessage; }
    set 
    {
        _errorMessage = value;
        NotifyOfPropertyChange(() => IsErrorVisible);
        NotifyOfPropertyChange(() => ErrorMessage);
    }
}

...

public async Task LogIn()
{
    try
    {
        ErrorMessage = "";
        var result = await _apiHelper.Authenticate(UserName, Password);
    }
    catch (Exception ex)
    {
        ErrorMessage = ex.Message;
    }
}
```

## 09 - Getting User Data upon Login

Created a **spUserLookup** SQL stored procedure in the **RMSData** database project
```
CREATE PROCEDURE [dbo].[spUserLookup]
	@Id nvarchar(128)
AS
begin
	set nocount on;

	SELECT Id, FirstName, LastName, EmailAddress, CreatedDate
	FROM [User]
	WHERE Id = @Id;
end
```
Created a UserController class in **RMSDataManager** (Web API) project to pull user login data
```
[Authorize]
[RoutePrefix("api/User")]
public class UserController : ApiController
{
    [HttpGet]
    public UserModel GetById()
    {
        string id = RequestContext.Principal.Identity.GetUserId();
        UserData data = new UserData();

        return data.GetUserById(id).First();
    }
}
```
Updated the **Web.config** file to include the connection string to the RMS database
```
<add name="RMSData" connectionString=""/>
```
Created a new class library project referenced by RMSDataManager (Web API)
- Installed **Dapper** from NPM
- Created an **SqlDataAccess** class to communicate with the database
```
internal class SqlDataAccess
    {
        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        { ... }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        { ... }
    }
```
- Created a **UserData** class to work with the SqlDataAccess class and the **spUserLookup** stored procedure
```
public class UserData
{
    public List<UserModel> GetUserById(string Id)
    {
        SqlDataAccess sql = new SqlDataAccess();

        var p = new { Id = Id };

        var output = sql.LoadData<UserModel, dynamic>("spUserLookup", p, "RMSData");
        return output;
    }
}
```
Created a new class library project referenced by RMSDesktopUI (WPF app)
- Created a **LoggedInUserModel** class and moved the API helpers and models from the WPF app to the class library
- Updated the **APIHelper** class to get the logged in user info using the user's token
```
private ILoggedInUserModel _loggedInUser;

public APIHelper(ILoggedInUserModel loggedInUser)
{
    InitializeClient();
    _loggedInUser = loggedInUser;
}

...

public async Task GetLoggedInUserInfo(string token)
{
    apiClient.DefaultRequestHeaders.Clear();
    apiClient.DefaultRequestHeaders.Accept.Clear();
    apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
    apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

    using (HttpResponseMessage response = await apiClient.GetAsync("/api/User"))
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<LoggedInUserModel>();
            _loggedInUser.CreatedDate = result.CreatedDate;
            _loggedInUser.EmailAddress = result.EmailAddress;
            _loggedInUser.FirstName = result.FirstName;
            _loggedInUser.LastName = result.LastName;
            _loggedInUser.Id = result.Id;
            _loggedInUser.Token = token;
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }
}
```
- Updated the **Bootstapper** file
```
 _container
    .Singleton<IWindowManager, WindowManager>()
    .Singleton<IEventAggregator, EventAggregator>()
    .Singleton<ILoggedInUserModel, LoggedInUserModel>()
    .Singleton<IAPIHelper, APIHelper>();
```

## 10 - Sales Page Creation
Added a sales page XAML page and the supporting View Model. Caliburn.Micro is supported by inheriting the **Sreen** class