namespace WebAppAcademics.Client.OfflineServices
{
    public static class BlazorDBServices
    {
        public static void AddBlazorDBServices(this IServiceCollection services)
        {
            services.AddBlazorDB(options =>
            {
                options.Name = "SchoolMagnet";
                options.Version = 1;               
                options.StoreSchemas = new List<StoreSchema>()
                {
                    new StoreSchema()
                    {
                        Name = "ADMEmployee",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ADMEmployee{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "StaffID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ADMEmployee{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "SETSchSessions",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"SETSchSessions{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"SETSchSessions{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ADMSchlList",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ADMSchlList{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }                        
                    },
                    new StoreSchema()
                    {
                        Name = $"ADMSchlList{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ADMSchClassList",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ADMSchClassList{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ADMSchClassList{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDStudentsMarksCognitive",      // Name of entity
                        PrimaryKey = "StudentMarkID",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "StudentMarkID" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDStudentsMarksCognitive{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "StudentMarkID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "StudentMarkID" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDStudentsMarksCognitive{Globals.KeysSuffix}",
                        PrimaryKey = "StudentMarkID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "StudentMarkID" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDSubjects",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSubjects{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSubjects{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDSbjClassification",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSbjClassification{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSbjClassification{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDSbjAllocationTeachers",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSbjAllocationTeachers{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSbjAllocationTeachers{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDReportType",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDReportType{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDReportType{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDSettingsMarks",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsMarks{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsMarks{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "CBTExams",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"CBTExams{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"CBTExams{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ADMStudents",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ADMStudents{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ADMStudents{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDSbjAllocationStudents",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSbjAllocationStudents{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSbjAllocationStudents{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "CBTStudentScores",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"CBTStudentScores{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"CBTStudentScores{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDSettingsRating",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsRating{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsRating{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDSettingsRatingOptions",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsRatingOptions{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsRatingOptions{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDSettingsRatingText",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsRatingText{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsRatingText{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDStudentsMarksAssessment",      // Name of entity
                        PrimaryKey = "StudentMarkID",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "StudentMarkID" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDStudentsMarksAssessment{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "StudentMarkID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "StudentMarkID" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDStudentsMarksAssessment{Globals.KeysSuffix}",
                        PrimaryKey = "StudentMarkID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "StudentMarkID" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDReportCommentMidTerm",      // Name of entity
                        PrimaryKey = "CommentID",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "CommentID" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDReportCommentMidTerm{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "CommentID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "CommentID" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDReportCommentMidTerm{Globals.KeysSuffix}",
                        PrimaryKey = "CommentID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "CommentID" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDReportCommentsTerminal",      // Name of entity
                        PrimaryKey = "CommentID",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "CommentID" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDReportCommentsTerminal{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "CommentID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "CommentID" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDReportCommentsTerminal{Globals.KeysSuffix}",
                        PrimaryKey = "CommentID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "CommentID" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDReportCommentCheckPointIGCSE",      // Name of entity
                        PrimaryKey = "CommentID",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "CommentID" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDReportCommentCheckPointIGCSE{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "CommentID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "CommentID" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDReportCommentCheckPointIGCSE{Globals.KeysSuffix}",
                        PrimaryKey = "CommentID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "CommentID" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDSettingsGradeMock",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsGradeMock{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "StaffID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsGradeMock{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDSettingsGradeOthers",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsGradeOthers{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "StaffID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsGradeOthers{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDSettingsGradeCheckPoint",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsGradeCheckPoint{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "StaffID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsGradeCheckPoint{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDSettingsGradeIGCSE",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsGradeIGCSE{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "StaffID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsGradeIGCSE{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = "ACDSettingsGrade",      // Name of entity
                        PrimaryKey = "Id",      // Primary Key of entity
                        PrimaryKeyAuto = true,  // Whether or not the Primary key is generated
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsGrade{Globals.LocalTransactionsSuffix}",
                        PrimaryKey = "StaffID",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    },
                    new StoreSchema()
                    {
                        Name = $"ACDSettingsGrade{Globals.KeysSuffix}",
                        PrimaryKey = "Id",
                        PrimaryKeyAuto = true,
                        Indexes = new List<string> { "Id" }
                    }
                };
            });
        }
    }
}
