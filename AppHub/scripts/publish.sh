#!/bin/bash

configuration=${1:-Release}
root="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/.."
output="$root/../publish/$configuration"

echo "Root folder: $root"

exitCode=0

function PublishProject {
    path=$1
    platform=$2
    archiveFormat=$3

    mkdir -p "$output"

    pushd .
    cd "$path"

    dotnet publish -c "$configuration" -r "$platform"
    local result=$?
    if [ $result -ne 0 ]
    then
        exitCode=$result
    fi

    cd "./bin/$configuration/netcoreapp1.0/$platform"
    mkdir -p apphub
    cp -R ./publish/* ./apphub
    
    echo "Archive format: $archiveFormat"
    if [ $archiveFormat == "tar.gz" ]
    then
        dst="$output/app.$platform.tar.gz"
        tar -zcvf "$dst" apphub
    else
        dst="$output/app.$platform.zip"
        zip -r "$dst" apphub
    fi

    rm -R ./app

    popd
}

pushd .
cd "$root"

PublishProject "./src/AppHub.Cli" "osx.10.10-x64" "tar.gz"
PublishProject "./src/AppHub.Cli" "ubuntu.14.04-x64" "tar.gz"
PublishProject "./src/AppHub.Cli" "win10-x64" "zip"

popd

exit $exitCode
