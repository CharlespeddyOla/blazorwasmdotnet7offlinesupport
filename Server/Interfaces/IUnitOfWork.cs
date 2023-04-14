using WebAppAcademics.Server.Interfaces.Academics.CBT;
using WebAppAcademics.Server.Interfaces.Academics.Exam;
using WebAppAcademics.Server.Interfaces.Academics.Subjects;
using WebAppAcademics.Server.Interfaces.Financials.Banks;
using WebAppAcademics.Server.Interfaces.School;
using WebAppAcademics.Server.Interfaces.Settings;
using WebAppAcademics.Server.Interfaces.Staff;
using WebAppAcademics.Server.Interfaces.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAcademics.Server.Interfaces
{
    public interface IUnitOfWork
    {
        #region [Administration - School]
        IADMSchlListRepository ADMSchlList { get; }
        IADMSchClassListRepository ADMSchClassList { get; }
        IADMSchClassGroupRepository ADMSchClassGroup { get; }
        IADMSchClassCategoryRepository ADMSchClassCategory { get; }
        IADMSchClassDisciplineRepository ADMSchClassDiscipline { get; }
        IADMSchEducationInstituteRepository ADMSchEducationInstitute { get; }
        #endregion

        #region [Administration - Staff]       
        IADMStaffRepository ADMEmployee { get; }
        IADMStaffDeptsRepository ADMEmployeeDepts { get; }
        IADMStaffJobTypeRepository ADMEmployeeJobType { get; }
        IADMStaffLocationRepository ADMEmployeeLocation { get; }
        IADMStaffMaritalStatusRepository ADMEmployeeMaritalStatus { get; }
        IADMStaffTitleRepository ADMEmployeeTitle { get; }
        #endregion

        #region [Administration - Students] 
        IADMStudentsRepository ADMStudents { get; }
        IADMStudentTypeRepository ADMStudentType { get; }
        IADMStudentMEDHistoryRepository ADMStudentMEDHistory { get; }
        IADMStudentExitRepository ADMStudentExit { get; }
        IADMSchClubRepository ADMSchClub { get; }
        IADMSchClubRoleRepository ADMSchClubRole { get; }
        IADMSchParentsRepository ADMSchParents { get; }
        #endregion

        #region [Settings]
        ISETSchSessionsRepository SETSchSessions { get; }
        ISETSchCalendarRepository SETSchCalendar { get; }
        ISETSchInformationRepository SETSchInformation { get; }
        ISETCountriesRepository SETCountries { get; }
        ISETStatesRepository SETStates { get; }
        ISETLGARepository SETLGA { get; }
        ISETStatusTypeRepository SETStatusType { get; }
        ISETPayeeTypeRepository SETPayeeType { get; }
        ISETGenderRepository SETGender { get; }
        ISETMedicalRepository SETMedical { get; }
        ISETReligionRepository SETReligion { get; }
        ISETMonthListRepository SETMonthList { get; }
        ISETRoleRepository StaffAccessRoles { get; }
        ISETReportsRepository SETReports { get; }
        ISETAppLicenseRepository AppLicense { get; }
        #endregion

        #region [Financials - Banks]
        IFINBankAcctTypeRepository FINBankAcctType { get; }
        IFINBanksRepository FINBankDetails { get; }
        #endregion

        #region [Academics - Subjects]
        IACDSubjectsDeptRepository SubjectsDepartment { get; }
        IACDSubjectsClassRepository SubjectsClassification { get; }
        IACDSubjectsRepository Subjects { get; }
        IACDStudentSubjectsAllocationRepository StudentSubjectsAllocation { get; }
        IACDTeacherSubjectsAllocationRepsitory TeacherSubjectsAllocation { get; }
        #endregion

        #region [Academics - Mark Entry]
        IACDCognitiveMarkEntryRepository CognitiveMarkEntry { get; }
        IACDOtherMarksEntryRepository OtherMarksEntry { get; }
        IACDGradeSettingsRepository GradeSettings { get; }
        IACDMockGradeSettingRepository MockGradeSettings { get; }
        IACDCheckPointGradeSettingRepository CheckPointGradeSettings { get; }
        IACDIGCSEGradeSettingsRepository IGCSEGradeSettings { get; }
        IACDMarkSettingsRepository MarkSettings { get; }
        IACDRatingSettingsRepository RatingSettings { get; }
        IACDRatingOptionSettingsRepository RatingOptionSettings { get; }
        IACDRatingTextSettingsRepository RatingTextSettings { get; }
        IACDOtherSettingsRepository OtherSettings { get; }
        IACDOtherGradeSettingsRepository OtherGradeSettings { get; }
        IACDResultTypeSettingsRepository ResultTypeSettings { get; }
        IACDResultHeaderFooterSettingsRepository ResultHeaderFooterSettings { get; }
        IACDFlagsRepository AcademicsFlags { get; }
        #endregion

        #region [Academics - Results Comments Entry]
        IACDResultsMidTermCommentsRepository MidTermComments { get; }
        IACDResultsTermEndCommentsRepository TermEndComments { get; }
        IACDResultsCheckPointIGCSECommentsRepository CheckPointIGCSEComments { get; }
        #endregion

        #region [Academics - Results]
        IACDResultsCognitiveRepository CognitiveResults { get; }
        IACDResultsOtherMarksRepository OtherMarksResults { get; }
        IACDResultsBroadSheetRepository BroadSheet { get; }
        #endregion

        #region [Academics - CBT Exam]
        IACDCBTExamTypeRepository CBTExamType { get; }
        IACDCBTExamsRepository CBTExams { get; }
        IACDCBTExamQuestionTypeRepository CBTExamQuestionType { get; }
        IACDCBTExamQuestionsRepository CBTExamQuestions { get; }
        IACDCBTExamAnswersRepository CBTExamAnswers { get; }
        IACDCBTExamStudentAnswersRepository CBTExamStudentAnswers { get; }
        IACDCBTExamStudentScoreRepository CBTExamStudentScore { get; }
        IACDCBTExamLatexRepository CBTExamLatex { get; }
        ICBTConnectionInfoRepository CBTConnectionInfo { get; }
        ICBTExamTakenFlagsRepository CBTExamTaken { get; }
        #endregion
    }
}
