#!/bin/bash

OWNER_NAME="JoyBrick"

APP_NAME="Walkio-Android"

GROUP_ID="aeb3eff5-cc34-422f-8dd1-6f48799e75bb"

BUILD_TARGET="Default Android"

APP_FILE="$2/${BUILD_TARGET}.apk"

NOTE_FILE="scripts/commit.txt"

echo "In $1"
ls "$1"

echo "In $2"
ls "$2"

echo "Show the third one: $3"
# ls "$3"

# bash scripts/UploadAppCenter.sh $OWNER_NAME $APP_NAME $GROUP_ID "${APP_FILE}" "${NOTE_FILE}"
