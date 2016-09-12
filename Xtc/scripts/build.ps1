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

BuildProject "src\Xtc.Common"
BuildProject "src\Xtc.Common.Cli"
BuildProject "src\Xtc.TestCloud"

BuildProject "test\Xtc.Common.Cli.Tests"
BuildProject "test\Xtc.Common.Tests"
BuildProject "test\Xtc.TestCloud.Tests"

BuildProject "src\Xtc.Cli"

exit $script:exitCode