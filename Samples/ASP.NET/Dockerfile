# DotNet Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build
SHELL ["/bin/bash", "-c"]

COPY base.props /app/
COPY default.props /app/
COPY netstandard.props /app/
COPY versions.props /app/
COPY DotNET.SDK.sln /app/
COPY Source /app/Source/
COPY Samples/ASP.NET /app/Samples/ASP.NET

WORKDIR /app/Samples/ASP.NET
RUN dotnet restore
RUN dotnet publish -c "Release" -o out

# Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
SHELL ["/bin/bash", "-c"]

ENV Logging__Console__FormatterName=""

WORKDIR /app
COPY --from=dotnet-build /app/Samples/ASP.NET/out ./

EXPOSE 5000 5001

ENTRYPOINT ["dotnet", "ASP.NET.dll"]
