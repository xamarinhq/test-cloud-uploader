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

function BuildSolution([string] $path)
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

BuildProject "."
exit $script:exitCode
