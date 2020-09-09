#!/bin/bash

echo "In $1"
ls -la "$1"

echo "In $2"
ls -la "$2"

echo "Show the third one: $3"

MY_PATH="`dirname \"$0\"`"              # relative
MY_PATH="`( cd \"$MY_PATH\" && pwd )`"  # absolutized and normalized
if [ -z "$MY_PATH" ] ; then
  # error; for some reason, the path is not accessible
  # to the script (e.g. permissions re-evaled after suid)
  exit 1  # fail
fi
echo "$MY_PATH"

parentdir="$(dirname "$MY_PATH")"

echo "$parentdir"

echo "$KEYFILE_STORAGE" >> keyfile-storage.json

curl -o google-cloud-sdk.tar.gz  https://dl.google.com/dl/cloudsdk/channels/rapid/downloads/google-cloud-sdk-308.0.0-linux-x86_64.tar.gz
gunzip < google-cloud-sdk.tar.gz | tar xf -
./google-cloud-sdk/bin/gcloud auth activate-service-account --project=walkio-271711 --key-file=keyfile-storage.json

md5=$(cat "$parentdir/references-checksum") && ./google-cloud-sdk/bin/gsutil cp "gs://$GCS_BUCKET/references-$md5.zip" "$parentdir/references.zip"
md5=$(cat "$parentdir/preprocessed-assets-checksum") && ./google-cloud-sdk/bin/gsutil cp "gs://$GCS_BUCKET/preprocessed-assets-$md5.zip" "$parentdir/preprocessed-assets.zip"

# cd ..
cd "$parentdir"

unzip -qq "$parentdir/preprocessed-assets.zip"
unzip -qq "$parentdir/references.zip"

mv "$parentdir/game-specific" "$parentdir/references"
mv "$parentdir/preprocessed-assets" "$parentdir/references/game-specific"

# cd "$parentdir"
bash "./move-files.sh"
