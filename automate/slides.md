## Automate Everything
### Not Just Your Builds
<br />
<br />
#### Andy Davies

github.com/pondidum | twitch.tv/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->

Note:
* But I mean, still automate your builds :)


![Lindorff Oy](img/lindorff.svg) <!-- .element: class="no-border" -->

Note:
* Over in Finland
* No, I am not Finnish...
* But, if you want to come build cool online payment tech, come see me afterwards :)
* and that's enough of that



# What can we automate?

Note:
* basically anything you do more than a few times
* let's start with git, as that's where we spend a lot of time



# Git


* git s
* git cm  "my commit message"
* git dc

<!-- .element: class="list-unstyled" -->
Note:
Formatting!


* git save
* git undo
* git scrub
* git web

<!-- .element: class="list-unstyled" -->
Note:
Formatting


`git checkout -b feature-NewCoolThing-PAY-1234`

Note:
* task/backlog ids go at the end of branch names
* we also put them in commit messages for other integration


### prepare-commit-msg
```bash
#!/bin/sh

TAG=$(git rev-parse --abbrev-ref HEAD | grep -oP '(?<=-)([a-zA-Z]{3,4}-\d*)')

echo -n "[$TAG]"' '|cat - "$1" > /tmp/out && mv /tmp/out "$1"
```

Note:
* extracts the task tag from the branch name
* prefixes commit messages with it
* don't forget error handling...


```bash
$ git cm "my awesome feature"
[feature-NewCoolThing-PAY-1234 87b7d9a] [PAY-1234] my awesome feature
 1 file changed, 1 insertion(+), 1 deletion(-)
```



### pre-commit

```bash
#!/bin/sh
git stash -q --keep-index

gulp test --no-cover
RESULT=$?

git stash pop -q

[ $RESULT -ne 0 ] && exit 1
exit 0
```



# Pull Requests

Note:
* pull requests are the backbone of our dev process, so there is quite a lot to hang off them



# Github integration

OnPullRequest
    => Fix PR title
    => Add documentation links
    => Check merge target

Note:
* implemented in aws lambda
