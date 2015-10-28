#!/bin/bash

# Dependencies
nuget install Src/Penneo/packages.config -o Src/packages
nuget install Src/PenneoTests/packages.config -o Src/packages

# Compile
xbuild Src/Penneo.sln
