CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [Description] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);
