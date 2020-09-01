#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-1809 AS base
WORKDIR /app
COPY . .
EXPOSE 5002
ENV ASPNETCORE_URLS=http://*:5002

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1809 AS build
WORKDIR /src
COPY ["MoviesAPI.csproj", "./"]
RUN dotnet restore "./MoviesAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "MoviesAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MoviesAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
USER ContainerAdministrator
RUN icacls "C:\app\CSV Files\Movies.csv" /t /grant Users:M
USER ContainerUser
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MoviesAPI.dll"]
