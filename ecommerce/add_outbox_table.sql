-- 添加OutboxMessages表
USE ECommerceDb;

-- 创建OutboxMessages表
CREATE TABLE IF NOT EXISTS `OutboxMessages` (
    `Id` char(36) NOT NULL,
    `Type` varchar(200) NOT NULL,
    `Data` longtext NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `ProcessedAt` datetime(6) NULL,
    `Error` longtext NULL,
    `RetryCount` int NOT NULL DEFAULT 0,
    `NextRetryAt` datetime(6) NULL,
    `Status` int NOT NULL DEFAULT 0,
    `CorrelationId` varchar(255) NULL,
    `CausationId` varchar(255) NULL,
    PRIMARY KEY (`Id`)
);

-- 创建索引以优化查询性能
CREATE INDEX IF NOT EXISTS `IX_OutboxMessages_Status` ON `OutboxMessages` (`Status`);
CREATE INDEX IF NOT EXISTS `IX_OutboxMessages_CreatedAt` ON `OutboxMessages` (`CreatedAt`);
CREATE INDEX IF NOT EXISTS `IX_OutboxMessages_NextRetryAt` ON `OutboxMessages` (`NextRetryAt`);
CREATE INDEX IF NOT EXISTS `IX_OutboxMessages_CorrelationId` ON `OutboxMessages` (`CorrelationId`);

-- 添加注释
ALTER TABLE `OutboxMessages` COMMENT = 'Outbox pattern table for reliable event publishing';

-- 验证表创建
DESCRIBE `OutboxMessages`;

-- 显示索引信息
SHOW INDEX FROM `OutboxMessages`;
