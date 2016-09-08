.PHONY: default

build:
	./Xtc/scripts/build.sh

test:
	./Xtc/scripts/runTests.sh

publish:
	./Xtc/scripts/publish.sh

default: build test publish

.DEFAULT_GOAL := default
