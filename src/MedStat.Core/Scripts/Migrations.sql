-- 1) 20210125230950_Init
-- run 20210125230950_Init.sql


-- 2) 20210205142618_add_Device
BEGIN TRANSACTION;
GO

CREATE TABLE [Device] (
    [Id] int NOT NULL IDENTITY,
    [InventoryNumber] nvarchar(25) NOT NULL,
    [NormalizedEthernetMac] nvarchar(12) NULL,
    [NormalizedWifiMac] nvarchar(12) NULL,
    [CreatedUtc] datetime2 NOT NULL,
    [DeviceModelUid] nvarchar(20) NOT NULL,
    [CompanyId] int NULL,
    CONSTRAINT [PK_Device] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Device_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Device_CompanyId] ON [Device] ([CompanyId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210205142618_add_Device', N'5.0.0');
GO

COMMIT;
GO

