FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["VPVC-Backend/VPVC-Backend.csproj", "VPVC-Backend/"]
RUN dotnet restore "VPVC-Backend/VPVC-Backend.csproj"
COPY . .
WORKDIR "/src/VPVC-Backend"
RUN dotnet build "VPVC-Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VPVC-Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VPVC-Backend.dll"]
