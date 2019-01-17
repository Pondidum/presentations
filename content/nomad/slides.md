# Nomad: <!-- .element: class="stroke" -->
## Kubernetes, without the complexity <!-- .element: class="stroke" -->
<br/>
## Andy Davies <!-- .element: class="stroke" -->
github.com/pondidum | @pondidum | andydote.co.uk  <!-- .element: class="smaller white" -->

https://commons.wikimedia.org/wiki/File:Nomads_near_Namtso.jpg <!-- .element: class="attribution white" -->

<!-- .slide: data-background="content/nomad/img/Nomads_near_Namtso.jpg" data-background-size="cover" class="intro" -->



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



* Service Discovery,
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

Note:
* and probably others I forget.
* if you need all this, great
* brown-field apps probably have a lot of these already
* green + microservices = sure!
* green + monolith = pointless
* brown + monolith = pointless
* brown + microservices = maybe




![Nomad Logo](content/nomad/img/Nomad_PrimaryLogo_FullColor.png)

Note:
* just a container orchestrator
* single binary! (server, client, communicator)
* no extra stuff, but integrates



![Nomad architecture single region](content/nomad/img/nomad-architecture-region.png)

<!-- .slide: data-transition="slide-in out-none" -->

Note:
* architecture is pretty simple for a single region
* 3 or 5 servers
  * accept jobs, manage clients, select task placement
* as many clients as you want
  * run the tasks



![Nomad architecture multi region](content/nomad/img/nomad-architecture-global.png)

<!-- .slide: data-transition="in-none slide-out" -->

Note:
* same, but the servers gossip!



## Questions?
<br />

* https://www.nomadproject.io/
* go
* here

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
