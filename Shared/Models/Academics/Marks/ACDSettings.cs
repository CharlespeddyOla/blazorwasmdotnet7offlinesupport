using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Academics.Marks
{
    public class ACDSettingsGrade
    {
        public int GradeID { get; set; }
        public decimal LowerGrade { get; set; }
        public decimal HigherGrade { get; set; }
        public string GradeLetter { get; set; }
        public string GradeRemark { get; set; }
        public string GradeComments { get; set; }
        public string TeachersComment { get; set; }
        public string PrincipalComment { get; set; }
        public int Id { get; set; }
    }

    public class ACDSettingsGradeMock
    {
        public int GradeID { get; set; }
        public decimal LowerGrade { get; set; }
        public decimal HigherGrade { get; set; }
        public string GradeLetter { get; set; }
        public string GradeRemark { get; set; }
        public int Id { get; set; }
    }

    public class ACDSettingsGradeOthers
    {
        public int GradeID { get; set; }
        public decimal LowerGrade { get; set; }
        public decimal HigherGrade { get; set; }
        public string GradeLetter { get; set; }
        public string GradeRemark { get; set; }
        public int Id { get; set; }
    }

    public class ACDSettingsGradeCheckPoint
    {
        public int GradeID { get; set; }
        public decimal HigherGrade { get; set; }
        public decimal LowerGrade { get; set; }
        public decimal HigherRating { get; set; }
        public decimal LowerRating { get; set; }
        public string GradeLetter { get; set; }
        public string GradeRemark { get; set; }
        public string AutoComments { get; set; }
        public int Id { get; set; }
    }

    public class ACDSettingsGradeIGCSE
    {
        public int GradeID { get; set; }
        public decimal LowerGrade { get; set; }
        public decimal HigherGrade { get; set; }
        public string GradeLetter { get; set; }
        public string GradeRemark { get; set; }
        public string AutoComments { get; set; }
        public int Id { get; set; }
    }

    public class ACDSettingsMarks
    {
        public int MarkID { get; set; }
        public string MarkType { get; set; }
        public int Mark { get; set; }
        public int PassMark { get; set; }
        public bool ApplyPassMark { get; set; }
        public string PassMarkColor { get; set; }
        public string FailMarkColor { get; set; }
        public bool ApplyCBT { get; set; }
        public int Id { get; set; }
    }

    public class ACDSettingsRating
    {
        public int RatingID { get; set; }
        public int Rating { get; set; }
        public int LowScore { get; set; }
        public int HighScore { get; set; }
        public string GradeLetter { get; set; }
        public string RatingLevel { get; set; }
        public string RatingKey { get; set; }
        public int ShownCol { get; set; }

        public decimal minRatingValue { get; set; }
        public decimal maxRatingValue { get; set; }
        public decimal minRatingScore { get; set; }
        public decimal maxRatingScore { get; set; }
        public int Id { get; set; }
    }

    public class ACDSettingsRatingOptions
    {
        public int OptionID { get; set; }
        public string RatingOption { get; set; }
        public bool UsedOption { get; set; }
        public int Id { get; set; }
    }

    public class ACDSettingsRatingText
    {
        public int TextID { get; set; }
        public string RatingText { get; set; }
        public bool UsedText { get; set; }
        public int Id { get; set; }
    }

    public class ACDSettingsAutoComments
    {
        public int AutoCommentID { get; set; }
        public decimal LowerMark { get; set; }
        public decimal UpperMark { get; set; }
        public string TeachersComment { get; set; }
        public string PrincipalComment { get; set; }
        public int Id { get; set; }
    }

    public class ACDReportType
    {
        public int ReportTypeID { get; set; }
        public string ReportType { get; set; }
        public bool SelectedExam { get; set; }
        public int Id { get; set; }
    }

    public class ACDReportFooter
    {
        public int FooterID { get; set; }
        public string HeaderCA { get; set; }
        public string HeaderExam { get; set; }
        public string Footer { get; set; }
        public int Id { get; set; }
    }

    public class ACDSettingsOthers
    {
        public int OtherSettingID { get; set; }
        public string Description { get; set; }
        public bool BoolValue { get; set; }
        public string TextValue { get; set; }
        public int Id { get; set; }
    }

    public class ACDFlags
    {
        public int FlagID { get; set; }
        public string FlagType { get; set; }
        public string Description { get; set; }
        public bool Flag { get; set; }
        public int Id { get; set; }
    }
}
