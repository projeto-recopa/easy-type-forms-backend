#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base


WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["document-ml-predict.csproj", "document-ml-predict/"]
RUN dotnet restore "document-ml-predict/document-ml-predict.csproj"
WORKDIR "/src/document-ml-predict"
COPY . .
RUN dotnet build "document-ml-predict.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "document-ml-predict.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet document-ml-predict.dll
#ENTRYPOINT ["dotnet", "document-ml-predict.dll"]