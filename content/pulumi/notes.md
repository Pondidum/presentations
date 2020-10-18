# pulumi


Pros
- programming language
  - for loops!
  - if statements!
  - type checking!
- multiple languages
  - go, js, ts, python
- no state management
  - you can run a state server yourself if you want
- no secret managemnt
  - you can switch out the secret backend
- testing!
  - write tests in your favourite language
- libraries
  - distribute common infra patterns as a lib
  - pulumi do this too (aws vs aws-x)
- naming
  - pulumi makes sure resources have unique names
  - can be bad for security groups


Cons
- fragmentation
  - using js? can't use that golang version
  - example
    - i wanted to use James Nugent's AWS-VPC lib, which is only available in js/ts and python



Testing!

- Unit Testing
  - kinda sucks
  - mock out some functions
  - you can only verify things that you input, or have mocked the result of
- Property Testing
  - aka policy testings
    ```
    const clusterPolicy: ResourceValidationPolicy = {
      name: "clusters-must-have-odd-instances",
      description:
        "Clusters work better when there are an odd number of machines involved",
      enforcementLevel: "advisory",
      validateResource: validateResourceOfType(
        aws.autoscaling.Group,
        (asg, args, report) => {
          if (asg.desiredCapacity && asg.desiredCapacity % 2 == 0) {
            report(
              asg.namePrefix +
                "'s Desired capacity should be an odd number, but it was " +
                asg.desiredCapacity
            );
          }
        }
      ),
    };
    ```
  - pretty cool
  - this applies to _all_ asgs
