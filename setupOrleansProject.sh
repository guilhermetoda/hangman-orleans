#!/bin/bash

echo "Creating Grain Interfaces Class Dir"
mkdir GrainInterfaces
cd GrainInterfaces
dotnet new classlib
echo "Getting Orleans packages for GrainInterfaces"
dotnet add package Microsoft.Orleans.Core.Abstractions
dotnet add package Microsoft.Orleans.OrleansCodeGenerator.Build
cd .. 

echo "Creating Grains class lib"
mkdir Grains
cd Grains
dotnet new classlib
echo "Getting Orleans Packages"
dotnet add package Microsoft.Orleans.Core.Abstractions
dotnet add package Microsoft.Orleans.OrleansCodeGenerator.Build
echo "Creating Reference with Interfaces"
dotnet add reference ../GrainInterfaces/GrainInterfaces.csproj
cd .. 

echo "Creating Client Console App"
mkdir Client
cd Client
dotnet new console
echo "Getting Orleans Packages"
dotnet add package Microsoft.Extensions.Logging.Console
dotnet add package Microsoft.Orleans.Client
echo "Creating Reference with Interfaces"
dotnet add reference ../GrainInterfaces/GrainInterfaces.csproj
cd ..

echo "Creating Server/Silo Console App"
mkdir Silo
cd Silo
dotnet new console
echo "Getting Orleans Packages"
dotnet add package Microsoft.Extensions.Logging.Console
dotnet add package Microsoft.Orleans.Server
echo "Creating Reference with Grains"
dotnet add reference ../Grains/Grains.csproj
echo "Creating Reference with Interfaces"
dotnet add reference ../GrainInterfaces/GrainInterfaces.csproj





