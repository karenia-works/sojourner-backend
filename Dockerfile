FROM microsoft/dotnet:2.2-sdk as build
WORKDIR /app
COPY ./* sojourner/
WORKDIR /app/sojourner/
RUN dotnet restore
RUN dotnet publish -c Release -v m -o /app/bin/sojourner

FROM microsoft/dotnet:2.2-runtime as runtime
ENV ASPNETCORE_URLS http://+:80
WORKDIR /app
COPY --from=build /app/bin/sojourner ./
ENTRYPOINT ["dotnet", "sojourner_backend.dll"]
