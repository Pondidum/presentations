# Observability

## questions

- Will you tell us about Honeycomb.io
- what are the key differences between the traditional approach that encumbents are peddling and the new approach, with practical examples
- real world top 3 cases where observality saved the day



## quotes
- its granuality is better (glasses vs microscope)
- it should feel wrong to write code without some observability

## outline

- [x] it's more than just metrics and logging
- [x] 3 pillars bs (logs, metrics, traces)
  - [ ] they are useful, but it doesn't mean observability
  - [ ] mostly just old vendors trying to sound relevant
- [x] metrics (for code) are bad
  - [x] aggregations, averages of averages
  - [x] low cardinality (can't add user/correlation/request ids)
  - [x] cost (related to cardinality, datadog pricing e.g.)
- [x] logs are better
  - [x] strings of text are bad!
  - [x] structured logging is a major step forward
    - [x] repeated data (correlationid, requestid, etc.)
  - [x] shipping to elk has problems
    - [x] dropped events
    - [x] clashing field types (object/string int/string)
    - [x] elk cost (self hosted, saas)
    - [x] testing logstash
- [x] but cost
  - [x] 1 request generates x log lines
  - [x] so when request volume goes up, logs go exponentially up
  - [x] sampling :(
  - [x] dynamic sampling?
- [x] hard to understand
  - [x] looking for non existing lines is hard
- [x] why not send 1 log event per service per request?
  - [x] at start of request, create new event
  - [x] fill it with high cardinality data
    - [x] userid, correlationid, requestid...
  - [x] fill it with interesting data
    - [x] cache_size, cache_hit, cache item age, cache expiry
    - [x] query time ...
    - [x] build id, app uptime, config options
  - [x] at end, send it to obs service
  - [x] what happens on error?
    - [x] send the event
- [ ] why is this better?
  - [x] linear scaling (1 event per request per service)
  - [x] cost (self hosting/elk/etc)
  - [x] no full text search needed
  - [ ] filtering on properties
    - [ ] answer questions you didn't plan for
    - [ ] bubble up "what was different about these events?"
    - [ ] e.g. "our response times have dropped"
  - [x] no "AI"
    - [x] if you need "AI" to understand your metrics...try logging less crap
  - [ ] vendor offered magic APM
    - [ ] this is a lie
- [x] ODD
  - [x] you should be able to verify what is happening in prod
  - [x] different mindeset
    - [x] how can I tell this works, later?
  - [ ] deriving internal app state from events
  - [x] hypothosis "this should increase x by 30%..."
- [ ] example log output vs event
  - [ ] cache miss example
- [ ] I need metrics!
  - [ ] calculate them from your events
  - [ ] events are the source of truth, you can generate log statements and metrics from them...
  - [ ] if you _really_ want to, that is
- [ ] but I am building a monolith / cli
  - [ ] observability is still better!

#c57c21

---

Story

<!-- When the user is in the cache, there is no logline written, which is fine when everything is working.  However, when something unexpected happens, like daylight savings time or sudden clock drift, and suddenly all cache entries are never stale.  You have a decrease in latency (which looks good), your `cache_misses` counter goes down (looks good), but your data is older than you realised, and bad things are happening down the line. -->


- Noticed that latency had decreased recently
  - yay!
  - hmm
  - we haven't made any changes
  - we deploy immutable AMIs, so no patches (spectre/meltdown)
  - ummm?
- code:
  ```go
  func handleRequest(request *Request) {

      now := time.Now()

      if cache[request.UserID] == nil || cache[request.UserID].IsStale(now) {
          logger.Write("Cache miss for user", request.UserID))
          stats.Increment("cache_misses")
          fillCache(cache, request.UserID)
      }

      //...

      stats.set("request_duration", time.Since(now))
  }
  ```
- symptoms:
  - latency decreased
  - cache misses decreased
- ideas why?
  - memory pressure
  - cache sizes
  - `IsStale` bug
- hints:
  - only affected europe
  - canada a week later
  - restarting fixes the issue for months
  - only happens in march and october/november
- answer
  - daylight savings time!
  - or clock drift
- observable version
  ```go
  func handleRequest(w http.ResponseWriter, r *http.Request) {

      ctx := request.Context
      now := time.Now()
      userID := getUserID(request)

      beeline.AddField(ctx, "user_id", userID)

      beeline.AddField(ctx, "cache_size", len(cache))
      beeline.AddField(ctx, "cache_time", now)

      userData, found := cache[userID]

      beeline.AddField(ctx, "cache_hit", found)

      if !found || userData.IsStale(now) {
          userData = fillCache(ctx, cache, userID)
      }

      beeline.AddField(ctx, "cache_expires", userData.CacheUntil)
      beeline.AddField(ctx, "cache_is_stale", userData.IsStale(now))


      //...
  }
  ```
- includes a lot more information
- we try to anticipate useful information as we write the code
- ideally we should be able to tell exactly what is happening from the events we store
- filters useful:
  - `cache_is_stale=false` && `diff(cache_expires, cache_time) > 10m`



