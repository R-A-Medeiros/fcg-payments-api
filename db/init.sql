CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260901174853_InitialCreate') THEN
    CREATE TABLE payments (
        "Id" uuid NOT NULL,
        "OrderId" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "GameId" uuid NOT NULL,
        "Amount" numeric(10,2) NOT NULL,
        "Result" character varying(20) NOT NULL,
        "Reason" character varying(300) NOT NULL,
        "ProcessedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_payments" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260901174853_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_payments_OrderId" ON payments ("OrderId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260901174853_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260901174853_InitialCreate', '10.0.11');
    END IF;
END $EF$;
COMMIT;

