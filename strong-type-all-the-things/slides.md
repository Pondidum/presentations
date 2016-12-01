### What should be strong typed?

All The Things <!-- .element: class="pic-label quote fragment" data-fragment-index="1"-->

![Strong Type All The Things Image](img/all-the-things.png) <!-- .element: class="fragment" data-fragment-index="1"-->

(mostly)<!-- .element: class="fragment" data-fragment-index="2"-->

fatslowtriathlete.com/wp-content/uploads/2015/03/NO-PAIN-NO-GAIN.jpg<!-- .element: class="attribution fragment" data-fragment-index="1"-->



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
public interface IConfiguration
{
    Url Endpoint { get; }
}

public class Configuration : IConfiguration
{
    public Uri Endpoint { get; }

    public Configuration()
    {
        Endpoint = new Uri(ConfigurationManager.AppSetting["XyzEndpoint"]);
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



<!-- .slide: class="gains" -->
![No Pain No Gain](img/no-pain-no-gain-trans.png)
<ul class="left">
    <li>Magic Strings Everywhere</li>
    <li>Coupling to App.Config</li>
</ul>

<ul class="right">
    <li>Single Responsibility</li>
    <li>Testability</li>
    <li>Discoverability</li>
</ul>

fatslowtriathlete.com/wp-content/uploads/2015/03/NO-PAIN-NO-GAIN.jpg<!-- .element: class="attribution" -->
