#!/bin/bash
[[ ! -d Source ]] && { echo "You're probably in the wrong folder. Execute this command from the root of the repository"; exit 1; }

if [ ! -z "$1" ]; then
  VERSION=$1.0.0
else
  {
    git fetch origin

  } &> /dev/null
  [[ $? -ne 0 ]] && { echo "An error happened while trying to get latest tags. There is probably not a remote called '$REMOTE'"; exit 1; }
  VERSION=$(git tag --sort=-version:refname | head -1) 
fi

MAJOR_VERSION=$(echo $VERSION | sed 's/v*\([0-9]*\).*$/\1/g')
MINOR_VERSION=$(echo $VERSION | sed 's/v*[0-9]*.\([0-9]*\).*$/\1/g')
PATCH_VERSION=$(echo $VERSION | sed 's/v*[0-9]*.[0-9]*.\([0-9]*\).*$/\1/g')
PRE_RELEASE_TAG=$(echo $VERSION | sed 's/v*[0-9]*.[0.9]*.[0-9]-\([a-zA-Z]*\).*$/\1/g')

if [ $PRE_RELEASE_TAG = $VERSION ]; then
    PRE_RELEASE_TAG=
fi

if [ -n "$PRE_RELEASE_TAG" ]; then
    PACKAGE_VERSION=$MAJOR_VERSION.$MINOR_VERSION.$PATCH_VERSION-$PRE_RELEASE_TAG.1000
else
    PACKAGE_VERSION=$MAJOR_VERSION.1000.0
fi

echo "Git Version : " $VERSION
echo "Major Version : " $MAJOR_VERSION
echo "Minor Version : " $MINOR_VERSION
echo "Patch Version : " $PATCH_VERSION
echo "Package Version : " $PACKAGE_VERSION

VERSION=

PACKAGEDIR=$PWD/Packages
TARGETROOT=~/.nuget/packages

if [ ! -d "$PACKAGEDIR" ]; then
    mkdir $PACKAGEDIR
fi

rm $PACKAGEDIR/*
dotnet pack --output $PACKAGEDIR -p:PackageVersion=$PACKAGE_VERSION  -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

for f in $PACKAGEDIR/*.nupkg; do
    packagename=$(basename ${f%.$PACKAGE_VERSION.nupkg})
    packagename=$(echo "$packagename" | tr [A-Z] [a-z])
    # Delete outdated .nupkg 
    find $TARGETROOT/$packagename -name $PACKAGE_VERSION -exec rm -rf {} \;

done

if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    mono /usr/local/bin/nuget.exe init "$PACKAGEDIR" "$TARGETROOT" -Expand -NonInteractive
else
    nuget init "$PACKAGEDIR" "$TARGETROOT" -Expand -NonInteractive
fi