IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO


---- ASP.NET Identity ----

CREATE TABLE [SystemUsers] (
    [Id] int NOT NULL IDENTITY,
    [FirstName] nvarchar(20) NOT NULL,
    [Surname] nvarchar(20) NOT NULL,
    [Patronymic] nvarchar(20) NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [NormalizedPhoneNumber] nvarchar(20) NOT NULL,
    [PasswordChangeRequired] bit NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_SystemUsers] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [AspNetRoles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_SystemUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [SystemUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_SystemUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [SystemUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_SystemUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [SystemUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] int NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_SystemUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [SystemUsers] ([Id]) ON DELETE CASCADE
);
GO


---- Company ----

CREATE TABLE [Companies] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(max) NULL,
    [CreatedUtc] datetime2 NOT NULL,
    [UpdatedUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [CompanyRequisites] (
    [Id] int NOT NULL IDENTITY,
    [UpdatedUtc] datetime2 NOT NULL,
    [MainRequisites_Name] nvarchar(50) NOT NULL,
    [MainRequisites_FullName] nvarchar(150) NULL,
    [MainRequisites_LegalAddress] nvarchar(300) NULL,
    [MainRequisites_PostalAddress] nvarchar(300) NULL,
    [MainRequisites_OGRN] nvarchar(50) NULL,
    [MainRequisites_OKPO] nvarchar(50) NULL,
    [MainRequisites_OKATO] nvarchar(50) NULL,
    [MainRequisites_INN] nvarchar(50) NULL,
    [MainRequisites_KPP] nvarchar(50) NULL,
    [BankRequisites_AccountNumber] nvarchar(50) NULL,
    [BankRequisites_BIC] nvarchar(50) NULL,
    [BankRequisites_CorrespondentAccount] nvarchar(50) NULL,
    [BankRequisites_Bank] nvarchar(300) NULL,
    [CompanyId] int NOT NULL,
    CONSTRAINT [PK_CompanyRequisites] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CompanyRequisites_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [CompanyUsers] (
    [Id] int NOT NULL IDENTITY,
    [Description] nvarchar(max) NULL,
    [CompanyId] int NOT NULL,
    [SystemUserId] int NOT NULL,
    CONSTRAINT [PK_CompanyUsers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CompanyUsers_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CompanyUsers_SystemUsers_SystemUserId] FOREIGN KEY ([SystemUserId]) REFERENCES [SystemUsers] ([Id]) ON DELETE NO ACTION
);
GO


---- Indexes ----

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE UNIQUE INDEX [IX_CompanyRequisites_CompanyId] ON [CompanyRequisites] ([CompanyId]);
GO

CREATE INDEX [IX_CompanyUsers_CompanyId] ON [CompanyUsers] ([CompanyId]);
GO

CREATE UNIQUE INDEX [IX_CompanyUsers_SystemUserId] ON [CompanyUsers] ([SystemUserId]);
GO

CREATE INDEX [EmailIndex] ON [SystemUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [SystemUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

---- End Migration ----

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210125230950_Init', N'5.0.0');
GO

COMMIT;
GO

