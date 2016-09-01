#!/bin/bash

configuration=${1:-Release}
output="`pwd`/publish/$configuration"
root="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/.."

exitCode=0

function RunTests {
    path=$1
    pushd .
    cd "$path"

    local projectName="`basename $path`"
    local outputXml="./bin/$configuration/netcoreapp1.0/$projectName.xunit-results.xml"
    dotnet test -c $configuration -xml "$outputXml"
    local result=$?
 
    if [ $result -ne 0 ]
    then
        exitCode=$result
    fi

    popd
}

pushd .
cd "$root"

RunTests "./test/AppHub.Common.Cli.Tests"
RunTests "./test/AppHub.Common.Tests"
RunTests "./test/AppHub.TestCloud.Tests"

popd

exit $exitCode
