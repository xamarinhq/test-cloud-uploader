param (
    [string] $configuration = "Release"
)

$rootDir = Resolve-Path ($PSScriptRoot + "\..")
$script:exitCode = 0

function RunTests([string] $path)
{
    try 
    {
        Push-Location
        Set-Location (Join-Path $rootDir $path)
        $projectName = [System.IO.Path]::GetFileName($path)

        dotnet test -c $configuration 

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

RunTests "test\Xtc.Common.Cli.Tests"
RunTests "test\Xtc.Common.Tests"
RunTests "test\Xtc.TestCloud.Tests"

exit $script:exitCode
