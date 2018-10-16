FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
RUN apt-get update && apt-get install -y libfontconfig
COPY --from=build-env /app/out .
COPY --from=build-env /app/phantomJS/ phantomJS/
RUN chmod -R 777 phantomJS/
ENTRYPOINT ["dotnet", "HeliosBot.dll"]