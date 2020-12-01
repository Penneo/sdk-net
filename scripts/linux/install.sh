#!/bin/bash

# Dependencies
nuget install Src/Penneo/packages.config -o Src/packages
nuget install Src/PenneoTests/packages.config -o Src/packages

nuget install RestSharp -Version "106.11.7"  -o Src/packages
nuget install Newtonsoft.Json -Version "12.0.3"  -o Src/packages
nuget install FakeItEasy -Version "6.2.1"  -o Src/packages
nuget install NUnit -Version "3.12.0" -o Src/packages
nuget install NUnit.Console -Version "3.11.1" -o Src/packages
nuget install NUnit.ConsoleRunner  -Version "3.11.1" -o Src/packages
nuget install MSTest.TestFramework -Version "2.1.2" -o Src/packages
# Compile
xbuild Src/Penneo.sln