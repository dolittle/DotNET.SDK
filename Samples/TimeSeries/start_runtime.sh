#!/bin/bash
docker run -p 50052:50052 -p 50053:50053 -p 9700:9700 -v $PWD/config:/config dolittle/runtime
