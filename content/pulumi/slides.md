# Adventures with Pulumi <!-- .element: class="stroke-black" -->
<br/>
<br/>
<br/>
## Andy Davies <!-- .element: class="stroke-black text-right" -->
github.com/pondidum | @pondidum | andydote.co.uk  <!-- .element: class="smaller white text-right" -->

https://marvel-movies.fandom.com/wiki/Palladium <!-- .element: class="attribution white" -->

<!-- .slide: data-background="content/pulumi/img/bg.png" data-background-size="cover" class="intro" -->
Note:
* personal project in pulumi



- if statements! <!-- .element: class="fragment" -->
- loops! <!-- .element: class="fragment" -->
- type checking! <!-- .element: class="fragment" -->

<!-- .element: class="list-unstyled list-spaced" -->



- ![TS](content/pulumi/img/ts.png)
- ![JS](content/pulumi/img/js.png)
- ![GO](content/pulumi/img/go.png)
- ![CS](content/pulumi/img/cs.png)
- ![Py](content/pulumi/img/py.png)

<!-- .element: class="list-unstyled list-inline" -->



```ts
const endpoint = new awsx.apigateway.API("hello", {
  routes: [
    {
      path: "/",
      localPath: "www",
    },
    {
      path: "/source",
      method: "GET",
      eventHandler: (req, ctx, cb) => {
        cb(undefined, {
          statusCode:       200,
          body:             base64Json({ name: "AWS" }),
          isBase64Encoded:  true,
          headers:          { "content-type": "application/json" },
        })
      }
    }
  ]
});
```

<!-- .element: class="full-height" -->



![Pulumi Up Output](content/pulumi/img/pulumi-api.png)



# Unit Testing



```ts
test("listeners create target groups", async () => {
  const infra = await import("./loadbalancer");

  const lb = new infra.LoadBalancer("front", {
    listeners: [{ port: 80, protocol: "HTTP" }],
    subnets: [],
    vpcId: "",
  });

  expect(lb.targets).toHaveLength(1);
});
```

Note:
- can only test inputs
- mocking pulumi
- kinda sucks



# Policy Testing



```ts
const clusterPolicy: ResourceValidationPolicy = {
  name:             "clusters-have-odd-instance-count",
  description:      "Clusters work better when there are an odd number of machines involved",
  enforcementLevel: "advisory",

  validateResource: validateResourceOfType(
    aws.autoscaling.Group,
    (asg, args, report) => {
      if (asg.desiredCapacity && asg.desiredCapacity % 2 == 1) {
        return
      }

      const name = asg.name
      const count = asg.desiredCapacity

      report(
        `${name}'s capacity should be an odd number, but was ${count}`
      );
    }
  ),
};
```

<!-- .element: class="full-height" -->



![Pulumi Policy Advisory](content/pulumi/img/pulumi-policy.png)



# Integration Testing



```go
func TestExamples(t *testing.T) {
  awsRegion := os.Getenv("AWS_REGION")
  cwd, _ := os.Getwd()

  integration.ProgramTest(t, &integration.ProgramTestOptions{
    Quick:       true,
    SkipRefresh: true,
    Dir:         path.Join(cwd, ".."),
    Config: map[string]string{
      "aws:region": awsRegion,
    },
  })
}
```

<!-- .element: class="full-height" -->
<!-- .slide: data-transition="slide-in fade-out" -->




```go
func TestExamples(t *testing.T) {
  awsRegion := os.Getenv("AWS_REGION")
  cwd, _ := os.Getwd()

  integration.ProgramTest(t, &integration.ProgramTestOptions{
    Quick:       true,
    SkipRefresh: true,
    Dir:         path.Join(cwd, ".."),
    Config: map[string]string{
      "aws:region": awsRegion,
    },
    ExtraRuntimeValidation: func(t *testing.T, stack integration.RuntimeValidationStackInfo) {
      var foundBuckets int
      for _, res := range stack.Deployment.Resources {
        if res.Type == "aws:s3/bucket:Bucket" {
          foundBuckets++
        }
      }
      assert.Equal(t, 1, foundBuckets, "Expected to find a single AWS S3 Bucket")
    },
  })
}
```

<!-- .element: class="full-height" -->
<!-- .slide: data-transition="fade-in slide-out" -->



# The Negatives



![Habitat Fragmentation](content/pulumi/img/fragmentation.jpg)
http://www.irishenvironment.com/iepedia/habitat-fragmentation/  <!-- .element: class="attribution" -->

Note:
- jen20's vpc module is ts/py only



![Footgun](content/pulumi/img/footgun.png)
https://allthingslearning.wordpress.com/2012/07/22/between-a-rock-and-a-very-hard-placept-01/ <!-- .element: class="attribution" -->



## Questions?
<br />

* https://pulumi.com/

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
