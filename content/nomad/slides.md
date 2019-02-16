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
* everything else is up to you
* add the components you want or need, when you want or need them




```javascript
job "rabbit" {
  datacenters = ["dc1"]

  group "cluster" {
    task "rabbit" {
      driver = "docker"
      config {
        image = "pondidum/rabbitmq:consul"
      }
    }
  }
}
```

<!-- .slide: data-transition="slide-in fade-out" -->
Note:
* we have to specify a datacenter
* a group can have multiple tasks
* nomad will keep all tasks for a group on one host



```javascript
task "rabbit" {
  driver = "docker"

  config {
    image = "pondidum/rabbitmq:consul"
    hostname = "${attr.unique.hostname}"
    port_map {
      amqp = 5672
    }
  }

  resources {
    network {
      port "amqp" { }
    }
  }

  service {
    name = "rabbitmq"
    port = "amqp"
    check {
      name = "alive"
      type = "tcp"
      interval = "10s"
      timeout = "2s"
    }
  }
}
```

<!-- .element class="full-height" -->
<!-- .slide: data-transition="fade-in slide-out" -->



# image of hosts and groups?



# Demo
Note:
* deploying
* run rabbit.nomad
* HA would be good...
* update count => 3
* plan rabbit.nomad
* run rabbit.nomad



# Application Image
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
curl http://localhost:8500/v1/catalog/rabbitmq
```
Note:
* we can also use a dns interface to consul
* load balancing:
  * random!
  * client side
  * fabio



# Not using Consul?
Note:
* fine! no `service` stanza for you!
* service registration by you
  * in-app
  * another task



# Demo
Note:
* app code: service discovery
* run `microservice.nomad`
* publish messages



# But...
```csharp
var broker = await _configuration.GetRabbitHost();

var cf = new ConnectionFactory
{
  HostName = broker.Host,
  Port = broker.Port,
  DispatchConsumersAsync = true,
  Username: "guest",
  Password: "guest"
};
```
Note:
* connecting with guest/guest is not great!
* your credentials should be, well, secret...



# Secrets
Image of a "shhh" (perhaps simpsons lenny and carl "shutuuuup")

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
* Compliant with xxxxxxxxxxx
* mutliple backends (storage), including Consul
* multiple providers (e.g. sql, ssh, kv)
* not too much depth on this



```bash
vault write rabbitmq/config/connection \
    connection_uri="http://localhost:15672" \
    username="admin" \
    password="password"

vault write rabbitmq/roles/read-write \
    vhosts='{ "/" : { "write" : ".*", "read" : ".*" } }'
```

Note:
* this is setup by a vault Operator (admin)
* you'd use servicediscovery for `connection_uri`



```bash
var credentials = await _vault
    .Secrets
    .RabbitMQ
    .GetCredentialsAsync("read-write");
```

```json
template {
  data = <<EOF
    {{ with secret "rabbitmq/creds/read-write" }}
      {
        "rabbitUsername": "{{ .Data.username }}",
        "rabbitPassword": "{{ .Data.password }}"
      }
    {{ end }}
    EOF
  destination = "secrets/config.json"
}
```
<!-- .element: class="fragment" -->

Note:
* our app can fetch credentials itself
* of we can use Nomad integration



## Did you notice?

```javascript
task "api" {
  driver = "exec"

  config {
    command = "/usr/bin/dotnet"
    args = [
      "local/HelloApi.dll",
      "urls=http://*:${NOMAD_PORT_http}"
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
      "local/HelloApi.dll",
      "urls=http://*:${NOMAD_PORT_http}"
    ]
  }

  artifact {
    source = "http://172.27.48.17:3030/Hello.zip"
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



## Questions?
<br />

* https://www.nomadproject.io/
* go
* here

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
