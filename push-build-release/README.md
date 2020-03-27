# Overview

Use the following command to execute the docker but the command has to be given directly below game-unity.

```sh
time DOCKER_BUILDKIT=1 docker image build -t push-build-release:latest --no-cache --build-arg SERVICE_NAME="use-kms" --build-arg PROJECT_ID="walkio-271711" --build-arg JSON_FILE="$(cat ./credentials/gcp/keyfile.json)" -f ./push-build-release/Dockerfile .

time DOCKER_BUILDKIT=1 docker image build -t push-build-release:latest --no-cache -f ./push-build-release/Dockerfile .

docker container run -it push-build-release /bin/sh
```


```sh
gcloud iam service-accounts create use-kms
gcloud projects add-iam-policy-binding walkio-271711 --member "serviceAccount:use-kms@walkio-271711.iam.gserviceaccount.com"
gcloud iam service-accounts keys create keyfile.json --iam-account use-kms@walkio-271711.iam.gserviceaccount.com

cat keyfile.json | docker login -u _json_key --password-stdin https://gcr.io
docker login -u _json_key -p "$(cat keyfile.json)" https://gcr.io
```

Check out [gcloud auth activate-service-account](https://cloud.google.com/sdk/gcloud/reference/auth/activate-service-account) for more detail.

```sh
gcloud auth activate-service-account \
  use-kms@google.com \
          --key-file=JSON_FILE --project=walkio-271711
```
