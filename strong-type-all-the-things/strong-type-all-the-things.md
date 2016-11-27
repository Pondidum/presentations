# Strong Type All The Things!


* example bug at work

endpoint delivering a dto with

```csharp
class Invoice {
  public decimal TotalAmount { get; set; }
}
```

New change:
```csharp
class Invoice {
  public decimal PurchaseAmount { get; set; }
  public decimal InvoiceFee { get; set; }
  public decimal TotalAmount { get; set; }
}
```

Somewhere, this was happening:

var total = invoice.TotalAmount + invoice.InvoiceFee

This isn't valid...but what is? What are our rules?

* purchaseAmount + purchaseAmount => purchaseAmount
* purchaseAmount + invoiceFee => totalAmount

* there is only one invoiceFee per invoice
* an invoice can have multiple items in it
* items can have 1 or higher quantity
* we dont combine invoices, so no need to add fees together
* a total is total item cost + invoice fee
* anything else is not valid.


If we model this with value types, we could end up with:

* ItemPrice(decimal)
* ItemQuantity(int)
* InvoiceFee(decimal)

* LineTotal(ItemPrice price, int quantity)
* PurchaseTotal(LineTotal[] lineTotals)
* Total(PurchaseAmount, InvoiceFee)

We can now add operator overloads to model our allowed rules:

```csharp
public static LineTotal operator *(ItemPrice price, ItemQuantity quantity)
{
    return new LineTotal(price.Value * quantity.Value);
}

public static PurchaseTotal Total(IEnumerable<LineTotal> lines)
{
    return new PurchaseTotal(lines.Sum(line => line.Value));
}

public static Total operator +(PurchaseTotal total, InvoiceFee fee)
{
    return new Total(total.Value + fee.Value);
}
```

If you don't like using operator overloads, static methods on the relevant types can work well too:

```csharp
public struct Total {

  public static Total For(PurchaseTotal purchaseTotal, InvoiceFee invoiceFee) {
      //...
  }
}
```


```charp
// shame this doesn't exist
using PurchaseAmount : decimal;
```

So we have to use structs

```csharp
public struct PurchaseAmount {}

public struct InvoiceFee {}

public struct TotalAmount {}
```
