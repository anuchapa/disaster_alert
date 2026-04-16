FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5147
ENV ASPNETCORE_URLS=http://+:5147

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY *.sln .
COPY DisasterAlert.api/*.csproj DisasterAlert.api/  
RUN dotnet restore "DisasterAlert.api/DisasterAlert.api.csproj"
COPY . .
WORKDIR "/src/DisasterAlert.api"
RUN dotnet build "DisasterAlert.api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "DisasterAlert.api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DisasterAlert.api.dll"]
