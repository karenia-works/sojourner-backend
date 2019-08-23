FROM microsoft/dotnet:2.2-sdk as build
WORKDIR /app
COPY sojourner_backend.csproj sojourner/
WORKDIR /app/sojourner/
RUN dotnet publish -c Release -v m -o ./bin/sojourner

FROM microsoft/dotnet:2.2-runtime as runtime
ENV ASPNETCORE_URLS http://+:80
WORKDIR /app
COPY --from=build /app/bin/sojourner ./
ENTRYPOINT ["dotnet", "sojourner_backend.dll"]
