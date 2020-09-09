#!/bin/bash

# GCS_BUCKET="joybrick-walkio-dev"

echo "In $1"
ls -la "$1"

echo "In $2"
ls -la "$2"

echo "Show the third one: $3"

# ./google-cloud-sdk/bin/gcloud init --console-only --skip-diagnostics

# ./google-cloud-sdk/bin/gcloud config set project walkio-271711
# ./google-cloud-sdk/bin/gcloud init

# ./google-cloud-sdk/bin/gsutil cp gs://joybrick-walkio-dev/references.zip /tmp/references.zip

echo "${KEYFILE_STORAGE}" >> keyfile-storage.json

# curl -o google-cloud-sdk.tar.gz  https://dl.google.com/dl/cloudsdk/channels/rapid/downloads/google-cloud-sdk-308.0.0-linux-x86_64.tar.gz
# gunzip < google-cloud-sdk.tar.gz | tar xvf -
# ./google-cloud-sdk/bin/gcloud auth activate-service-account --project=walkio-271711 --key-file=keyfile-storage.json
# # ./google-cloud-sdk/bin/gsutil cp -n "some-file.zip" gs://joybrick-walkio-dev/references.zip

md5=$(md5sum "some-file.zip" | cut -d ' ' -f 1) \
    && ./google-cloud-sdk/bin/gsutil cp "$md5.zip" "gs://${GCS_BUCKET}/addressable-assets-$md5.zip" \
    && rm "$md5.zip"
