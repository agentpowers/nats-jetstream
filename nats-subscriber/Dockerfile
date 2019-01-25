# base
FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

# restore dependencies
FROM microsoft/dotnet:2.1-sdk AS publish
WORKDIR /src
COPY . .
RUN dotnet build
RUN dotnet publish --no-restore --no-build -o /app

# final
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "nats-subscriber.dll"]
