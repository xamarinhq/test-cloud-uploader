.PHONY: default

build:
	./scripts/build.sh

test:
	./scripts/runTest.sh

publish:
	./scripts/publish.sh

default: build test publish

.DEFAULT_GOAL := default
