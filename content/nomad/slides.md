# Nomad <!-- .element: class="stroke text-right" -->
### Kubernetes, <!-- .element: class="stroke text-right" -->
### without the complexity <!-- .element: class="stroke text-right" -->
<br/>
<br/>
<br/>
## Andy Davies <!-- .element: class="stroke text-right" -->
github.com/pondidum | @pondidum | andydote.co.uk  <!-- .element: class="smaller red text-right" -->

CC BY-NC-SA 3.0 <!-- .element: class="smaller red text-right" -->

<br/>
https://www.techjunkies.nl/2018/05/02/why-the-term-can-it-run-crysis-is-still-justified-in-2018/ <!-- .element: class="attribution red" -->

<!-- .slide: data-background="content/nomad/img/crysis.jpg" data-background-size="cover" class="intro" -->
Note:
* `vagrant up`
* `./scripts/host.sh`
* `source .machine/env`



# Disclaimer

I don't like (non-essentail) complexity <!-- .element: class="fragment" -->



* ![kubernetes is the systemd of the container world](content/nomad/img/tweet.png) <!-- .element: class="no-border" -->
https://twitter.com/Pondidum/status/1068758260965101568 <!-- .element: class="attribution" -->

* ![kubernetes was and remains a hostile piece of software to learn, run, operate, maintain.](content/nomad/img/tweet-cindy.png) <!-- .element: class="no-border" -->
https://twitter.com/copyconstruct/status/1194701905248673792 <!-- .element: class="attribution" -->

<!-- .element: class="list-unstyled list-columns-2" -->

Note:
* complexity



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



`nomad agent -dev`



![kubernetes feature list](content/nomad/img/kubernetes-features.png) <!-- .element: class="no-border" -->

<!-- .slide: data-transition="slide-in fade-out" -->

Note:
* if you need all this, great
* brown-field apps probably have a lot of these already
* green-field: make choices



![nomad feature list](content/nomad/img/nomad-features.png) <!-- .element: class="no-border" -->

<!-- .slide: data-transition="fade-in fade-out" -->

Note:
* just a container orchestrator
* maybe storage too
* everything else is up to you



![nomad feature list](content/nomad/img/nomad-features-choice.png) <!-- .element: class="no-border" -->

<!-- .slide: data-transition="fade-in fade-out" -->



![nomad feature list](content/nomad/img/kubernetes-features-plus.png) <!-- .element: class="no-border" -->

<!-- .slide: data-transition="fade-in slide-out" -->

Note:

* add the components you want or need, when you want or need them
* fabio: routing and loadbalancing
* autoscaling is metrics scaling



# No YAML!
<br />

## [noyaml.com](https://noyaml.com)



```javascript
job "api" {

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
<!-- .slide: data-transition="slide-in fade-out" -->



![nomad task groups](content/nomad/img/nomad-groups-bad.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="fade-in fade-out" -->



![nomad task groups](content/nomad/img/nomad-groups-good.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="fade-in slide-out" -->



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
  * client side: random, weights from consul
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



## Kubernetes
# "secrets"

> ...secret data is stored in etcd; therefore:
> Administrators should limit access to etcd to admin users

https://kubernetes.io/docs/concepts/configuration/secret/#risks <!-- .element: class="attribution" -->



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
* vault generates credentials
* requested by nomad or app
* pros|cons to both methods



#### In App
```bash
var credentials = await _vault
    .Secrets
    .RabbitMQ
    .GetCredentialsAsync("consumer");
```

#### In .nomad file <!-- .element: class="fragment"  data-fragment-index="1" -->
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
<!-- .element: class="fragment" data-fragment-index="1" -->

Note:
* either in app, or in nomad job
* app = complexity, but flexibility
* nomad = simple, but restart for new creds



# Docker Only?



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



# Batch & System Jobs

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
* https://noyaml.com

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
