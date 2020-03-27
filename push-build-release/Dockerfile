FROM google/cloud-sdk:alpine as get-git-ssh-key

ARG SERVICE_NAME
ARG PROJECT_ID
ARG JSON_FILE
ARG GITHUB_RSA

RUN echo "${JSON_FILE}" > /root/keyfile.json
RUN gcloud auth activate-service-account \
    ${SERVICE_NAME}@${PROJECT_ID}.iam.gserviceaccount.com \
    --key-file=/root/keyfile.json --project=${PROJECT_ID}

# RUN echo "${GITHUB_RSA}" > id_rsa_jb_github.enc
ADD ../id_rsa_jb_github.enc id_rsa_jb_github.enc
ADD ../config config
RUN mkdir /root/.ssh/
RUN gcloud kms decrypt \
    --location=global \
    --keyring=joybrick-walkio-development \
    --key=access-git-key \
    --ciphertext-file=id_rsa_jb_github.enc \
    --plaintext-file=/root/.ssh/id_rsa_jb_github \
    --verbosity=debug \
    --log-http

FROM alpine:latest as fetch-unity-project

RUN apk update && apk add git git-lfs openssh-client rsync
RUN mkdir /root/.ssh/
COPY --from=get-git-ssh-key /root/.ssh /root/.ssh
COPY --from=get-git-ssh-key config /root/.ssh
RUN chmod 600 /root/.ssh/id_rsa_jb_github
RUN touch /root/.ssh/known_hosts \
    && ssh-keyscan github.com >> /root/.ssh/known_hosts
RUN git clone git@github.com-jb_github:Ray-JoyBrick/walkio.git