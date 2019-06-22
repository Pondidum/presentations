# Architecture Decision Records <!-- .element: class="stroke-black text-left" -->
## Andy Davies <!-- .element: class="stroke-black text-left" -->
github.com/pondidum | @pondidum | andydote.co.uk  <!-- .element: class="smaller text-left" -->

https://www.pesark.com/news2016-2015.html <!-- .element: class="attribution text-left" -->

<!-- .slide: data-background="content/adr/img/hki.jpg" data-background-size="cover" class="intro" -->
Note:
wat



# Documentation Sucks



## Why on earth was it done like this???



* Architecture Decision
* Architecture Decision Record <!-- .element: class="fragment" -->
* Architecture Decision Log <!-- .element: class="fragment" -->
* Architecturally Significant Requirement <!-- .element: class="fragment" -->

<!-- .element: class="list-spaced list-unstyled" -->
Note:




* Easy to write
* Easy to read <!-- .element: class="fragment" -->
* Easy to find <!-- .element: class="fragment" -->

<!-- .element: class="list-spaced list-unstyled" -->
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
Note:
* Immutable (except `status`)
* multiple formats available
* this is a hybrid one I invented



```markdown
# Serialization Format

## Status

In Development, Proposed, Accepted, Rejected, Superseded, Deprecated
```
Note:
* short title
* `Superseded`/`Deprecated` should include link



```markdown
## Context

We need to have a consistent serialization scheme for the API.  It needs to be backwards and forwards compatible, as we don't control all of the clients.  Messages will be fairly high volume, and don't *need* to be human readable.
```
<!-- .element: class="wrap" -->



```markdown
## Considered Options

1. **Json**: Very portable, and with seriaizers available for all languages.  We need to agree a date format, and numeric precision however.  The serialization should not include white space to save payload size.  Forwards and Backwards compatability exists, but is the developer's responsibility.

2. **Apache Avro**: Binary format which includes the schema with the data, meaning no need for schema distribution.  No code generator to run, and libraries are available for most languages.

3. **Inbuilt Binary**: The API is awkward to use, and it's output is not portable to other programming languages, so wouldn't be easy to consume for other teams, as well as some of our internal services.

4. **Custom Built**: A lot of overhead for little to no benefit over Avro/gRPC etc.

5. ...
```
<!-- .element: class="wrap full-height" -->



```markdown
## Chosen Decision

**2. Apache Avro**

Avro was chosen because it has the best combination of message size and schema definition.  No need to have a central schema repository set up is also a huge benefit.
```
<!-- .element: class="wrap" -->



```markdown
## Consequences

As the messages are binary format, we cannot directly view them on the wire.  However a small CLI will be built to take a message and pretty print it to aid debugging.
```
<!-- .element: class="wrap" -->
Note:
* what do we need to start doing because of this?



## Questions?
<br />

* https://github.com/joelparkerhenderson/architecture_decision_record
* https://www.thoughtworks.com/radar/techniques/lightweight-architecture-decision-records

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
