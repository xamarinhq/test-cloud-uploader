#!/bin/bash

configuration=${1:-Release}
output="`pwd`/publish/$configuration"
root="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/.."

echo "Root folder: $root"

exitCode=0

function PublishProject {
    path=$1
    platform=$2

    mkdir -p $output

    pushd .
    cd "$path"

    dotnet publish -c $configuration -r $platform
    local result=$?
    if [ $result -ne 0 ]
    then
        exitCode=$result
    fi

    cd "./bin/$configuration/netcoreapp1.0/$platform"
    mkdir apphub
    cp -R ./publish/* ./apphub
    
    dst="$output/app.$platform.tar.gz"
    tar -zcvf "$dst" apphub

    rm -R ./app

    popd
}

pushd .
cd "$root"

PublishProject "./src/AppHub.Cli" "osx.10.10-x64"
PublishProject "./src/AppHub.Cli" "osx.10.11-x64"
PublishProject "./src/AppHub.Cli" "ubuntu.14.04-x64"

popd

exit $exitCode
