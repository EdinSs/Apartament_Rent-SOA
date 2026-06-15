FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY *.sln ./
COPY EasyRent.API/*.csproj ./EasyRent.API/
COPY EasyRent.Application/*.csproj ./EasyRent.Application/
COPY EasyRent.Domain/*.csproj ./EasyRent.Domain/
COPY EasyRent.Infrastructure/*.csproj ./EasyRent.Infrastructure/
COPY EasyRent.Tests/*.csproj ./EasyRent.Tests/

RUN dotnet restore

COPY . ./
RUN dotnet publish EasyRent.API/EasyRent.API.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "EasyRent.API.dll"]