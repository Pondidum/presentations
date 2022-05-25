# Observability Driven Development <!-- .element: class="stroke-black white text-right" -->
## Andy Davies <!-- .element: class="stroke-black white text-right" -->
github.com/pondidum | andydote.co.uk  <!-- .element: class="white smaller text-right" -->

https://images.newscientist.com/wp-content/uploads/2018/12/21114218/hubble.jpg <!-- .element: class="attribution" -->

<!-- .slide: data-background="content/observability/img/hubble.jpg" data-background-size="cover" class="intro" -->



> Observability is a measure of how well internal states of a system can be inferred from knowledge of its external outputs

https://en.wikipedia.org/wiki/Observability <!-- .element: class="attribution" -->



![3 store pillars](content/odd/img/3-pillars.jpg)
https://www.irmi.com/articles/expert-commentary/three-pillars-of-litigation-management <!-- .element: class="attribution" -->
<!-- .slide: data-transition="slide-in fade-out" -->



![3 store pillars](content/odd/img/3-pillars-bs.jpg)
https://st3.depositphotos.com/1031343/13382/v/450/depositphotos_133827724-stock-illustration-bullshit-sign-or-stamp.jpg <!-- .element: class="attribution" -->
<!-- .slide: data-transition="fade-in slide-out" -->



## Everything contributes to observability

- Metrics <!-- .element: class="fragment" -->
- Logs <!-- .element: class="fragment" -->
- Tracing <!-- .element: class="fragment" -->
- Pingdom <!-- .element: class="fragment" -->
- Twitter rage <!-- .element: class="fragment" -->



# Observability Driven Development
<!-- .slide: data-transition="slide-in fade-out" -->



# Hypothesis Driven Development
<!-- .slide: data-transition="fade-in slide-out" -->



1. Hypothesis
2. Measure Current
3. Implement
4. Measure Change



# 1. Hypothesis

_How can I tell if this change is working?_



> Switching this library out should have no noticeable effect



> Changing to xxxx HTTP library should increase responsiveness of the UI



> Moving the signups button should give higher conversions



# 2. Measure Current

...how?



## Metrics..?

* Lossy
* Contextless
* Expensive
* Low Cardinality

Note:
- request time
- can't figure out in relation to what



> Twitter: Omg your site is down _AGAIN_

> You: Dashboard is green



## Metrics..?

* Lossy
* Contextless
* Expensive
* Low Cardinality



# Cardinality

> is a measure of uniqueness in a set of data



| Cardinality | Examples |
|----------|--------|
| Infinite |  guids |
| High |  names, ip addresses, mac addresses |
| Medium |  Browser, url path |
| Low |  HTTP Verb, Http status code |
| None |  home planet |

Note:
- Higher is good, we can filter by it



> Twitter: Omg your site is down _AGAIN_

> You: Dashboard is green



> The site is timing out for users in Europe, on Android phones, using Firefox, only when roaming



## Structured Logs?

* Verbose
* Repetitive
* Expensive
* Need Indexing




| Timestamp | UserID      | Message                      |
|----------|--------------|------------------------------|
| 12:51:27 | 3fcce385be9e | fetched 3rd party prefrences |
| 12:51:27 | 3fcce385be9e | found user in cache          |
| 12:51:27 | 915d273db25c | fetched 3rd party prefrences |
| 12:51:27 | 3fcce385be9e | saved successfully           |
| 12:51:27 | 8507d369d11c | fetched 3rd party prefrences |
| 12:51:27 | c4e71b4a29f2 | fetched 3rd party prefrences |
| 12:51:27 | 915d273db25c | saved successfully           |
| 12:51:27 | c4e71b4a29f2 | found user in cache          |
| 12:51:27 | c4e71b4a29f2 | saved successfully           |
| 12:51:27 | 8507d369d11c | found user in cache          |
| 12:51:27 | 8507d369d11c | saved successfully           |

<!-- .element: class="small" -->



| Timestamp | UserID      | Fetched | In Cache | Saved |
|-----------|-------------|---------|----------|-------|
| 12:51:27 | 3fcce385be9e |  true   |   true   |  true |
| 12:51:27 | 915d273db25c |  true   |   false  |  true |
| 12:51:27 | 8507d369d11c |  true   |   true   |  true |
| 12:51:27 | c4e71b4a29f2 |  true   |   true   |  true |

<!-- .element: class="small" -->



# Traces

> Structured Logs on Steroids

- Smaller
- Queryable
- Linked



<!-- .slide: data-background="/content/odd/img/trace.png" data-background-size="contain"-->



![Open Telemetry Logo](/content/odd/img/opentelemetry-horizontal-color.png) <!-- .element: class="no-border" -->

- Language agnostic
- Vendor agnostic
- Enrichable
- Handles logs and metrics too



## If you are writing logs, you are doing it wrong

![troll-peek](/content/odd/img/troll-peek.png) <!-- .element: class="no-border" -->



# 2. Measure Current

If you don't have tracing in place, add it and deploy!



# 3. Implement Change



```ts
try {
  const params = {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body)
  }

  return fetch(remoteUrl, params)
    .then(validateHttpSuccessCode)
    .then(res => res.json());

} catch (ex) {
  span.recordException(ex);
  span.setStatus({ code: otel.SpanStatusCode.ERROR });
} finally {
  span.end()
}
```
<!-- .element class="full-height" -->
<!-- .slide: data-transition="slide-in fade-out" -->



```diff
+ const axiosEnabled = ldClient.variation("enable_axios", ldUser, false) as boolean;
+ span.setAttribute("axiosEnabled", axiosEnabled);

try {

+   if (axiosEnabled) {
+     return axios.post(remoteUrl, { body }).then(res => res.data);
+   }

  const params = {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body)
  }

  return fetch(remoteUrl, params)
    .then(validateHttpSuccessCode)
    .then(res => res.json());

} catch (ex) {
  span.recordException(ex);
  span.setStatus({ code: otel.SpanStatusCode.ERROR });
} finally {
  span.end()
}
```
<!-- .element class="full-height" -->
<!-- .slide: data-transition="fade-in slide-out" -->



# 4. Measure After



## Experiment Succeeded!

- Remove old implementation
- deploy again!
- Go to Step 1. Hypothesis



## Experiment Failed!

- Switch off the feature flag
- Go to Step 1. Hypothesis



# Bug Fixing



```ts
const getToken = async (accountId: string, accountToken: string): Promise<string | null> => {

  const cacheKey = getTokenCacheKey(accountId)

  try {
    return cacheGet<string>(cacheKey)
  } catch (error) {
    console.error(`Error in reading token from cache: ${error}`)
  }

  const newToken = await fetchToken(accountId, accountToken)

  await cachePut<string>(cacheKey, newToken)

  return newToken
};
```



```diff
const getToken = async (accountId: string, accountToken: string): Promise<string> => {
+ trace.setAttribute("account_id", accountId)
+ trace.setAttribute("account_token_hash", hash(accountToken))

  const cacheKey = getTokenCacheKey(accountId)

+ trace.setAttribute("cache_key", cacheKey)

  try {
    const cachedToken = await cacheGet<string>(cacheKey)
+   trace.setAttribute("read_cached_token", true)
+   trace.setAttribute("cached_token_hash", hash(cachedToken))

    return cachedToken
  } catch (error) {
+   trace.setAttribute("error_reading_cache", error)
    console.error(`Error in reading token from cache: ${error}`)
  }

  const newToken = await fetchToken(accountId, accountToken)
+ trace.setAttribute("new_token_hash", hash(newToken))

  await cachePut<string>(cacheKey, newToken)
+ trace.setAttribute("cached_new_token", true)

  return newToken
};
```

<!-- .element class="full-height" -->



| account_id | read_cached_token | cached_token_hash |
|------------|-------------------|-------------------|
| 1234       | true | d41d8cd98f00b204e9800998ecf8427e |
| 81237      | true | d41d8cd98f00b204e9800998ecf8427e |
| 9534       | true | 62134694a0a06161b3c56db70534a864 |
| 45345      | true | d41d8cd98f00b204e9800998ecf8427e |
| 16494      | true | fe5e7a05ffeaf8c69cba219bfe1f3168 |
| 342324     | true | 7247351092ab63ed0398abfee8b8da1f |



```sql
select count(*), cached_token_hash
from traces
group by cached_token_hash
order by count(*)
```

| count | cached_token_hash                |
|-------|----------------------------------|
| 78923 | d41d8cd98f00b204e9800998ecf8427e |
| 47    | fe5e7a05ffeaf8c69cba219bfe1f3168 |
| 19    | 7247351092ab63ed0398abfee8b8da1f |




```bash
echo -n "" | md5sum   # d41d8cd98f00b204e9800998ecf8427e
```



```typescript
  try {
    const cachedToken = await cacheGet<string>(cacheKey)
    trace.setAttribute("read_cached_token", true)
    trace.setAttribute("cached_token_hash", hash(cachedToken))

    return cachedToken

  } catch (error) {
    trace.setAttribute("error_reading_cache", error)
    console.error(`Error in reading token from cache: ${error}`)
  }

  const newToken = await fetchToken(accountId, accountToken)
  trace.setAttribute("has_new_token", Bool(newToken))
  trace.setAttribute("new_token_hash", hash(newToken))

  await cachePut<string>(cacheKey, newToken)
  trace.setAttribute("cached_new_token", true)

  return newToken
```

<!-- .element class="full-height" -->



```diff
  try {
    const cachedToken = await cacheGet<string>(cacheKey)
    trace.setAttribute("read_cached_token", true)
+   trace.setAttribute("has_cached_token", Bool(cachedToken))
    trace.setAttribute("cached_token_hash", hash(cachedToken))

-   return cachedToken
+   if (cachedToken) {
+     return cachedToken
+   }

  } catch (error) {
    trace.setAttribute("error_reading_cache", error)
    console.error(`Error in reading token from cache: ${error}`)
  }

  const newToken = await fetchToken(accountId, accountToken)
+ trace.setAttribute("has_new_token", Bool(newToken))
  trace.setAttribute("new_token_hash", hash(newToken))

  await cachePut<string>(cacheKey, newToken)
  trace.setAttribute("cached_new_token", true)

  return newToken
```

<!-- .element class="full-height" -->



```sql
select count(*), new_token_hash
from traces
where has_cached_token == false
group by new_token_hash
order by count(*)
```

| count | new_token_hash                   |
|-------|----------------------------------|
| 25    | 52195dbf41504a9db796ca1fd2790ac7 |
| 4     | 555f724a70b04907b56eb688c50cc1f1 |




<!-- .slide: data-background="/content/odd/img/honeycomb-bubble.png" data-background-size="contain"-->



# Getting Started

1. Pick your worst code
2. Instrument with OpenTelemetry
3. Send to a Vendor



# Vendors

- ![honeycomb.io logo](/content/odd/img/honeycomb.png)
- ![Lightstep logo](/content/odd/img/lightstep.png)

<!-- .element: class="list-unstyled list-spaced" -->



# Local Dev

- ![honeycomb.io logo](/content/odd/img/honeycomb.png)
- ![jaeger logo](/content/odd/img/jaeger.png)

<!-- .element: class="list-unstyled list-spaced" -->



![Observability Engineering Achieving Production Excellence book](/content/odd/img/observability-book.png)

https://info.honeycomb.io/observability-engineering-oreilly-book-2022



## Questions?
<br />

* https://honeycomb.io
* https://opentelemetry.io
* https://www.heavybit.com/library/podcasts/o11ycast
* https://info.honeycomb.io/observability-engineering-oreilly-book-2022

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
