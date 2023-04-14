DECLARE @cols   AS NVARCHAR(MAX),   -- for pivot
        @cols2  AS NVARCHAR(MAX),   -- for select
        @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(A.[Subject]) 
            FROM ACDStudentsResultCognitive A
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

-- this is for the SELECT
SET @cols2 = STUFF((SELECT distinct ',' + 'ISNULL(' + QUOTENAME(A.[Subject]) + ', 0) ' + QUOTENAME(A.[Subject])
            FROM ACDStudentsResultCognitive A 
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT ROW_NUMBER() Over (Order by FullName) AS SN, STDID, StudentNo, FullName, ' + @cols2 + ' INTO ACDBroadsheet from (select STDID, StudentNo, FullName, Subject, CA3
	from ACDStudentsResultCognitive) x
	pivot
	(
		sum(CA3)
		for Subject in (' + @cols + ')
	) p '
	execute(@query)
