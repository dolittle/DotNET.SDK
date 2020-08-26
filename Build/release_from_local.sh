#!/bin/bash
# Assumes that the NuGet API key has been set: 
# $ nuget setApiKey <key> -Source https://api.nuget.org/v3/index.json

export PACKAGEDIR=$PWD/Artifacts/NuGet
export DOLITTLERELEASE=true
export VERSION=$(git tag --sort=-version:refname | head -1)

MAJOR_VERSION=$(echo $VERSION | sed 's/v*\([0-9]*\).*$/\1/g')
MINOR_VERSION=$(echo $VERSION | sed 's/v*[0-9]*.\([0-9]*\).*$/\1/g')
PATCH_VERSION=$(echo $VERSION | sed 's/v*[0-9]*.[0-9]*.\([0-9]*\).*$/\1/g')
PRE_RELEASE_TAG=$(echo $VERSION | sed 's/v*[0-9]*.[0.9]*.[0-9]-\([a-zA-Z]*\).*$/\1/g')
BUILD_VERSION=$(echo $VERSION | sed 's/v*[0-9]*.[0.9]*.[0-9]-[a-zA-Z]*.\([0-9]*\)/\1/g')

[[ $PRE_RELEASE_TAG == $BUILD_VERSION ]] &&
    PACKAGE_VERSION=$MAJOR_VERSION.$MINOR_VERSION.$PATCH_VERSION ||
    PACKAGE_VERSION=$MAJOR_VERSION.$MINOR_VERSION.$PATCH_VERSION-$PRE_RELEASE_TAG.$BUILD_VERSION

echo "Git Version : " $VERSION
echo "Major Version : " $MAJOR_VERSION
echo "Minor Version : " $MINOR_VERSION
echo "Patch Version : " $PATCH_VERSION
echo "Build Version : " $BUILD_VERSION
echo "Package Version : " $PACKAGE_VERSION

VERSION=

rm -rf $PWD/Artifacts

dotnet pack -p:PackageVersion=$PACKAGE_VERSION -c release -o $PACKAGEDIR -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

function push_to_nuget() {
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        mono /usr/local/bin/nuget.exe push $f -Source https://api.nuget.org/v3/index.json
    else
        nuget push $f -Source https://api.nuget.org/v3/index.json
    fi
}

for f in $PACKAGEDIR/*.symbols.nupkg
do
    push_to_nuget
done

for f in $PACKAGEDIR/*.nupkg
do
    push_to_nuget
done
