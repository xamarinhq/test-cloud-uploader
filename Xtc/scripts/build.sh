#!/bin/bash

export DOTNET_RUNTIME_ID=osx.10.11-x64

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

function BuildSolution {
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

BuildSolution "."
popd

exit $exitCode
