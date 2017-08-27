
* Intro
* What we are going to cover
  * architecture of the solution
    * event storage
    * sticking to serverless
    * trade-offs
* aws things
  * cognito is great
    * terraform doesnt support it
    * and neither does cloudformation...
  * kinesis
    * kafka like
    * 2 week max age is problematic
    * selfhost using ec2/docker?
    * or just stuff events into dynamo/aurora/rds
  * s3
    * use it to host the website itself
      * react with hashrouting
    * store our views here, "free" reads
    * little bits of configuration data
  * api gateway
    * auth requests using cognito (manually apply this)
    * invoke the entrypoint lambda
  * lambda
    * the backbone of the app
    * implemented in javascript
      * run time is slightly faster
      * c# lambdas seem to take an age on first execute (and cause the lambda to timeout)
    * entrypoint lambda
      * stores events to dynamo
      * invokes the aggregate lambdas
    * aggregate lambdas
      * one per aggregate (and per plugin)
      * does "projection" to it's views
* architecture evolution
  * view inconsistencty
    * aggregate lambdas execeuting in parallel
    * s3 might miss some messages/update out of order
    * solution: sns & sqs
      * entrypoint lambda pushes event to sns
      * sns fanout to sqs queues
      * one aggregate lambda per queue
  * event replay
    * currently nothing implemented, but would be with sns/sqs.
    * so the same solution as above
  * event storage
    * kafka would be a better choice
    * but there is no aws facility for this
    * so you cant trigger a lambda on a message etc
    
