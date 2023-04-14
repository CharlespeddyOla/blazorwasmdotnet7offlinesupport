DECLARE @cols AS NVARCHAR(MAX),
		@query AS NVARCHAR(MAX);
		SELECT @cols = STUFF((SELECT distinct ',' + QUOTENAME(Subject)
	FROM ACDStudentsResultCognitive FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'') 
	set @query = 'SELECT ROW_NUMBER() Over (Order by FullName) AS SN, STDID, StudentNo, FullName, ' + @cols + ' INTO ACDBroadsheet from (select STDID, StudentNo, FullName, Subject, Exam
	from ACDStudentsResultCognitive) x
	pivot
	(
		sum(Exam)
		for Subject in (' + @cols + ')
	) p '
	execute(@query)