#!/bin/bash

type brew >/dev/null 2>&1 || { echo >&2 "This scripts requires Homebrew - an OS X package manager. Please install it by following instructions at http://brew.sh"; exit 1; }

brew update
brew install openssl

ln -s /usr/local/opt/openssl/lib/libcrypto.1.0.0.dylib /usr/local/lib/
ln -s /usr/local/opt/openssl/lib/libssl.1.0.0.dylib /usr/local/lib/
