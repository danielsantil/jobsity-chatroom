#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY JobsityChatroom.StockBot/JobsityChatroom.StockBot.csproj JobsityChatroom.StockBot/
COPY JobsityChatroom.Common/JobsityChatroom.Common.csproj JobsityChatroom.Common/
RUN dotnet restore "JobsityChatroom.StockBot/JobsityChatroom.StockBot.csproj"
COPY . .
WORKDIR "/src/JobsityChatroom.StockBot"
RUN dotnet build "JobsityChatroom.StockBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobsityChatroom.StockBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobsityChatroom.StockBot.dll"]
