# Samples

This folder contains samples of how to use the TimeSeries SDK.

**Prerequisites:**

- Docker
- Dotnet

## Run samples

Start the runtime from bash:

```shell
$ ./start_runtime.sh
```

Once the runtime is running, you can run one of the connectors (PullConnector, PushConnector).
Navigate to the folder in your shell and do the following:

```shell
$ dotnet run
```

Once this is running, open a browser and navigate to: [http://localhost:9700/metrics](http://localhost:9700/metrics).
This will give you the Prometheus compatible endpoint used for scraping datapoints.