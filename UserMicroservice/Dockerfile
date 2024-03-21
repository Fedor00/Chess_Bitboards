# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Use SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["UserMicroservice.csproj", "UserMicroservice/"]
RUN dotnet restore "UserMicroservice/UserMicroservice.csproj"
WORKDIR "/src/UserMicroservice"
COPY . .

RUN dotnet build "UserMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserMicroservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserMicroservice.dll"]
