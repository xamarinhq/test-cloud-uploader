#!/bin/bash

export DOTNET_RUNTIME_ID=osx.10.12-x64

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

    cd "./bin/$configuration/netcoreapp2.0/$platform"
    mkdir -p tmp/xtc
    cp -R ./publish/* ./tmp/xtc
    cd ./tmp

    echo "Archive format: $archiveFormat"
    if [ $archiveFormat == "tar.gz" ]
    then
        dst="$output/xtc.$platform.tar.gz"
        tar -zcvf "$dst" xtc
    else
        dst="$output/xtc.$platform.zip"
        zip -r "$dst" xtc
    fi
    
    cd ..
    rm -R ./tmp

    popd
}

pushd .
cd "$root"

PublishProject "./src/Xtc.Cli" "osx.10.10-x64" "tar.gz"
PublishProject "./src/Xtc.Cli" "osx.10.12-x64" "tar.gz"
PublishProject "./src/Xtc.Cli" "ubuntu.14.04-x64" "tar.gz"
PublishProject "./src/Xtc.Cli" "ubuntu.16.04-x64" "tar.gz"
PublishProject "./src/Xtc.Cli" "win7-x64" "zip"

popd

exit $exitCode
