
/*
How DB was used
Docker Deskop SQL DB system
https://hub.docker.com/_/microsoft-mssql-server

docker run --restart unless-stopped -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MySaPassword123" -e "MSSQL_PID=Evaluation" -p 1433:1433  --name sqlpreview --hostname sqlpreview -d mcr.microsoft.com/mssql/server:2025-latest
*/

Create Database VideoGameCatalogueDB;

