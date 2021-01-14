#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN apt-get update && apt-get install -y libgdiplus

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["image-cloud-processor.csproj", "image-cloud-processor/"]
RUN dotnet restore "image-cloud-processor/image-cloud-processor.csproj"
WORKDIR "/src/image-cloud-processor"
COPY . .
RUN dotnet build "image-cloud-processor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "image-cloud-processor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet image-cloud-processor.dll
#ENTRYPOINT ["dotnet", "image-cloud-processor.dll"]