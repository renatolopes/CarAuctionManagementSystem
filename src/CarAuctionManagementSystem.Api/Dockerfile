FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8090

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["src/CarAuctionManagementSystem.Api/CarAuctionManagementSystem.Api.csproj", "src/CarAuctionManagementSystem.Api/"]
COPY ["src/CarAuctionManagementSystem.Infrastructure/CarAuctionManagementSystem.Infrastructure.csproj", "src/CarAuctionManagementSystem.Infrastructure/"]
COPY ["src/CarAuctionManagementSystem.Application/CarAuctionManagementSystem.Application.csproj", "src/CarAuctionManagementSystem.Application/"]
COPY ["src/CarAuctionManagementSystem.Domain/CarAuctionManagementSystem.Domain.csproj", "src/CarAuctionManagementSystem.Domain/"]

RUN dotnet restore "src/CarAuctionManagementSystem.Api/CarAuctionManagementSystem.Api.csproj"

COPY . .

WORKDIR "/src/src/CarAuctionManagementSystem.Api"

RUN dotnet build "CarAuctionManagementSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CarAuctionManagementSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV ASPNETCORE_URLS=http://0.0.0.0:8090;
ENV DOTNET_ENVIRONMENT=Release

WORKDIR /app

COPY --from=publish /app/publish .
COPY src/CarAuctionManagementSystem.Api/entrypoint.sh .
RUN chmod u+x entrypoint.sh

CMD ./entrypoint.sh
