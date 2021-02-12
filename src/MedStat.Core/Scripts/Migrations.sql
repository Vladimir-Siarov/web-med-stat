-- 1) 20210125230950_Init
-- run 20210125230950_Init.sql


-- 2) 20210212203411_add_Device
BEGIN TRANSACTION;
GO

CREATE TABLE [Devices] (
    [Id] int NOT NULL IDENTITY,
    [InventoryNumber] nvarchar(25) NOT NULL,
    [NormalizedEthernetMac] nvarchar(12) NULL,
    [NormalizedWifiMac] nvarchar(12) NULL,
    [CreatedUtc] datetime2 NOT NULL,
    [DeviceModelUid] nvarchar(20) NOT NULL,
    [CompanyId] int NULL,
    CONSTRAINT [PK_Devices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Devices_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Devices_CompanyId] ON [Devices] ([CompanyId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210212203411_add_Device', N'5.0.0');
GO

COMMIT;
GO
