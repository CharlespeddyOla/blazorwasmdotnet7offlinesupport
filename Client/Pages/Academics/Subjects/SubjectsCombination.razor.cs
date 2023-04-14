using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;

namespace WebAppAcademics.Client.Pages.Academics.Subjects
{
    public partial class SubjectsCombination
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ACDSbjDept> subjectDepartmentService { get; set; }
        [Inject] IAPIServices<ACDSubjects> subjectService { get; set; }
        [Inject] IAPIServices<CombinesSubjects> combinedSubjectService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int schid { get; set; }
        int selectedDeptId { get; set; }
        int selectedSubjectId { get; set; }
        string selectedSchool { get; set; }
        bool hover = true;

        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools = new();
        List<CombinesSubjects> _combinedSubjects = new();
        HashSet<CombinesSubjects> selectedItems = new();

        CombinesSubjects subjectdetails = new();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            selectedItems = new HashSet<CombinesSubjects>();
            Layout.currentPage = "Subjects Combination";
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            await base.OnInitializedAsync();
        }

        #region [Section - Using Table]
       
        async Task OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;

            _combinedSubjects.Clear();           
            _combinedSubjects = await combinedSubjectService.GetAllAsync("AcademicsSubjects/GetSubjects/6/" + schid + "/0/1/true");

            foreach (var item in _combinedSubjects)
            {
                if (item.SbjMerge)
                {
                    selectedItems.Add(new CombinesSubjects { SubjectID = item.SubjectID, Subject = item.Subject });
                }
            }
        }

        async Task SelectedSubjectRow(TableRowClickEventArgs<CombinesSubjects> model)
        {
            selectedSubjectId = model.Item.SubjectID;
            bool IsSelected = selectedItems.Where(s => s.SubjectID == selectedSubjectId).Any();

            subjectdetails.SubjectID = selectedSubjectId;
            if (IsSelected)
            {
                subjectdetails.SbjMergeID = schid; // model.Item.SbjMergeID;
                subjectdetails.SbjMerge = IsSelected;
                subjectdetails.SbjMergeName = model.Item.SbjMergeName;

                await combinedSubjectService.UpdateAsync("AcademicsSubjects/UpdateSubject/", 2, subjectdetails);
                Snackbar.Add(model.Item.Subject + " Has Been Added For Subjects Combination");
            }
            else
            {
                subjectdetails.SbjMergeID = 0;
                subjectdetails.SbjMerge = IsSelected;
                subjectdetails.SbjMergeName = string.Empty;

                await combinedSubjectService.UpdateAsync("AcademicsSubjects/UpdateSubject/", 2, subjectdetails);
                Snackbar.Add(model.Item.Subject + " Has Been Removed Ffrom Subjects Combination");
            }            
        }

        #endregion

        #region [Section - Click Events]

        void GoBack()
        {
            navManager.NavigateTo("/subjectlist");
        }
        #endregion
    }
}
