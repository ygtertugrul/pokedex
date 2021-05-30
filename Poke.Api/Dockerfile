FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
LABEL author="Yigit Ertugrul"

ENV ASPNETCORE_ENVIRONMENT="Development"

EXPOSE    5000
WORKDIR /src
COPY Poke.Api.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Poke.Api.dll"]
