param (
    [string] $configuration = "Release",
    [string] $outputDir = "publish\$configuration"
)

function BuildProject([string] $path)
{
    try 
    {
        Push-Location
        Set-Location $path
        dotnet restore
        dotnet build -c $configuration
    }
    finally 
    {
        Pop-Location
    }
}

function RunTests([string] $path)
{
    try 
    {
        Push-Location
        Set-Location $path
        dotnet test -c $configuration
    }
    finally 
    {
        Pop-Location
    }
}

function PublishProject([string] $path, [string] $platform)
{
    try 
    {
        if (!(Test-Path $outputDir))
        {
            mkdir $outputDir
        }

        $outputDir = Resolve-Path $outputDir

        Push-Location
        Set-Location $path
        dotnet publish -c $configuration -r $platform

        $source = "$pwd\bin\$configuration\netcoreapp1.0\$platform\publish"
        $destination = "$outputDir\app.$platform.zip"

        if (Test-Path $destination)
        {
            Remove-Item $destination
        }

        Add-Type -AssemblyName "System.IO.Compression.FileSystem"
        [IO.Compression.ZipFile]::CreateFromDirectory($source, $destination)
    }
    finally 
    {
        Pop-Location
    }
}

BuildProject "src\AppHub.Common"
BuildProject "src\AppHub.Common.Cli"
BuildProject "src\AppHub.TestCloud"

BuildProject "test\AppHub.Common.Cli.Tests"
BuildProject "test\AppHub.Common.Tests"
BuildProject "test\AppHub.TestCloud.Tests"

RunTests "test\AppHub.Common.Cli.Tests"
RunTests "test\AppHub.Common.Tests"
RunTests "test\AppHub.TestCloud.Tests"

BuildProject "src\AppHub.Cli"

PublishProject "src\AppHub.Cli" "win10-x64"
PublishProject "src\AppHub.Cli" "osx.10.10-x64"
PublishProject "src\AppHub.Cli" "ubuntu.14.04-x64"
