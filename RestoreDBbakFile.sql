
--Assuming the bak file exists in the shared docker volume


RESTORE DATABASE VideoGameCatalogueDB
FROM DISK = '/shared/VideoGameCatalogueDB_Feb22.bak'
WITH
    MOVE 'VideoGameCatalogueDB' TO '/var/opt/mssql/data/VideoGameCatalogueDB.mdf',
    MOVE 'VideoGameCatalogueDB_log' TO '/var/opt/mssql/data/VideoGameCatalogueDB_log.ldf',
    REPLACE,
    RECOVERY;
