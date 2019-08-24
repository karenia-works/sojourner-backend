FROM mcr.microsoft.com/dotnet/core-nightly/sdk:3.0 as build
WORKDIR /app
COPY ./* sojourner/
WORKDIR /app/sojourner/
RUN dotnet restore
RUN dotnet publish -c Release -v m -o /app/bin/sojourner

FROM mcr.microsoft.com/dotnet/core-nightly/aspnet:3.0 as runtime
ENV ASPNETCORE_URLS http://+:80
WORKDIR /app
COPY --from=build /app/bin/sojourner ./
ENTRYPOINT ["dotnet", "sojourner_backend.dll"]
