FROM alpine:latest as make-call-to-ucb

RUN apk update && apk add rsync zip unzip

ARG UCB_API_KEY
ARG ORG_ID
ARG PROJECT_ID
ARG BUILD_TARGET_ID

# Only this one is correct
RUN curl \
        -X GET \
        -H "Authorization: Basic ${UCB_API_KEY}" \
        https://build-api.cloud.unity3d.com/api/v1/projects

# The following are not correct
RUN curl \
        -X POST \
        -H "Content-Type: application/json" \
        -H "Authorization: Basic ${UCB_API_KEY}" \
        -d '{"name": "New Project", "settings": { "remoteCacheStrategy": "library", "scm": { "type": "git", "url": "git://github.com/example-org/newproject.git", "user": "julie.smith", "pass": "password"}}}' \
        https://build-api.cloud.unity3d.com/api/v1/orgs/${ORG_ID}/projects

RUN curl \
        -X PUT \
        -d '{"MY_ENV_VARIABLE":"yellow","BUILD_SERVICE":"https://build-api.cloud.unity3d.com/","PORT":"1000"}' \
        -H "Authorization: Basic ${UCB_API_KEY}" \
        https://build-api.cloud.unity3d.com/api/v1/orgs/${ORG_ID}/projects/${PROJECT_ID}/envvars

RUN curl \
          -X POST \
        -d '{"clean": true, "delay": 30}' \
        -H "Content-Type: application/json" \
        -H "Authorization: Basic ${UCB_API_KEY}" \
        https://build-api.cloud.unity3d.com/api/v1/orgs/${ORG_ID}/projects/${PROJECT_ID}/buildtargets/${BUILD_TARGET_ID}/builds
