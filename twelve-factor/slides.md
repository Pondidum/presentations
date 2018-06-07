# 12 Factor Microservices <!-- .element: class="stroke white" -->
<br />
## Andy Davies <!-- .element: class="stroke white" -->
github.com/pondidum | @pondidum | andydote.co.uk  <!-- .element: class="smaller white" -->

http://cdn.wonderfulengineering.com/wp-content/uploads/2014/06/Engineering-pictures-2.jpg <!-- .element: class="attribution white" -->

<!-- .slide: data-background="img/gears.jpg" data-background-size="" class="intro" -->

Note:
* Originally written by Heroku
* Set of principals for building software
* aims to make it
  * consistent
  * portable
  * scalable
  * declarative
* There are some tradeoffs and dissagreements



## 1. Codebase
One codebase tracked in revision control, many deploys
Note:
* 1 to 1 mapping repos to services
* where is service x? in repo named x
* mutliple codebases means not app, but system



# But x uses a monorepo
Note:
* Google, Facebook, Microsoft use monorepo
* you are not their scale
* 2 of them have written filesystems to deal with it
* perhaps they are solving the wrong problem?



- Twelve.Consumer/
  - .git/
  - deploy/
  - src/
- Twelve.Api/
  - .git/
  - deploy/
  - src/
- Twelve.Background/
  - .git/
  - deploy/
  - src/

Note:
* what about common things?
* terraform



- Twelve
  - .git
  - Consumer/
    - deploy/
    - src/
  - Api/
    - deploy/
    - src/
  - Background/
    - deploy/
    - src/
  - infrastructure/
    - variables.tf
    - database.tf
Note:
* shared code? e.g. models
* nugets
  * on nuget feed?
  * only used by this?
* acceptance tests
  * event > consumer > api > bacground



## 2. Dependencies
Explicitly declare and isolate dependencies
Note:
* use nuget/package manager
* dont rely on system packages
* in dotnet this means no GAC!



```csharp
Process.Start("dig @127.0.0.1 some.other.service +short");
```
Note:
* shelling out to system utilities is banned too
* include local version in your build



## 3. Config
Store config in the environment
Note:
* this means environment variables...
* no app.configs...?



```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables(ev => ev.Prefix = "twelve:")
            .Build()
            .Get<Configuration>();

        services.AddSingleton(config);
    }
}
```



```csharp
public class Configuration
{
    public string PostgresConnection { get; set; }
    public string DatabaseName { get; set; }

    public Uri RabbitHost { get; set; }
    public string RabbitUsername { get; set; }
    public string RabbitPassword { get; set; }
}
```



# Don't Store
* Connection Strings
* Passwords
* ApiKeys

<!-- .element: class="list-unstyled list-spaced" -->
Note:
* anything sensitive
* should be centralised



![consul logo](img/consul.png) <!-- .element: width="50%" class="no-border" -->
![vault logo](img/vault.png) <!-- .element: width="50%" class="no-border fragment" -->
https://www.hashicorp.com/brand <!-- .element: class="attribution" -->



```bash
dotnet add package Consul.Microsoft.Extensions.Configuration
```

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables(ev => ev.Prefix = "twelve:")
            .AddConsul(prefix: "appsettings/twelve/")
            .Build()
            .Get<Configuration>();

        services.AddSingleton(config);
    }
}
```
<!-- .element: class="fragment" -->



## 4. Backing services
Treat backing services as attached resources
Note:
* bit obtuse...what does it mean?



![service dependencies](img/attached-resources.png) <!-- .element: class="no-border" -->
Note:
* anything over the network
* kafka: who knows where it is!
* postgres: on premise, RDS, etc.
* twilio: definitely not self hosted!
* no-code changes to change (see F3)



## 5. Build, Release, Run
Strictly separate build and run stages
Note:
* no modifying things in prod!
* build.sh, release.sh, deploy...



![build release deploy pipeline](img/build-release-deploy-1-branch.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in none-out" -->



```bash
MODE=${1:-Debug}
NAME=$(basename $(ls *.sln | head -n 1) .sln)

dotnet restore
dotnet build --configuration $MODE

find ./src -iname "*.Tests.csproj" -type f -exec dotnet test \
  --configuration $MODE \
  --no-build \
  --no-restore
  "{}" \;

dotnet pack \
  --configuration $MODE \
  --no-build \
  --no-restore \
  --output ../../.build
```
Note:
* we deploy with octopus, so generate a nuget for apps
* you could create a docker container
* bake an ami with packer etc.
<!-- .slide: data-transition="fade" -->



![build release deploy pipeline](img/build-release-deploy-2-publish.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none-in none-out" -->
Note:
* pr accepted
* runs build.sh and release.sh



```bash
APIKEY="$1"
find .build -iname "*.nupkg" -type f -exec .tools/octo/octo.exe push \
  --package "{}" \
  --server https://octopus.internal.net \
  --apiKey $APIKEY \
  \;
```

```bash
CONTAINER=$(docker images | grep myapp | grep latest | awk '{print $3}')

docker tag $CONTAINER docker.internal.net/myapp
docker push docker.internal.net/myapp
```
<!-- .element: class="fragment" -->
<!-- .slide: data-transition="fade" -->



![build release deploy pipeline](img/build-release-deploy-3-octopus.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none-in none-out" -->



![build release deploy pipeline](img/build-release-deploy-4-test.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none-in none-out" -->



![build release deploy pipeline](img/build-release-deploy-5-prod.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none-in slide-out" -->



## 6. Process
Execute the app as one or more stateless processes
Note:
* state in a backing service (e.g. db)
* dont assume memory or disk last more than one operation



# Caching?
Note:
* single operation only
* multiple copies, low hit rate
* maybe in-memory cache where it makes sense
* don't require it however



## 7. Port Binding
Export services via port binding
Note:
* Completely self contained
* bind to a port, service requests
* not just for http!
  * xmpp
  * kafka etc
* no IIS!



![iis logo](img/iis.png) <!-- .element: class="no-border" -->
* XML Configuration! <!-- .element: class="fragment" -->
* Complexity! <!-- .element: class="fragment" -->
* Killing AppPools at random! <!-- .element: class="fragment" -->
* SSL Termination <!-- .element: class="fragment" -->

<!-- .element: class="list-unstyled list-spaced center" -->
Note:
* before core, owin and netsh is not fun
* configuring reverse proxy...



![web nginx yourapp](img/port-binding.png) <!-- .element: class="no-border" -->
Note:
* bind to port x on localhost
* https offload on nginx or other webserver



```csharp
public class Program
{
    public static void Main(string[] args)
    {
        WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseUrls("http://localhost:1234") //optional force port
            .Build()
            .Run();
    }
}
```
Note:
* standard .netcore startup
* added `UseUrls` to force port
* don't publish this url to consul for service discovery!



```nginx
server {
    listen                    *:443 ssl;
    server_name               example.com;
    ssl_certificate           /etc/ssl/certs/testCert.crt;
    ssl_certificate_key       /etc/ssl/certs/testCert.key;
    ssl_protocols             TLSv1.1 TLSv1.2;
    ssl_prefer_server_ciphers on;
    ssl_ciphers               "EECDH+AESGCM:EDH+AESGCM:AES256+EECDH:AES256+EDH";
    ssl_ecdh_curve            secp384r1;
    ssl_session_cache         shared:SSL:10m;
    ssl_session_tickets       off;

    location / {
        proxy_pass  http://localhost:1234;
    }
}
```
https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx <!-- .element: class="attribution" -->
Note:
* configure nginx to do ssl offload
* example only!
  * more headers
  * HSTS
  * stapling
* publish this url to consul for service discovery!



```bash
dotnet add package Microsoft.AspNetCore.HttpOverrides
```

```csharp
public class Startup
{
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All;
        });

        app.UseAuthentication();
        app.UseMvc();
    }
}
```
<!-- .element: class="fragment" -->
Note:
* add `HttpOverrides`
* call `UseForwardedHeaders`
* Update scheme, host, remoteIpAddress so auth middleware works



## 8. Concurrency
Scale out via the process model
Note:
* split by work type (http, rabbitmq, background indexing)
* doesn't ban threads
* but you should be able to run multiple copies for scale-out



![web haproxy yourapp](img/scaleout-1.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in none-out" -->
Note:
* normal state
* maybe aws lb instead of haproxy



![web haproxy yourapp](img/scaleout-2.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none-in none-out" -->
Note:
* viral post on reddit/hackernews about your new js framework



![web haproxy yourapp](img/scaleout-3.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none-in slide-out" -->
Note:
* more instances!
* be careful of downstream effects



## 9. Disposability
Maximize robustness with fast startup and graceful shutdown
Note:
* quicker it can handle requests the better
    * quick scale up!
* shutdown: stop handling new requests, finish current ones
    * cost!
* bouncing service has lower downtime
* a sudden shutdown should not cause data loss



# MTTR > MTTF



![mean time to failure - bmw engine](img/mttf-bmw.jpg)
http://image.superstreetonline.com/f/features/1308_10_top_bmw_3_series_features/42920882/04+2009-bmw-320d+engine-bay.jpg <!-- .element: class="attribution" -->
Note:
* doesnt break often!
* when it does though...
* take to bmw
* cost 1000s
* takes weeks



![mean time to recovery - jeep engine](img/mttf-jeep.jpg)
http://car-from-uk.com/ebay/carphotos/full/ebay698037.jpg <!-- .element: class="attribution" -->
Note:
* breaks...regularly
* fixed in 5min with a wrench



## 10. Dev/prod parity
Keep development, staging, and production as similar as possible
Note:
* not only environments
* small ttl of dev => prod (hours)
* devs support prod
* backing services: local sqllite vs prod postgres bad



## 11. Logs
Treat logs as event streams
Note:
* its a datastream (of events)
* dont handle routing at all (e.g. filesystem)
* write to stdout
* pipe to filebeat/fluentd/etc when deployed



## 12. Admin processes
Run admin/management tasks as one-off processes
Note:
* what else would you do?!
* separate exe with cli tasks (migration, status check, smoketest)
* deployed with main app



## Questions?
![questions](img/questions.jpg)

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
