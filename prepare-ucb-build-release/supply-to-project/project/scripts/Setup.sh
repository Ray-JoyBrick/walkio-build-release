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

# cd ..
cd "$parentdir"

unzip -qq "$parentdir/preprocessed-assets.zip"
unzip -qq "$parentdir/references.zip"

mv "$parentdir/game-specific" "$parentdir/references"
mv "$parentdir/preprocessed-assets" "$parentdir/references/game-specific"

# cd "$parentdir"
bash "./move-files.sh"
