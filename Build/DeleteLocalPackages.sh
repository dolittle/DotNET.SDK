#!/bin/bash

export PACKAGEVERSION=3.1000.0
export TARGETROOT=~/.nuget/packages

find $TARGETROOT/ -name $PACKAGEVERSION -exec rm -rf {} \;