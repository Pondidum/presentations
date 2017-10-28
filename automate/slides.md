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
* let's start at the beginning...



# Dev Environment

Note:
* time to set up a new dev machine!
* it'll be so much quicker than my last
* I just need to install...everything


![depressing](img/depressing.png)
http://www.blastwave-comic.com/comics/14.jpg <!-- .element: class="attribution" -->


![chocolatey](img/chocolatey.svg) <!-- .element: class="no-border" -->
https://chocolatey.org/ <!-- .element: class="attribution" -->


```bash
cinst Microsoft-Hyper-V-All -source windowsFeatures
cinst -y docker-for-windows
```

Note:
* scripting installs is easy now!
* but what about restarts?


![boxstarter](img/boxstarter.png)
http://boxstarter.org/ <!-- .element: class="attribution" -->

Note:
* BoxStarter builds on top of chocolatey
* automatic restart detection
* automatic re-login and resume of scripts!


```powershell
Set-WindowsExplorerOptions `
    -EnableShowHiddenFilesFoldersDrives `
    -EnableShowFileExtensions `
    -DisableOpenFileExplorerToQuickAccess

cinst visualstudiocode
cinst visualstudio2015professional
cinst resharper
cinst git.install

cinst Microsoft-Hyper-V-All -source windowsFeatures
cinst docker-for-windows
```

Note:
* we use a much bigger script than this at work!
* Now we have a running machine, what next?



# Git

Note:
* Lets speed up our git workflow a bit!


* `git s`
* `git cm "my commit message"` <!-- .element: class="fragment" -->
* `git dc` <!-- .element: class="fragment" -->
* `git scrub` <!-- .element: class="fragment" -->

<!-- .element: class="list-unstyled" -->
Note:
* Look at how many characters we can save typing!


# Rebasing?

Note:
* So how many people here use rebase?
* those of you who don't, I assume you dont use git?
* seriously rebase.
* who doesn't love seeing this message


```bash
$ git rebase master
Cannot rebase: You have unstaged changes.
Please commit or stash them.
```


* `git save`
* `git rebase` <!-- .element: class="fragment" -->
* `git undo` <!-- .element: class="fragment" -->
* `git push` <!-- .element: class="fragment" -->
* `git web` <!-- .element: class="fragment" -->

<!-- .element: class="list-unstyled" -->
Note:
* you could make an alias to do all of this...


`git pr`

Note:
* now, talking of pull requests


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

Note:
* runs your tests (best hope they're fast!)
* demo...
* note we stash everything, then test, then pop stash
* this is so we only test the things being committed :)


```bash
$ git cm "hooks: add prepare commit message"
[21:24:08] Using gulpfile ./gulpfile.js
[21:24:08] Starting 'test'...
[21:24:08] 'test' errored after 700 us
[21:24:08] Error in plugin 'test-runner'
Message:
    There were an odd number of characters.
```


```bash
$ git cm "hooks: add prepare commit message"
[21:24:08] Using gulpfile ./gulpfile.js
[21:24:08] Starting 'test'...
[21:25:22] Finished 'test' after 484 us
[master 5221827] hooks: add prepare commit message
 1 file changed, 5 insertions(+)
 create mode 100644 hooks/prepare-commit-msg
```


# Installation?


```bash
#!/bin/sh

# windows not happy about symbolic links
if [[ -n "$WINDIR" ]]; then
  find hooks/ -exec sh -c 'ln --force ./{} .git/hooks/$(basename {})' \;
else
  find hooks/ -exec sh -c 'ln -s --force ./{} .git/hooks/$(basename {})' \;
fi

echo 'hooks installed.'
```

Note:
* scripted of course!
* windows doesnt like symlinked files through ln, so hard links
* shame we cant write a hook to install hooks on pull...



# Pull Requests

Note:
* pull requests are the backbone of our dev process, so there is quite a lot to hang off them


# Github integration


```
OnPullRequest
    => Fix PR title
    => Add documentation links
    => Check merge target
```

Note:
* implemented in aws lambda
