# Event Sourincg
### Andy Davies
![Whois](img/whois.png)



# What is it?
Note: deltas rather than current State this is how source control works!



# Why use it?
![Eventsourcing Advantage](img/advantage.jpg)

Note: completely lossless
Can construct any other data store, e.g. relational, warehouse



### Example

* Create Vacancy <!-- .element: class="fragment" -->
* Candidate 1 Applied  <!-- .element: class="fragment" -->
* Candidate 2 Applied <!-- .element: class="fragment" -->
* Candidate 1 Interviewed <!-- .element: class="fragment" -->
* Candidate 2 Withdrew <!-- .element: class="fragment" -->
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



![Read performance](img/reading.jpg)
Note: replaying 1000s of events is slow (well comparitively)


![events-separate-snapshots](img/events.png) <!-- .element: class="no-border" -->
Note: conceptual only


![events-separate-snapshots](img/events-separate-snapshots.png) <!-- .element: class="no-border" -->


```sql
select snapshotType, snapshot
from snapshots
where aggregateID = @id
order by sequence desc
limit 1

select e.eventType, e.event
from events e
where e.aggregateID = @id
  and e.sequence > (
    select max(sequence)
    from snapshots s
    where s.aggregateID = @id
  )
order by e.sequence asc
```



![Jack Sparrow Searching](/img/search.jpg)
Note: how do we handle searching?


# Read Models
One model for each purpose
Note: formed for the view/purpose. e.g. search vs candidate screen vs candidates on a vacancy screen
Can also be stored in different places, e.g. sql, elasticsearch, redis


## Candidate Screen
```json
{
 "id": 12345,
 "name": "Andy Davies",
 "emails": [
   { "type": "personal", "address":"andy@home.com" },
   { "type": "work", "address":"andy@work.com" },
 ],
 "addresses": [
    {
      "type": "home",
      "line1": "7 home street",
      "town": "home town",
      "postcode": "bh22 9xr"
    }
 ]
}
```


## Search

```json
{
 "id": 12345,
 "name": "Andy Davies",
 "emails": [ "andy@home.com","andy@work.com" ],
 "phones": ["0721231333","0123123132" ],
 "addresses": [ "home, 7, home street, home town, bh22 9xr" ]
}```
Note: notice how the data we dont care about has been removed, and the object is flattened


## Vacancy
```json
{
  "title": "software developer",
  "company": "Matchtech Group Plc",
  "positions": 1,
  "candidates": [
    {
      "id": 12345,
      "source": "website",
      "applicationStatus": "interview_arranged",
      "date":"2016-06-16T16:32:52.1942008+01:00"
    }
  ]
}
```
Note: notice the candidate data is different here!