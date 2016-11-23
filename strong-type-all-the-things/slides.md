

# Things that should be strong typed
* [Strong Type All The Things Image]
* (mostly)



# But seriously
* Application configuration
* IDs
* Value types



# Application Configuration
* Don't spatter your codebase with this:
  ```csharp
  var endpoint = ConfigurationManager.AppSetting["Endpoint"];
  ```
* Or was it `ConfigurationManager.AppSetting["endpoint"]`
* Or was it `ConfigurationManager.AppSetting["service_url"]`
* Or was it `ConfigurationManager.ConnectionStrings["endpoint"].ConnectionString`



# Centralise it
```csharp
public interface IConfiguration {
    Url Endpoint { get; }
}

public class Configuration : IConfiguration
{
    public Url Endpoint { get; }

    public Configuration()
    {
        Endpoint = ConfigurationManager.ConnectionString["Endpoint"].ConnectionString;
    }
}
```



# And inject it
```csharp
public class SomeActionHandler
{
    public SomeActionHandler(IConfiguration config)
    {
    }
}
```
