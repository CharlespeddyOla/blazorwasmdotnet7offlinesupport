using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.CBT;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Financials.Banks;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Services
{
    public static class ServicesRegistration
    {
        public static void AddAPIServices(this IServiceCollection services)
        {
            #region [Administration - School]
            services.AddScoped<IAPIServices<ADMSchlList>, APIServices<ADMSchlList>>();
            services.AddScoped<IAPIServices<ADMSchClassList>, APIServices<ADMSchClassList>>();
            services.AddScoped<IAPIServices<ADMSchClassGroup>, APIServices<ADMSchClassGroup>>();
            services.AddScoped<IAPIServices<ADMSchClassCategory>, APIServices<ADMSchClassCategory>>();
            services.AddScoped<IAPIServices<ADMSchClassDiscipline>, APIServices<ADMSchClassDiscipline>>();
            #endregion

            #region [Administration - Students]
            services.AddScoped<IAPIServices<ADMStudentType>, APIServices<ADMStudentType>>();
            services.AddScoped<IAPIServices<ADMStudents>, APIServices<ADMStudents>>();
            services.AddScoped<IAPIServices<ADMSchParents>, APIServices<ADMSchParents>>();
            #endregion

            #region [Administration - Staff]
            services.AddScoped<IAPIServices<ADMEmployeeDepts>, APIServices<ADMEmployeeDepts>>();
            services.AddScoped<IAPIServices<ADMEmployeeJobType>, APIServices<ADMEmployeeJobType>>();
            services.AddScoped<IAPIServices<ADMEmployeeLocation>, APIServices<ADMEmployeeLocation>>();
            services.AddScoped<IAPIServices<ADMEmployee>, APIServices<ADMEmployee>>();
            services.AddScoped<IAPIServices<ADMEmployeeTitle>, APIServices<ADMEmployeeTitle>>();
            services.AddScoped<IAPIServices<ADMEmployeeMaritalStatus>, APIServices<ADMEmployeeMaritalStatus>>();
            #endregion

            #region [Settings - Academic Session]
            services.AddScoped<IAPIServices<SETSchSessions>, APIServices<SETSchSessions>>();
            services.AddScoped<IAPIServices<SETSchInformation>, APIServices<SETSchInformation>>();
            services.AddScoped<IAPIServices<SETSchCalendar>, APIServices<SETSchCalendar>>();
            services.AddScoped<IAPIServices<SETMonthList>, APIServices<SETMonthList>>();
            services.AddScoped<IAPIServices<SETCountries>, APIServices<SETCountries>>();
            services.AddScoped<IAPIServices<SETStates>, APIServices<SETStates>>();
            services.AddScoped<IAPIServices<SETLGA>, APIServices<SETLGA>>();
            services.AddScoped<IAPIServices<SETStatusType>, APIServices<SETStatusType>>();
            services.AddScoped<IAPIServices<SETGender>, APIServices<SETGender>>();
            services.AddScoped<IAPIServices<ADMSchClassDiscipline>, APIServices<ADMSchClassDiscipline>>();
            services.AddScoped<IAPIServices<SETReligion>, APIServices<SETReligion>>();
            services.AddScoped<IAPIServices<ADMSchClub>, APIServices<ADMSchClub>>();
            services.AddScoped<IAPIServices<ADMSchClubRole>, APIServices<ADMSchClubRole>>();
            services.AddScoped<IAPIServices<SETPayeeType>, APIServices<SETPayeeType>>();
            services.AddScoped<IAPIServices<SETMedical>, APIServices<SETMedical>>();
            services.AddScoped<IAPIServices<ADMStudentMEDHistory>, APIServices<ADMStudentMEDHistory>>();
            services.AddScoped<IAPIServices<ADMSchEducationInstitute>, APIServices<ADMSchEducationInstitute>>();
            services.AddScoped<IAPIServices<ADMStudentExit>, APIServices<ADMStudentExit>>();
            services.AddScoped<IAPIServices<SETRole>, APIServices<SETRole>>();
            #endregion

            #region [Financials - Bank]
            services.AddScoped<IAPIServices<FINBankDetails>, APIServices<FINBankDetails>>();
            #endregion

            #region [Academics - Subjects]
            services.AddScoped<IAPIServices<ACDSbjDept>, APIServices<ACDSbjDept>>();
            services.AddScoped<IAPIServices<ACDSbjClassification>, APIServices<ACDSbjClassification>>();
            services.AddScoped<IAPIServices<ACDSubjects>, APIServices<ACDSubjects>>();
            services.AddScoped<IAPIServices<ACDSbjAllocationStudents>, APIServices<ACDSbjAllocationStudents>>();
            services.AddScoped<IAPIServices<ACDSbjAllocationTeachers>, APIServices<ACDSbjAllocationTeachers>>();
            services.AddScoped<IAPIServices<CombinesSubjects>, APIServices<CombinesSubjects>>();
            #endregion

            #region [Academics - Exam Mark Settings]
            services.AddScoped<IAPIServices<ACDSettingsGrade>, APIServices<ACDSettingsGrade>>();
            services.AddScoped<IAPIServices<ACDSettingsGradeMock>, APIServices<ACDSettingsGradeMock>>();
            services.AddScoped<IAPIServices<ACDSettingsGradeOthers>, APIServices<ACDSettingsGradeOthers>>();
            services.AddScoped<IAPIServices<ACDSettingsGradeCheckPoint>, APIServices<ACDSettingsGradeCheckPoint>>();
            services.AddScoped<IAPIServices<ACDSettingsGradeIGCSE>, APIServices<ACDSettingsGradeIGCSE>>();
            services.AddScoped<IAPIServices<ACDSettingsMarks>, APIServices<ACDSettingsMarks>>();
            services.AddScoped<IAPIServices<ACDSettingsRating>, APIServices<ACDSettingsRating>>();
            services.AddScoped<IAPIServices<ACDSettingsRatingOptions>, APIServices<ACDSettingsRatingOptions>>();
            services.AddScoped<IAPIServices<ACDSettingsRatingText>, APIServices<ACDSettingsRatingText>>();
            services.AddScoped<IAPIServices<ACDReportType>, APIServices<ACDReportType>>();
            services.AddScoped<IAPIServices<ACDReportFooter>, APIServices<ACDReportFooter>>();
            services.AddScoped<IAPIServices<ACDSettingsOthers>, APIServices<ACDSettingsOthers>>();
            services.AddScoped<IAPIServices<ACDFlags>, APIServices<ACDFlags>>();
            services.AddScoped<IAPIServices<CBTExamTakenFlags>, APIServices<CBTExamTakenFlags>>();
            #endregion

            #region [Academics - Exam Mark Entry]
            services.AddScoped<IAPIServices<ACDStudentsMarksCognitive>, APIServices<ACDStudentsMarksCognitive>>();
            services.AddScoped<IAPIServices<ACDStudentsMarksAssessment>, APIServices<ACDStudentsMarksAssessment>>();
            #endregion

            #region [Academics - Results]
            services.AddScoped<IAPIServices<ACDStudentsResultCognitive>, APIServices<ACDStudentsResultCognitive>>();
            services.AddScoped<IAPIServices<ACDStudentsResultAssessmentBool>, APIServices<ACDStudentsResultAssessmentBool>>();
            services.AddScoped<IAPIServices<ACDBroadSheet>, APIServices<ACDBroadSheet>>();
            services.AddScoped<IAPIServices<SETReports>, APIServices<SETReports>>();
            services.AddScoped<IAPIServices<ReportModel>, APIServices<ReportModel>>();
            services.AddScoped<IAPIServices<string>, APIServices<string>>();
            services.AddScoped<IAPIServices<dynamic>, APIServices<dynamic>>();
            services.AddScoped<IAPIServices<bool>, APIServices<bool>>();
            services.AddScoped<IAPIServices<byte[]>, APIServices<byte[]>>();
            #endregion

            #region [Academics - Results Comments]
            services.AddScoped<IAPIServices<ACDReportCommentMidTerm>, APIServices<ACDReportCommentMidTerm>>();
            services.AddScoped<IAPIServices<ACDReportCommentsTerminal>, APIServices<ACDReportCommentsTerminal>>();
            services.AddScoped<IAPIServices<ACDReportCommentCheckPointIGCSE>, APIServices<ACDReportCommentCheckPointIGCSE>>();
            services.AddScoped<IAPIServices<ACDSettingsAutoComments>, APIServices<ACDSettingsAutoComments>>();
            #endregion

            #region [Academics - CBT]
            services.AddScoped<IAPIServices<CBTExamType>, APIServices<CBTExamType>>();
            services.AddScoped<IAPIServices<CBTExams>, APIServices<CBTExams>>();
            services.AddScoped<IAPIServices<CBTQuestionType>, APIServices<CBTQuestionType>>();
            services.AddScoped<IAPIServices<CBTQuestions>, APIServices<CBTQuestions>>();
            services.AddScoped<IAPIServices<CBTAnswers>, APIServices<CBTAnswers>>();
            services.AddScoped<IAPIServices<CBTLatex>, APIServices<CBTLatex>>();
            services.AddScoped<IAPIServices<CBTStudentAnswers>, APIServices<CBTStudentAnswers>>();
            services.AddScoped<IAPIServices<CBTStudentScores>, APIServices<CBTStudentScores>>(); 
            services.AddScoped<IAPIServices<CBTConnectionInfo>, APIServices<CBTConnectionInfo>>();
            #endregion
                      
            services.AddScoped<IAPIServices<FileChunkDTO>, APIServices<FileChunkDTO>>();
            services.AddScoped<IAPIServices<SETAppLicense>, APIServices<SETAppLicense>>();
        }
    }
}
