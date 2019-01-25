# base
FROM microsoft/dotnet:2.1-sdk AS base
WORKDIR /app

# restore dependencies
FROM microsoft/dotnet:2.1-sdk AS publish
WORKDIR /src
COPY . .
RUN dotnet publish -o /app

# final
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "benchmark.dll"]
