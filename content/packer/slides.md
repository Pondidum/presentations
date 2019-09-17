# WTF is Packer  <!-- .element: class="stroke-black blue" -->
## And why should I care?, <!-- .element: class="stroke-black blue" -->
<br/>
## Andy Davies <!-- .element: class="stroke-black blue" -->
github.com/pondidum | @pondidum | andydote.co.uk  <!-- .element: class="smaller white" -->

http://wallpaper21.com/wp-content/uploads/2016/10/green-bay-packer-wallpaper-HD7.jpg <!-- .element: class="attribution white" -->

<!-- .slide: data-background="content/packer/img/green-bay-packer-wallpaper-HD7.jpg" data-background-size="cover" class="intro intro-full" -->



![Packer Logo](content/packer/img/Packer_PrimaryLogo_FullColor.png)



![flow overview](content/packer/img/flow.png)



```json
{
  "variables": {
    "is_release":      "false",
    "image_tag":       "local",
    "pwd":             "{{ env `PWD` }}",
    "current_version": "{{ consul_key `versions/deployed/current` }}",
    "access_key":      "{{ vault `/secret/data/api` `access_key`}}"
  }
}
```

```json
{
  "type": "shell",
  "environment_vars": [
    "RELEASE_BRANCH={{ user `is_release` }}",
  ],
}
```
<!-- .element: class="fragment" -->

```bash
packer build -var "image_tag=$(git rev-parse HEAD)" agent.json
```
<!-- .element: class="fragment" -->



```json
{
  "builders": [
    {
      "type": "docker",
      "image": "node:12.8.1-buster-slim",
      "commit": true
    },
  ]
}
```



```json
{
  "builders": [
    {
      "type": "amazon-ebs",
      "region": "eu-west-1",
      "instance_type": "t2.small",

      "source_ami_filter": {
        "filters": {
          "virtualization-type": "hvm",
          "name": "ubuntu/images/*ubuntu-xenial-16.04-amd64-server-*",
          "root-device-type": "ebs"
        },
        "owners": ["099720109477"],
        "most_recent": true
      },

      "ami_name": "build-agent-{{ timestamp }}",
      "subnet_id": "subnet-xxxxxxxx",
      "associate_public_ip_address": false
    },
  ]
}
```
<!-- .element class="full-height" -->



```json
{
  "builders": [
    {
      "type": "docker",
    },
    {
      "type": "amazon-ebs",
    },
    {
      "type": "vagrant",
      "provider": "libvirt",
      "source_path": "hashicorp/precise64",
      "communicator": "ssh"
    }
  ]
}
```



```json
"provisioners": [
```
```json
  {
    "type": "shell",
    "script": "./deploy/packer/install.sh"
  },
```
<!-- .element class="fragment" -->
```json
  {
    "type": "file",
    "source": "./src",
    "destination": "/src"
  },
```
<!-- .element class="fragment" -->
```json
  {
    "type": "shell",
    "environment_vars": [
      "RELEASE_BRANCH={{user `is_release`}}",
    ],
    "scripts": [
      "./deploy/packer/build.sh",
      "./deploy/packer/finalise.sh"
    ]
  }
```
<!-- .element class="fragment" -->
```json
]
```



```bash
#!/bin/sh -e

apt-get update > /dev/null
apt-get upgrade -y > /dev/null
apt-get install -yq \
  git \
  zip \
  curl \
  sshpass \
  python \
  time \
  > /dev/null

yarn config set cache-folder /tmp/yarn-cache

mkdir -p src
```



```bash
#!/bin/sh -e

cd /src

yarn install --no-progress --frozen-lockfile --production
cp -R node_modules /srv/app/
yarn install --no-progress --frozen-lockfile

yarn run build
yarn run test

if [ "$RELEASE_BRANCH" = "true" ]; then
  yarn run precache
fi

mkdir -p /srv/app
mv /src/dist /srv/app/
```



```bash
#!/bin/bash -e

shopt -s dotglob

rm -rf /root/*
rm -rf /tmp/*
rm -rf /var/log/*
rm -rf /src

# Clean apt
apt-get -y purge libx11-data xauth libxmuu1 libxcb1 libx11-6 libxext6;
apt-get -y autoremove;
apt-get -y clean;
```



```json
"post-processors": [
  {
    "type": "docker-tag",
    "repository": "presentations",
    "tag": "{{ user `image_tag` }}"
  }
]
```



# But Why?



## Immutable Infrastructure

* Tested Units of Deployment
* Portable (ish)
* Fast (to start/stop)

<!-- .element: class="list-spaced list-unstyled" -->



## Local Development

* Docker
* Vagrant Box
* Virtualbox/hyper-v/libvirt/etc.

<!-- .element: class="list-spaced list-unstyled" -->



## CI

* Amazon Machine Image
* GCP Images
* Virtualbox/hyper-v/libvirt/etc.

<!-- .element: class="list-spaced list-unstyled" -->



# Dockerfiles?

* Dockerfile DSL <!-- .element: class="fragment" -->
* Layers are fun <!-- .element: class="fragment" -->
* Multistage build are more fun <!-- .element: class="fragment" -->

<!-- .element: class="list-spaced list-unstyled" -->



## Questions?
<br />

* https://packer.io
* https://www.vagrantup.com/
* https://andydote.co.uk/presentations/index.html?packer

<!-- .element: class="list-spaced small" -->
<br />

github.com/pondidum | twitter.com/pondidum | andydote.co.uk  <!-- .element: class="small" -->
