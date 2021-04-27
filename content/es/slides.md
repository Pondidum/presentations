# Event Sourcing
### Andy Davies
github.com/pondidum | andydote.co.uk  <!-- .element: class="smaller" -->



| ID | Name | Joined | Address |
|---|---|---|---|
| 51 | Andy | 2019-04-08 | Yliopistonkatu 44, 00100 |



```sql
update employees
set address = "Yliopistonkatu 4, 00100"
where id = 51
```

```sql
update employees
set address = "Läntinen Rantakatu 15, 20100"
where id = 51
```



# Store Events, Not State



# Events

- Something Happened
- Domain Specific
- Named in the past-tense
- Immutable



```js
{
  type: "employee_created",
  id: 51,
  name: "Andy"
}
```

```js
{
  type: "home_address_added",
  address: "Yliopistonkatu 44, 00100"
}
```
<!-- .element: class="fragment" -->

```js
{
  type: "employee_started",
  firstDay: "2019-04-08"
}
```
<!-- .element: class="fragment" -->

```js
{
  type: "home_address_corrected",
  address: "Läntinen Rantakatu 15, 20100"
}
```
<!-- .element: class="fragment" -->



# Aggregate

- Single thing in your domain
- Has a strong identity
- Consistency boundary



![Process](content/es/img/process.png)



```js
api.post('/:employeeId/address', async (req, res) => {

  const employee = await loadEmployee(req.params.employeeId)
  employee.correctAddress(req.body)

  await saveAggregate(employee)
})
```



```js
function loadEmployee(employeeId) {

  const events = await sql.select(
    "select * from events where aggregate = @id order by sequence",
    { id: employeeId }
  );

  let employee = blankEmployee()

  for (let event in events) {
    employee.onEvent(event)
  }

  return employee
}
```



```js
class Employee {

  onEvent(event) {
    switch (event.type) {

      case "employee_created":
        this.id = event.id
        this.name = event.name

      case "address_corrected":
      case "address_changed":
        this.address = event.address

    }
  }
}
```



> Paanis: You replay all the events???



```js
class Employee {

  correctAddress(newAddress) {
    if (this.address === newAddress) {
      return
    }

    if (validateAddress(newAddress) === false) {
      return
    }

    this.apply({
      type: "address_corrected",
      address: newAddress
    })
  }
}
```



```js
class Employee {

  apply(event) {
    this.pendingEvents.append(event)
    this.onEvent(event)
  }

  onEvent(event) {
    switch (event.type) {

      case "employee_created":
        this.id = event.id
        this.name = event.name

      case "address_corrected":
      case "address_changed":
        this.address = event.address

    }
  }
}
```
<!-- .element: class="full-height" -->



```js
function saveAggregate(aggregate) {

  if (aggregate.sequence !== sql.getCurrentSequence(aggregate.id)) {
    throw "Aggregate has changed since you loaded it"
  }

  const events = aggregate.pendingEvents()
  const tx = sql.startTransaction()

  for (let event in events) {
    event.sequence = aggregate.sequence++;
    writeEvent(tx, aggregate.id, event);
  }

  tx.commit()

  aggregate.clearPendingEvents()
}
```
<!-- .element: class="full-height" -->



![Process](content/es/img/process.png)



# Projections

Process **Events** into **Views**

or

Send messages to external systems



# Views

A read only perspective on data



```js
function project(state, event) {
  switch (event.type) {

    case "employee_created":
      sql.execute(
        "insert into all_users (id, name) values ($1, $2)",
        event.id,
        event.name)

    case "address_corrected":
    case "address_changed":
      sql.execute(
        "update all_users set address = $1 where id = $2",
        event.address,
        event.aggregateId)
  }
}
```



# Tech



## Storage

- SQL (postgres!)
- EventStore (https://www.eventstore.com/)
- Files (really)



### Kafka? Probably Not

- 1 topic for all aggregates
- topic for each aggregate type
- topic for each aggregate instance

Note:
- RavenDB is a footgun



## Libraries

- C#: Marten (document + eventstore on postgres)
- Go: I wrote one, don't use it



# Event Sourcing: The Bad Parts



## Mindset Shift



```sql
update  invoices
set     paid = 1,
        paid_at = CURRENT_TIMESTAMP
where   userid = $1
```
<!-- .element: class="pull-left"-->

```
- Create an event
- Raise the event
- Update aggregate event handler
- Update projections
- Rebuild views
```
<!-- .element: class="pull-right"-->



## Identifiers

- Should be stable
- Should be queryable
- Don't use emails!



```js
app.post("/project/:projectName", async (req, res) => {

  const view = await loadProjectsView()

  const projectName = req.params.projectName
  const projectId = view.byName(projectName).aggregateId

  const project = await loadProject(projectId);

  // ...
})
```



## PII & GDPR

- Keep it out of the event stream
- Rewrite history
- Encrypt it in events



```js
class Employee {

  onEvent(event) {
    switch (event.type) {

      case "employee_created":
        this.id = event.id

        const key = employeeDecryptionKey(employee.id);
        const pii = decrypt(key, event.encrypted);

        this.name = pii.name
        this.dateOfBirth = pii.dateOfBirth

    }
  }
}
```



## Versioning

- Semantic Changes
- Schema Upgrades



### Upgrade on read

```js
{
  version: 1,
  temperateure: 25  // Celcius or Fahenheit?
}
```

```js
{
  version: 2,
  temperateureCelcius: 25
  temperateureFahenheit: 77
}
```



```js
function upgrade(event) {

  if (event.version === 1) {
    event.version = 2
    event.temperateureCelcius = event.temperateure
    event.temperateureFahenheit = (event.temperature * 9 / 5) + 32
  }

  return event
}
```



### Rewrite history



1. Read from events
2. upgrade event(s)
3. Write to events_new
4. Switch applications to read events_new



## Fixing Mistakes

```js
{
  type: "interest_added"
  amount: "35.73"
}
```



# NO CHANGING EVENTS



### Compensating Events

```js
{
  type: "interest_reverted"
  percentage: "35.73"
  event: 235 // id of event we're undoing
}
{
  type: "interest_added"
  amount: "45.21"
}
```
Note:
- same as git revert



## Eventual Consistency

- Run projections with random delay in dev
- Run projections synchronously



## Snapshots

Only if you need to!

```sql
select  *
from    snapshots
where   aggregate = @id
limit 1
```

```sql
select   *
from     events
where    aggregate = @id
  and    sequence > @snapshot
order by sequence
```



# Usages

- Audit logs
- state machines
- replay / debugging
- long running processes



# EventStorming

![evenstorming](content/es/img/eventstorming.png)

https://buildplease.com/pages/modeling-better/ <!-- .element: class="attribution" -->



## Questions?
<br />

* https://martinfowler.com/eaaDev/EventSourcing.html
* https://leanpub.com/esversioning
* http://ziobrando.blogspot.com/2013/11/introducing-event-storming.html
* https://buildplease.com/pages/modeling-better/

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
