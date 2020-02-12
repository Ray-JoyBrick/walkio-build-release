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

## Understand GitLab runner

[CI/CD essentials from scratch with Gitlab](https://medium.com/faun/ci-cd-essentials-from-scratch-with-gitlab-61502acf318e)