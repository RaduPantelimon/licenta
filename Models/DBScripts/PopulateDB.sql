USE RATV3
MERGE INTO Departments AS Target 
USING (VALUES 
        (1,'Mobile',15,'Solutions and Applications for mobile devices','10/05/05',100000),
		(2,'QA',45,'Quality assurance department','10/05/98',60000),
		(3,'ASP.NET',25,'Tasked with developing Web solutions in ASP.NET.','10/05/02',60000)
) 
AS Source (DepartmentID,Title,MaxSize,DeptDescription,StartDate,MonthlyExpenses) 
ON Target.DepartmentID = Source.DepartmentID 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Title,MaxSize,DeptDescription,StartDate,MonthlyExpenses) 
VALUES (Title,MaxSize,DeptDescription,StartDate,MonthlyExpenses);


/*management*/
MERGE INTO Employees AS Target 
USING (VALUES 
        
		(1,NULL,'ipo','parola',NULL,1,'Ion','M','Popescu','Ion Popescu','1060680234565',2000,1500,'10/05/13',NULL,'Manager','test@test.com','0722233344')
) 
AS Source (EmployeeID,RoleID,Account,Password,ManagerID,DepartmentID,FirstName,MiddleInitial,LastName,Title,CNP,Salary,PriorSalary,HireDate,TerminationDate,Administrator,Email,PhoneNumber) 
ON Target.EmployeeID = Source.EmployeeID 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (RoleID,Account,Password,ManagerID,DepartmentID,FirstName,MiddleInitial,LastName,Title,CNP,Salary,PriorSalary,HireDate,TerminationDate,Administrator,Email,PhoneNumber) 
VALUES (RoleID,Account,Password,ManagerID,DepartmentID,FirstName,MiddleInitial,LastName,Title,CNP,Salary,PriorSalary,HireDate,TerminationDate,Administrator,Email,PhoneNumber);


/*plebe*/
MERGE INTO Employees AS Target 
USING (VALUES 
        
		(2,NULL,'mio','parola',1,1,'Maria','I','Ionescu','Maria Ionescu','2060670296665',1200,800,'10/10/14',NULL,'Employee','test@test.com','0722233344')
) 
AS Source (EmployeeID,RoleID,Account,Password,ManagerID,DepartmentID,FirstName,MiddleInitial,LastName,Title,CNP,Salary,PriorSalary,HireDate,TerminationDate,Administrator,Email,PhoneNumber) 
ON Target.EmployeeID = Source.EmployeeID 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (RoleID,Account,Password,ManagerID,DepartmentID,FirstName,MiddleInitial,LastName,Title,CNP,Salary,PriorSalary,HireDate,TerminationDate,Administrator,Email,PhoneNumber) 
VALUES (RoleID,Account,Password,ManagerID,DepartmentID,FirstName,MiddleInitial,LastName,Title,CNP,Salary,PriorSalary,HireDate,TerminationDate,Administrator,Email,PhoneNumber);
