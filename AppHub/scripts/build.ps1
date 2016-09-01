param (
    [string] $configuration = "Release"
)

$rootDir = Resolve-Path ($PSScriptRoot + "\..")
$script:exitCode = 0

function Restore()
{
    try
    {
        Push-Location
        Set-Location $rootDir
        dotnet restore

        if ($LASTEXITCODE -ne 0)
        {
            $script:exitCode = $LASTEXITCODE
        }
    }
    finally
    {
        Pop-Location
    }
}

function BuildProject([string] $path)
{
    try 
    {
        Push-Location
        Set-Location (Join-Path $rootDir $path)
        dotnet build -c $configuration

        if ($LASTEXITCODE -ne 0)
        {
            $script:exitCode = $LASTEXITCODE
        }
    }
    finally 
    {
        Pop-Location
    }
}

Restore

BuildProject "src\AppHub.Common"
BuildProject "src\AppHub.Common.Cli"
BuildProject "src\AppHub.TestCloud"

BuildProject "test\AppHub.Common.Cli.Tests"
BuildProject "test\AppHub.Common.Tests"
BuildProject "test\AppHub.TestCloud.Tests"

BuildProject "src\AppHub.Cli"

exit $script:exitCode