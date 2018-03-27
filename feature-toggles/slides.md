# Feature Toggles <!-- .element: class="push-down stroke" -->
## Andy Davies <!-- .element: class="stroke" -->
github.com/pondidum | @pondidum | andydote.co.uk  <!-- .element: class="smaller" -->

https://www.reddit.com/r/gaming/comments/692hqa/mccree_looking_good_in_ac_black_flag/dh3gu1a/ <!-- .element: class="attribution" -->

<!-- .slide: data-background="img/good-bad-ugly.png" data-background-size="contain" class="gbu" -->



![knight capital logo](img/knight-capital.png) <!-- .element: class="no-border" -->
https://en.wikipedia.org/wiki/File:Knight_Capital_Group_logo.svg <!-- .element: class="attribution" -->
Note:
* Who has heard of a company called Knight Capital?
* Ok, and *why* have you heard of them?
* American global financial services firm, doing market-making, trading
* They had a "slight" hiccup involving deployment and feature toggling
* on the day in question, they had $365 million in cash and equivalents



![knight capital shares](img/knight-capital-shares.jpg)
https://infocus.dellemc.com/dave_bagatelle/knight-capital-group-kcg-a-lesson-on-the-importance-of-sdlc-and-multi-environment-multi-user-testing/ <!-- .element: class="attribution" -->
Note:
* this is there shareprice
* that is a logarithmic graph
* look at that drop...



* Deployed to all servers <!-- .element: class="fragment" -->
* Enabled feature toggle <!-- .element: class="fragment" -->
* Problem is noticed... <!-- .element: class="fragment" -->
* Rolled back to last release <!-- .element: class="fragment" -->
* Problem gets worse... <!-- .element: class="fragment" -->

<!-- .element: class="list-unstyled" -->
Note:
* SMARS (split big orders into smaller ones)
* Powerpeg: counts shares against an order, stops orders once parent fulfilled. tracking had been moved earlier in pipeline, so powerpeg couldnt count. unused 8 years.
* 1 failed deployment, 8x problem
* 45mins toggle disabled
* $365 million in assets to start with
* the lost $450 million in 45 minutes



# Dead Code
Note:
* why is it still there!?
* delete it
* source-control history
* trunk based: deploy version with only the removal



#### Lesson 1
## NEVER Reuse A Toggle
Note:
* if they had a new toggle, problem wouldnt have happened



#### Lesson 2
## Name Toggles Well



![hp logo](img/hp.png) <!-- .element: class="no-border" -->
https://en.wikipedia.org/wiki/File:Hewlett-Packard_logo.svg <!-- .element: class="attribution" -->
Note:
* hp printers
* 400-800 devs, 10+ million loc
* 2x releases per year
* 5% features, 20% merging to master, integration
* build: 2 days, 1 week for merge status



## branching image
Note:
* branch per printer model
* `ifdefs` everywhere
* port fixes to all branches



* Single branch
* config file with printer capabilities
Note:
* compile-time to run-time toggles
* trunk based development



>Getting rid of code branching will often be your biggest efficiency gain

https://itrevolution.com/the-amazing-devops-transformation-of-the-hp-laserjet-firmware-team-gary-gruver/ <!-- .element: class="attribution" -->
Note:
* using branches as feature toggles
* toggles were fairly static, so a config file in the repo
* branching by abstraction



#### Lesson 3
## Architecture Matters
Note:
* toggles should be implemented smartly
* dont spatter the same if statement everywhere



## Toggle Types
Note:
* many dimensions to toggles
* how often it changes (static -> dynamic)
* how specific (user/group/all)



![toggle-table](img/toggle-table-release.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="slide-in none-out" -->



![toggle-table](img/toggle-table-experiment.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none" -->



![toggle-table](img/toggle-table-ops.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none" -->



![toggle-table](img/toggle-table-permission.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none" -->



![toggle-table](img/toggle-table-time.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none" -->



![toggle-table](img/toggle-table-time-release.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none" -->



![toggle-table](img/toggle-table-time-experiment.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none" -->



![toggle-table](img/toggle-table-time-ops.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none" -->



![toggle-table](img/toggle-table-time-permission.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none" -->



![toggle-table](img/toggle-table-time-permission-no.png)  <!-- .element: class="no-border" -->
<!-- .slide: data-transition="none-in slide-out" -->




## Questions?
![questions](img/questions.jpg)

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->