-- Inserts roles and permissions into Identity database. Does NOT assign user to role.

-- 1) Insert role Payer if not exists
INSERT INTO "AspNetRoles" ("Id","Name","NormalizedName","ConcurrencyStamp")
SELECT '00000000-0000-0000-0000-000000000001','Payer','PAYER','stamp-payer'
WHERE NOT EXISTS (SELECT 1 FROM "AspNetRoles" WHERE "Id" = '00000000-0000-0000-0000-000000000001');

-- 2) Insert permissions
INSERT INTO "Permissions" ("Id","Name","Description")
SELECT 1, 'payments.create', 'Create payments'
WHERE NOT EXISTS (SELECT 1 FROM "Permissions" WHERE "Id" = 1);

INSERT INTO "Permissions" ("Id","Name","Description")
SELECT 2, 'payments.read', 'List payments'
WHERE NOT EXISTS (SELECT 1 FROM "Permissions" WHERE "Id" = 2);

INSERT INTO "Permissions" ("Id","Name","Description")
SELECT 3, 'payments.view', 'View a payment'
WHERE NOT EXISTS (SELECT 1 FROM "Permissions" WHERE "Id" = 3);

INSERT INTO "Permissions" ("Id","Name","Description")
SELECT 4, 'payments.delete', 'Delete payment'
WHERE NOT EXISTS (SELECT 1 FROM "Permissions" WHERE "Id" = 4);

-- 3) Assign permissions to role Payer
INSERT INTO "RolePermissions" ("RoleId","PermissionId")
SELECT '00000000-0000-0000-0000-000000000001', 1
WHERE NOT EXISTS (SELECT 1 FROM "RolePermissions" WHERE "RoleId" = '00000000-0000-0000-0000-000000000001' AND "PermissionId" = 1);

INSERT INTO "RolePermissions" ("RoleId","PermissionId")
SELECT '00000000-0000-0000-0000-000000000001', 2
WHERE NOT EXISTS (SELECT 1 FROM "RolePermissions" WHERE "RoleId" = '00000000-0000-0000-0000-000000000001' AND "PermissionId" = 2);

INSERT INTO "RolePermissions" ("RoleId","PermissionId")
SELECT '00000000-0000-0000-0000-000000000001', 3
WHERE NOT EXISTS (SELECT 1 FROM "RolePermissions" WHERE "RoleId" = '00000000-0000-0000-0000-000000000001' AND "PermissionId" = 3);
