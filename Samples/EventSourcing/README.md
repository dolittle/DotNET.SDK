# EventSourcing Sample

## Running

This sample uses MongoDB and needs it to be running. For the EventStore part, it relies on a specific version as there is
a breaking change that has not been supported in Dolittle yet.

Run Mongo as a daemon:

```shell
$ docker run -p 27017:27017 -d mongo:4.0.13
```
