FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["GitGetter/GitGetter.csproj", "GitGetter/"]
RUN dotnet restore "GitGetter/GitGetter.csproj"
COPY . .
WORKDIR "/src/GitGetter"
RUN dotnet build "GitGetter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GitGetter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GitGetter.dll"]
