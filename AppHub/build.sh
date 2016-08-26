configuration=${1:-Release}
output="`pwd`/publish/$configuration"

function BuildProject {
    path=$1

    pushd .

    cd $path
    dotnet restore
    dotnet build -c $configuration

    popd
}

function RunTests {
    path=$1

    pushd .

    cd $path
    dotnet test

    popd
}

function PublishProject {
    path=$1
    platform=$2

    mkdir -p $output

    pushd .
    cd $path

    dotnet publish -c $configuration -r $platform

    src="`pwd`/bin/$configuration/netcoreapp1.0/$platform/publish"
    dst="$output/app.$platform.zip"

    zip -r $dst $src    

    popd
}

BuildProject "./src/AppHub.Common"
BuildProject "./src/AppHub.Common.Cli"
BuildProject "./src/AppHub.TestCloud"
BuildProject "./src/AppHub.Cli"

BuildProject "./test/AppHub.Common.Cli.Tests"
BuildProject "./test/AppHub.Common.Tests"
BuildProject "./test/AppHub.TestCloud.Tests"

RunTests "./test/AppHub.Common.Cli.Tests"
RunTests "./test/AppHub.Common.Tests"
RunTests "./test/AppHub.TestCloud.Tests"

PublishProject "./src/AppHub.Cli" "win10-x64"
PublishProject "./src/AppHub.Cli" "osx.10.10-x64"
PublishProject "./src/AppHub.Cli" "ubuntu.14.04-x64"