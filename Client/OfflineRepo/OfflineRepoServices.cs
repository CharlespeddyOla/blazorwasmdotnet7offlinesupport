using WebAppAcademics.Client.OfflineRepo.Academics.Exam.CBT;
using WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Comments;
using WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Entry;
using WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Grades;
using WebAppAcademics.Client.OfflineRepo.Academics.Subjects;
using WebAppAcademics.Client.OfflineRepo.Admin.School;
using WebAppAcademics.Client.OfflineRepo.Admin.Student;
using WebAppAcademics.Client.OfflineRepo.Auth;
using WebAppAcademics.Client.OfflineRepo.Settings;
using WebAppAcademics.Client.OfflineServices;

namespace WebAppAcademics.Client.OfflineRepo
{
    public static class OfflineRepoServices
    {
        public static void AddOfflineRepoServices(this IServiceCollection services)
        {
            services.AddScoped<AuthDBSyncRepo>();
            services.AddScoped<SessionsDBSyncRepo>();
            services.AddScoped<SchoolDBSyncRepo>();
            services.AddScoped<ClassListDBSyncRepo>();
            services.AddScoped<CognitiveDBSyncRepo>();
            services.AddScoped<SubjectsDBSyncRepo>();
            services.AddScoped<SbjAllocTeacherDBSyncRepo>();
            services.AddScoped<SbjAllocStudentDBSyncRepo>();
            services.AddScoped<ResultTypeDBSyncRepo>();
            services.AddScoped<MarkSettingsDBSyncRepo>();
            services.AddScoped<CBTExamsDBSyncRepo>();
            services.AddScoped<CBTScoresDBSyncRepo>();
            services.AddScoped<StudentDBSyncRepo>();
            services.AddScoped<RatingOptionsDBSyncRepo>();
            services.AddScoped<RatingTextDBSyncRepo>();
            services.AddScoped<RatingDBSyncRepo>();
            services.AddScoped<AssessmentDBSyncRepo>();
            services.AddScoped<SubjectClassificationDBSyncRepo>();
            services.AddScoped<TermEndCommentsDBSyncRepo>();
            services.AddScoped<MidTermCommentsDBSyncRepo>();
            services.AddScoped<CheckPointIGCSECommentsDBSyncRepo>();
            services.AddScoped<SeniorMockGradesDBSyncRepo>();
            services.AddScoped<JuniorMockGradesDBSyncRepo>();
            services.AddScoped<CheckPointGradesDBSyncRepo>();
            services.AddScoped<IGCSEGradesDBSyncRepo>();
            services.AddScoped<GeneralGradesDBSyncRepo>();

            services.AddTransient<INetworkStatus, NetworkStatus>();
        }
    }
}
