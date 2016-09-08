#!/bin/bash

configuration=${1:-Release}
output="`pwd`/publish/$configuration"
root="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/.."

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
    cd "$path"
 
    dotnet build -c $configuration
    local result=$?

    if [ $result -ne 0 ]
    then
        exitCode=$result
    fi

    popd
}

pushd .
cd "$root"

echo "Using .NET Core version `dotnet --version`"

Restore

BuildProject "./src/Xtc.Common"
BuildProject "./src/Xtc.Common.Cli"
BuildProject "./src/Xtc.TestCloud"
BuildProject "./src/Xtc.Cli"

BuildProject "./test/Xtc.Common.Cli.Tests"
BuildProject "./test/Xtc.Common.Tests"
BuildProject "./test/Xtc.TestCloud.Tests"

popd

exit $exitCode
