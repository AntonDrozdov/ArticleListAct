ALTER TABLE [Articles] ADD [EnTitle] nvarchar(max);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20170131105033_AddEnTitleFieldToArticle', N'1.0.1');

GO

