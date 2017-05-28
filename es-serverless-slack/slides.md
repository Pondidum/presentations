## Building a Serverless, EventSourced, Slack Clone.
![Serverless](img/serverless.png)
{some attribution here}<!-- .element: class="image-attribution"-->




### Andy Davies
### AndyDote.co.uk | github.com/pondidum | @pondidum
Note:
* I work for Lindorff Oy,  helping to build an online payment service



# Why build this?
Note:
* to see if complex apps could be made without a single EC2 instance
* And as you can guess from that, this is AWS based
* practical experience with aws services I havent used before



# Chat App ==  Simple
# right? <!-- .element: class="fragment" -->



* channel_created
* message_sent
* message_edited
* user_joined_channel
* user_left_channel
* user_registered
Note:
* this was my initial idea for structure
* what are our aggregates?
  * can messages be posted to multiple channels at once? (e.g. hashtags on twitter)
  * or are channels owners of messages?
* storage of events
  * store as a single stream?
  * split by aggregate?



<ul class="left">
  <li><h2>UserAggregate</h2></li>
  <li>user_registered</li>
  <li>channel_created</li>
  <li>user_joined_channel</li>
  <li>user_left_channel</li>
</ul>
<ul class="right">
  <li><h2>ChannelAggregate</h2></li>
  <li>channel_created</li>
  <li>user_joined_channel</li>
  <li>user_left_channel</li>
  <li>message_sent</li>
  <li>message_edited</li>
</ul>
Note:
* we have events which multiple aggregates care about
* we can either introduce a separation



# "events from the ui" (commands)
Note:
* command -> store -> router -> aggregate methods -> aggregate events
* each aggregate would create it's own eventsourcing event based on the command



# "aggregate events" (events)
Note:
* event -> store -> router -> aggregate
* events from the ui get sent to each



Note:
* or not bother to start with.
* single stream is an easier starting point
* storing in a single stream helps here, we can re-process events to make the split later



![kafka logs](img/log_consumer.png)
https://kafka.apache.org/intro <!-- .element: class="image-attribution"-->
Note:
* Kafka would be pretty ideal for this
* But it's not serverless



## Kinesis
![Kinesis](img/AWS-Summit_recap_KinesisStreams.png)
Note:
* Kinesis streams are pretty similar to Kafka
* But there is a problem, we want permanent log storage
* "the retention period and is configurable in hourly increments from 1 to 7 days."
* choice:
  * use kafka (ec2/docker)
  * work around Kinesis
  * something else
* not going to break the serverless requirement that easily!
  * events will have timestamps & uuids
  * so store in rds/dynamo
  * if an aggregate needs stream position, it can store last-read uuid



## Processing
Note:
* basic idea is a endpoint
  * stores the event to permanent storage
  * forwards it to aggregates (async)
  * returns "ok"
* api gateway and lambda for this



```javascript
exports.handler = function(awsEvent, context, callback) {

  const metadata = {
    timestamp: new Date().getTime(),
    eventId: uuid()
  }

  const event = Object.assign(
    {},
    JSON.parse(awsEvent.body),
    metadata)

  writeToStorage(event)
    .then(data => triggerAggregates(event)
    .then(data => sendOkResponse(callback)))
}
```
Note:
* error handling has been omitted
* the lambda controls the timestamp and eventId
* no sequence number used
  * don't want to cause blocking resource
  * ordering by date should be good enough



## Aggregates
note:
* we only need two aggregates to start with
* channels are the primary aggregate
* has messages, users watching



```c#
public class ChannelAggregate : AggregateRoot
{
  public void Join(User user)
  {
    Apply(new UserJoinedChannelEvent(
      UserId: user.id,
      ChannelId: this.id
    );
  }

  private void Handle(UserJoinedChannelEvent e)
  {
    _users.Add(e.UserId);
  }
}
```
Note:
* event creation happens within the aggregate (commands to api)
* handler execution is called by `Apply` and by loading
* Projections are elsewhere



```c#
public class ChannelProjection : Projection
{
  public IEnumerable<string> => _users;
  public string Name { get; private set; }
  public string Description { get; private set; }

  private readonly HashSet<string> _users;

  private void Handle(UserJoinedChannelEvent e)
  {
    _users.Add(_userView.GetOrDefault(e.UserId).Name);
  }

  private void Handle(ChannelCreatedEvent e)
  {
    Name = e.Name;
    Description = e.Description;
  }
}
```
Note:
* uses a userview projection/service to get usernames from the id
* this would be async to saving of events
* we have the option of splitting the aggregates
  * public methods clientside
  * handlers serverside



```javascript
const handleUserJoinedChannel = event => {

  updateView({ viewName: 'CHANNEL', id: event.channelId,
    callback: view => {
      view.users.push(userView[event.userId].name)
    }
  })

  updateView({ viewName: 'ALL_CHANNELS', defaultView: [],
    callback: view => {
      view[event.channelId].users += 1
    }
  })

}
```
Note:
* views are just json files in s3
* `updateView` is a function which upserts them



```
const updateView = (viewName, id, callback) => {
  const path = id
    ? `events/views/${viewName}.json`
    : `events/views/${viewName}/${id}.json`
  const query = { Bucket: 'crowbar-store', Key: key }

  s3.getObject(query, (err, data) => {

    const body = data && data.Body ? JSON.parse(data.Body) : null
    const content = callback(body) || body

    const command = Object.assign({}, query, {
      Body: JSON.stringify(content, null, 2),
      ContentType: 'application/json',
      ACL: 'public-read'
    })

    s3.putObject(command, (err, data) => {})
  }
}
```
Note:
* again no error handling, or most-recent update time
* this has some consistency problems which we will go through later too



![high level architecture](img/crowbar-architecture.png)
Note:
* a static website deployed to s3 (react based)
* an api-gateway, protected by cognito
* a lambda to receive events from the website
* one lambda per aggregate root to process events
* views stored as json in s3
* static site loads s3 objects
* n.b., not really using the event storage for anything yet



# Scalability?
Note:
* to a point, there are issues however
* s3 updates are potentially lossy
* also entries could appear out of order
  * especially if you are getting multiple messages a second



![sns](img/crowbar-architecture-sns.png)
Note:
* use sns and sqs
* sns will publish a message to multiple sqs queues
* have one aggregate per queue
* this enforces in-order messaage processing
* why not do this to start with? start with mvp



# Event replay
Note:
* should be fairly simple to manage
* for each event, sns -> sqs it
* or for a specific aggregate, just event -> sqs
