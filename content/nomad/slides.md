# Nomad <!-- .element: class="stroke text-right" -->
### Kubernetes, <!-- .element: class="stroke text-right" -->
### without the complexity <!-- .element: class="stroke text-right" -->
<br/>
<br/>
<br/>
## Andy Davies <!-- .element: class="stroke text-right" -->
github.com/pondidum | @pondidum | andydote.co.uk  <!-- .element: class="smaller red text-right" -->

<br/>
https://www.techjunkies.nl/2018/05/02/why-the-term-can-it-run-crysis-is-still-justified-in-2018/ <!-- .element: class="attribution red" -->

<!-- .slide: data-background="content/nomad/img/crysis.jpg" data-background-size="cover" class="intro" -->
Note:
* vagrant up
* admin prompt: `./scripts/demo.sh`
* admin prompt: `cd nomad-demo`
* vscode on nomad-demo
* rider:
  * light theme
  * built
  * presentation mode



# Disclaimer

Note:
* not about kubernetes
* if you're hoping to learn something about kubernetes, I won't be offended if you leave
* but maybe if you stay you'll have an alternative to consider?



![kubernetes is the systemd of the container world](content/nomad/img/tweet.png) <!-- .element: class="no-border" -->

https://twitter.com/Pondidum/status/1068758260965101568 <!-- .element: class="attribution" -->

Note:
* who's heard of this? likes it?
* It does a lot of things.
* If you need a lot of things, it's fine.
* If you just want init...maybe just use upstart/sysvinit?
* why bring this up?



![kubernetes control plane architecture](content/nomad/img/kubernetes-control-plane.png)

https://docs.google.com/presentation/d/1Gp-2blk5WExI_QR59EUZdwfO2BWLJqa626mK2ej-huo/edit#slide=id.g1e639c415b_0_56 <!-- .element: class="attribution" -->

Note:
* don't run this yourself! waste of time/resources
* use a cloud provider service
* compare this to nomad's architecture



![nomad cloud asg layout](content/nomad/img/nomad-cloud.png)
https://github.com/hashicorp/terraform-aws-nomad/tree/master <!-- .element: class="attribution" -->

Note:
* single binary, either server, client, or cli modes
* put it in an asg and don't worry
* you can definitely run this yourself
* there are terraform modules for it too
* vagrant box for local playing



![kubernetes logo](content/nomad/img/kubernetes.png) <!-- .element: class="no-border" -->

* Service Discovery, <!-- .element: class="fragment" -->
* Load Balancing, <!-- .element: class="fragment" -->
* Configuration Management, <!-- .element: class="fragment" -->
* Secret Storage, <!-- .element: class="fragment" -->
* Feature Gates, <!-- .element: class="fragment" -->
* Routing, <!-- .element: class="fragment" -->
* Storage Orchestration, <!-- .element: class="fragment" -->
* AutoScaling, <!-- .element: class="fragment" -->
* Container Management, <!-- .element: class="fragment" -->
* Rollouts & Rollbacks <!-- .element: class="fragment" -->

<!-- .element: class="list-unstyled list-inline" -->

https://assets.rightscale.com/870a9090f829755c6720d173dcb0f72a58a5d2d9/web/images/kubernetes-lg.png <!-- .element: class="attribution" -->

Note:
* and probably others I forget.
* if you need all this, great
* brown-field apps probably have a lot of these already
* green + microservices = sure!
* green + monolith = pointless
* brown + monolith = pointless
* brown + microservices = maybe



![Nomad Logo](content/nomad/img/Nomad_PrimaryLogo_FullColor.png) <!-- .element: class="no-border" -->

1. Container Orchestration <!-- .element: class="fragment" -->

Note:
* just a container orchestrator
* maybe storage too
* everything else is up to you
* add the components you want or need, when you want or need them



![initial microservices, some containers, some not, using consul and vault](content/nomad/img/migration-initial.png) <!-- .element: class="no-border" -->

<!-- .slide: data-transition="slide-in fade-out" -->
Note:
* not all services are in containers
* consul and vault
* decide to add kubernetes



![additional kubernetes cluster with only containers](content/nomad/img/migration-kubernetes.png) <!-- .element: class="no-border" -->

<!-- .slide: data-transition="fade-in fade-out" -->
Note:
* can't move everything over (non-containers)
* big-bang sucks anyway
* but things probably need to talk to each other



![cluster sync between consult vault and kubernetes](content/nomad/img/migration-sync.png) <!-- .element: class="no-border" -->

<!-- .slide: data-transition="fade-in fade-out" -->
Note:
* sync our infra services
  * consul -> kubernetes sd
  * vault -> kubernetes secrets
  * how do you sync secrets?! (etcd plaintext...)
* adds complexity...



![kubernetes replaced by nomad](content/nomad/img/migration-nomad.png) <!-- .element: class="no-border" -->

<!-- .slide: data-transition="fade-in slide-out" -->
Note:
* conceptially the same
* but our services directly use consul and vault
* no translation/sync required



# No YAML!



```javascript
job "api" {
  datacenters = ["dc1"]

  group "api-group" {

    task "app" {
      driver = "docker"
      config {
        image = "company/app:latest"
      }
    }

    task "nginx" {
      driver = "docker"
      config {
        image = "nginx:latest"
      }
    }

  }
}
```
<!-- .element class="full-height" -->

Note:
* we have to specify a datacenter
* multiple groups are allowed
* a group can have multiple tasks
* nomad will keep all tasks for a group on one host



![nomad task groups](content/nomad/img/nomad-groups.png) <!-- .element: class="no-border" -->



```ruby
task "rabbit" {
  driver = "docker"

  config {
    image = "pondidum/rabbitmq:consul"
    hostname = "${attr.unique.hostname}"
    port_map {
      amqp = 5672
      ui = 15672
    }
  }

  resources {
    network {
      port "amqp" { }
      port "ui" { }
    }

    cpu = 500 # MHz
    memory = 256 #MB
  }
}
```

<!-- .element class="full-height" -->



# Demo
Note:
* deploying
* run rabbit.nomad
* HA would be good...
* update count => 3
* plan rabbit.nomad
* run rabbit.nomad



![mutliple consumers, single api](content/nomad/img/application-architecture.png) <!-- .element: class="no-border" -->
Note:
* consume messages from rabbitmq
* expose http api
* but where do we connect?



![3 nomad hosts](content/nomad/img/cluster-3.png) <!-- .element: class="no-border" -->

<!-- .slide: data-transition="slide-in fade-out" -->

Note:
* 3 Nomad hosts
* and we know their names at least



![3 nomad hosts with a rabbitmq container on each](content/nomad/img/cluster-3-rabbit.png) <!-- .element: class="no-border" -->

<!-- .slide: data-transition="fade-in fade-out" -->

Note:
* but the containers are mapped to random ports



![n nomad hosts](content/nomad/img/cluster-n.png) <!-- .element: class="no-border" -->

<!-- .slide: data-transition="fade-in slide-out" -->
Note:
* but what if we have 10, 100, hosts?



```javascript
service {
  name = "rabbitmq"
  port = "amqp"
  tags = ["amqp"]
  check {
    name     = "alive"
    type     = "tcp"
    interval = "10s"
    timeout  = "2s"
  }
}

service {
  name = "rabbitmq"
  port = "ui"
  tags = ["management", "http"]
}
```
Note:
* this registers the service into Consul
* no need to know host or port



```bash
curl http://localhost:8500/v1/catalog/service/rabbitmq?tag=amqp
```

```csharp
var services = await _consul.Catalog.Service("rabbitmq", tag: "amqp");
```
<!-- .element: class="fragment" -->

```json
[
  {
    "ID": "9187bde1-3bfa-1b90-dc62-6b7e075175a1",
    "Node": "nomad3",
    "Address": "192.168.148.73",
    "ServiceName": "rabbitmq",
    "ServiceTags": [ "amqp" ],
    "ServiceAddress": "nomad3",
    "ServicePort": 20755
  }
]
```
<!-- .element: class="fragment" -->

Note:
* we can also use a dns interface to consul
* load balancing:
  * random!
  * client side
  * fabio



# Not using Consul?
Note:
* e.g. Netflix Eureka, Zookeeper, Etcd.
* diy
  * no `service` stanza
  * in-app registration



# Demo
Note:
* app code: service discovery
* run `consumer.nomad`
* publish messages
* increase consumers



# But...
```csharp
var broker = await _configuration.GetRabbitBroker();

var bus = Bus.Factory.CreateUsingRabbitMq(c =>
{
    var host = c.Host(broker, r =>
    {
        r.Username("guest");
        r.Password("guest");
    });

    //...
});
```
Note:
* connecting with guest/guest is not great!
* your credentials should be, well, secret...



![librarian shhhh](content/nomad/img/library-silence.jpg)
https://gistofthegrist.wordpress.com/2013/01/12/library-observations/ <!-- .element: class="attribution" -->

Note:
* two kinds of secret
* static - e.g. apikeys from 3rd parties
* dynamic - short lifetime or limited use count
* ideally rabbit connections will be dynamic
* if something leaks, it's time limited, and auditable
* Secrets as a Service is one of those things you shouldnt write



![Vault Logo](content/nomad/img/vault.png) <!-- .element: class="no-border" -->

Note:
* SaaS
* Hashicorp (I really should get paid by them...)
* Compliant with FIPS 140-2
* mutliple backends (storage), including Consul
* multiple providers (e.g. sql, ssh, kv)
* not too much depth on this



![apps and nomad use vault, vault uses aws/azure/github auth](content/nomad/img/vault-usage.png) <!-- .element: class="no-border" -->

Note:
* this is setup by a vault Operator (admin)



#### In App
```bash
var credentials = await _vault
    .Secrets
    .RabbitMQ
    .GetCredentialsAsync("consumer");
```
#### In .nomad file
```json
template {
  data = <<EOF
    {{ with secret "rabbitmq/creds/consumer" }}
      {
        "rabbitUsername": "{{ .Data.username }}",
        "rabbitPassword": "{{ .Data.password }}"
      }
    {{ end }}
    EOF
  destination = "secrets/config.json"
}
```

Note:
* either in app, or in nomad job
* app = complexity, but flexibility
* nomad = simple, but restart for new creds



## Did you notice?

```javascript
task "api" {
  driver = "exec"

  config {
    command = "/usr/bin/dotnet"
    args = [
      "local/Consumer.dll"
    ]
  }
}
```

<!-- .slide: data-transition="slide-in fade-out" -->

Note:
* no docker container!
  * nomad supports many `drivers`
  * LXC, RKT, Docker...and exec.
* but where did it come from?
* you saw me compile it, and I didn't scp it to a host...



```javascript
task "api" {
  driver = "exec"

  config {
    command = "/usr/bin/dotnet"
    args = [
      "local/Consumer.dll"
    ]
  }

  artifact {
    source = "http://artifacts.service.consul:3030/Consumer.zip"
  }
}
```

<!-- .slide: data-transition="fade-in slide-out" -->

Note:
* artifacts are a way of fetching things your task needs to run
  * http, s3,
  * auto extract tar, zip, gz
* so if your app isn't dockerized, dont worry!
  * zip it, push to s3
* you do lose out on isolation of containers though



# One more thing

Note:
* batch jobs
* rendering pipeline
* stream processing etc.



## Questions?
<br />

* https://andydote.co.uk/presentations/index.html?nomad
* https://github.com/Pondidum/nomad-demo
* https://nomadproject.io
* https://vaultproject.io
* https://consul.io

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
