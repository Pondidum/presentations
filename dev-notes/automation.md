# Titles

* Automating the Automation
* Who will Automate the Automations
* Automating all the things!
* Automate your entire environment
* Automation; it's not just build scripts
* Automate Everything, not just your builds *submitted*

# Abstracts

When we think of automation, we tend to think of build and deployment scripts, but there is so much more that can be automated; we can automate infrastructure, development environments, workflows and processes and regular tasks.  We can save a lot of time, and remove mental workloads by having scripts do things for us.

Come to this talk to see all the automation we have embedded in our organisation so far, and what we are going to be trying out in the future.


* naming pull requests
* pull request to the correct branch
* links to documentation
* checks for docs, tests, logging dashboard, etc.
* auto update tickets with resolutions
  * e.g. "fixes pay-123"
* auto create a branch (for features starting)
* prevent merging to master until deployment
* auto merge to master after x hours of deployment
* auto fixing tickets
  * 1st step is adding to our fix tool `paf purchase-export --purchaseNumber 123132123 --store 123`
  * this does the right checks, then tells you what it will do to fix things, and asks if you want to proceed
  * 2nd step is to hook up execution of this to our error logging
  * add a parameter so it can add a comment to the alert ticket, and close the ticket.
* creating projects
  * `yo lindorff`
  * creates different project types: mvc, api, service, tooling, test projects, acceptance projects etc.
  * can also create github repos!
  * todo:
    * teamcity build configurations
    * octopus deploy configurations
