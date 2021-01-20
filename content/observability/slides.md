# Observability Driven Development <!-- .element: class="stroke-black white text-right" -->
## Andy Davies <!-- .element: class="stroke-black white text-right" -->
github.com/pondidum | andydote.co.uk  <!-- .element: class="white smaller text-right" -->

https://images.newscientist.com/wp-content/uploads/2018/12/21114218/hubble.jpg <!-- .element: class="attribution" -->

<!-- .slide: data-background="content/observability/img/hubble.jpg" data-background-size="cover" class="intro" -->



> Observability is a measure of how well internal states of a system can be inferred from knowledge of its external outputs

https://en.wikipedia.org/wiki/Observability <!-- .element: class="attribution" -->



![3 store pillars](content/observability/img/3-pillars.jpg)
https://www.irmi.com/articles/expert-commentary/three-pillars-of-litigation-management <!-- .element: class="attribution" -->
<!-- .slide: data-transition="slide-in fade-out" -->
Note:
- it's more than this



![3 store pillars](content/observability/img/3-pillars-bs.jpg)
https://st3.depositphotos.com/1031343/13382/v/450/depositphotos_133827724-stock-illustration-bullshit-sign-or-stamp.jpg <!-- .element: class="attribution" -->
<!-- .slide: data-transition="fade-in slide-out" -->
Note:
- vendors trying to be relevant
- 3 products, buy them all
- diff uis



# Metrics

Are Terrible



* Lossy
* Low Cardinality
* Expensive

<!-- .element: class="list-unstyled list-spaced" -->



# Logs

Are Better



* Verbose
* Repetitive
* Expensive
* Need Indexing

<!-- .element: class="list-unstyled list-spaced" -->



# Tracing

Pretty good, actually

Note:
- don't believe auto apm



```json
{"timestamp":"2020-09-14T12:51:27.104282677+03:00","request_id":"4d718e06-f74c-49de-bf8c-353f2fb045a6","user_id":"c7a940fd-e8c5-42b2-a506-3fcce385be9e","correlation_id":"fa21f3cc-37f1-4369-8642-f46dd87c4764","http_status":200,"http_method":"POST","http_path":"/user/preferences","message":"found user in cache"}
```
<!-- .slide: data-transition="slide-in fade-out" -->



```json
{
  "timestamp":      "2020-09-14T12:51:27.104282677+03:00",
  "request_id":     "4d718e06-f74c-49de-bf8c-353f2fb045a6",
  "user_id":        "c7a940fd-e8c5-42b2-a506-3fcce385be9e",
  "correlation_id": "fa21f3cc-37f1-4369-8642-f46dd87c4764",
  "http_status":    200,
  "http_method":    "POST",
  "http_path":      "/user/preferences",
  "message":        "found user in cache"
}
```
<!-- .slide: data-transition="fade-in slide-out" -->



```json
{
  "timestamp":      "2020-09-14T12:51:27.104282677+03:00",
  "request_id":     "4d718e06-f74c-49de-bf8c-353f2fb045a6",
  "user_id":        "c7a940fd-e8c5-42b2-a506-3fcce385be9e",
  "correlation_id": "fa21f3cc-37f1-4369-8642-f46dd87c4764",
  "http_method":    "POST",
  "http_path":      "/user/preferences",
  "message":        "fetched 3rd party prefrences"
}
{
  "timestamp":      "2020-09-14T12:51:27.104282677+03:00",
  "request_id":     "4d718e06-f74c-49de-bf8c-353f2fb045a6",
  "user_id":        "c7a940fd-e8c5-42b2-a506-3fcce385be9e",
  "correlation_id": "fa21f3cc-37f1-4369-8642-f46dd87c4764",
  "http_method":    "POST",
  "http_path":      "/user/preferences",
  "message":        "found user in cache"
}
{
  "timestamp":      "2020-09-14T12:51:27.104282677+03:00",
  "request_id":     "4d718e06-f74c-49de-bf8c-353f2fb045a6",
  "user_id":        "c7a940fd-e8c5-42b2-a506-3fcce385be9e",
  "correlation_id": "fa21f3cc-37f1-4369-8642-f46dd87c4764",
  "http_method":    "POST",
  "http_path":      "/user/preferences",
  "message":        "saved successfully"
}
```
<!-- .element class="full-height" -->
Note:
- we'll come back to this later



# Log Shipping

```json
{ "http_status": "200 OK" }

{ "http_status": 200 }
```
<!-- .element class="pull-left" -->

```json
{ "http_status": 200 }

{ "http_status": "200 OK" }
```
<!-- .element class="pull-right fragment" -->

</div>

Note:
- mostly logstash/Elasticsearch
- Testing logstash is :(
- dropped events
- clashing field types



# Cost

- 1 Request in
- 17 log lines
- 1 Response

<!-- .element: class="list-unstyled list-spaced" -->



# Sampling

> Let's just throw potentially useful data away



| Timestamp | UserID | Message |
|-----------|--------|---------|
| 12:51:27 | 3fcce385be9e | fetched 3rd party prefrences |
| 12:51:27 | 3fcce385be9e | found user in cache |
| 12:51:27 | 915d273db25c | fetched 3rd party prefrences |
| 12:51:27 | 3fcce385be9e | saved successfully |
| 12:51:27 | 8507d369d11c | fetched 3rd party prefrences |
| 12:51:27 | c4e71b4a29f2 | fetched 3rd party prefrences |
| 12:51:27 | 915d273db25c | saved successfully |
| 12:51:27 | c4e71b4a29f2 | found user in cache |
| 12:51:27 | c4e71b4a29f2 | saved successfully |
| 12:51:27 | 8507d369d11c | found user in cache |
| 12:51:27 | 8507d369d11c | saved successfully |

<!-- .element: class="smaller" -->



| Timestamp |      UserID | Fetched | In Cache | Saved |
|-----------|-------------|---------|----------|-------|
| 12:51:27 | 3fcce385be9e |  true   |   true   |  true |
| 12:51:27 | 915d273db25c |  true   |   false  |  true |
| 12:51:27 | 8507d369d11c |  true   |   true   |  true |
| 12:51:27 | c4e71b4a29f2 |  true   |   true   |  true |

<!-- .element: class="smaller" -->



# One Event
# Per Request
# Per Service



```json
{
  "timestamp":      "2020-09-14T12:51:27.104282677+03:00",
  "request_id":     "4d718e06-f74c-49de-bf8c-353f2fb045a6",
  "user_id":        "c7a940fd-e8c5-42b2-a506-3fcce385be9e",
  "correlation_id": "fa21f3cc-37f1-4369-8642-f46dd87c4764",
  "http_method":    "POST",
  "http_path":      "/user/preferences",
  "message":        "fetched 3rd party prefrences"
}
{
  "timestamp":      "2020-09-14T12:51:27.104282677+03:00",
  "request_id":     "4d718e06-f74c-49de-bf8c-353f2fb045a6",
  "user_id":        "c7a940fd-e8c5-42b2-a506-3fcce385be9e",
  "correlation_id": "fa21f3cc-37f1-4369-8642-f46dd87c4764",
  "http_method":    "POST",
  "http_path":      "/user/preferences",
  "message":        "found user in cache"
}
{
  "timestamp":      "2020-09-14T12:51:27.104282677+03:00",
  "request_id":     "4d718e06-f74c-49de-bf8c-353f2fb045a6",
  "user_id":        "c7a940fd-e8c5-42b2-a506-3fcce385be9e",
  "correlation_id": "fa21f3cc-37f1-4369-8642-f46dd87c4764",
  "http_method":    "POST",
  "http_path":      "/user/preferences",
  "message":        "saved successfully"
}
```
<!-- .element class="full-height" -->
<!-- .slide: data-transition="slide-in fade-out" -->



```json
{
  "timestamp":              "2020-09-14T12:51:27.104282677+03:00",
  "request_id":             "4d718e06-f74c-49de-bf8c-353f2fb045a6",
  "user_id":                "c7a940fd-e8c5-42b2-a506-3fcce385be9e",
  "correlation_id":         "fa21f3cc-37f1-4369-8642-f46dd87c4764",
  "http_method":            "POST",
  "http_path":              "/user/preferences",
  "3rd_party_fetched":      true,
  "user_cache_hit":         true,
  "saved":                  true,
}
```
<!-- .slide: data-transition="fade-in fade-out" -->



```json
{
  "timestamp":              "2020-09-14T12:51:27.104282677+03:00",
  "request_id":             "4d718e06-f74c-49de-bf8c-353f2fb045a6",
  "user_id":                "c7a940fd-e8c5-42b2-a506-3fcce385be9e",
  "correlation_id":         "fa21f3cc-37f1-4369-8642-f46dd87c4764",
  "http_method":            "POST",
  "http_path":              "/user/preferences",
  "3rd_party_fetched":      true,
  "3rd_party_duration_ms":  58,
  "user_cache_hit":         true,
  "user_cache_size":        20,
  "user_cache_expiry":      "2020-09-14T12:51:27.104282677+03:00",
  "saved":                  true,
  "save_duration_ms":       12
}
```
<!-- .slide: data-transition="fade-in slide-out" -->



- Build ID
- App start time
- App uptime
- Machine/container/host name
- Config data

<!-- .element: class="list-unstyled list-spaced" -->



```go
func WrapHandlerFunc(hf func(http.ResponseWriter, *http.Request)) func(http.ResponseWriter, *http.Request) {
  return func(w http.ResponseWriter, r *http.Request) {

    ctx, span := beeline.StartSpan(r.Context(), c.Name())
    defer span.Send()

    r.WithContext(ctx)

    hf(w, r)
  }
}
```



```go
func updatePrefs(req *http.Request, w http.ResponseWriter) {

  ctx := req.Context()

  userID, err := parseUser(request)
  if err != nil {
    beeline.AddField(ctx, "user_err", err)
    return userError(w)
  }

  beeline.AddField(ctx, "user_id", userID)

  user, found := userCache.Get(userID)
  beeline.AddField(ctx, "user_cache_hit", found)

  if found == false {
    user, err := loadUser(userID)
    if err != nil {
      beeline.AddField(ctx, "user_load_err", err)
      return userError(w)
    }

    userCache.Add(user)
  }

  beeline.AddFields(ctx, "user_cache", cacheStats(userCache))
  //...
}
```

<!-- .element: class="full-height" -->




```js
const beeline = require("honeycomb-beeline")();

app.post('/user/:id', function (request, res) {

  beeline.customContext.add("userId", request.params.id);

  beeline.addContext({ cache_size: cache.getSize() });
})
```



# Why is this better?



## Linear Scaling / Cheaper
![honeycomb cost chart](content/observability/img/honey-cost.png)

https://www.honeycomb.io/pricing <!-- .element: class="attribution" -->

Note:
- 20 = 7.5/s
- 100 = 38/s
- 450 = 171/s
- 1500 = 570/s



## Fast

No Full Text Searching



## No "AI"

> If you need "AI" to understand your data...
> Try storing less crap instead

Me <!-- .element: class="attribution" -->



# What's it look like?



![honeycomb walkthrough 1](content/observability/img/honey-tracing-1.png)



![honeycomb walkthrough 2](content/observability/img/honey-tracing-2-bubble.png)



![honeycomb walkthrough 3](content/observability/img/honey-tracing-3-endpoint.png)



![honeycomb walkthrough 4](content/observability/img/honey-tracing-4-user.png)



![honeycomb walkthrough 5](content/observability/img/honey-tracing-5-trace.png)



![honeycomb walkthrough 5](content/observability/img/honey-tracing-5-trace-view.png)



![honeycomb walkthrough 6](content/observability/img/honey-tracing-6-span-view.png)



![honeycomb walkthrough 7](content/observability/img/honey-tracing-7-trace-fast.png)



![honeycomb walkthrough 8](content/observability/img/honey-tracing-8-n+1.png)



## 1 User Had a Problem



# Observability Driven Development



1. Hypothesis
2. Measure current
3. Implement change
4. Verify hypothesis



- > This should increase signups by 30%
- > Switching this library out should have no noticable effect
- > How can I tell if this change is working?
- > Did my refactor change anything?

<!-- .element: class="list-unstyled list-spaced" -->



# Example

Goal: Improve Build Times



1. **Hypothesis**
2. Measure current
3. Implement change
4. Verify hypothesis


> Nginx build takes longer than it should


1. Hypothesis
2. **Measure current**
3. Implement change
4. Verify hypothesis


![build timings graph](content/observability/img/build-before.png)


1. Hypothesis
2. Measure current
3. **Implement change**
4. Verify hypothesis


```diff
 docker build \
   --build-arg "BUILD_RELEASE_VERSION=$IMAGE_VERSION" \
   --build-arg "BUILD_JS=$build_js" \
   --file "./build/Dockerfile.builder" \
-  --tag "$CONTAINER_BUILDER"
+  --tag "$CONTAINER_BUILDER" \
+  . | observe watch docker "$step_id"
```


1. Hypothesis
2. **Measure current**
3. Implement change
4. Verify hypothesis


![build timings detailed](content/observability/img/build-slow.png)


1. Hypothesis
2. Measure current
3. **Implement change**
4. Verify hypothesis


```diff
+ observe cmd "$step_id" -- docker pull --quiet "$CONTAINER_BUILDER_CACHE"

# ...

 docker build \
+  --cache-from "$CONTAINER_BUILDER_CACHE" \
   --build-arg "BUILD_RELEASE_VERSION=$IMAGE_VERSION" \
   --build-arg "BUILD_JS=$build_js" \
   --file "./build/Dockerfile.builder" \
   --tag "$CONTAINER_BUILDER" \
   . | observe watch docker "$step_id"
```


1. Hypothesis
2. Measure current
3. Implement change
4. **Verify hypothesis**


![build timings detailed](content/observability/img/build-fast.png)
note:
- original 507s
- saving 322s / 5m22s



## Questions?
<br />

* https://honeycomb.io
* https://www.heavybit.com/library/podcasts/o11ycast

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
