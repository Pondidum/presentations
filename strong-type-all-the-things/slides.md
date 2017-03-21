## Strong Type All The Things
![Strong Type All The Things Image](img/all-the-things.png)

http://hyperboleandahalf.blogspot.fi/2010/06/this-is-why-ill-never-be-adult.html<!-- .element: class="attribution"-->

### Andy Davies
### AndyDote.co.uk | github.com/pondidum | @pondidum
Note: there are two (and a bit areas to talk about). First Configuration



```csharp
var x = ConfigurationManager.AppSetting["some_url_name"];
```

```csharp
var x = ConfigurationManager.AppSetting["someUrlName"];
```
<!-- .element: class="fragment" -->

```csharp
var x = ConfigurationManager.ConnectionString["someUrlName"].ConnectionString;
```
<!-- .element: class="fragment" -->
Note:
* All too often...
* magic strings
* typing
  * what is the type here? uri? ipaddress? name of someone to go and ask?
  * Sleeper errors
* coupling (speed)
* visibility
  * refactor?
  * libraries which depend on ConfigurationManager



```csharp
public interface IConfiguration {
    Uri Endpoint { get; }
    TimeSpan Timeout { get; }
}

public class Configuration : IConfiguration {
    public Uri Endpoint { get; }
    TimeSpan Timeout { get; }

    public Configuration() {
        Endpoint = new Uri(ConfigurationManager.AppSetting["XyzEndpoint"]);
        Timeout = TimeSpan.Parse(ConfigurationManager.AppSetting["MaxWait"]);
    }
}
```
Note:
* This solves problems:
  * coupling - injected
  * testable - test the config, or fake the config for others
  * Discoverability - one place to find all configuration usage
  * stronger-typing - all prop assignment is done on ctor.  errors fast.
* Stronk!


```csharp
public class Configuration : IConfiguration {

    public Uri Endpoint { get; private set; }
    TimeSpan Timeout { get; private set; }

    public Configuration() {
        this.FromAppConfig(); // magic...
    }
}
```



# Primitive Types
Note:
* Or rather their overuse
* Two areas
* first is identifiers



```csharp
public class Customer {
  public int Id { get; set; }
}

public class Merchant {
  public int Id { get; set; }
}
```
Note:
* Looks pretty reasonable
* Given that...



```csharp
public interface IGetOpenOrders {
    IEnumerable<Order> Execute(int id);
}

var orders = orderQuery.Execute(customer.Id);

var orders = orderQuery.Execute(merchant.Id);
```
Note:
* which of these calls is correct?
  * intellitype can help
  * but only if you've named your parameters
  * or if you use named arguments for all calls
* why not use the compiler?



```csharp
using ProductId : int;
using CustomerId : int;
```
Note: lolnope



```csharp
public struct CustomerId {
    private readonly int _key;

    public CustomerId(int key) {
        _key = key;
    }
}
```
<!-- .slide: data-transition="slide-in none-out" -->
Note:
* dont forget IEquatable, serialization (http, db)


```csharp
public struct CustomerId : IEquatable<CustomerId> {
  private readonly int _key;

  public CustomerId(int key) {
    _key = key;
  }

  public override bool Equals(object obj) {
    if (ReferenceEquals(null, obj)) return false;
    return obj is CustomerId && Equals((CustomerId)obj);
  }

  public bool Equals(CustomerId other) => _key == other._key;
  public override int GetHashCode() => _key;

  public static bool operator ==(CustomerId left, CustomerId right)
      => left.Equals(right);
  public static bool operator !=(CustomerId left, CustomerId right)
      => !left.Equals(right);
}
```
<!-- .slide: data-transition="none-in slide-out" -->
Note:
* More work than ideal, but at least the compiler will help you now



```csharp
public interface IOrderQuery {
    IEnumerable<Order> Execute(CustomerId id);
}

//this will compile
var orders = orderQuery.Execute(customer.Id);

//this won't :)
var orders = orderQuery.Execute(merchant.Id);
```



# Let's talk domain



```csharp
public class Person {
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
}
```
Note: given the following, whats wrong with it?



```csharp
person.DateOfBirth = new DateTime(1743, 9, 17);
```
Note:
* having a 273 year old person is probably not valid for a webshop
  * unless you are selling anti-aging products maybe
* but if you are a family tree site, this is probably fine



```csharp
customer.EmailAddress = "Dave";
```
Note:
* just because all emails are strings, doesn't mean all emails are strings



```csharp
public class Email {

    private static readonly RegEx EmailExpression = new RegEx(/* ... */);
    private readonly string _email;

    public Email(string address) {
        if (EmailExpression.IsMatch(address) == false)
            throw new InvalidEmailException(address);

        _email = address;
    }
}
```
<!-- .slide: data-transition="slide-in none-out" -->
Note:
* I leave the regex for you to decide on
  * I'm happy with "contains some text either side of an @"
* Now we can have only valid emails...kinda
  * valid vs verified


```csharp
public abstract class Email {

    private static readonly RegEx EmailExpression = new RegEx(/* ... */);
    private readonly string _email;

    protected Email(string address) {
        if (EmailExpression.IsMatch(address) == false)
            throw new InvalidEmailException(address);

        _email = address;
    }

    public abstract bool IsVerified { get; }
}
```
<!-- .slide: data-transition="none-in slide-out" -->
Note: All we've really done is make it abstract, and added an abstract property



```csharp
public class UnVerifiedEmail : Email {

    protected Email(string address) : base(address)
    {}

    public abstract bool IsVerified => false;
}
```
Note:
No different than before right?



```csharp
public class VerifiedEmail : Email
{
  public VerifiedEmail(VerificationService service, UnVerifiedEmail email)
      : base(email) {

      if (service.IsVerified(email) == false)
          throw new UnVerifiedEmailException(email);
  }

  public override bool IsVerified => true;
}
```
Note:
* new domain concept!
* how verification is achived isn't it's concern
* can only make a verified email from an unverified email



```csharp
public void SendVerificationEmail(UnVerifiedEmail email) {
    //...
}

public void SendWelcomeEmail(VerifiedEmail email) {
    //...
}
```



# strong type all the things?

## at least think about it :) <!-- .element: class="fragment" -->



# Andy Davies
### AndyDote.co.uk | github.com/pondidum | @pondidum
