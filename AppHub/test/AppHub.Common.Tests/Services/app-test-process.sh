#!/bin/bash

exitCode="$1"

echo Output 1
>&2 echo Error 1
echo Output 2

exit $exitCode