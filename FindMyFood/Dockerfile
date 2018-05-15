FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 51900
EXPOSE 44311

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY FindMyFood/FindMyFood.csproj FindMyFood/
RUN dotnet restore FindMyFood/FindMyFood.csproj
COPY . .
WORKDIR /src/FindMyFood
RUN dotnet build FindMyFood.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish FindMyFood.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FindMyFood.dll"]
