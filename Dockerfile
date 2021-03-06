#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY ["ImageClueAPI.csproj", ""]
RUN dotnet restore ImageClueAPI.csproj
COPY . .
WORKDIR "/src/."
RUN dotnet build ImageClueAPI.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ImageClueAPI.csproj -c Release -o /app/publish

# Run unit tests
RUN dotnet test --verbosity normal

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy the certs
RUN mkdir https
COPY aspnetapp_localhost.pfx /https/
COPY imageclue.pfx /https/
ENTRYPOINT ["dotnet", "ImageClueAPI.dll"]