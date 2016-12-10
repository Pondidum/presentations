### What should be strong typed?

All The Things <!-- .element: class="pic-label quote fragment" data-fragment-index="1"-->

![Strong Type All The Things Image](img/all-the-things.png) <!-- .element: class="fragment" data-fragment-index="1"-->

(mostly)<!-- .element: class="fragment" data-fragment-index="2"-->

http://hyperboleandahalf.blogspot.fi/2010/06/this-is-why-ill-never-be-adult.html<!-- .element: class="attribution fragment" data-fragment-index="1"-->



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
    Uri Endpoint { get; }
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
    private readonly IConfiguration _config;

    public SomeActionHandler(IConfiguration config)
    {
        _config = config;
    }

    public void DoSomething()
    {
        MakeRequest(_config.Endpoint);
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

. <!-- .element: class="attribution" -->
https://fatslowtriathlete.com/wp-content/uploads/2015/03/NO-PAIN-NO-GAIN.jpg <!-- .element: class="attribution" -->



# Identifiers



```csharp
public class Customer
{


    public string Name { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string EmailAddress { get; private set; }
    public Address { get; private set; }
}
```
<!-- .slide: data-transition="slide-in none-out"-->
Note: everyone strong types their entities right?  We have a reasonable entity here...we'll get to it's problems later



```csharp
public class Customer
{
    public int ID { get; private set; }

    public string Name { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string EmailAddress { get; private set; }
    public Address { get; private set; }
}
```
<!-- .slide: data-transition="none-in slide-out"-->
Note: sure, your db column is usually int or guid, but your objects dont have to be.
You don't have Foreign Keys in code...



```csharp
public interface IGetOpenOrders
{
    IEnumerable<Order> Execute(int id);
}
```



```csharp
var orders = orderQuery.Execute(customer.id);

var orders = orderQuery.Execute(product.id);
```



## Use Separate Types
```csharp
using ProductId : int;
using CustomerId : int;
```
Note: Scott Wlaschin has a great talk on this kind of thing with F#



```csharp
public struct CustomerId
{
	private readonly int _key;

	public CustomerId(int key)
	{
		_key = key;
	}
}
```
<!-- .slide: data-transition="slide-in none-out"-->
Note: dont forget to be IEquatable!



```csharp
public struct CustomerId : IEquatable<CustomerId>
{
    private readonly int _key;

    public CustomerId(int key)
    {
        _key = key;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is CustomerId && Equals((CustomerId)obj);
    }

    public bool Equals(CustomerId other) => _key == other._key;
    public override int GetHashCode() => _key;
}
```
<!-- .slide: data-transition="none-in none-out"-->



```csharp
public struct CustomerId : IEquatable<CustomerId>
{
    private readonly int _key;

    public CustomerId(int key)
    {
        _key = key;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is CustomerId && Equals((CustomerId)obj);
    }

    public bool Equals(CustomerId other) => _key == other._key;
    public override int GetHashCode() => _key;

    public static bool operator ==(CustomerId left, CustomerId right)
        => Equals(left, right);

    public static bool operator !=(CustomerId left, CustomerId right)
        => !Equals(left, right);
}
```
<!-- .slide: data-transition="none-in slide-out"-->
Note: don't forget serialization!


```csharp
public interface IOrderQuery
{
    IEnumerable<Order> Execute(CustomerId id);
}
```



```csharp
var orders = orderQuery.Execute(product.id);
```



<!-- .slide: class="gains" -->
![No Pain No Gain](img/no-pain-no-gain-trans.png)
<ul class="left">
    <li class="fragment">Silent Errors</li>
</ul>

<ul class="right">
    <li class="fragment">Compiler Checking</li>
    <li class="fragment warn">Serialization (DB)</li>
    <li class="fragment warn">Serialization (Json)</li>
</ul>

. <!-- .element: class="attribution" -->
https://fatslowtriathlete.com/wp-content/uploads/2015/03/NO-PAIN-NO-GAIN.jpg <!-- .element: class="attribution" -->
