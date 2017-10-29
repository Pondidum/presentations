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


![assembly line](img/automation.jpg)
http://www.oemupdate.com/cover-story/how-automation-could-impact-make-in-india/ <!-- .element: class="attribution" -->

Note:
* while I love our product, and enjoy seeing people use it
* what I really love is making other dev's lives easier
* if they want to add business value, I want to make it easier for them to do
* I dont want them to have to think about repo structure, build scripts, teamcity, infrastructure etc
* just producing value
* I work on 1 or 2 teams, which means I work with 11 other developers
* which means there are at least 12 machines
* with mostly the same software (little differences)



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



# Project Creation

Note:
* Not really a tech talk without mentioning microservices is it?
* Having a different folder structure for each service is bad
  * mental effort
  * sln/src/source/lib, build script, dependencies, docs
* So as we are a dotnet team, we used...


![Yeoman](img/yeoman.png)
http://yeoman.io/  <!-- .element: class="attribution" -->

Note:
* Yeoman! which is really aimed at javascript applications
* It is a cli based tool, and you can write your own templates easily


```bash
npm install --global yo
git clone https://github/.../generator-lindorff
cd generator-lindorff
npm link
```


```bash
$ yo --generators

  lindorff
    api
    github
    mvc
    nuget
    service
    solution
    tooling
```

Note:
* api, mvc, service, nuget are the most used
* the use other sub-generators (project prefix usually)
* yo lindorff:nuget


```bash
$ yo lindorff:nuget
? enter the name of the solution: Lindorff.Demo
? enter a short description of the solution: This is a demo nuget package
? enter the name of the project: Lindorff.Demo
```

Note:
* asks many questions
* runs npm install
* runs gulp script


```
|-- Lindorff.Demo.sln
|-- gulpfile.js
|-- nuget.config
|-- package.json
|-- readme.md
`-- src
    |-- Lindorff.Demo
    |   |-- Lindorff.Demo.csproj
    |   |-- Lindorff.Demo.csproj.nuspec
    |   `-- Properties
    |       `-- AssemblyInfo.cs
    `-- Lindorff.Demo.Tests
        |-- Lindorff.Demo.Tests.csproj
        |-- Properties
        |   `-- AssemblyInfo.cs
        |-- Scratchpad.cs
        `-- packages.config
```

Note:
* leaves you with a directory something like this


`yo lindorff:github`

Note:
* uses the github api to create a private repository, setting up:
  * correct teams (owners, admins etc)
  * branch protection
  * webhooks (we'll come back to these later)
* we want to do similar for TeamCity
  * but it's rest api is awful
* and for octopus
  * which has a nice api
  * but we haven't worked out a standard pipeline yet



# Git

Note:
* Now we have created our new repository we can move on to automating git


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


* Fix PR title
* Add documentation links <!-- .element: class="fragment" -->
* Check merge target <!-- .element: class="fragment" -->

<!-- .element: class="list-unstyled list-spaced" -->

Note:
* implemented in aws lambda


* `git push origin feature-NewCoolThing-PAY-1234`
* ![PR title before](img/pr-title-1.png) <!-- .element: class="fragment no-border" -->
* ![PR title before](img/pr-title-2.png) <!-- .element: class="fragment no-border" -->

<!-- .element: class="list-unstyled list-spaced center" -->


* ![PR docs before](img/pr-docs-1.png) <!-- .element: class="no-border" -->
* ![PR docs after](img/pr-docs-2.png) <!-- .element: class="fragment no-border" -->

<!-- .element: class="list-unstyled list-spaced center" -->