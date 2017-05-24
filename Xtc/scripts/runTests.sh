#!/bin/bash

export DOTNET_RUNTIME_ID=osx.10.11-x64

configuration=${1:-Release}
output="`pwd`/publish/$configuration"
root="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/.."

echo "Root folder: $root"

exitCode=0

function RunTests {
    path=$1
    pushd .
    cd "$path"

    local projectName="`basename $path`"

    dotnet test -c $configuration 
    
    local result=$?
 
    if [ $result -ne 0 ]
    then
        exitCode=$result
    fi

    popd
}

pushd .
cd "$root"

RunTests "./test/Xtc.Common.Cli.Tests"
RunTests "./test/Xtc.Common.Tests"
RunTests "./test/Xtc.TestCloud.Tests"

popd

exit $exitCode
