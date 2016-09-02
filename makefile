.PHONY: default

build:
	./AppHub/scripts/build.sh

test:
	./AppHub/scripts/runTests.sh

publish:
	./AppHub/scripts/publish.sh

default: build test publish

.DEFAULT_GOAL := default
