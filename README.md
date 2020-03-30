# Project Overview

- asset-manipulation-unity
- behavior-centric-unity(expanded later)
- complete-unity
- dots-roll-a-ball

## Unity Project - Asset and Data

- asset-manipulation-unity

## Unity Project - Behavior

Plan to expand the current one project into multiple to make the development easier as different projects might have different setup

- behavior-client-normal-unity
- behavior-client-observer-unity
- behavior-server-unity

- dots-roll-a-ball

### dots-roll-a-ball

This project starts with an official roll-a-ball tutorial in DOTS form. A great reference to know how the project is done in DOTS.

## Unity Project - Complete

- complete-unity

## Create Git Repo branch

```sh

time DOCKER_BUILDKIT=1 docker image build -t make-asset:latest --no-cache --build-arg SSH_PRIVATE_KEY="$(cat ./credentials/GitLab/ssh-keys/auto_id_rsa)" -f ./docker-configs/Dockerfile-dev .

```

The following is not correct. Need to modify to suit the use.

```sh

time DOCKER_BUILDKIT=1 docker image build -t prepare-build-release:latest --no-cache --build-arg SSH_PRIVATE_KEY="$(cat ./credentials/Bitbucket/ssh-keys/auto_id_rsa)" -f ./prepare-ucb-build-release/Dockerfile-push-remote .

docker container run -it prepare-build-release /bin/sh

```

## Repo and cost consideration

Setup git repo on GitLab will have advantage over the users and space limit compared to other service. However, the integration to other service will be a bit complicated compared to GitHub or Bitbucket.

There are at least two branches setup for the project. One is used for asset making which will be using huge space generally speaking. The other one is used for the acutal project release. This will be using less space as most asset will be packed into addressble and served from CDN during the game starts.

Mirror the repo to Google source repo will be a good approach but there are some caveats to overcome. First, the size of Google source will impacts the price, this might not be the biggest issue but pricing certainly is important. Second, at time writing the document, Google source repo has no LFS support. This means the sync will be incomplete if GitLab part uses LFS. Third, need to learn how to write GitLab pipeline in addition to the current method being deviced in Google Cloud Version.

Asset making branch contains huge data. When data sending to Google source repo, it takes time. It is better to get the required data into Google storage first. This reudce the time and cost to build.

### Mutliple git repo setup

Need a way to trigger the build when there is any changes to development use repo or build use repo.

For development purpose, there will be lots of members to devote the project development. Using GitLab will be great as it permits lots of people. But since its integration to other service is a bit hard. It is better to have another repo setup for build use.

GitHub will be a great choice over BitBucket as it has app that trigger the build on Google Cloud Build service. Once the build triggers, it will make a project. This made project is supposed to have huge space constraint. Pushing into GitHub or BitBucket won't be ok. As the space is limited except to buy additional space. So pushing back to GitLab becomes viable solution again.

- GitLab
- GitHub

So up until, the overall plan for asset making project is

1. Development use repo is at GitLab
2. Build preparation repo is at GitHub
3. Build ready repo is at GitLab
4. Unity Cloud Build takes build ready repo to build
5. UCB builds mostly asset bundle via Addressable and store file in S3 or Google Storage


The release build can follows, so it looks like

1. Development use repo is at GitLab
2. Build preparation repo is at GitHub
3. Build ready repo is at GitLab
4. Unity Cloud Build takes build ready repo to build
5. UCB builds artifact and push to AppCenter

### More on the asset making project flow

It is complicated for non-programmer to exercise such flow at daily basis. The best approach for artists and designers will be push to asset making project develpment use git repo normally. But when the build of asset making project is requsted, there will be automated process kicked in to relif the complexity.

The flow might be

At branch **master** which has the folder **asset-manipulation-unity**, using a container to extract asset and make a zipped file then uploaded to Google Cloud Storage. Then forming a branch **prepare-build-asset** which has removed asset and requried code pushing to GitHub git repo.

After pushing to GitHub, Google Cloud Build(Version) should be triggered. Once it is triggered, it should fetches

- references.zip
- assets.zip

from Google Cloud Storage, and code from GitHub. It then setup a project which pushes into GitLab git repo.

Technically, all the process, including getting zip files can be done locally, but then there will be round trip time spending on it. One is getting file from remote storage(in this case Google Cloud Storage), another is to push the project into GitLab(on the remote). Most of the time, local to remote will be slower and it has to do  the process twice.

Unity Cloud Build should monitor this GitLab repo via webhook. It builds and upload file(s) to Google Cloud Storage.

### More on the release project flow

Remove asset part and form branch **prepare-build-release**. Push this to GitLab, Unity Cloud Build should pick it up whenever there is any change.

## Terraform

Can these asset be manipulated by Terraform?

## Understand GitLab runner

[CI/CD essentials from scratch with Gitlab](https://medium.com/faun/ci-cd-essentials-from-scratch-with-gitlab-61502acf318e)

## Connect Remote Git Repo

On Windows10, usually ssh agent is not started yet, but in terminal(git bash) simply typing

```
ssh-agent
```

won't work, need to use the following command

```
eval `ssh-agent -s`
```

to bring up ssh agent.

Although on Window10, PuTTY can be used to generated private and public key pair, somehow, it is not compatible to key gen by OpenSSH_7.7p1 tool on Windows10. To make it less problematic, just use the same method from all major Git cloud service told

```
ssh-keygen
```

Once the pair of key is generated, just use

```
ssh-add name-of-private-key
```

Check to see if the key is added

```
ssh-add -l
```

For checking to see if Bitbucket repo can be accessed by the key, after adding the key into SSH Key in Bitbucket and run

```
ssh -T git@bitbucket.org
```

If the response shows something like the following, it is set

```
You can use git or hg to connect to Bitbucket. Shell access is disabled
```

### For LFS

Nothing particular for SSH part, but using the command

```
git lfs fetch
```

only fetch but not merge(make the placeholder to be the actual file), use

```
git lfs pull
```

will get the result.

### Setup for different remote

```sh
git remote add origin https://github.com/Ray-JoyBrick/walkio.git


git remote add github https://github.com/Ray-JoyBrick/walkio.git
git branch -u github/prepare-build-asset prepare-build-asset
git branch -u github/prepare-build-release prepare-build-release

git remote add github git://github.com/foo/myrepo.git
git remote add mydomain git://git.mydomain.com/foo/myrepo.git

git branch -u mydomain/private private
git branch -u github/public public

git push -u github prepare-build-asset
git push -u github prepare-build-release
```

```
git config --local user.name "FIRST_NAME LAST_NAME"
git config --local user.email "MY_NAME@example.com"
```

## Get source from Google Source Repo

```sh
git clone ssh://ray@joybrick.cc@source.developers.google.com:2022/p/walkio-271711/r/ucb-build-release
git clone ssh://ray@joybrick.cc@source.developers.google.com:2022/p/walkio-271711/r/ucb-build-asset

# Using multiple ssh config
git clone ssh://ray@joybrick.cc@source.developers.google.com-jb_google-cloud-source:2022/p/walkio-271711/r/ucb-build-release
git clone ssh://ray@joybrick.cc@source.developers.google.com-jb_google-cloud-source:2022/p/walkio-271711/r/ucb-build-asset
```

Possible multiple ssh config setup
```txt
# Work account JoyBrick GitHub
Host github.com-jb_github
   HostName github.com
   User git
   IdentityFile ~/.ssh/id_rsa_jb_github

# Work account JoyBrick Google Cloud Source Repo
Host source.developers.google.com-jb_google-cloud-source
   HostName source.developers.google.com
   User git
   IdentityFile ~/.ssh/id_rsa_jb_google_cloud_source
```
