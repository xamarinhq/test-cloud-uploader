param (
    [string] $configuration = "Release",
    [string] $output = "publish\$configuration"
)

function BuildProject([string] $path, [string] $configuration)
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

function PublishProject([string] $path, [string] $configuration, [string] $outputDir, [string] $platform)
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

        Add-Type -AssemblyName "System.IO.Compression.FileSystem"
        [IO.Compression.ZipFile]::CreateFromDirectory($source, $destination)
    }
    finally 
    {
        Pop-Location
    }
}

BuildProject "src\AppHub.Common" $configuration
BuildProject "src\AppHub.Common.Cli" $configuration
BuildProject "src\AppHub.TestCloud" $configuration

BuildProject "test\AppHub.Common.Cli.Tests" $configuration
BuildProject "test\AppHub.Common.Tests" $configuration
BuildProject "test\AppHub.TestCloud.Tests" $configuration

RunTests "test\AppHub.Common.Cli.Tests"
RunTests "test\AppHub.Common.Tests"
RunTests "test\AppHub.TestCloud.Tests"

BuildProject "src\AppHub.Cli" $configuration

PublishProject "src\AppHub.Cli" $configuration $output "win10-x64"
PublishProject "src\AppHub.Cli" $configuration $output "osx.10.10-x64"
PublishProject "src\AppHub.Cli" $configuration $output "ubuntu.14.04-x64"
