# Event Sourincg
### Andy Davies
![Whois](img/whois.png)



# What is it?
Note: deltas rather than current State this is how source control works!



# Why use it?
Note: completely lossless
Can construct any other data store, e.g. relational, warehouse



### Example

* Create Vacancy <!-- .element: class="fragment" -->
* Candidate 1 Applied  <!-- .element: class="fragment" -->
* Candidate 2 Applied <!-- .element: class="fragment" -->
* Candidate 1 Interviewed <!-- .element: class="fragment" -->
* Candidate 2 Withdraws <!-- .element: class="fragment" -->
* Candidate 1 Offered Job <!-- .element: class="fragment" -->


*Find all candidates who were interviewed and then withdrew*

```csharp
store
  .Events<CandidateInterviewed>(e => e.CandidateID)
  .FollowedBy<CandidateWithdrawn>(e => e.CandidateID)
```


*Find all people who added a product to their basket more than once*

```csharp
store
  .Events<ProductAdded>(e => e.ProductID)
  .FollowedBy<ProductRemoved>(e => e.ProductID)
  .FollowedBy<ProductAdded>(e => e.ProductID)
  .FollowedBy<ProductRemoved>(e => e.ProductID)
  .Select(e => e.UserID)
```

*Find people who bought a product once the price decreased*

```csharp
store
  .Events<ProductAdded>(e => e.ProductID)
  .FollowedBy<ProductPriceReduced>(e => e.ProductID)
  .Without<ProductRemoved>(e => e.ProductID)
  .FollowedBy<OrderCompleted>()
```
