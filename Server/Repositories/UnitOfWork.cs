using WebAppAcademics.Server.Interfaces;
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

namespace WebAppAcademics.Server.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {        
        public UnitOfWork(IADMSchlListRepository schoolRepository, IADMSchClassListRepository classListRepository,
                            IADMSchClassGroupRepository classGroupRepository, IADMSchClassCategoryRepository classCategoryRepository,
                            IADMSchClassDisciplineRepository classDisciplieRepository, IADMSchEducationInstituteRepository previousSchoolsRepository,
                            IADMStaffDeptsRepository staffDeptsRepository, IADMStaffRepository staffRepository, 
                            IADMStaffJobTypeRepository staffJobTyprRepository, IADMStaffLocationRepository staffLocationRepsoitory,
                            IADMStaffMaritalStatusRepository staffMaritalStatusRepository, IADMStaffTitleRepository staffTitleRepository,
                            ISETSchSessionsRepository academiSession, ISETSchCalendarRepository schoolCalendar, 
                            ISETSchInformationRepository schoolInformation, ISETCountriesRepository counties,
                            ISETStatesRepository states, ISETLGARepository lgas, ISETStatusTypeRepository statusType, ISETPayeeTypeRepository payeeType,
                            ISETGenderRepository gender, ISETMedicalRepository medicalInfo, ISETReligionRepository religion,
                            IADMStudentsRepository studentRepository, IADMStudentTypeRepository studentTypeRepository, 
                            IADMStudentMEDHistoryRepository studentMedicalRepository, IADMStudentExitRepository studentExitTypeRepository,
                            IADMSchClubRepository clubRepository, IADMSchClubRoleRepository clubRoleRepository, IADMSchParentsRepository parentRepository,
                            ISETMonthListRepository monthList, IFINBankAcctTypeRepository bankAcctType, IFINBanksRepository banks,
                            IACDSubjectsDeptRepository subjectsDepartment, IACDSubjectsClassRepository subjectsClassification, 
                            IACDSubjectsRepository subjects, IACDStudentSubjectsAllocationRepository studentSubjectsAllocation,
                            IACDTeacherSubjectsAllocationRepsitory teacherSubjectsAllocation, 
                            IACDCognitiveMarkEntryRepository cognitivemarkEntry, IACDOtherMarksEntryRepository otherMarksEntry,
                            IACDGradeSettingsRepository gradeSettings, IACDMockGradeSettingRepository mockGradeSettings,
                            IACDCheckPointGradeSettingRepository checkPointGradeSettings,
                            IACDIGCSEGradeSettingsRepository igcseGradeSettings, IACDMarkSettingsRepository markSettings,
                            IACDRatingSettingsRepository ratingSettings, IACDRatingOptionSettingsRepository ratingOptionSettings,
                            IACDRatingTextSettingsRepository ratingTextSettings, IACDOtherSettingsRepository otherSettings,
                            IACDOtherGradeSettingsRepository otherGradeSettings,
                            IACDResultTypeSettingsRepository resultTypeSettings, 
                            IACDResultHeaderFooterSettingsRepository resultHeaderFooterSettings,
                            IACDCBTExamTypeRepository cbtExamType, IACDCBTExamsRepository cbtExams,
                            IACDCBTExamQuestionTypeRepository cbtExamQuestionType, 
                            IACDCBTExamQuestionsRepository cbtExamQuestions, IACDCBTExamAnswersRepository cbtExamAnswers,
                            IACDCBTExamStudentAnswersRepository cbtStudentExamAnswers, 
                            IACDCBTExamStudentScoreRepository cbtStudentScore, IACDCBTExamLatexRepository cbtExamLatex,
                            IACDResultsMidTermCommentsRepository midTermResultsComments,
                            IACDResultsTermEndCommentsRepository termEndResultsComments,
                            IACDResultsCheckPointIGCSECommentsRepository checkpointIGCSEComments,
                            IACDResultsCognitiveRepository cognitiveResults, IACDResultsOtherMarksRepository otherMarksResults,
                            IACDResultsBroadSheetRepository broadsheet, ISETRoleRepository staffAccessRoles,
                            ICBTConnectionInfoRepository cbtConnectionInfo, ISETReportsRepository setReports,
                            ICBTExamTakenFlagsRepository examTakenFlags, IACDFlagsRepository acdFlags,
                            ISETAppLicenseRepository appLicense)
        {
            #region [Administration - School]
            ADMSchlList = schoolRepository;
            ADMSchClassList = classListRepository;
            ADMSchClassGroup = classGroupRepository;
            ADMSchClassCategory = classCategoryRepository;
            ADMSchClassDiscipline = classDisciplieRepository;
            ADMSchEducationInstitute = previousSchoolsRepository;
            #endregion

            #region [Administration - Staff]
            ADMEmployee = staffRepository;
            ADMEmployeeDepts = staffDeptsRepository;
            ADMEmployeeJobType = staffJobTyprRepository;
            ADMEmployeeLocation = staffLocationRepsoitory;
            ADMEmployeeMaritalStatus = staffMaritalStatusRepository;
            ADMEmployeeTitle = staffTitleRepository;
            #endregion

            #region [Administration - Staff]
            ADMStudents = studentRepository;
            ADMStudentType = studentTypeRepository;
            ADMStudentMEDHistory = studentMedicalRepository;
            ADMStudentExit = studentExitTypeRepository;
            ADMSchClub = clubRepository;
            ADMSchClubRole = clubRoleRepository;
            ADMSchParents = parentRepository;
            #endregion

            #region [Settings]
            SETSchSessions = academiSession;
            SETSchCalendar = schoolCalendar;
            SETSchInformation = schoolInformation;
            SETCountries = counties;
            SETStates = states;
            SETLGA = lgas;
            SETStatusType = statusType;
            SETPayeeType = payeeType;
            SETGender = gender;
            SETMedical = medicalInfo;
            SETReligion = religion;
            SETMonthList = monthList;
            StaffAccessRoles = staffAccessRoles;
            SETReports = setReports;
            AppLicense = appLicense;
            #endregion

            #region [Financials - Banks]
            FINBankAcctType = bankAcctType;
            FINBankDetails = banks;
            #endregion

            #region [Academics - Subjects]
            SubjectsDepartment = subjectsDepartment;
            SubjectsClassification = subjectsClassification;
            Subjects = subjects;
            StudentSubjectsAllocation = studentSubjectsAllocation;
            TeacherSubjectsAllocation = teacherSubjectsAllocation;
            #endregion

            #region [Academics - Mark Entry]
            CognitiveMarkEntry = cognitivemarkEntry;
            OtherMarksEntry = otherMarksEntry;
            GradeSettings = gradeSettings;
            MockGradeSettings = mockGradeSettings;
            CheckPointGradeSettings = checkPointGradeSettings;
            IGCSEGradeSettings = igcseGradeSettings;
            MarkSettings = markSettings;
            RatingSettings = ratingSettings;
            RatingOptionSettings = ratingOptionSettings;
            RatingTextSettings = ratingTextSettings;
            OtherSettings = otherSettings;
            OtherGradeSettings = otherGradeSettings;
            ResultTypeSettings = resultTypeSettings;
            ResultHeaderFooterSettings = resultHeaderFooterSettings;
            AcademicsFlags = acdFlags;
            #endregion

            #region [Academics - CBT Exam]
            CBTExamType = cbtExamType;
            CBTExams = cbtExams;
            CBTExamQuestionType = cbtExamQuestionType;
            CBTExamQuestions = cbtExamQuestions;
            CBTExamAnswers = cbtExamAnswers;
            CBTExamStudentAnswers = cbtStudentExamAnswers;
            CBTExamStudentScore = cbtStudentScore;
            CBTExamLatex = cbtExamLatex;
            CBTConnectionInfo = cbtConnectionInfo;
            CBTExamTaken = examTakenFlags;
            #endregion

            #region [Academics - Results Comments Entry]
            MidTermComments = midTermResultsComments;
            TermEndComments = termEndResultsComments;
            CheckPointIGCSEComments = checkpointIGCSEComments;
            #endregion

            #region [Academics - Results]
            CognitiveResults = cognitiveResults;
            OtherMarksResults = otherMarksResults;
            BroadSheet = broadsheet;
            #endregion
        }

        #region [Administration - School]
    public IADMSchlListRepository ADMSchlList { get; }
        public IADMSchClassListRepository ADMSchClassList { get; }
        public IADMSchClassGroupRepository ADMSchClassGroup { get; }
        public IADMSchClassCategoryRepository ADMSchClassCategory { get; }
        public IADMSchClassDisciplineRepository ADMSchClassDiscipline { get; }
        public IADMSchEducationInstituteRepository ADMSchEducationInstitute { get; }
        #endregion

        #region [Administration - Staff]
        public IADMStaffRepository ADMEmployee { get; }
        public IADMStaffDeptsRepository ADMEmployeeDepts { get; }
        public IADMStaffJobTypeRepository ADMEmployeeJobType { get; }
        public IADMStaffLocationRepository ADMEmployeeLocation  { get; }
        public IADMStaffMaritalStatusRepository ADMEmployeeMaritalStatus { get; }
        public IADMStaffTitleRepository ADMEmployeeTitle { get; }
        #endregion

        #region [Administration - Students]
        public IADMStudentsRepository ADMStudents { get; }
        public IADMStudentTypeRepository ADMStudentType { get; }
        public IADMStudentMEDHistoryRepository ADMStudentMEDHistory { get; }
        public IADMStudentExitRepository ADMStudentExit { get; }
        public IADMSchClubRepository ADMSchClub { get; }
        public IADMSchClubRoleRepository ADMSchClubRole { get; }
        public IADMSchParentsRepository ADMSchParents { get; }
        #endregion

        #region [Settings]
        public ISETSchSessionsRepository SETSchSessions { get; }
        public ISETSchCalendarRepository SETSchCalendar { get; }
        public ISETSchInformationRepository SETSchInformation { get; }
        public ISETCountriesRepository SETCountries { get; }
        public ISETStatesRepository SETStates { get; }
        public ISETLGARepository SETLGA { get; }
        public ISETStatusTypeRepository SETStatusType { get; }
        public ISETPayeeTypeRepository SETPayeeType { get; }
        public ISETGenderRepository SETGender { get; }
        public ISETMedicalRepository SETMedical { get; }
        public ISETReligionRepository SETReligion { get; }
        public ISETMonthListRepository SETMonthList { get; }
        public ISETRoleRepository StaffAccessRoles { get; }
        public ISETReportsRepository SETReports { get; }
        public ISETAppLicenseRepository AppLicense { get; }
        #endregion

        #region [Financials - Banks]
        public IFINBankAcctTypeRepository FINBankAcctType { get; }
        public IFINBanksRepository FINBankDetails { get; }
        #endregion

        #region [Academics - Subjects]
        public IACDSubjectsDeptRepository SubjectsDepartment { get; }
        public IACDSubjectsClassRepository SubjectsClassification { get; }
        public IACDSubjectsRepository Subjects { get; }
        public IACDStudentSubjectsAllocationRepository StudentSubjectsAllocation { get; }
        public IACDTeacherSubjectsAllocationRepsitory TeacherSubjectsAllocation { get; }
        #endregion

        #region [Academics - Mark Entry]
        public IACDCognitiveMarkEntryRepository CognitiveMarkEntry { get; }
        public IACDOtherMarksEntryRepository OtherMarksEntry { get; }
        public IACDGradeSettingsRepository GradeSettings { get; }
        public IACDMockGradeSettingRepository MockGradeSettings { get; }
        public IACDCheckPointGradeSettingRepository CheckPointGradeSettings { get; }
        public IACDIGCSEGradeSettingsRepository IGCSEGradeSettings { get; }
        public IACDMarkSettingsRepository MarkSettings { get; }
        public IACDRatingSettingsRepository RatingSettings { get; }
        public IACDRatingOptionSettingsRepository RatingOptionSettings { get; }
        public IACDRatingTextSettingsRepository RatingTextSettings { get; }
        public IACDOtherSettingsRepository OtherSettings { get; }
        public IACDOtherGradeSettingsRepository OtherGradeSettings { get; }
        public IACDResultTypeSettingsRepository ResultTypeSettings { get; }
        public IACDResultHeaderFooterSettingsRepository ResultHeaderFooterSettings { get; }
        public IACDFlagsRepository AcademicsFlags { get; }
        #endregion

        #region [Academics - Results Comments Entry]
        public IACDResultsMidTermCommentsRepository MidTermComments { get; }
        public IACDResultsTermEndCommentsRepository TermEndComments { get; }
        public IACDResultsCheckPointIGCSECommentsRepository CheckPointIGCSEComments { get; }
        #endregion

        #region [Academics - Results]
        public IACDResultsCognitiveRepository CognitiveResults { get; }
        public IACDResultsOtherMarksRepository OtherMarksResults { get; }
        public IACDResultsBroadSheetRepository BroadSheet { get;  }
        #endregion

        #region [Academics - CBT Exam]
        public IACDCBTExamTypeRepository CBTExamType { get; }
        public IACDCBTExamsRepository CBTExams { get; }
        public IACDCBTExamQuestionTypeRepository CBTExamQuestionType { get; }
        public IACDCBTExamQuestionsRepository CBTExamQuestions { get; }
        public IACDCBTExamAnswersRepository CBTExamAnswers { get; }
        public IACDCBTExamStudentAnswersRepository CBTExamStudentAnswers { get; }
        public IACDCBTExamStudentScoreRepository CBTExamStudentScore { get; }
        public IACDCBTExamLatexRepository CBTExamLatex { get; }
        public ICBTConnectionInfoRepository CBTConnectionInfo { get; }
        public ICBTExamTakenFlagsRepository CBTExamTaken { get; }
        #endregion


    }
}
