FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as build
WORKDIR /app
COPY sojourner_backend.csproj sojourner/
WORKDIR sojourner/
RUN dotnet publish -c Release -v m -o ./bin/sojourner

FROM microsoft/dotnet:2.2-runtime as runtime
ENV ASPNETCORE_URLS http://+:80
WORKDIR /app
COPY --from=build /app/bin/sojourner ./
ENTRYPOINT ["dotnet", "sojourner_backend.dll"]
