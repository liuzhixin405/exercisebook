/*
 Navicat Premium Data Transfer

 Source Server         : localdb
 Source Server Type    : SQL Server
 Source Server Version : 15004153 (15.00.4153)
 Source Host           : np:\\.\pipe\LOCALDB#AFFA3F25\tsql\query:1433
 Source Catalog        : BuyStuffer
 Source Schema         : dbo

 Target Server Type    : SQL Server
 Target Server Version : 15004153 (15.00.4153)
 File Encoding         : 65001

 Date: 30/08/2023 17:14:41
*/


-- ----------------------------
-- Table structure for __EFMigrationsHistory
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type IN ('U'))
	DROP TABLE [dbo].[__EFMigrationsHistory]
GO

CREATE TABLE [dbo].[__EFMigrationsHistory] (
  [MigrationId] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ProductVersion] nvarchar(32) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL
)
GO

ALTER TABLE [dbo].[__EFMigrationsHistory] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for Customers
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type IN ('U'))
	DROP TABLE [dbo].[Customers]
GO

CREATE TABLE [dbo].[Customers] (
  [Id] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [PasswordHash] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FirstName] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [LastName] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Email] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Gender] int  NOT NULL,
  [Avatar] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Address_Street] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Address_Number] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Address_City] nvarchar(15) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Address_Zip] nvarchar(15) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Address_Country] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Payment_Type] int  NULL,
  [Payment_Expires_Month] int  NULL,
  [Payment_Expires_Year] int  NULL,
  [Payment_Expires_When] datetime2(7)  NULL,
  [Payment_Owner] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Payment_Number] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[Customers] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for FidelityCards
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[FidelityCards]') AND type IN ('U'))
	DROP TABLE [dbo].[FidelityCards]
GO

CREATE TABLE [dbo].[FidelityCards] (
  [Number] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [OwnerCustomerId] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Points] int  NULL
)
GO

ALTER TABLE [dbo].[FidelityCards] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for OrderItems
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderItems]') AND type IN ('U'))
	DROP TABLE [dbo].[OrderItems]
GO

CREATE TABLE [dbo].[OrderItems] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Quantity] int  NOT NULL,
  [ProductId] int  NOT NULL,
  [OrderId] int  NOT NULL
)
GO

ALTER TABLE [dbo].[OrderItems] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for Orders
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type IN ('U'))
	DROP TABLE [dbo].[Orders]
GO

CREATE TABLE [dbo].[Orders] (
  [OrderId] int  NOT NULL,
  [BuyerCustomerId] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Total_Currency_Symbol] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Total_Value] decimal(18,2)  NULL,
  [State] int  NOT NULL,
  [Date] datetime2(7)  NOT NULL
)
GO

ALTER TABLE [dbo].[Orders] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for Products
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type IN ('U'))
	DROP TABLE [dbo].[Products]
GO

CREATE TABLE [dbo].[Products] (
  [Id] int  NOT NULL,
  [Description] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [UnitPrice_Currency_Symbol] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [UnitPrice_Value] decimal(18,2)  NOT NULL,
  [StockLevel] int  NULL,
  [Featured] bit  NULL
)
GO

ALTER TABLE [dbo].[Products] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for Subscribers
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Subscribers]') AND type IN ('U'))
	DROP TABLE [dbo].[Subscribers]
GO

CREATE TABLE [dbo].[Subscribers] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Email] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL
)
GO

ALTER TABLE [dbo].[Subscribers] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Primary Key structure for table __EFMigrationsHistory
-- ----------------------------
ALTER TABLE [dbo].[__EFMigrationsHistory] ADD CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table Customers
-- ----------------------------
ALTER TABLE [dbo].[Customers] ADD CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Indexes structure for table FidelityCards
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_FidelityCards_OwnerCustomerId]
ON [dbo].[FidelityCards] (
  [OwnerCustomerId] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table FidelityCards
-- ----------------------------
ALTER TABLE [dbo].[FidelityCards] ADD CONSTRAINT [PK_FidelityCards] PRIMARY KEY CLUSTERED ([Number])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for OrderItems
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[OrderItems]', RESEED, 1)
GO


-- ----------------------------
-- Indexes structure for table OrderItems
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderId]
ON [dbo].[OrderItems] (
  [OrderId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_OrderItems_ProductId]
ON [dbo].[OrderItems] (
  [ProductId] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table OrderItems
-- ----------------------------
ALTER TABLE [dbo].[OrderItems] ADD CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Indexes structure for table Orders
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_Orders_BuyerCustomerId]
ON [dbo].[Orders] (
  [BuyerCustomerId] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table Orders
-- ----------------------------
ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([OrderId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table Products
-- ----------------------------
ALTER TABLE [dbo].[Products] ADD CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Subscribers
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Subscribers]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table Subscribers
-- ----------------------------
ALTER TABLE [dbo].[Subscribers] ADD CONSTRAINT [PK_Subscribers] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Foreign Keys structure for table FidelityCards
-- ----------------------------
ALTER TABLE [dbo].[FidelityCards] ADD CONSTRAINT [FK_FidelityCards_Customers_OwnerCustomerId] FOREIGN KEY ([OwnerCustomerId]) REFERENCES [dbo].[Customers] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table OrderItems
-- ----------------------------
ALTER TABLE [dbo].[OrderItems] ADD CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([OrderId]) ON DELETE CASCADE ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[OrderItems] ADD CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table Orders
-- ----------------------------
ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [FK_Orders_Customers_BuyerCustomerId] FOREIGN KEY ([BuyerCustomerId]) REFERENCES [dbo].[Customers] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
GO

