#!/bin/bash

root="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
$root/build.sh
$root/runTests.sh
$root/publish.sh

