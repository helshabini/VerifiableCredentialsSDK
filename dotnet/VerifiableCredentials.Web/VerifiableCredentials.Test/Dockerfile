FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["VerifiableCredentials.App/VerifiableCredentials.App.csproj", "VerifiableCredentials.App/"]
RUN dotnet restore "VerifiableCredentials.App/VerifiableCredentials.App.csproj"
COPY . .
WORKDIR "/src/VerifiableCredentials.App"
RUN dotnet build "VerifiableCredentials.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VerifiableCredentials.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VerifiableCredentials.App.dll"]
