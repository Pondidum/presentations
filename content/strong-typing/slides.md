## Strong Type All The Things <!-- .element: class="push-down stroke" -->
## Andy Davies <!-- .element: class="stroke" -->
github.com/pondidum | @pondidum | andydote.co.uk  <!-- .element: class="smaller white" -->

Hulk, by Jonas Thornqvist <!-- .element: class="attribution white" -->

<!-- .slide: data-background="content/strong-typing/img/hulkfan1.jpg" data-background-size="contain" class="intro" -->



# I Have Opinions
Note:
* only opinions...
* who thinks types are usesless?
* who thinks testing is useless?



Unit Testing, correctness vs cost
Note:
* at some point, it's not worth adding to



## Correctness

![cost vs correctness](content/strong-typing/img/correctness-cost-unit.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in fade-out" -->



## Correctness

![cost vs correctness](content/strong-typing/img/correctness-cost-typing.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="fade" -->



## Correctness

![cost vs correctness](content/strong-typing/img/correctness-cost-combined.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="fade-in slide-out" -->



```typescript
export function BuildWelcomeEmail(name: string, email: string): IEmail {
  return {
    to: email,
    name: name,
    subject: "Welcome to Shinra",
    body: "For all you Mako needs"
  };
}

it("should build the email", () => {
  const email = BuildWelcomeEmail("test.nain@koti.fi", "Mrs Nain");

  expect(email.name).toBe("Mrs Nain");
  expect(email.to).toBe("test.nain@koti.fi");
});
```



```typescript
export class Email {
  private _email: string;

  constructor(email: string) {
    this._email = email;
  }

  public toString(): string {
    return this._email;
  }
}
```



```typescript
export function BuildWelcomeEmail(email: Email, name: string): IEmail {
  return {
    to: email,
    name: name
  };
}

const testName = "Mrs Nain";
const testEmail = new Email("test.nain@koti.fi");

it("should build the email", () => {
  const email = BuildWelcomeEmail(testEmail, testName);

  expect(email.name).toBe(testName);
  expect(email.to).toBe(testEmail);
});
```



```typescript
const testName = "Mrs Nain";
const testEmail = new Email("test.nain@koti.fi");

it("should build the email", () => {

  BuildVerificationEmail(testEmail, testName);
  BuildWelcomeEmail(testEmail, testName);
  BuildAccountStatement(testEmail, testName, {});

});
```



```typescript
export class Email {
  public IsVerified: bool;

  private _email: string;

  constructor(email: string) {
    if (!email.match(/^(.+)@(.+)$/)) {
      throw new Error("Invalid email address format");
    }

    this._email = email;
  }

  public toString(): string {
    return this._email;
  }
}
```



```typescript
type Email = UnverifiedEmail | VerifiedEmail;
```

```typescript
class VerifiedEmail {
  private _email: string;

  constructor(email: UnverifiedEmail, IVerification service) {
    service.ThrowIfUnverified(email);
    this._email = email.toString();
  }

  public toString(): string {
    return this._email;
  }
}
```
<!-- .element: class="fragment" -->



```typescript
function BuildVerificationEmail(email: UnverifiedEmail): IEmail

function BuildWelcomeEmail(email: Email, name: string): IEmail

function BuildStatement(email: VerifiedEmail, name: string): IEmail
```



```typescript
import { fetchOrders } from "./orders"

const merchant = new Merchant(1234);
const customer = new Customer(1234);

const orders = fetchOrders(merchant.id);
const orders = fetchOrders(customer.id);
```
Note:
* unit testing wont help you here
* both are valid ids, but integers are the same
* so we have a semantic/runtime error



```typescript
class MerchantID {
  private _key: number;

  constructor(key: number) {
    this._key = key;
  }

  public toKey(): number {
    return this._key;
  }

  public toString(): string {
    return `Merchant: ${this._key}`;
  }
}
```



```typescript
export function fetchOrders(ownerID: MerchantID) {
    //dome db magic
}

export function fetchOrders(ownerID: CustomerID) {
    //dome db magic
}
```



```typescript
import { fetchOrders } from "./orders"

const merchant = new Merchant(1234);
const customer = new Customer(1234);

const merchantOrders = fetchOrders(merchant.id);
const customerOrders = fetchOrders(customer.id);
```



# Configuration



## Questions?
<br />

<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
