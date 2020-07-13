FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
EXPOSE 12345
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["Server/Server.csproj", "Server/"]
RUN dotnet restore "Server.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Server.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "Server.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Server.dll"]