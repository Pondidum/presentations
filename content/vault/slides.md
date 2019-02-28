# How to Secure Your Microservices <!-- .element: class="stroke" -->
## Andy Davies <!-- .element: class="stroke" -->
github.com/pondidum | @pondidum | andydote.co.uk  <!-- .element: class="smaller white" -->

wat <!-- .element: class="attribution white" -->

<!-- .slide: data-background="" data-background-size="contain" class="intro" -->
Note:
* Checklist
  * docker-compose up -d
  * dbeaver running
  * bash setup



![initial architecture, fat clients, 1 db, 3 services](content/vault/img/architecture.png) <!-- .element: class="no-border" -->



```xml
<configuraton>
  <connectionStrings>
    <add name="maindb" connectionString="some_long_encrypted_value" />
  </connectionStrings>
</configuraton>
```



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




# Goals

* unique credentials (environment, app, and user) <!-- .element: class="fragment" -->
* no manual steps or scripts <!-- .element: class="fragment" -->
* securely stored <!-- .element: class="fragment" -->
* auditable <!-- .element: class="fragment" -->
* minimal code written <!-- .element: class="fragment" -->

<!-- .element: class="list-spaced" -->



![Vault Logo](content/vault/img/Vault_PrimaryLogo_FullColor.png)



# Demo
Note:
* create db secrets engine
* create roles
* get credentials, show expiry in dbeaver
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



![approles, with roleid embedded in app](content/vault/img/approles-embedded.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none-in slide-out" -->



# Demo
Note:
* AppRoleAccess demo



## Questions?
<br />

* https://www.vaultproject.io
* https://learn.hashicorp.com/vault/security/iam-approle-trusted-entities
* some
* links
* here

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
