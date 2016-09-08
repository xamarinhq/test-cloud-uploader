$exitCode = $args[0]

[Console]::WriteLine("Output 1")
[Console]::Error.WriteLine("Error 1")
[Console]::WriteLine("Output 2")

exit $exitCode


