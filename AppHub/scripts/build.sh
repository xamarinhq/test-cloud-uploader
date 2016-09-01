#!/bin/bash

configuration=${1:-Release}
output="`pwd`/publish/$configuration"
root="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/../"

echo "Root folder: $root"

exitCode=0

function Restore {
    dotnet restore
    local result=$?

    if [ $result -ne 0 ]
    then
        exitCode=$result
    fi
}

function BuildProject {
    path=$1
    pushd .
    cd $path
 
    dotnet build -c $configuration
    local result=$?

    if [ $result -ne 0 ]
    then
        exitCode=$result
    fi

    popd
}

pushd .
cd $root

Restore

BuildProject "./src/AppHub.Common"
BuildProject "./src/AppHub.Common.Cli"
BuildProject "./src/AppHub.TestCloud"
BuildProject "./src/AppHub.Cli"

BuildProject "./test/AppHub.Common.Cli.Tests"
BuildProject "./test/AppHub.Common.Tests"
BuildProject "./test/AppHub.TestCloud.Tests"

popd

exit $exitCode
