IF OBJECT_ID(N'__EFMigrationsHistory') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [ExternalLink] (
    [Id] int NOT NULL IDENTITY,
    [Link] nvarchar(max),
    [Title] nvarchar(max),
    CONSTRAINT [PK_ExternalLink] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [AccessFailedCount] int NOT NULL,
    [CompanyId] int,
    [ConcurrencyStamp] nvarchar(max),
    [Email] nvarchar(256),
    [EmailConfirmed] bit NOT NULL,
    [LockoutEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset,
    [NormalizedEmail] nvarchar(256),
    [NormalizedUserName] nvarchar(256),
    [PasswordHash] nvarchar(max),
    [PhoneNumber] nvarchar(max),
    [PhoneNumberConfirmed] bit NOT NULL,
    [SecurityStamp] nvarchar(max),
    [TwoFactorEnabled] bit NOT NULL,
    [UserName] nvarchar(256),
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Images] (
    [Id] int NOT NULL IDENTITY,
    [Description] nvarchar(max),
    [ImageContent] varbinary(max),
    [ImageMimeType] nvarchar(max),
    [IsMain] bit NOT NULL,
    [ToDelete] bit NOT NULL,
    CONSTRAINT [PK_Images] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [CategoryId] int,
    [CategoryType] int NOT NULL,
    [EnTitle] nvarchar(100) NOT NULL,
    [Title] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Categories_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [ConcurrencyStamp] nvarchar(max),
    [Name] nvarchar(256),
    [NormalizedName] nvarchar(256),
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max),
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name])
);

GO

CREATE TABLE [Companies] (
    [Id] int NOT NULL IDENTITY,
    [AccountType] int NOT NULL,
    [ApplicationUserId] nvarchar(450),
    [ContactEmail] nvarchar(max),
    [Description] nvarchar(max),
    [Telephone] nvarchar(max),
    [Title] nvarchar(max),
    CONSTRAINT [PK_Companies] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Companies_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [ClaimType] nvarchar(max),
    [ClaimValue] nvarchar(max),
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max),
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [Articles] (
    [Id] int NOT NULL IDENTITY,
    [CategoryId] int,
    [CategoryType] int NOT NULL,
    [Description] nvarchar(max),
    [EnTitle] nvarchar(max),
    [HashTags] nvarchar(max),
    [Link] nvarchar(max),
    [Title] nvarchar(max),
    [UpdateTime] datetime2 NOT NULL,
    CONSTRAINT [PK_Articles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Articles_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [ClaimType] nvarchar(max),
    [ClaimValue] nvarchar(max),
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [RelCompanyImage] (
    [CompanyId] int NOT NULL,
    [ImageId] int NOT NULL,
    CONSTRAINT [PK_RelCompanyImage] PRIMARY KEY ([CompanyId], [ImageId]),
    CONSTRAINT [FK_RelCompanyImage_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RelCompanyImage_Images_ImageId] FOREIGN KEY ([ImageId]) REFERENCES [Images] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [RelCompanyGood] (
    [CompanyId] int NOT NULL,
    [GoodId] int NOT NULL,
    CONSTRAINT [PK_RelCompanyGood] PRIMARY KEY ([CompanyId], [GoodId]),
    CONSTRAINT [FK_RelCompanyGood_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RelCompanyGood_Articles_GoodId] FOREIGN KEY ([GoodId]) REFERENCES [Articles] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [RelGoodExternalLink] (
    [ExternalLinkId] int NOT NULL,
    [GoodId] int NOT NULL,
    CONSTRAINT [PK_RelGoodExternalLink] PRIMARY KEY ([ExternalLinkId], [GoodId]),
    CONSTRAINT [FK_RelGoodExternalLink_ExternalLink_ExternalLinkId] FOREIGN KEY ([ExternalLinkId]) REFERENCES [ExternalLink] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RelGoodExternalLink_Articles_GoodId] FOREIGN KEY ([GoodId]) REFERENCES [Articles] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [RelGoodImage] (
    [ImageId] int NOT NULL,
    [GoodId] int NOT NULL,
    CONSTRAINT [PK_RelGoodImage] PRIMARY KEY ([ImageId], [GoodId]),
    CONSTRAINT [FK_RelGoodImage_Articles_GoodId] FOREIGN KEY ([GoodId]) REFERENCES [Articles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RelGoodImage_Images_ImageId] FOREIGN KEY ([ImageId]) REFERENCES [Images] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

GO

CREATE INDEX [IX_Articles_CategoryId] ON [Articles] ([CategoryId]);

GO

CREATE INDEX [IX_Categories_CategoryId] ON [Categories] ([CategoryId]);

GO

CREATE UNIQUE INDEX [IX_Companies_ApplicationUserId] ON [Companies] ([ApplicationUserId]) WHERE [ApplicationUserId] IS NOT NULL;

GO

CREATE INDEX [IX_RelCompanyGood_CompanyId] ON [RelCompanyGood] ([CompanyId]);

GO

CREATE INDEX [IX_RelCompanyGood_GoodId] ON [RelCompanyGood] ([GoodId]);

GO

CREATE INDEX [IX_RelCompanyImage_CompanyId] ON [RelCompanyImage] ([CompanyId]);

GO

CREATE INDEX [IX_RelCompanyImage_ImageId] ON [RelCompanyImage] ([ImageId]);

GO

CREATE INDEX [IX_RelGoodExternalLink_ExternalLinkId] ON [RelGoodExternalLink] ([ExternalLinkId]);

GO

CREATE INDEX [IX_RelGoodExternalLink_GoodId] ON [RelGoodExternalLink] ([GoodId]);

GO

CREATE INDEX [IX_RelGoodImage_GoodId] ON [RelGoodImage] ([GoodId]);

GO

CREATE INDEX [IX_RelGoodImage_ImageId] ON [RelGoodImage] ([ImageId]);

GO

CREATE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]);

GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

GO

CREATE INDEX [IX_AspNetUserRoles_UserId] ON [AspNetUserRoles] ([UserId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20170218100007_Initial', N'1.0.1');

GO

