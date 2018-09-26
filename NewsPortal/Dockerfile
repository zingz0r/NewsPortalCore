FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["NewsPortal/NewsPortal.csproj", "NewsPortal/"]
RUN dotnet restore "NewsPortal/NewsPortal.csproj"
COPY . .
WORKDIR "/src/NewsPortal"
RUN dotnet build "NewsPortal.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "NewsPortal.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NewsPortal.dll"]