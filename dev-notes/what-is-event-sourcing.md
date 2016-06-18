
# Why EventSourcing will make you Happier, Wiser, and Efficient

* event sourcing is not that complicated
* infact you interact with systems which are eventsourced daily
	* sourcecontrol is a kind of eventsourcing

* before we look at what it is, lets look at the standard way of storing things
* bank accounts make a great example...but that is getting to be a bit of a tired example
* same goes for online shopping baskets
* so why not a timesheet and payment system? nice and simple (or not).
* ignoring the domain itself, our traditional (and simplified) relation storage would look something like this:

	* User Table: id, name
	* Timesheets Table: id, user_id, approver_id, week_date, hours_worked, pay_rate
	* Payments Table: id, user_id, timesheet_id, date_paid
	* Accounts Table: id, user_id, account_name, account_number, sort_code

* this works well to start with, but we encounter problems over time:
	* the user changes bank account
	* the user wants an old payslip re-sending
	* someone wants to know which account got paid into for a given week.

* and questions we can't answer:
	* was the timesheet approved first time, or did it take multiple goes?
	* does this happen often for a given user?
	* does this happen often for a given approver?

* we could solve these problems by:
	* add change-history tables
	* add links between accounts and payslips etc

* these work, but can't be applied historically

* this model can be described as transient - we have the current values of things, but historic information has to be hacked onto it.
	* i have seen the audits done with some fairly nasty triggers :(

* event sourcing changes the model to be a stream of events, which builds up the current state instead

* the system above could be represented with the following events:

	* userCreated
	* userCreatedTimeSheet
	* userUpdatedTimeSheet
	* userSubmitsTimeSheet
	* approverApprovesTimeSheet
	* approverRejectsTimeSheet
	* systemPaysUser
	* etc.

* each event has the data relating to a change, and only that data.

	* a userUpdatedTimeSheet event would have:
		* timestamp, user_id, timesheet_id, hours_worked
		* timestamp, user_id, timesheet_id, pay_rate

	* a approverRejectsTimeSheet might have:
		* timestamp, approver_id, timesheet_id, comments
