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



![angry security guy](content/vault/img/056300321-angry-geek-faces.jpeg)
https://protectedbytrust.com/wp-content/uploads/2016/04/056300321-angry-geek-faces.jpeg <!-- .element: class="attribution" -->
Note:
* end of project



![graph of time vs difficulty, security is difficult at the end](content/vault/img/difficulty-late.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in fade-out" -->



![graph of time vs difficulty, security is moving to easy at the beginning](content/vault/img/difficulty-early.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="fade-in slide-out" -->
Note:
* pit of success



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
* migration to separate connections



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
* started using templates
* done using octopus deploy
* but manual: create user, enter octo values
* errors!



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
  * influx, cassandra, mongo
* create roles
* `vault read database/creds/writer`
* show expiry in dbeaver
* DirectAccess app
* pit of success!



# Who wants to know?
Note:
* dont distribute root token
* how to authenticate apps?
* many backends:
  * github (for devs)
  * aws (iam), azure (ad)
  * jwt, ldap, tls



# AppRoles
Note:
* RoleID (username)
* SecretID (password)



![policies for app defined in repo, ci writes to vault](content/vault/img/approles-1.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in none-out" -->



```bash
vault write auth/approle/role/$repo_name \
    token_ttl=20m \
    token_max_ttl=1h \
    policies="$policies_csv"
```
<!-- .slide: data-transition="fade" -->
Note:
* CI can only run this operation



![policies for app defined in repo, ci writes to vault](content/vault/img/approles-1.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="fade" -->
Note:
* ci -> vault = PR approval
* optional, but single source of truth



![ci pushes artifacts to deployment tool](content/vault/img/approles-2.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="fade" -->
Note:
* push artifacts
* octopus etc.



![deployment tool fetches secretid from vault](content/vault/img/approles-3.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="fade" -->
Note:
* octopus can only fetch secretids
* secretid in config file, only readable by app/process



![terraform fetches roleid from vault and writes to host environment variables](content/vault/img/approles-4.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="fade" -->
Note:
* terraform can only fetch roleids
* roleid in environment
* chef/puppet/ansible/etc




![instead of terraform, roleid is read by dev, and embedded in in sourcecode](content/vault/img/approles-embedded.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="fade" -->
Note:
* roleid embedded by dev in binary
* secretid in config by octopus as before
* slightly less secure



![instead of terraform and octopus, nomad is used](content/vault/img/approles-nomad.png) <!-- .element: class="no-border" -->
<!-- .slide: data-transition="fade-in slide-out" -->
Note:
* nomad fetches both



# Demo
Note:
* ./approles.sh
* update roleid and secretid
* AppRoleAccess demo



# Auditing
Note:
* file, syslog, socket
* multiple
* &gt;1 must write



```bash
vault audit enable file file_path=/var/log/vault/audit.log
```
Note:
* enabled before presentation ;)
* if using `ext{2,3,4}` use `chattr +a <file>` to make append only



# Demo
Note:
* `tail -1 .vault/audit.log | jq .`



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

* https://vaultproject.io
* https://learn.hashicorp.com/vault/security/iam-approle-trusted-entities
* https://consul.io/
* https://github.com/Pondidum/vault-demo
* https://andydote.co.uk/presentations/index.html?vault

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
