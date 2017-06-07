CREATE DATABASE RATV3
ON
PRIMARY (NAME = 'RATV3',
FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\RATV3.mdf',
SIZE = 10,
MAXSIZE = 500,
FILEGROWTH = 10),
FILEGROUP NorthPoleFS CONTAINS FILESTREAM( 
      NAME = NorthPoleFS,
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\RATV3FS')
LOG ON
(NAME = 'RATV3Log',
FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\RATV3Log.ldf',
SIZE = 5,
MAXSIZE = 100,
FILEGROWTH = 5);
GO

USE RATV3
CREATE TABLE Files(
   FileID UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL PRIMARY KEY,
   FileNumber VARCHAR(50),
   FileDescription VARCHAR(50),
   ItemImage VARBINARY(MAX) FILESTREAM NULL
)
CREATE TABLE Departments
(
DepartmentID int IDENTITY NOT NULL PRIMARY KEY,
Title varchar(50) NOT NULL,
MaxSize int NOT NULL,
DeptDescription varchar(max) NULL,
StartDate date NOT NULL,
MonthlyExpenses money NOT NULL,
BannerImageID UNIQUEIDENTIFIER NULL,
MainImageID UNIQUEIDENTIFIER   NULL,
CONSTRAINT FK_BannerID FOREIGN KEY(BannerImageID)
REFERENCES RATV3.dbo.Files (FileID) ON DELETE NO ACTION  ON UPDATE NO ACTION,
CONSTRAINT FK_MainImageID FOREIGN KEY(MainImageID)
REFERENCES RATV3.dbo.Files (FileID) ON DELETE NO ACTION  ON UPDATE NO ACTION,
)

CREATE TABLE Roles
(
RoleID int IDENTITY NOT NULL PRIMARY KEY,
Name varchar(50) NOT NULL,
JobDescription varchar(max) NULL,
AverageSalary money NOT NULL,
)

CREATE TABLE Employees
(
EmployeeID int IDENTITY  NOT NULL PRIMARY KEY,
RoleID int NULL,
Account varchar(50) NOT NULL UNIQUE,
Password varchar(50) NOT NULL,
ManagerID int NULL,
DepartmentID int NULL,
FirstName varchar(50) NOT NULL,
MiddleInitial char(1) NULL,
LastName varchar(50) NOT NULL,
Title varchar(50) NOT NULL,
CNP varchar(13) NOT NULL,
Email varchar(300) NOT NULL,
PhoneNumber varchar(20) NOT NULL,
Salary money NOT NULL,
PriorSalary money NOT NULL,
LastRaise AS Salary - PriorSalary,
HireDate date NOT NULL,
TerminationDate date NULL,
Administrator varchar(50) NOT NULL
CONSTRAINT FK_RoleID FOREIGN KEY (RoleID) 
REFERENCES RATV3.dbo.Roles (RoleID) ON DELETE SET NULL,
CONSTRAINT FK_DepartmentID FOREIGN KEY (DepartmentID) 
REFERENCES RATV3.dbo.Departments (DepartmentID) ON DELETE SET NULL,
CONSTRAINT FK_ManagerID FOREIGN KEY(ManagerID)
REFERENCES RATV3.dbo.Employees (EmployeeID) ON DELETE NO ACTION  ON UPDATE NO ACTION,
)
CREATE TABLE Educations
(
EducationID int IDENTITY NOT NULL PRIMARY KEY,
Title varchar(50) NOT NULL,
StartDate date NOT NULL,
EndDate date NOT NULL,
Degree varchar(50) NOT NULL,
EmployeeID int NOT NULL,
Duration AS datediff(day, StartDate, EndDate),
CONSTRAINT FK_EmployeeID FOREIGN KEY (EmployeeID) 
REFERENCES RATV3.dbo.Employees (EmployeeID) ON DELETE CASCADE ON UPDATE NO ACTION,
)
CREATE TABLE Contacts
(
ContactID int IDENTITY NOT NULL PRIMARY KEY,
ContactName varchar(50) NOT NULL,
PhoneNumber varchar(13) NULL,
FaxNumber varchar(13) NULL,
Email varchar(50) NULL,
PhysicalAddress varchar(75) NULL,
)
CREATE TABLE Projects
(
ProjectID int IDENTITY NOT NULL PRIMARY KEY,
Title varchar(50) NOT NULL,
StartDate date NOT NULL,
EndDate date NOT NULL,
Duration AS datediff(day, StartDate, EndDate),
ContractNumber varchar(50) NOT NULL,
PJDescription varchar(max) NOT NULL,
Budget money NOT NULL,
DepartmentID int NULL,
ContactID int NOT NULL,
CONSTRAINT FK_DepartmentIDProject FOREIGN KEY (DepartmentID) 
REFERENCES RATV3.dbo.Departments (DepartmentID) ON DELETE SET NULL,
CONSTRAINT FK_ContactID FOREIGN KEY (ContactID) 
REFERENCES RATV3.dbo.Contacts (ContactID) ON DELETE CASCADE ON UPDATE NO ACTION,
)

CREATE TABLE Sprints (
	SprintID int IDENTITY NOT NULL PRIMARY KEY,
	ProjectID int NOT NULL,
	StartDate date NOT NULL,
	EndDate date NOT NULL,
	Duration as datediff(day, StartDate,EndDate),
	CONSTRAINT FK_ProjectID FOREIGN KEY (ProjectID) 
	REFERENCES RATV3.dbo.Projects (ProjectID) ON DELETE CASCADE ON UPDATE NO ACTION,	
)

CREATE TABLE Tasks (
	TaskID int IDENTITY NOT NULL PRIMARY KEY,

	SprintID int NOT NULL,
	EmployeeID int NOT NULL,
	RoleID int NOT NULL,
	
	StartDate date NOT NULL,
	EndDate date NOT NULL,
	Duration as datediff(day, StartDate, EndDate),
	
	TaskDescription varchar(max) NOT NULL,
	Difficulty int NOT NULL CHECK (Difficulty BETWEEN 1 AND 10),

	Estimation int null,

	CONSTRAINT FK_SprintID FOREIGN KEY (SprintID)
	REFERENCES RATV3.dbo.Sprints ON DELETE CASCADE ON UPDATE NO ACTION,

	CONSTRAINT FK_EmployeeIDTasks FOREIGN KEY (EmployeeID)
	REFERENCES RATV3.dbo.Employees ON DELETE NO ACTION ON UPDATE NO ACTION,
	
	CONSTRAINT FK_RoleIDTasks FOREIGN KEY (RoleID)
	REFERENCES RATV3.dbo.Roles ON DELETE NO ACTION ON UPDATE NO ACTION,
)

AlTER TABLE Employees ADD ProfileImageID UNIQUEIDENTIFIER NULL;
ALTER TABLE Employees ADD CONSTRAINT fk_ProfileImageID FOREIGN KEY (ProfileImageID) REFERENCES Files(FileID);

CREATE TABLE SkillCategories
(
	CategoryID int IDENTITY NOT NULL PRIMARY KEY,
	Description varchar(max) NULL,
	Title varchar(50) NOT NULL,
)

CREATE TABLE Skills
(
	SkillID int IDENTITY NOT NULL PRIMARY KEY,
	Title varchar(50) NOT NULL,
	Description varchar(max) NULL,
	CategoryID int NULL,
	CONSTRAINT FK_CategoryID FOREIGN KEY (CategoryID) 
	REFERENCES RATV3.dbo.SkillCategories (CategoryID) ON DELETE CASCADE ON UPDATE NO ACTION,

)


CREATE TABLE SkillLevels
(
EmployeeID int NOT NULL,
SkillID int  NOT NULL,
Level int NOT NULL,
CONSTRAINT FK_Employee1ID FOREIGN KEY (EmployeeID) 
REFERENCES RATV3.dbo.Employees (EmployeeID),
CONSTRAINT FK_SkillID FOREIGN KEY (SkillID) 
REFERENCES RATV3.dbo.Skills (SkillID),
CONSTRAINT CHECK_LevelSize
CHECK (Level BETWEEN 0 AND 100),
CONSTRAINT PK_EmployeeSkill 
PRIMARY KEY (EmployeeID, SkillID)
)



USE RATV3
CREATE TABLE Events
(
EventID int IDENTITY NOT NULL PRIMARY KEY,
StartTime datetime NOT NULL,
EndTime datetime NULL,
EventType varchar(50) NULL,
Location varchar(500) NULL,
CreatorID int NOT NULL,
IcsGuid UNIQUEIDENTIFIER NULL,

CONSTRAINT FK_CreatorID FOREIGN KEY (CreatorID) 
REFERENCES RATV3.dbo.Employees (EmployeeID) 
)

USE RATV3
CREATE TABLE Attendants
(
EmployeeID int NOT NULL,
EventID int  NOT NULL,
Status varchar(50) NULL

CONSTRAINT PK_Attendant 
PRIMARY KEY (EmployeeID, EventID)

CONSTRAINT FK_Attendant FOREIGN KEY (EmployeeID) 
REFERENCES RATV3.dbo.Employees (EmployeeID) ON DELETE CASCADE,
CONSTRAINT FK_EventID FOREIGN KEY (EventID) 
REFERENCES RATV3.dbo.Events (EventID) ON DELETE CASCADE,
)


