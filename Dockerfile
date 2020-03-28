FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build

WORKDIR /app

COPY *.csproj ./
RUN dotnet restore FixApp.csproj

COPY . ./
RUN dotnet publish FixApp.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime

WORKDIR /app

COPY ./spec ./spec

EXPOSE 5080

EXPOSE 5001

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "FixApp.dll"]

CMD ["executor.cfg"]