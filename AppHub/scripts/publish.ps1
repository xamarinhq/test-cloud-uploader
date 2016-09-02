param (
    [string] $configuration = "Release",
    [string] $outputDir = "publish\$configuration"
)

$rootDir = Resolve-Path ($PSScriptRoot + "\..")
$script:exitCode = 0

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
        Set-Location (Join-Path $rootDir $path)
        dotnet publish -c $configuration -r $platform

        if ($LASTEXITCODE -ne 0)
        {
            $script:exitCode = $LASTEXITCODE
        }

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

#PublishProject "src\AppHub.Cli" "win10-x64"
#PublishProject "src\AppHub.Cli" "win7-x64"
#PublishProject "src\AppHub.Cli" "osx.10.10-x64"
#PublishProject "src\AppHub.Cli" "ubuntu.14.04-x64"

exit $script:exitCode