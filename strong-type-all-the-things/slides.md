### Things that should be strong typed
* [Strong Type All The Things Image]
* (mostly)



### But seriously
* Application configuration
* IDs
* Value types



# Application Configuration



```csharp
var url = ConfigurationManager.AppSetting["Endpoint"];
```

```csharp
var url = ConfigurationManager.AppSetting["endpoint"];
```
<!-- .element: class="fragment" -->

```csharp
var url = ConfigurationManager.AppSetting["service_url"];
```
<!-- .element: class="fragment" -->

```csharp
var url = ConfigurationManager.ConnectionStrings["endpoint"].ConnectionString;
```
<!-- .element: class="fragment" -->



### Centralise it
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



### And inject it
```csharp
public class SomeActionHandler
{
    public SomeActionHandler(IConfiguration config)
    {
    }
}
```
