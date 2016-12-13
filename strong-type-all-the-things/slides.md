
## Strong Type All The Things
![Strong Type All The Things Image](img/all-the-things.png)

http://hyperboleandahalf.blogspot.fi/2010/06/this-is-why-ill-never-be-adult.html<!-- .element: class="attribution"-->

Andy Davies | AndyDote.co.uk | github.com/pondidum | @pondidum<!-- .element class="small"-->



## But seriously
* Application Configuration<!-- .element: class="fragment"-->
* Identifiers<!-- .element: class="fragment"-->
* Value Types<!-- .element: class="fragment"-->



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
    <li class="fragment strike" data-fragment-index="1">Magic Strings Everywhere</li>
    <li class="fragment strike" data-fragment-index="2">Coupling to App.Config</li>
</ul>

<ul class="right">
    <li class="fragment" data-fragment-index="1">Centralised strings</li>
    <li class="fragment" data-fragment-index="2">Loose Coupling</li>
    <li class="fragment">Single Responsibility</li>
    <li class="fragment">Testability</li>
    <li class="fragment">Discoverability</li>
</ul>

https://fatslowtriathlete.com/wp-content/uploads/2015/03/NO-PAIN-NO-GAIN.jpg <!-- .element: class="attribution" -->



# Identifiers



```csharp
public class Customer
{


    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
    public Address { get; set; }
}
```
<!-- .slide: data-transition="slide-in none-out"-->
Note: everyone strong types their entities right?  We have a reasonable entity here...we'll get to it's problems later



```csharp
public class Customer
{
    public int ID { get; private set; }

    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
    public Address { get; set; }
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
    <li class="fragment strike" data-fragment-index="1">Silent Errors</li>
</ul>

<ul class="right">
    <li  class="fragment" data-fragment-index="1">Build Time Errors</li>
    <li class="fragment warn">Serialization (DB & Json)</li>
</ul>

https://fatslowtriathlete.com/wp-content/uploads/2015/03/NO-PAIN-NO-GAIN.jpg <!-- .element: class="attribution" -->



# Value Types



```csharp
public class Customer
{
    public CustomerId ID { get; private set; }

    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
    public Address { get; set; }
}
```



```csharp
customer.DateOfBirth = new DateTime(1743, 9, 17);
```
Note: this is a valid date...but is it valid in our domain?
It might be for a family tree, but for an HR system maybe not.



```csharp
customer.EmailAddress = "Dave";
```
Note: what about this?  It compiles...



```csharp
public class Email
{
    private static readonly RegEx EmailExpression = new RegEx(/* ... */);

    private readonly string _email;

    public Email(string address)
    {
        if (EmailExpression.IsMatch(address) == false)
            throw new InvalidEmailException(address);

        _email = address;
    }

    //iequatable, operators omitted...
}
```
Note: Regex is left to an exercise for you...
We can go further with email addresses though, what about proof of ownership?



```csharp
class Email
{
    private static readonly RegEx EmailExpression = new RegEx(/* ... */);

	private readonly string _email;

    public Email(string address)
    {
        if (EmailExpression.IsMatch(address) == false)
            throw new InvalidEmailException(address);

        _email = address;
    }

    public virtual bool Verified => false;
}
```



```csharp
class VerifiedEmail : Email
{
	public VerifiedEmail(VerificationService service, EmailAddress email)
		: base(email.ToString())
	{
		if (service.IsVerified(email) == false)
			throw new UnVerifiedEmailException(email);
	}

	public override bool Verified => true;
}
```



```csharp
public void SendVerificationEmail(Email email)
{
    if (email.Verified)
        throw new EmailAlreadyVerifiedException(email);
    //...
}

public void SendWelcomeEmail(VerifiedEmail email)
{
    //...
}
```



<!-- .slide: class="gains" -->
![No Pain No Gain](img/no-pain-no-gain-trans.png)
<ul class="left">
    <li class="fragment strike" data-fragment-index="1">Silent Errors</li>
    <li class="fragment strike" data-fragment-index="2">Domain Errors</li>
</ul>

<ul class="right">
    <li class="fragment" data-fragment-index="1">Build Time Errors</li>
    <li class="fragment" data-fragment-index="2">Explicit Business Rules</li>
    <li class="fragment">Centralisation</li>
    <li class="fragment warn">Serialization (DB & Json)</li>
</ul>

https://fatslowtriathlete.com/wp-content/uploads/2015/03/NO-PAIN-NO-GAIN.jpg <!-- .element: class="attribution" -->



![Questions](img/questions.jpg)

https://31.media.tumblr.com/22e951b9a924a3f83f03e3c535860e91/tumblr_inline_n32k4mN8SF1qfyjta.jpg <!-- .element: class="attribution" -->
