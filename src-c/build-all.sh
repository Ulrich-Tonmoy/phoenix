#!/bin/bash
# Build script for rebuilding everything
set echo on

echo "Building All Projects..."


pushd engine
source build.sh
popd

ERRORLEVEL=$?
if [ $ERRORLEVEL -ne 0 ]
then
echo "Error:"$ERRORLEVEL && exit
fi

pushd test
source build.sh
popd
ERRORLEVEL=$?
if [ $ERRORLEVEL -ne 0 ]
then
echo "Error:"$ERRORLEVEL && exit
fi

echo "All Assemblies Built Successfully."