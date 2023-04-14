using Microsoft.Extensions.DependencyInjection;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Server.Interfaces.Academics.CBT;
using WebAppAcademics.Server.Interfaces.Academics.Exam;
using WebAppAcademics.Server.Interfaces.Academics.Subjects;
using WebAppAcademics.Server.Interfaces.Financials.Banks;
using WebAppAcademics.Server.Interfaces.School;
using WebAppAcademics.Server.Interfaces.Settings;
using WebAppAcademics.Server.Interfaces.Staff;
using WebAppAcademics.Server.Interfaces.Students;
using WebAppAcademics.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAcademics.Server
{
    public static class ServicesRegistration
    {
        public static void AddDataAccess(this IServiceCollection services)
        {            
            #region [Administration - School]
            services.AddTransient<IADMSchlListRepository, ADMSchlListRepository>();
            services.AddTransient<IADMSchClassListRepository, ADMSchClassListRepository>();
            services.AddTransient<IADMSchClassGroupRepository, ADMSchClassGroupRepository>();
            services.AddTransient<IADMSchClassCategoryRepository, ADMSchClassCategoryRepository>();
            services.AddTransient<IADMSchClassDisciplineRepository, ADMSchClassDisciplineRepository>();
            services.AddTransient<IADMSchEducationInstituteRepository, ADMSchEducationInstituteRepository>();
            #endregion

            #region [Administration - Staff]
            services.AddTransient<IADMStaffRepository, ADMStaffRepository>();
            services.AddTransient<IADMStaffDeptsRepository, ADMStaffDeptsRepository>();
            services.AddTransient<IADMStaffJobTypeRepository, ADMStaffJobTypeRepository>();
            services.AddTransient<IADMStaffLocationRepository, StaffLocationRepository>();
            services.AddTransient<IADMStaffMaritalStatusRepository, StaffMaritalStatusRepository>();
            services.AddTransient<IADMStaffTitleRepository, ADMStaffTitleRepository>();
            #endregion

            #region [Administration - Students]
            services.AddTransient<IADMStudentsRepository, ADMStudentsRepository>();
            services.AddTransient<IADMStudentTypeRepository, ADMStudentTypeRepository>();
            services.AddTransient<IADMStudentMEDHistoryRepository, ADMStudentMEDHistoryRepository>();
            services.AddTransient<IADMStudentExitRepository, ADMStudentExitRepository>();
            services.AddTransient<IADMSchClubRepository, ADMSchClubRepository>();
            services.AddTransient<IADMSchClubRoleRepository, ADMSchClubRoleRepository>();
            services.AddTransient<IADMSchParentsRepository, ADMSchParentsRepository>();
            #endregion

            #region [Settings]
            services.AddTransient<ISETSchSessionsRepository, SETSchSessionsRepository>();
            services.AddTransient<ISETSchCalendarRepository, SETSchCalendarRepository>();
            services.AddTransient<ISETSchInformationRepository, SETSchInformationRepository>();
            services.AddTransient<ISETCountriesRepository, SETCountriesRepository>();
            services.AddTransient<ISETStatesRepository, SETStatesRepository>();
            services.AddTransient<ISETLGARepository, SETLGARepository>();
            services.AddTransient<ISETStatusTypeRepository, SETStatusTypeRepository>();
            services.AddTransient<ISETPayeeTypeRepository, SETPayeeTypeRepository>();
            services.AddTransient<ISETGenderRepository, SETGenderRepository>();
            services.AddTransient<ISETMedicalRepository, SETMedicalRepository>();
            services.AddTransient<ISETReligionRepository, SETReligionRepository>();
            services.AddTransient<ISETMonthListRepository, SETMonthListRepository>();
            services.AddTransient<ISETRoleRepository, SETRoleRepository>();
            services.AddTransient<ISETReportsRepository, SETReportsRepository>();
            services.AddTransient<ISETAppLicenseRepository, SETAppLicenseRepository>();
            #endregion

            #region [Financials - Banks]
            services.AddTransient<IFINBankAcctTypeRepository, FINBankAcctTypeRepository>();
            services.AddTransient<IFINBanksRepository, FINBanksRepository>();
            #endregion

            #region [Academics - Subjects]
            services.AddTransient<IACDSubjectsClassRepository, ACDSubjectsClassRepository>();
            services.AddTransient<IACDSubjectsDeptRepository, ACDSubjectsDeptRepository>();           
            services.AddTransient<IACDSubjectsRepository, ACDSubjectsRepository>();
            services.AddTransient<IACDStudentSubjectsAllocationRepository, ACDStudentSubjectsAllocationRepository>();
            services.AddTransient<IACDTeacherSubjectsAllocationRepsitory, ACDTeacherSubjectsAllocationRepsitory>();
            #endregion

            #region [Academics - Mark Entry]
            services.AddTransient<IACDCognitiveMarkEntryRepository, ACDCognitiveMarkEntryRepository>();
            services.AddTransient<IACDOtherMarksEntryRepository, ACDOtherMarksEntryRepository>();
            services.AddTransient<IACDGradeSettingsRepository, ACDGradeSettingsRepository>();
            services.AddTransient<IACDMockGradeSettingRepository, ACDMockGradeSettingRepository>();
            services.AddTransient<IACDCheckPointGradeSettingRepository, ACDCheckPointGradeSettingRepository>();
            services.AddTransient<IACDIGCSEGradeSettingsRepository, ACDIGCSEGradeSettingsRepository>();
            services.AddTransient<IACDMarkSettingsRepository, ACDMarkSettingsRepository>();
            services.AddTransient<IACDRatingSettingsRepository, ACDRatingSettingsRepository>();
            services.AddTransient<IACDRatingOptionSettingsRepository, ACDRatingOptionSettingsRepository>();
            services.AddTransient<IACDRatingTextSettingsRepository, ACDRatingTextSettingsRepository>();
            services.AddTransient<IACDOtherSettingsRepository, ACDOtherSettingsRepository>();
            services.AddTransient<IACDOtherGradeSettingsRepository, ACDOtherGradeSettingsRepository>();
            services.AddTransient<IACDResultTypeSettingsRepository, ACDResultTypeSettingsRepository>();
            services.AddTransient<IACDResultHeaderFooterSettingsRepository, ACDResultHeaderFooterSettingsRepository>();
            services.AddTransient<IACDFlagsRepository, ACDFlagsRepository>();
            #endregion

            #region [Academics -CBT Exam]
            services.AddTransient<IACDCBTExamTypeRepository, ACDCBTExamTypeRepository>();
            services.AddTransient<IACDCBTExamsRepository, ACDCBTExamsRepository>();
            services.AddTransient<IACDCBTExamQuestionTypeRepository, ACDCBTExamQuestionTypeRepository>();
            services.AddTransient<IACDCBTExamQuestionsRepository, ACDCBTExamQuestionsRepository>();
            services.AddTransient<IACDCBTExamAnswersRepository, ACDCBTExamAnswersRepository>();
            services.AddTransient<IACDCBTExamStudentAnswersRepository, ACDCBTExamStudentAnswersRepository>();
            services.AddTransient<IACDCBTExamStudentScoreRepository, ACDCBTExamStudentScoreRepository>();
            services.AddTransient<IACDCBTExamLatexRepository, ACDCBTExamLatexRepository>();
            services.AddTransient<ICBTConnectionInfoRepository, CBTConnectionInfoRepository>();
            services.AddTransient<ICBTExamTakenFlagsRepository, CBTExamTakenFlagsRepository>();
            #endregion

            #region [Academics - Results Comments Entry]
            services.AddTransient<IACDResultsMidTermCommentsRepository, ACDResultsMidTermCommentsRepository>();
            services.AddTransient<IACDResultsTermEndCommentsRepository, ACDResultsTermEndCommentsRepository>();
            services.AddTransient<IACDResultsCheckPointIGCSECommentsRepository, ACDResultsCheckPointIGCSECommentsRepository>();
            #endregion

            #region [Academics - Results]
            services.AddTransient<IACDResultsCognitiveRepository, ACDResultsCognitiveRepository>();
            services.AddTransient<IACDResultsOtherMarksRepository, ACDResultsOtherMarksRepository>();
            services.AddTransient<IACDResultsBroadSheetRepository, ACDResultsBroadSheetRepository>();
            #endregion

            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
