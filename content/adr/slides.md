# Architecture Decision Records <!-- .element: class="stroke" -->
## Andy Davies <!-- .element: class="stroke" -->
github.com/pondidum | @pondidum | andydote.co.uk  <!-- .element: class="smaller white text-left" -->

some url here <!-- .element: class="attribution white text-left" -->

<!-- .slide: data-background="content/adr/" data-background-size="cover" class="intro" -->
Note:
wat



# Documentation Sucks



## Why on earth was it done like this???



* Easy to write <!-- .element: class="fragment" -->
* Easy to read <!-- .element: class="fragment" -->
* Easy to find <!-- .element: class="fragment" -->

<!-- .element: class="list-spaced" -->
Note:



# Not buried in Confluence



```bash
$ tree ~/dev/projects/awesome-api
.
|-- docs
|   `-- arch
|       |-- api-error-codes.md
|       |-- controller-convention.md
|       `-- serialization-format.md
|-- src
|-- test
`-- readme.md
```



# Markdown
![all the things meme](content/adr/img/all-the-things.png) <!-- .element: class="no-border" -->
##All The Things!



```markdown
# TITLE

## Status

## Context

## Considered Options

## Chosen Decision

## Consequences
```



```markdown
# Serialization Format

## Status

In Development | Proposed | Accepted | Superseded | Invalid
```
Note:
* `Superseded` should include link



```markdown
## Context

We need to have a consistent serialization scheme for the API.  It needs to be backwards and forwards compatible, as we don't control all of the clients.  Messages will be fairly high volume, but don't *need* to be human readable.
```



```markdown
## Considered Options

1. Json
1. ProtoBuf / gRPC
1. Apache Avro
1. Inbuilt Binary
1. Custom Built
```



```markdown
## Chosen Decision

* 3. Apache Avro

Avro was chosen because...
```



```markdown
## Consequences

We won't be able to view messages on the wire without some custom tooling, such as a small cli which can take a message and type and produce an output.
```



## Questions?
<br />

* some
* list
* of
* links

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
