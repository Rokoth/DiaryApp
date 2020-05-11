/*
Скрипт развертывания для Diary

Этот код был создан программным средством.
Изменения, внесенные в этот файл, могут привести к неверному выполнению кода и будут потеряны
в случае его повторного формирования.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "Diary"
:setvar DefaultFilePrefix "Diary"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\"

GO
:on error exit
GO
/*
Проверьте режим SQLCMD и отключите выполнение скрипта, если режим SQLCMD не поддерживается.
Чтобы повторно включить скрипт после включения режима SQLCMD выполните следующую инструкцию:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'Для успешного выполнения этого скрипта должен быть включен режим SQLCMD.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO

IF (SELECT OBJECT_ID('tempdb..#tmpErrors')) IS NOT NULL DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
GO
BEGIN TRANSACTION
GO
PRINT N'Выполняется создание [dbo].[Diary]...';


GO
CREATE TABLE [dbo].[Diary] (
    [Id]          UNIQUEIDENTIFIER   NOT NULL,
    [VersionDate] DATETIMEOFFSET (7) NOT NULL,
    [IsDeleted]   BIT                NOT NULL,
    [EntryType]   SMALLINT           NOT NULL,
    [Title]       VARCHAR (50)       NULL,
    [Description] NVARCHAR (MAX)     NULL,
    [BeginDate]   DATETIMEOFFSET (7) NOT NULL,
    [EndDate]     DATETIMEOFFSET (7) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[MeetingPlace] (
    [Id]    UNIQUEIDENTIFIER NOT NULL,
    [Place] NVARCHAR (200)   NULL,
    [VersionDate] DATETIMEOFFSET NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[Contact]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [VersionDate] DATETIMEOFFSET NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [FirstName] VARCHAR(50) NULL, 
    [SecondName] VARCHAR(50) NULL, 
    [ThirdName] VARCHAR(50) NULL, 
    [BirthDate] DATETIMEOFFSET NULL, 
    [Company] VARCHAR(200) NULL, 
    [Position] VARCHAR(50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[ContactInfo]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [VersionDate] DATETIMEOFFSET NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [ContactId] UNIQUEIDENTIFIER NOT NULL, 
    [ContactInfoType] SMALLINT NOT NULL, 
    [Value] VARCHAR(200) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) 
);

GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT N'Транзакции обновления базы данных успешно завершены.'
COMMIT TRANSACTION
END
ELSE PRINT N'Сбой транзакций обновления базы данных.'
GO
DROP TABLE #tmpErrors
GO
PRINT N'Обновление завершено.';


GO
