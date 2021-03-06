# Feature Toggles <!-- .element: class="push-down stroke" -->
## Andy Davies <!-- .element: class="stroke" -->
github.com/pondidum | @pondidum | andydote.co.uk  <!-- .element: class="smaller white" -->

https://www.reddit.com/r/gaming/comments/692hqa/mccree_looking_good_in_ac_black_flag/dh3gu1a/ <!-- .element: class="attribution white" -->

<!-- .slide: data-background="content/feature-toggles/img/good-bad-ugly.png" data-background-size="contain" class="intro" -->



![knight capital logo](content/feature-toggles/img/knight-capital.png) <!-- .element: class="no-border" -->
https://en.wikipedia.org/wiki/File:Knight_Capital_Group_logo.svg <!-- .element: class="attribution" -->
Note:
* Who has heard of a company called Knight Capital?
* Ok, and *why* have you heard of them?
* American global financial services firm, doing market-making, trading
* They had a "slight" hiccup involving deployment and feature toggling
* on the day in question, they had $365 million in cash and equivalents



![knight capital shares](content/feature-toggles/img/knight-capital-shares.jpg)
https://infocus.dellemc.com/dave_bagatelle/knight-capital-group-kcg-a-lesson-on-the-importance-of-sdlc-and-multi-environment-multi-user-testing/ <!-- .element: class="attribution" -->
Note:
* this is their shareprice
* that is a logarithmic graph
* look at that drop...



## PowerPeg and SMARS
Note:
* Powerpeg: counts shares against an order, stops orders once parent fulfilled. tracking had been moved earlier in pipeline, so powerpeg couldnt count. unused 8 years.
* SMARS (split big orders into smaller ones)
* Smars replaces PowerPeg



```csharp
void ProcessOrder(Order order)
{
    if (feature_enabled)
    {
        while (order.current < order.total)
        {
            submitNewOrder(1)
        }
    }
    else
    {
        submitNewOrder(order.total)
    }
}

void submitNewOrder(howMany)
{
    // complex order submission logic...
    order.current += howMany
}
```
<!-- .element class="full-height" -->
<!-- .slide: data-transition="slide-in out-none" -->



```diff
void ProcessOrder(Order order)
{
    if (feature_enabled)
    {
        while (order.current < order.total)
        {
            submitNewOrder(1)
        }
    }
    else
    {
        submitNewOrder(order.total)
+       order.current = order.total;
    }
}

void submitNewOrder(howMany)
{
    // complex order submission logic...
-   order.current += howMany
}
```
<!-- .element class="full-height" -->
<!-- .slide: data-transition="in-none out-none" -->



```csharp
void ProcessOrder(Order order)
{
    if (feature_enabled && order.IsBig)
    {
        splitIntoSmallerOrders();
        return;
    }

    submitNewOrder(order.total)
    order.current = order.total;
}

void submitNewOrder(howMany)
{
    // complex order submission logic...
}
```
<!-- .element class="full-height" -->
<!-- .slide: data-transition="in-none slide-out" -->



![knight-capital-process](content/feature-toggles/img/knight-capital-process.png) <!-- .element: class="no-border" -->
https://cloud.google.com/icons/ <!-- .element: class="attribution" -->
<!-- .slide: data-transition="slide-in out-none" -->



![knight-capital-process](content/feature-toggles/img/knight-capital-process-deployed.png) <!-- .element: class="no-border" -->
https://cloud.google.com/icons/ <!-- .element: class="attribution" -->
<!-- .slide: data-transition="in-none out-none" -->



![knight-capital-process](content/feature-toggles/img/knight-capital-process-toggled.png) <!-- .element: class="no-border" -->
https://cloud.google.com/icons/ <!-- .element: class="attribution" -->
<!-- .slide: data-transition="none" -->



![knight-capital-process](content/feature-toggles/img/knight-capital-process-rolledback.png) <!-- .element: class="no-border" -->
https://cloud.google.com/icons/ <!-- .element: class="attribution" -->
<!-- .slide: data-transition="none" -->



![knight-capital-process](content/feature-toggles/img/knight-capital-process-detoggled.png) <!-- .element: class="no-border" -->
https://cloud.google.com/icons/ <!-- .element: class="attribution" -->
<!-- .slide: data-transition="none-in slide-out" -->

Note:
* 45mins toggle disabled
* $365 million in assets to start with
* the lost $450 million in 45 minutes



#### Rule 1
## NEVER Reuse A Toggle
Note:
* if they had a new toggle, problem wouldnt have happened



![knight-capital-process](content/feature-toggles/img/knight-capital-process-deployed.png) <!-- .element: class="no-border" -->
https://cloud.google.com/icons/ <!-- .element: class="attribution" -->
<!-- .slide: data-transition="slide-in out-none" -->



![knight-capital-process](content/feature-toggles/img/knight-capital-process-separate.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="in-none slide-out" -->



#### Rule 2
## Name Toggles Well



* `EnablePowerPeg`
* `SmarsActive`
* `PAY-123`
* `NewFeature`

<!-- .element: class="list-unstyled list-spaced" -->
Note:
* jira tag should also have a name
* new feature? wonder what that does



#### Rule 3
## Keep Lifespan Short
Note:
* why is it still there!?
* delete it
* source-control history
* trunk based: deploy version with only the removal



# Flexibility
<br />
<br />
![toggle-types](content/feature-toggles/img/toggle-types.png) <!-- .element: class="no-border" -->
Note:
* compile: debug only (e.g. sql profiler)
* startup: microservices, bounce them
* periodic: background
* activity: actually uses background (latency)



![hp logo](content/feature-toggles/img/hp.png) <!-- .element: class="no-border" -->
https://en.wikipedia.org/wiki/File:Hewlett-Packard_logo.svg <!-- .element: class="attribution" -->
Note:
* hp printers
* 400-800 devs, 10+ million loc
* 2x releases per year
* 5% features
* 8 weeks for feedback (6 weeks manual integration)



![hp-branching-printers](content/feature-toggles/img/hp-branching-1.png) <!-- .element: class="no-border" -->
Note:
* branch per printer model
* `ifdefs` everywhere
* build per branch



```csharp
bool CanDoubleSidePrint()
{
#if (PRINTER_LASERJET_1080 || PRINTER_LASERJET_1090 || PRINTER_DESKJET || PRINTER_DESKJET_PRO_A)
    return true;
#else
    return false;
#end
}
```



![hp-branching-printers](content/feature-toggles/img/hp-branching-1.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in none-out" -->
Note:
* branch per printer model
* `ifdefs` everywhere
* build per branch



![hp-branching-merging](content/feature-toggles/img/hp-branching-2.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none-in slide-out" -->
Note:
* port fixes (not configs) to all branches



# Trunk Based Development



![hp-branching-trunk-based](content/feature-toggles/img/hp-branching-tbd.png) <!-- .element: class="no-border" -->
Note:
* compile-time to startup toggles
* trunk based development
* Single branch
* config file with printer capabilities



```csharp
void OnPrinterStart()
{
    var metadata = ReadFirmwareMetadata();
    var config = ReadConfig(metadata.Model);

    //...
}

IConfiguration ReadConfig(string printerModel)
{
    var path = Path.Combine(configs, printerModel + ".json");
    var json = File.ReadAllText(path);

    return JsonConvert.Deserialize<IConfiguration>(json);
}
```



```json
{
    "isColour": true,
    "features": [
        "double_siding",
        "scanning",
        "emailing",
        "wifi"
    ]
}
```



```csharp
public class PrinterConfiguration : IConfiguration
{
    private const string DoubleSidingKey = "double_siding";

    public bool CanDoubleSidePrint(IConfiguration config)
    {
        return _features.Contains(DoubleSidingKey);
    }
}
```



#### Rule 4
## Architecture Matters
Note:
* toggles should be implemented smartly
* dont spatter the same if statement everywhere



![toggle-table](content/feature-toggles/img/toggle-table-time.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in out-none" -->
Note:
* Release: switch everyone/group
* Feature: switch for subset. AB testing.
* Ops: performance
* Permission:
  * switch for user/group.
  * dont do it.
  * blurred boundaries
  * use authorization service (identityserver, keycloak, etc.)



![toggle-table](content/feature-toggles/img/toggle-table-time-ops.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="in-none slide-out" -->
Note:
* effects everyone
* periodic check most likely



![ops-toggle](content/feature-toggles/img/ops-toggle.png) <!-- .element: class="no-border" -->
Note:
* background process could cause load
* usually not the root cause
* monitoring started off manual



![toggle-table](content/feature-toggles/img/toggle-table-time-ops.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in none-out" -->
Note:
* effects everyone
* periodic check most likely



![toggle-table](content/feature-toggles/img/toggle-table-time-experiment.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none" -->
Note:
* to a subset of users, usually
* scale (risk): large=activity, small=startup



![toggle-table](content/feature-toggles/img/toggle-table-time-release.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="in-none slide-out" -->
Note:
* compile time not great, needs redeploy
* startup: change toggle, bounce service
* periodic: change toggle, wait



![phased-rollout](content/feature-toggles/img/phased-rollout-initial.png) <!-- .element: class="no-border" -->
Note:
* soap service was crap
* did magic also
* exchange random slowdowns



# Branch By Abstraction



```csharp
public interface IEmailConnector
{
    Task Dispatch(EmailMessage message);
}
```

```csharp
public class WebServiceConnector : IEmailConnector
{
    // ...
}
```

```csharp
public class RabbitMqConnector : IEmailConnector
{
    // ...
}
```



```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var toggles = services
            .BuildServiceProvider()
            .GetService<IToggles>();

        if (toggles.EmailDispatchQueue.Enabled)
        {
            services.AddScoped<IEmailConnector, RabbitMqConnector>();
        }
        else
        {
            services.AddScoped<IEmailConnector, WebServiceConnector>();
        }
    }
}
```
<!-- .element class="full-height" -->
Note:
* great for startup toggles!



```csharp
public class EmailConnectorRouter : IEmailConnector
{
    public Task Dispatch(EmailMessage message)
    {
        if (_toggles.EmailDispatchQueue.Enabled)
            return _whenEnabled.Dispatch(message);
        else
            return _whenDisabled.Dispatch(message);
    }
}
```
Note:
* better for periodic or activity
* could be a factory also
* what about the frontend?



![phased-rollout](content/feature-toggles/img/phased-rollout-new.png) <!-- .element: class="no-border" -->
Note:
* magic was implemented properly in other pipeline connector
* IEmailConnector, decorator to choose impl
* default/toggle off = old
* toggle on, queues
* rabbitmq plus a couple of workers



![phased-rollout-progress](content/feature-toggles/img/phased-rollout-progress-1.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in out-none" -->
Note:
* picked person who got most problems
* asked him to beta test (keen!)
* toggle a few times based on feedback



![phased-rollout-progress](content/feature-toggles/img/phased-rollout-progress-2.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none" -->
Note:
* added teammate



![phased-rollout-progress](content/feature-toggles/img/phased-rollout-progress-3.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none slide-out" -->
Note:
* perf issues found!



![phased-rollout-perf](content/feature-toggles/img/phased-rollout-perf-initial.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in none" -->



![phased-rollout-perf](content/feature-toggles/img/phased-rollout-perf-problem.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none" -->



![phased-rollout-perf](content/feature-toggles/img/phased-rollout-perf-solution.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none slide-out" -->
Note:
* Worker reads both queues
* favours direct queue
* competing workers added later



![phased-rollout-progress](content/feature-toggles/img/phased-rollout-progress-4.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in none" -->
Note:
* no new problems...



![phased-rollout-progress](content/feature-toggles/img/phased-rollout-progress-5.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none" -->
Note:
* no new problems...



![phased-rollout-progress](content/feature-toggles/img/phased-rollout-progress-6.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="in-none slide-out" -->
Note:
* process took 6-8 weeks
* faster towards the end
* toggle deleted another 4 weeks later



![phased-rollout-progress](content/feature-toggles/img/phased-rollout-graph.png) <!-- .element: class="no-border" -->
Note:
* percent of users with toggle on



#### Rule 5
## Monitor Toggles
Note:
* when/how often queried
* what state is it



![toggle-graphs](content/feature-toggles/img/monitoring-graph.png) <!-- .element: class="no-border" -->
Note:
* left: ramp up of users querying
* middle: everyone active
* right: stopped querying (toggle can be deleted)



# Alerts
* Toggle isn't queried
* Toggle state hasn't changed

Note:
* how long? it depends
* state: active % hasn't changed



# User Perception
<br />
![phased-rollout-progress](content/feature-toggles/img/phased-rollout-perception-first.png) <!-- .element: class="no-border fragment" -->
![phased-rollout-progress](content/feature-toggles/img/phased-rollout-perception-early.png) <!-- .element: class="no-border fragment" -->
![phased-rollout-progress](content/feature-toggles/img/phased-rollout-perception-everyone.png) <!-- .element: class="no-border fragment" -->
Note:
* first: pita. Harness it. Feels special, first! helped write feature.
* early: "we had early access", feels special
* all: big bang release, flawless.



# Complexity
Note:
* but what doesn't?
* we cant remove complexity
* can hide it or abstract it a bit
* toggles can help reduce it...



![long-lived-branches](content/feature-toggles/img/long-branches-1.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in out-none" -->
Note:
* monolith repo
* 2x inhouse, 1x outsourced (incompetent)
* day to day work
* external branch always out of date



![long-lived-branches](content/feature-toggles/img/long-branches-2.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="in-none slide-out" -->
Note:
* priorities changed
* multiple merge and revert
* rebases!



# Testing is harder
Note:
Is it?



![testing-existing](content/feature-toggles/img/testing-toggles-original.png) <!-- .element: class="no-border fragment fade-out" data-fragment-index="2" -->
![testing-new-toggle](content/feature-toggles/img/testing-toggles-toggle.png) <!-- .element: class="no-border fragment" data-fragment-index="1"-->
Note:
* greg young: tests are immutable
* old tests don't change (toggle off)
* new tests (toggle on)
* delete old when toggle removed!
* manual testing basically the same



## Rules

1. NEVER Reuse A Toggle
1. Name Toggles Well
1. Keep Lifespan Short
1. Architecture Matters
1. Monitor Toggles

<!-- .element: class="list-spaced" -->



## Questions?
<br />

* https://dougseven.com/2014/04/17/knightmare-a-devops-cautionary-tale
* https://itrevolution.com/the-amazing-devops-transformation-of-the-hp-laserjet-firmware-team-gary-gruver
* https://martinfowler.com/articles/feature-toggles.html
* https://trunkbaseddevelopment.com
* https://andydote.co.uk/presentations/feature-toggles

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
