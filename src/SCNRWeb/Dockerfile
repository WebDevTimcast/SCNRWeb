FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-amd64 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src/node/
RUN git clone https://github.com/phillipfisher/ON.Install.git
WORKDIR /src/SCNRWeb/src/SCNRWeb
COPY . .
RUN dotnet restore "SCNRWeb.csproj"
RUN dotnet build "SCNRWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SCNRWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SCNRWeb.dll"]