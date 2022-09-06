USE [ContosoUniversity4]
GO

/****** 对象: Table [dbo].[__EFMigrationsHistory] 脚本日期: 2022/4/2 1:10:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[__EFMigrationsHistory] (
    [MigrationId]    NVARCHAR (150) NOT NULL,
    [ProductVersion] NVARCHAR (32)  NOT NULL
);


USE [ContosoUniversity4]
GO

/****** 对象: Table [dbo].[Course] 脚本日期: 2022/4/2 1:10:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Course] (
    [CourseID]     INT           NOT NULL,
    [Title]        NVARCHAR (50) NULL,
    [Credits]      INT           NOT NULL,
    [DepartmentID] INT           NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Course_DepartmentID]
    ON [dbo].[Course]([DepartmentID] ASC);


GO
ALTER TABLE [dbo].[Course]
    ADD CONSTRAINT [PK_Course] PRIMARY KEY CLUSTERED ([CourseID] ASC);


GO
ALTER TABLE [dbo].[Course]
    ADD CONSTRAINT [FK_Course_Department_DepartmentID] FOREIGN KEY ([DepartmentID]) REFERENCES [dbo].[Department] ([DepartmentID]) ON DELETE CASCADE;


USE [ContosoUniversity4]
GO

/****** 对象: Table [dbo].[CourseAssignment] 脚本日期: 2022/4/2 1:10:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CourseAssignment] (
    [InstructorID] INT NOT NULL,
    [CourseID]     INT NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_CourseAssignment_InstructorID]
    ON [dbo].[CourseAssignment]([InstructorID] ASC);


GO
ALTER TABLE [dbo].[CourseAssignment]
    ADD CONSTRAINT [PK_CourseAssignment] PRIMARY KEY CLUSTERED ([CourseID] ASC, [InstructorID] ASC);


GO
ALTER TABLE [dbo].[CourseAssignment]
    ADD CONSTRAINT [FK_CourseAssignment_Course_CourseID] FOREIGN KEY ([CourseID]) REFERENCES [dbo].[Course] ([CourseID]) ON DELETE CASCADE;


GO
ALTER TABLE [dbo].[CourseAssignment]
    ADD CONSTRAINT [FK_CourseAssignment_Instructor_InstructorID] FOREIGN KEY ([InstructorID]) REFERENCES [dbo].[Person] ([ID]) ON DELETE CASCADE;


USE [ContosoUniversity4]
GO

/****** 对象: Table [dbo].[Department] 脚本日期: 2022/4/2 1:10:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Department] (
    [DepartmentID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50) NULL,
    [Budget]       MONEY         NOT NULL,
    [StartDate]    DATETIME2 (7) NOT NULL,
    [InstructorID] INT           NULL,
    [RowVersion]   ROWVERSION    NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Department_InstructorID]
    ON [dbo].[Department]([InstructorID] ASC);


GO
ALTER TABLE [dbo].[Department]
    ADD CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED ([DepartmentID] ASC);


GO
ALTER TABLE [dbo].[Department]
    ADD CONSTRAINT [FK_Department_Instructor_InstructorID] FOREIGN KEY ([InstructorID]) REFERENCES [dbo].[Person] ([ID]);


USE [ContosoUniversity4]
GO

/****** 对象: Table [dbo].[Enrollment] 脚本日期: 2022/4/2 1:10:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Enrollment] (
    [EnrollmentID] INT IDENTITY (1, 1) NOT NULL,
    [CourseID]     INT NOT NULL,
    [StudentID]    INT NOT NULL,
    [Grade]        INT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Enrollment_CourseID]
    ON [dbo].[Enrollment]([CourseID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Enrollment_StudentID]
    ON [dbo].[Enrollment]([StudentID] ASC);


GO
ALTER TABLE [dbo].[Enrollment]
    ADD CONSTRAINT [PK_Enrollment] PRIMARY KEY CLUSTERED ([EnrollmentID] ASC);


GO
ALTER TABLE [dbo].[Enrollment]
    ADD CONSTRAINT [FK_Enrollment_Course_CourseID] FOREIGN KEY ([CourseID]) REFERENCES [dbo].[Course] ([CourseID]) ON DELETE CASCADE;


GO
ALTER TABLE [dbo].[Enrollment]
    ADD CONSTRAINT [FK_Enrollment_Person_StudentID] FOREIGN KEY ([StudentID]) REFERENCES [dbo].[Person] ([ID]) ON DELETE CASCADE;


USE [ContosoUniversity4]
GO

/****** 对象: Table [dbo].[Instructor] 脚本日期: 2022/4/2 1:10:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Instructor] (
    [ID]        INT           IDENTITY (1, 1) NOT NULL,
    [LastName]  NVARCHAR (50) NOT NULL,
    [FirstName] NVARCHAR (50) NOT NULL,
    [HireDate]  DATETIME2 (7) NOT NULL
);


USE [ContosoUniversity4]
GO

/****** 对象: Table [dbo].[OfficeAssignment] 脚本日期: 2022/4/2 1:11:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OfficeAssignment] (
    [InstructorID] INT           NOT NULL,
    [Location]     NVARCHAR (50) NULL
);


USE [ContosoUniversity4]
GO

/****** 对象: Table [dbo].[Person] 脚本日期: 2022/4/2 1:11:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Person] (
    [ID]             INT            IDENTITY (1, 1) NOT NULL,
    [LastName]       NVARCHAR (50)  NOT NULL,
    [FirstName]      NVARCHAR (50)  NOT NULL,
    [HireDate]       DATETIME2 (7)  NULL,
    [EnrollmentDate] DATETIME2 (7)  NULL,
    [Discriminator]  NVARCHAR (128) NOT NULL
);


USE [ContosoUniversity4]
GO

/****** 对象: Table [dbo].[Student] 脚本日期: 2022/4/2 1:11:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Student] (
    [ID]             INT           IDENTITY (1, 1) NOT NULL,
    [LastName]       NVARCHAR (50) NOT NULL,
    [FirstName]      NVARCHAR (50) NOT NULL,
    [EnrollmentDate] DATETIME2 (7) NOT NULL
);


