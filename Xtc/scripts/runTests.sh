#!/bin/bash

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
    local outputXml="./bin/$configuration/netcoreapp1.0/$projectName.xunit-results.xml"
    echo "Output XML: $outputXml"

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

RunTests "./test/Xtc.Common.Cli.Tests"
RunTests "./test/Xtc.Common.Tests"
RunTests "./test/Xtc.TestCloud.Tests"

popd

exit $exitCode
