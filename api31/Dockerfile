FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build

WORKDIR /app
EXPOSE 8080

COPY . .

RUN dotnet publish ./api31.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime

WORKDIR /app
COPY --from=build /app/build .

ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "WJR.WebApi.dll"]