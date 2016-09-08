## Test Cloud support for Appium

Currently the upload tools supports JUnit-based [Appium](http://appium.io/) tests. 
[This guide](https://github.com/xamarinhq/test-cloud-appium-java-extensions/blob/master/README.md)
has more details how to prepare your tests to run in the Test Cloud. 

### Workspace validation
The upload tool recognizes the workspace as Appium tests workspace if it:

1. Contains file pom.xml
2. Contains directory dependency-jars
3. Contains directory test-classes, with at least one *.class file inside.