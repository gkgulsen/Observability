#FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
#WORKDIR /src
#COPY ["Observability.MetricAPI/Observability.MetricAPI.csproj", "Observability.MetricAPI/"]
#RUN dotnet restore "Observability.MetricAPI/Observability.MetricAPI.csproj"
#COPY . .
#WORKDIR "/src/Observability.MetricAPI"
#RUN dotnet build "Observability.MetricAPI.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "Observability.MetricAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Observability.MetricAPI.dll"]
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

COPY *.csproj .
RUN	dotnet restore

COPY . ./

RUN dotnet publish --no-restore -c Release -o out

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Observability.MetricAPI.dll"]