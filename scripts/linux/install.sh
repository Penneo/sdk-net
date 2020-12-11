#!/bin/bash

dotnet restore Src/Penneo.sln
dotnet msbuild Src/Penneo.sln
