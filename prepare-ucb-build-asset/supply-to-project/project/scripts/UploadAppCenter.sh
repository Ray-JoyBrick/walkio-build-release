#!/bin/bash

# upload to appcenter
# locations of various tools

CURL=curl

# Put your HockeyApp APP_TOKEN here. Find it in your HockeyApp account settings.
API_TOKEN=deb9239604214e30beffc06a88251046ba63d4cb

# owner name - required
OWNER_NAME=$1

# app name - required
APP_NAME=$2

# group Id - required
GROUP_ID=$3

# ipa - required, file data of the .ipa for iOS, .app.zip for OS X, or .apk file for Android
FILE="$4"

# note - optional
NOTE_FILE="$5"

usage() {
    echo "Usage: UploadAppCenter.sh $OWNER_NAME $APP_NAME $GROUP_ID $FILE $NOTE_FILE"
    echo
    exit 1
}

verify_tools() {
    # Windows users: this script requires curl. If not installed please get from http://cygwin.com/

    # Check 'curl' tool
    "${CURL}" --help >/dev/null
    if [ $? -ne 0 ]; then
        echo "Could not run curl tool, please check settings"
        exit 1
    fi
}

verify_settings() {
    if [ -z "${API_TOKEN}" ]; then
        usage
        echo "Please update API_TOKEN with your private API key, as noted in the Settings page"
        exit 1
    fi
}

# if [ $# -lt 4 ]; then
#     usage
#     exit 1
# fi

# if [ ! -f "${FILE}" ]; then
#     usage
#     exit 1
# fi

# before even going on, make sure all tools work
verify_tools
verify_settings

get_json_value() {
    local json=$1
    local key=$2

    echo $(echo "${json}" | sed 's/\\\//\//g' | sed -n "s/.*\"${key}\"\s*:\s*\"\([^\"]*\)\".*/\1/p" )
}

Step_A() {
    local api_url="https://api.appcenter.ms/v0.1/apps/$OWNER_NAME/$APP_NAME/release_uploads"
    local api_token="X-API-Token: ${API_TOKEN}"

    local result=$( "${CURL}" \
        -X POST \
        --header "Content-Type: application/json" \
        --header "Accept: application/json" \
        --header "$api_token" \
        --connect-timeout 60 \
        --retry 10 \
        --retry-delay 5 \
        "$api_url"
    )

    echo "$result"
}

Step_B() {
    local json=$1
    local api_token="X-API-Token: ${API_TOKEN}"

    local upload_url=$(get_json_value "$json" "upload_url")
    local filePath="ipa=@$FILE"

    local result=$( "${CURL}" \
            -X POST \
            --header "Content-Type: multipart/form-data" \
            -F "$filePath" \
            --connect-timeout 60 \
            --retry 10 \
            --retry-delay 5 \
            "$upload_url"
    )

    local upload_id=$(get_json_value "$json" "upload_id")

    echo "$upload_id"
}

Step_C() {
    local upload_id=$1
    local api_token="X-API-Token: ${API_TOKEN}"
    local api_url="https://api.appcenter.ms/v0.1/apps/$OWNER_NAME/$APP_NAME/release_uploads/$upload_id"

    local result=$( "${CURL}" \
            -X PATCH \
            --header "Content-Type: application/json" \
            --header "Accept: application/json" \
            --header "$api_token" \
            -d '{ "status": "committed"  }' \
            --connect-timeout 60 \
            --retry 100 \
            --retry-delay 5 \
            "$api_url" \
    )

    local release_id=$(get_json_value $result "release_id")

    echo "$release_id"
}

Step_D() {
    local release_id=$1
    local api_token="X-API-Token: ${API_TOKEN}"
    local release_notes=$(echo $(sed -E ':a;N;$!ba;s/\r{0,1}\n/\\n/g' ${NOTE_FILE}))
    local api_url="https://api.appcenter.ms/v0.1/apps/$OWNER_NAME/$APP_NAME/releases/$release_id"
    data=$( echo "{ 
        \"mandatory_update\": false,
        \"notify_testers\": false,
        \"destinations\":
        [
            {
                \"name\": \"Tester\",
                \"id\": \"$GROUP_ID\"
            }
        ]
    }")

    local result=$( "${CURL}" \
            -X PATCH \
            --header 'Content-Type: application/json' \
            --header 'Accept: application/json' \
            --header "$api_token" \
            -d "$data" \
            --connect-timeout 60 \
            --retry 100 \
            --retry-delay 5 \
            "$api_url"
    )

    echo "$result"
}

# a. Create an upload resource and get an upload_url
result_A=$(Step_A)

if [ -z $result_A ]; then
    echo "Failed at step A."
    exit 1
fi
# b.  Copy the upload_url (will be a rink.hockeyapp.net URL) from the response in the previous step,
# and also save the upload_id for the step after this one. Upload to upload_url using a POST request.
# Use multipart/form-data as the Content-Type, where the key is ipa (key is always IPA even when uploading Android APKs)
# and the value is @/path/to/your/build.ipa.
result_B=$(Step_B "$result_A")

if [ -z $result_B ]; then
    echo "Failed at step B."
    exit 1
fi
# c. After the upload has finished, update upload resource's status to committed and get a release_id, save that for the next step
result_C=$(Step_C "$result_B")

if [ -z $result_C ]; then
    echo "Failed at step C."
    exit 1
fi
# d. Distribute the uploaded release to destinations using testers, groups, or stores.
# This is nessesary to view uploaded releases in the developer portal.
if [ -f "${FILE}" ]; then
    result_D=$(Step_D "$result_C")
fi

message=$(get_json_value $result_D "message")
if [ ! -z $message ]; then
    echo "Failed at Step D."
    exit 1
fi

echo "Success."
