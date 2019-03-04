# How to Secure Your Microservices <!-- .element: class="stroke text-left" -->
## Andy Davies <!-- .element: class="stroke text-left" -->
github.com/pondidum | @pondidum | andydote.co.uk  <!-- .element: class="smaller white text-left" -->

https://michellegable.com/2014/03/landmarks-paris-apartment-pont-des-arts/the-hundreds-of-thousands-of-locks-on-the-pont-des-arts-bridge-paris-france/ <!-- .element: class="attribution white text-left" -->

<!-- .slide: data-background="content/vault/img/PontDesArts.jpg" data-background-size="cover" class="intro" -->
Note:
### Checklist
* docker-compose up -d
* dbeaver running
* rider open
* solution built
* export VAULT_TOKEN=vault
* export VAULT_ADDR=http://localhost:8200
* ./init.sh



![initial architecture, fat clients, 1 db, 3 services](content/vault/img/architecture.png) <!-- .element: class="no-border" -->
Note:
* monolithic codebase
* few services, 2 desktop apps
* shared config fie



```xml
<configuraton>
  <connectionStrings>
    <add name="maindb" connectionString="some_long_encrypted_value" />
  </connectionStrings>
</configuraton>
```
Note:
* shared connectionstring
* decryption key in binary



```toml
maindb = """
    data source={host};
    Initial Catalog={database};
    User Id={username};
    Password={password}
"""

host = db.internal.xyz
database = core

[desktop]
username = desktop_app
password = 123456

[worker_service]
username = worker
password = abcdef
```
Note:
* started using template cs
* done using octopus deploy
* but manual: create user, enter octo values



# Goals

* unique credentials (environment, app, and user) <!-- .element: class="fragment" -->
* no manual steps or scripts <!-- .element: class="fragment" -->
* securely stored <!-- .element: class="fragment" -->
* auditable <!-- .element: class="fragment" -->
* minimal code written <!-- .element: class="fragment" -->

<!-- .element: class="list-spaced" -->
Note:
* per user nice to have
* don't write your own crypto



![Vault Logo](content/vault/img/Vault_PrimaryLogo_FullColor.png)
Note:
* secrets as a service
* HA
* FIPS 140-2 (Federal Information Processing Standard)
* engines, backends, audit



# Demo
Note:
* create db secrets engine
* create roles
* `vault read database/creds/writer`
* show expiry in dbeaver
* DirectAccess app



# Who wants to know?
Note:
* is the app allowed to fetch this role?
* is the app actually who it claims to be?
* so, how do we authenticate with vault?



# AppRoles
Note:
* many ways to do this
* appRoles make the most sense (usually)
* RoleID (username)
* SecretID (password)



![approle role and secret id progression](content/vault/img/approles.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in none-out" -->
Note:
* ci -> vault = PR approval
* terraform can only fetch roleids
* spinnaker can only fetch secretids
* roleid in environment
* secretid in config file, only readable by app/process




![approles, with roleid embedded in app](content/vault/img/approles-embedded.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none-in slide-out" -->
Note:
* roleid embedded by dev in binary
* secretid in config by spinnaker as before
* slightly less secure



# Demo
Note:
* cat approles.sh
* ./approles.sh
* update roleid and secretid
* AppRoleAccess demo




# Goals

* unique credentials <span class="fragment">&#10004;</span>
* no manual steps or scripts <span class="fragment">&#10004;</span>
* securely stored <span class="fragment">&#10004;</span>
* auditable <span class="fragment">&#10004;</span>
* minimal code written <span class="fragment">&#10004;</span>

<!-- .element: class="list-spaced" -->
Note:
* unique users = expand with AD support later
* audit: not covered, but easy to configure



## Questions?
<br />

* https://www.vaultproject.io
* https://learn.hashicorp.com/vault/security/iam-approle-trusted-entities
* https://consul.io/
* https://github.com/Pondidum/vault-demo
* https://andydote.co.uk/presentations/index.html?vault

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
