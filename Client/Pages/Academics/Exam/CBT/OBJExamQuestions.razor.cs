using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.CBT;
using System.Drawing;
using WebAppAcademics.Client.Extensions;

namespace WebAppAcademics.Client.Pages.Academics.Exam.CBT
{
    public partial class OBJExamQuestions
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] IJSRuntime iJSRuntime { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IDialogService DialogService { get; set; }
        [Inject] IAPIServices<CBTQuestionType> questionTypeService { get; set; }
        [Inject] IAPIServices<CBTQuestions> questionsService { get; set; }
        [Inject] IAPIServices<CBTAnswers> answersService { get; set; }
        [Inject] IAPIServices<CBTLatex> equationEditorService { get; set; }
        [Inject] IAPIServices<CBTStudentAnswers> studentAnswersService { get; set; }
        [Inject] IAPIServices<CBTStudentScores> studentScoresService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Model Declaration]
        List<CBTQuestionType> questionTypeList = new();
        List<CBTQuestions> questions = new();
        List<CBTAnswers> answers = new();
        List<CBTAnswers> questionAnswers = new();
        List<AnswersStorage> ansStorage = new();
        List<ImageStore> imageStore = new();
        List<CBTLatex> latexlist = new();
        List<CBTStudentAnswers> studentAnswers = new();
        List<CBTStudentScores> studentScores = new();

        CBTQuestions question = new();
        CBTAnswers answer = new();
        CBTStudentAnswers studentAnswer = new();
        CBTStudentScores studentScore = new();

        #endregion

        #region [Variables Declaration]
        [Parameter] public int examid { get; set; }
        int toolBarMenuId { get; set; }
        int questionTypeID { get; set; }
        int questionID { get; set; }
        int newquestionID { get; set; }
        int selectedAnsID { get; set; }
        int numberOfMultipleChoice { get; set; }
        int alphabetCount { get; set; }
        int selectedTextBox { get; set; }       

        char[] alphabet = new char[0];

        bool AddMoreQuestion { get; set; }
        bool DisabledQuestionType { get; set; }
        bool DisabledAddMoreQuestion { get; set; }

        string academicSession { get; set; }
        string examname { get; set; }
        string selectedQuestionType { get; set; }
        string SelectedText { get; set; }
        string questionNo { get; set; } = "Question";
        string _ExamName { get; set; }
        string pagetitle = "Create A New Question";
        string equation { get; set; }
        string selectedAnswerLetter { get; set; }

        bool visible { get; set; }
        void Submit() => visible = false;
        bool textFormatVisible { get; set; }
        void SelectTextFormating() => textFormatVisible = false;
        DialogOptions dialogOptions = new() { FullWidth = true };


        string textFormattingToolTip { get; set; }

        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;


        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "CBT Exam Questions Setting";
            toolBarMenuId = 1;
            academicSession = await localStorageService.ReadEncryptedItemAsync<string>("academicsession");
            examname = await localStorageService.ReadEncryptedItemAsync<string>("examname");
            await LoadDefaultList();
            await base.OnInitializedAsync();
        }


        #region [[Section -  Questions List]
        async Task LoadDefaultList()
        {
            questionTypeList = await questionTypeService.GetAllAsync("AcademicsCBT/GetCBTExamQuestionTypes/2");
            questions = await questionsService.GetAllAsync("AcademicsCBT/GetCBTExamQuestions/1/" + examid);
            answers = await answersService.GetAllAsync("AcademicsCBT/GetCBTExamAnswers/1/" + examid);
            latexlist = await equationEditorService.GetAllAsync("AcademicsCBT/GetLatexList/1");
            question.ExamID = examid;
            _ExamName = academicSession + ": " + examname;
        }

        void OnSelectedQuestion(TableRowClickEventArgs<CBTQuestions> _question)
        {
            questionID = _question.Item.QID;
            question = questions.Where(qst => qst.QID == questionID).FirstOrDefault();
            questionAnswers = answers.Where(ans => ans.QID == questionID).ToList();

            selectedAnsID = 0;
            selectedAnswerLetter = string.Empty;
            ansStorage.Clear();
            RetrieveQuestion(questionID);
        }

        async Task DeleteQuestion(int _questionID)
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Delete Selected Question",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                question.QID = _questionID;
                question.DeleteQuestion = true;
                await questionsService.UpdateAsync("AcademicsCBT/UpdateCBTExamQuestion/", 2, question);
                foreach (var item in answers.Where(ans => ans.QID == _questionID))
                {
                    answer.AnsID = item.AnsID;
                    answer.DeleteAnswers = true;
                    await answersService.UpdateAsync("AcademicsCBT/UpdateCBTExamAnswer/", 2, answer);
                }
                await Swal.FireAsync("Selected Question", "Has Been Successfully Deleted.", "success");

            }
            else
            {
                await Swal.FireAsync("Question Deletion", "Operation Aborted", "info");
            }
        }


        #endregion

        #region [Section - Question Details]
        async Task AddQuestion()
        {
            await Refresh();

            pagetitle = "Create A New Question";

            if (questions.Count() == 0)
            {
                DisabledAddMoreQuestion = true;
            }
            else
            {
                DisabledAddMoreQuestion = false;
            }
        }

        async Task OnQuestionTypeChanged(IEnumerable<string> e)
        {
            selectedQuestionType = e.ElementAt(0);
            questionTypeID = questionTypeList.FirstOrDefault(c => c.QType == selectedQuestionType).QTypeID;
            question.QTypeID = questionTypeID;
            alphabetCount = 0;
            ansStorage.Clear();
            answers.Clear();
            imageStore.Clear();
            imgSrc = string.Empty;

            if (question.NAns < 2)
            {
                await Swal.FireAsync("Invalid Number of Multiple Choice.", "Please Enter At Least Two (2) for No. of Multiple Choice", "error");
            }
            else
            {
                if (questions.Count() == 0)
                {
                    //First Question
                    question.QNo = 1;
                    questionNo = "Quesion " + question.QNo;
                }
                else
                {
                    //Edit Or Add More Question
                    if (AddMoreQuestion)
                    {
                        //Add More Question
                        questions = await questionsService.GetAllAsync("AcademicsCBT/GetCBTExamQuestions/1/" + examid);
                        question.QNo = questions.Count() + 1;
                        questionNo = "Quesion " + question.QNo;
                    }
                }

                numberOfMultipleChoice = question.NAns;
                alphabet = Enumerable.Range('A', numberOfMultipleChoice).Select(x => (char)x).ToArray();
            }
        }

        void OnChanged(string TextID, object textValue)
        {
            try
            {
                if (ansStorage.Count() != numberOfMultipleChoice)
                {
                    ansStorage.Add(new AnswersStorage()
                    {
                        Options = TextID,
                        Answers = textValue.ToString().Trim(),
                    });
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        void OnEquationChanged(string TextID, object textValue)
        {
            try
            {
                if (ansStorage.Count() != numberOfMultipleChoice)
                {
                    ansStorage.Add(new AnswersStorage()
                    {
                        Options = TextID,
                        Equation = textValue.ToString().Trim(),
                    });
                }
                else
                {
                    var _ansStorage = ansStorage.FirstOrDefault(a => a.Options == TextID);

                    if (_ansStorage != null)
                    {
                        ansStorage.FirstOrDefault(a => a.Options == TextID).Equation = textValue.ToString().Trim();
                    }
                    else
                    {
                        ansStorage.Add(new AnswersStorage()
                        {
                            Options = TextID,
                            Equation = textValue.ToString().Trim(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        async Task Refresh()
        {
            selectedQuestionType = string.Empty;
            question.QType = string.Empty;
            questionTypeID = 0;
            question.NAns = 4;
            question.QTypeID = 0;
            question.QType = string.Empty;
            question.ExamID = examid;
            question.QPoints = 1;
            question.QTime = 60;
            question.Section = string.Empty;
            question.Question = null;
            question.Equation = null;
            selectedAnswerLetter = string.Empty;
            imgSrc = "";
            _fileBytes = null;
            question.SImage = null;
            question.QImage = null;
            sectionImage = null;
            questionImage = null;
            answer.AnsImage = null;
            questionTypeList = await questionTypeService.GetAllAsync("AcademicsCBT/GetCBTExamQuestionTypes/2");
            answers.Clear();
            ansStorage.Clear();
            imageStore.Clear();
            questions = await questionsService.GetAllAsync("AcademicsCBT/GetCBTExamQuestions/1/" + examid);
            question.QNo = questions.Count() + 1;
            questionNo = "Quesion " + question.QNo;
            answers = await answersService.GetAllAsync("AcademicsCBT/GetCBTExamAnswers/1/" + examid);

            numberOfMultipleChoice = question.NAns;
            alphabet = Enumerable.Range('A', numberOfMultipleChoice).Select(x => (char)x).ToArray();
        }

        void RetrieveQuestion(int _qid)
        {
            //Update Questions
            // Change page title and button text since this is an edit.
            pagetitle = "Edit Exam Question";
            toolBarMenuId = 2;

            question = questions.FirstOrDefault(q => q.QID == _qid);
            questionTypeID = question.QTypeID;
            questionNo = "Quesion " + question.QNo;

            if (question.SImage != null)
            {
                sectionImage = question.SImage;
            }
            else
            {
                sectionImage = null;
            }

            if (question.QImage != null)
            {
                questionImage = question.QImage;
            }
            else
            {
                questionImage = null;
            }

            bool IsAnswerExist = answers.Where(ans => ans.QID == _qid && ans.CorrectAns == true).Any();

            if (IsAnswerExist)
            {
                selectedAnswerLetter = answers.FirstOrDefault(ans => ans.QID == _qid && ans.CorrectAns == true).AnsLetter;
            }

            foreach (var item in answers.Where(ans => ans.QID == _qid).ToList())
            {
                ansStorage.Add(new AnswersStorage()
                {
                    Options = item.AnsLetter,
                    Answers = item.Answers,
                    Equation = item.Equation,
                    AnsImage = item.AnsImage,
                    AnsID = item.AnsID,
                });
            }

            foreach (var item in answers.Where(ans => ans.QID == _qid).ToList())
            {
                imgSrc = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(item.AnsImage));

                imageStore.Add(new ImageStore()
                {
                    Options = item.AnsLetter,
                    ImageURL = imgSrc,
                });
            }
        }

        async Task OnToggledChanged(bool toggled)
        {
            // Because variable is not two-way bound, we need to update it ourself
            AddMoreQuestion = toggled;

            if (AddMoreQuestion)
            {
                DisabledQuestionType = false;
                await Refresh();
            }
            else
            {
                DisabledQuestionType = true;
            }
        }

        async Task AddQuestionAndAnswers()
        {
            answers.Clear();
            var response = await questionsService.SaveAsync("AcademicsCBT/AddCBTExamQuestion/", question);
            newquestionID = response.QID;

            for (int i = 0; i < question.NAns; i++)
            {
                switch (questionTypeID)
                {
                    case 1:
                        answers.Add(new CBTAnswers()
                        {
                            QTypeID = questionTypeID,
                            QID = newquestionID,
                            ExamID = examid,
                            AnsLetter = alphabet[i].ToString().Trim(),
                            Answers = ansStorage.FirstOrDefault(o => o.Options == alphabet[i].ToString()).Answers,
                            Equation = ansStorage.FirstOrDefault(o => o.Options == alphabet[i].ToString()).Equation,
                            AnsImage = ansStorage.FirstOrDefault(o => o.Options == alphabet[i].ToString()).AnsImage,
                            CorrectAns = ansStorage.Where(o => o.Options == alphabet[i].ToString() && o.Options == selectedAnswerLetter).Any(),
                        });
                        break;
                }
            }

            foreach (var item in answers)
            {
                answer.QTypeID = item.QTypeID;
                answer.QID = item.QID;
                answer.ExamID = item.ExamID;
                answer.AnsLetter = item.AnsLetter.Trim();
                answer.Answers = item.Answers;
                answer.Equation = item.Equation;
                answer.AnsImage = item.AnsImage;
                answer.CorrectAns = item.CorrectAns;

                await answersService.SaveAsync("AcademicsCBT/AddCBTExamAnswer/", answer);
            }
        }

        async Task UpdateQuestionAndAnswers()
        {
            await questionsService.UpdateAsync("AcademicsCBT/UpdateCBTExamQuestion/", 1, question);

            switch (questionTypeID)
            {
                case 1:
                    foreach (var item in ansStorage)
                    {
                        answer.AnsID = item.AnsID;
                        answer.Answers = item.Answers;
                        answer.Equation = item.Equation;
                        answer.AnsImage = item.AnsImage;
                        if (item.Options == selectedAnswerLetter)
                        {
                            answer.CorrectAns = true;
                        }
                        else
                        {
                            answer.CorrectAns = false;
                        }

                        await answersService.UpdateAsync("AcademicsCBT/UpdateCBTExamAnswer/", 1, answer);
                    }
                    break;
            }
        }

        async Task Save()
        {
            if (!String.IsNullOrWhiteSpace(selectedAnswerLetter))
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Questions Create/Update Operation",
                    Text = "Do You Want To Continue With This Operation?",
                    Icon = SweetAlertIcon.Question,
                    ShowCancelButton = true,
                    ConfirmButtonText = "Yes, Contnue!",
                    CancelButtonText = "No"
                });

                if (result.IsConfirmed)
                {
                    question.QImage = questionImage;
                    question.SImage = sectionImage;
                    if (DisabledAddMoreQuestion)
                    {
                        await AddQuestionAndAnswers();
                        await Swal.FireAsync("A New Question", "Has Been Successfully Created", "success");
                    }
                    else
                    {
                        if (AddMoreQuestion)
                        {
                            await AddQuestionAndAnswers();
                            await Swal.FireAsync("A New Question", "Has Been Successfully Created", "success");
                        }
                        else
                        {
                            await UpdateQuestionAndAnswers();
                            await Swal.FireAsync("Selected Question", "Has Been Successfully Updated", "success");
                        }
                    }

                    await Refresh();
                    AddMoreQuestion = false;
                }
            }
            else
            {
                await Swal.FireAsync("Multiple Choice Answer", "Please, Select An Answer For Your Multiple Choice.", "error");
            }
        }


        #region [Image Processing Section]
        #region [Variable Declaration]
        int imageSelectionId { get; set; }
        bool selectedImageType { get; set; } = false;
        string selectedImageOpt { get; set; }
        string imgSrc { get; set; } = "";
        string questionImageURL { get; set; } = "";
        string sectionImageURL { get; set; } = "";
        IBrowserFile file { get; set; } = null;
        byte[] _fileBytes { get; set; } = null;
        byte[] questionImage { get; set; }
        byte[] sectionImage { get; set; }
        long maxFileSize { get; set; } = 1024 * 1024 * 15;
        Utilities utilities = new Utilities();
        #endregion

        //public Image byteArrayToImage(byte[] byteArrayIn)
        //{
        //    using (MemoryStream mStream = new MemoryStream(byteArrayIn))
        //    {
        //        return Image.FromStream(mStream);
        //    }
        //}

        void QuestionImageSelected()
        {
            selectedImageType = true;
            imageSelectionId = 1;
        }

        void SectionImageSelection()
        {
            selectedImageType = true;
            imageSelectionId = 2;
        }

        async Task UploadImages(InputFileChangeEventArgs e)
        {
            try
            {
                file = e.File;
                using var ms = new MemoryStream();
                var stream = file.OpenReadStream(maxFileSize);

                await stream.CopyToAsync(ms);

                if (selectedImageType == true)
                {
                    _fileBytes = ms.ToArray();

                    if (imageSelectionId == 1)
                    {
                        questionImage = _fileBytes;
                        question.QImage = _fileBytes;
                        var photoQuestion = utilities.GetImage(Convert.ToBase64String(_fileBytes));
                        questionImageURL = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(photoQuestion));
                        imgSrc = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(photoQuestion));
                    }
                    else if (imageSelectionId == 2)
                    {
                        sectionImage = _fileBytes;
                        question.SImage = _fileBytes;
                        var photoQuestion = utilities.GetImage(Convert.ToBase64String(_fileBytes));
                        sectionImageURL = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(photoQuestion));
                        imgSrc = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(photoQuestion));
                    }
                }
                else
                {
                    _fileBytes = ms.ToArray();
                    var photo = utilities.GetImage(Convert.ToBase64String(_fileBytes));
                    imgSrc = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(photo));

                    imageStore.Add(new ImageStore()
                    {
                        Options = selectedImageOpt,
                        ImageURL = imgSrc,
                    });

                    if (DisabledAddMoreQuestion)
                    {
                        if (!String.IsNullOrWhiteSpace(selectedImageOpt))
                        {
                            ansStorage.FirstOrDefault(a => a.Options == selectedImageOpt).AnsImage = _fileBytes;
                        }
                    }
                    else
                    {
                        if (AddMoreQuestion)
                        {
                            ansStorage.FirstOrDefault(a => a.Options == selectedImageOpt).AnsImage = _fileBytes;
                        }
                        else
                        {
                            ansStorage.FirstOrDefault(a => a.Options == selectedImageOpt).AnsImage = _fileBytes;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = file.Name + ex.Message;
            }
        }

        void AnswerImageLoader(string opt)
        {
            selectedImageOpt = opt;
            selectedImageType = false;
        }

        #region [PopUp Handlers]
       
        void ShowQuestionImage()
        {
            if (question.QImage != null)
            {
                //ResizeImage();
                imgSrc = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(question.QImage));
            }
            else
            {
                imgSrc = string.Empty;
            }

            visible = true;
        }

        void ShowSectionImage()
        {
            if (question.SImage != null)
            {
                imgSrc = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(question.SImage));
            }
            else
            {
                imgSrc = string.Empty;
            }

            visible = true;
        }

        void ShowAnswersImage(string ansopt)
        {
            var imgList = imageStore.FirstOrDefault(img => img.Options.Contains(ansopt));

            if (imgList != null)
            {
                imgSrc = imgList.ImageURL;
            }
            else
            {
                imgSrc = string.Empty;
            }

            visible = true;
        }


        #endregion

        #endregion

        #endregion

        #region [Section - Equation Editor]
        void InsertEquationComponent(string value)
        {
            equation = equation + value;
        }

        void DisplayEquation()
        {

        }

        void ResetEquation()
        {
            equation = string.Empty;
        }

        #endregion

        #region [Section - Text Formatting]
        async Task GetSelectedText()
        {
            SelectedText = await iJSRuntime.InvokeAsync<string>("getSelectedText");
        }

        void OnSectionTextBoxClick()
        {
            selectedTextBox = 1;
        }

        void OnSQuestionTextBoxClick()
        {
            selectedTextBox = 2;
        }

        void Bold()
        {
            if (SelectedText != null)
            {
                if (SelectedText.Length > 0)
                {
                    if (selectedTextBox == 1)
                    {
                        question.Section = question.Section.Replace(SelectedText, "<b>" + SelectedText + "</b>");
                    }
                    else if (selectedTextBox == 2)
                    {
                        question.Question = question.Question.Replace(SelectedText, "<b>" + SelectedText + "</b>");
                    }

                    SelectedText = string.Empty;
                    textFormattingToolTip = string.Empty;
                }
            }
        }

        void Underline()
        {
            if (SelectedText != null)
            {
                if (SelectedText.Length > 0)
                {
                    if (selectedTextBox == 1)
                    {
                        question.Section = question.Section.Replace(SelectedText, "<u>" + SelectedText + "</u>");
                    }
                    else if (selectedTextBox == 2)
                    {
                        question.Question = question.Question.Replace(SelectedText, "<u>" + SelectedText + "</u>");
                    }

                    SelectedText = string.Empty;
                    textFormattingToolTip = string.Empty;
                }
            }
        }

        void Italics()
        {
            if (SelectedText != null)
            {
                if (SelectedText.Length > 0)
                {
                    if (selectedTextBox == 1)
                    {
                        question.Section = question.Section.Replace(SelectedText, "<i>" + SelectedText + "</i>");
                    }
                    else if (selectedTextBox == 2)
                    {
                        question.Question = question.Question.Replace(SelectedText, "<i>" + SelectedText + "</i>");
                    }

                    SelectedText = string.Empty;
                    textFormattingToolTip = string.Empty;
                }
            }
        }

        void StrikeThrough()
        {
            if (SelectedText != null)
            {
                if (SelectedText.Length > 0)
                {
                    if (selectedTextBox == 1)
                    {
                        question.Section = question.Section.Replace(SelectedText, "<s>" + SelectedText + "</s>");
                    }
                    else if (selectedTextBox == 2)
                    {
                        question.Question = question.Question.Replace(SelectedText, "<s>" + SelectedText + "</s>");
                    }

                    SelectedText = string.Empty;
                    textFormattingToolTip = string.Empty;
                }
            }
        }

        void Spacing()
        {
            if (SelectedText != null)
            {
                if (SelectedText.Length > 0)
                {
                    if (selectedTextBox == 1)
                    {
                        question.Section = question.Section.Replace(SelectedText, SelectedText + "&nbsp;");
                    }
                    else if (selectedTextBox == 2)
                    {
                        question.Question = question.Question.Replace(SelectedText, SelectedText + "&nbsp;");
                    }

                    SelectedText = string.Empty;
                    textFormattingToolTip = string.Empty;
                }
            }
            //question.Question = await iJSRuntime.InvokeAsync<string>("dotNetToJsSamples.getValue", textAreaReference, "&nbsp;");
        }

        void LineBreak()
        {
            if (SelectedText != null)
            {
                if (SelectedText.Length > 0)
                {
                    if (selectedTextBox == 1)
                    {
                        question.Section = question.Section.Replace(SelectedText, SelectedText + "<br/>");
                    }
                    else if (selectedTextBox == 2)
                    {
                        question.Question = question.Question.Replace(SelectedText, SelectedText + "<br/>");
                    }

                    SelectedText = string.Empty;
                    textFormattingToolTip = string.Empty;
                }
            }
            //question.Question = await iJSRuntime.InvokeAsync<string>("dotNetToJsSamples.getValue", textAreaReference, "<br/>");
        }

        void SmallerText()
        {
            if (SelectedText != null)
            {
                if (SelectedText.Length > 0)
                {
                    if (selectedTextBox == 1)
                    {
                        question.Section = question.Section.Replace(SelectedText, "<small>" + SelectedText + "</small>");
                    }
                    else if (selectedTextBox == 2)
                    {
                        question.Question = question.Question.Replace(SelectedText, "<small>" + SelectedText + "</small>");
                    }

                    SelectedText = string.Empty;
                    textFormattingToolTip = string.Empty;
                }
            }
        }

        void Subscript()
        {
            if (SelectedText != null)
            {
                if (SelectedText.Length > 0)
                {
                    if (selectedTextBox == 1)
                    {
                        question.Section = question.Section.Replace(SelectedText, "<sub>" + SelectedText + "</sub>");
                    }
                    else if (selectedTextBox == 2)
                    {
                        question.Question = question.Question.Replace(SelectedText, "<sub>" + SelectedText + "</sub>");
                    }

                    SelectedText = string.Empty;
                    textFormattingToolTip = string.Empty;
                }
            }
        }

        void Superscript()
        {
            if (SelectedText != null)
            {
                if (SelectedText.Length > 0)
                {
                    if (selectedTextBox == 1)
                    {
                        question.Section = question.Section.Replace(SelectedText, "<sup>" + SelectedText + "</sup>");
                    }
                    else if (selectedTextBox == 2)
                    {
                        question.Question = question.Question.Replace(SelectedText, "<sup>" + SelectedText + "</sup>");
                    }

                    SelectedText = string.Empty;
                    textFormattingToolTip = string.Empty;
                }
            }
        }

        void RightArrow()
        {
            if (SelectedText != null)
            {
                if (SelectedText.Length > 0)
                {
                    if (selectedTextBox == 1)
                    {
                        question.Section = question.Section.Replace(SelectedText, SelectedText + "&#8594;");
                    }
                    else if (selectedTextBox == 2)
                    {
                        question.Question = question.Question.Replace(SelectedText, SelectedText + "&#8594;");
                    }

                    SelectedText = string.Empty;
                    textFormattingToolTip = string.Empty;
                }
            }
            //question.Question = await iJSRuntime.InvokeAsync<string>("dotNetToJsSamples.getValue", textAreaReference, "&nbsp;");
        }

        void TextFormatting()
        {
            if (toolBarMenuId == 2)
            {
                textFormattingToolTip = "Bold";
                textFormatVisible = true;
            }
        }

        #endregion

        #region [Preview Questions]
        DialogOptions fullScreen = new DialogOptions() { FullScreen = true, CloseButton = true };

        void EnlargeImage(DialogOptions options, byte[] _qImage)
        {
            var parameters = new DialogParameters();
            parameters.Add("_QImage", _qImage);

            DialogService.Show<DialogEnlargeQuestionImage>("", parameters, options);
        }

        #endregion

        #region [Update Student Scores - Re- Mark]
        int _ansID { get; set; } = 0;
        int _choiceID { get; set; } = 0;
        int TotalQuestions { get; set; }
        int questionsAnswered { get; set; }
        int questionsAnsweredCorrectly { get; set; }

        async Task UpdateStudentScores()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Student Scores Update Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                await StudentNewScores();
                await Swal.FireAsync("Student Scores Update", "CBT Student Scores Successfully Updated ", "success");
            }
        }

        async Task StudentNewScores()
        {
            studentScores.Clear();
            questions.Clear();
            answers.Clear();
            TotalQuestions = 0;

            questions = await questionsService.GetAllAsync("AcademicsCBT/GetCBTExamQuestions/1/" + examid);
            TotalQuestions = questions.Count();
            answers = await answersService.GetAllAsync("AcademicsCBT/GetCBTExamAnswers/1/" + examid);
            studentScores = await studentScoresService.GetAllAsync("AcademicsCBT/GetCBTStudentScores/1/" + examid + "/0/true");

            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait re-calculating student cbt scores...";

            int maxValue = studentScores.Count();

            foreach (var item in studentScores)
            {
                questionsAnswered = 0;
                questionsAnsweredCorrectly = 0;

                j++;
                i = ((decimal)(j) / maxValue) * 100;
                studentAnswers = await studentAnswersService.GetAllAsync("AcademicsCBT/GetCBTStudentAnswers/1/" + examid + "/" + item.STDID + "/" + true);
                int scoreID = studentScores.FirstOrDefault(s => s.STDID == item.STDID).StudentScoreID;

                studentAnswer.STDID = item.STDID;
                studentAnswer.ExamID = examid;
                studentAnswer.Correct = false;
                studentAnswer.CBTToUse = true;
                await studentAnswersService.UpdateAsync("AcademicsCBT/UpdateCBTStudentAnswer/", 3, studentAnswer);

                foreach (var question in questions)
                {
                    //int qNum = question.QNo;
                    int _stdAnsID = studentAnswers.OrderBy(q => q.QNo).FirstOrDefault(ans => ans.STDID == item.STDID && ans.QID == question.QID).StudentAnswerID;
                    int _stdQID = studentAnswers.OrderBy(q => q.QNo).FirstOrDefault(ans => ans.STDID == item.STDID && ans.QID == question.QID).QID;
                    string _stdAnsLetter = studentAnswers.OrderBy(q => q.QNo).FirstOrDefault(ans => ans.STDID == item.STDID && ans.QID == question.QID).Answer;

                    if (_stdAnsLetter != "N")
                    {
                        int _choiceID = answers.FirstOrDefault(ans => ans.QID == _stdQID && ans.AnsLetter == _stdAnsLetter).AnsID;
                        studentAnswer.StudentAnswerID = _stdAnsID;
                        studentAnswer.AnsID = _choiceID;
                        await studentAnswersService.UpdateAsync("AcademicsCBT/UpdateCBTStudentAnswer/", 5, studentAnswer);

                        if (studentAnswers.OrderBy(q => q.QNo).Where(q => q.QID == question.QID).Any())
                        {
                            studentAnswer.StudentAnswerID = studentAnswers.OrderBy(q => q.QNo).FirstOrDefault(c => c.QID == question.QID).StudentAnswerID;
                            _ansID = answers.FirstOrDefault(a => a.QID == question.QID && a.CorrectAns == true).AnsID;
                            //_choiceID = studentAnswers.FirstOrDefault(c => c.QID == question.QID).AnsID;

                            if (_ansID == _choiceID)
                            {
                                studentAnswer.Correct = true;
                            }
                            else
                            {
                                studentAnswer.Correct = false;
                            }
                        }

                        await studentAnswersService.UpdateAsync("AcademicsCBT/UpdateCBTStudentAnswer/", 4, studentAnswer);
                    }
                }

                studentAnswers = await studentAnswersService.GetAllAsync("AcademicsCBT/GetCBTStudentAnswers/1/" + examid + "/" + item.STDID + "/" + true);

                var _questionsAnswered = studentAnswers.Where(n => n.QAnswered == true).GroupBy(a => a.QID).Select(q => new
                {
                    QID = q.Key,
                    Count = q.Select(a => a.QNo).Distinct().Count()
                });

                questionsAnswered = _questionsAnswered.Count();

                var _questionsAnsweredCorrectly = studentAnswers.Where(n => n.Correct == true).GroupBy(a => a.QID).Select(q => new
                {
                    QID = q.Key,
                    Count = q.Select(a => a.QNo).Distinct().Count()
                });

                questionsAnsweredCorrectly = _questionsAnsweredCorrectly.Count();

                double YourScore = (Convert.ToDouble(questionsAnsweredCorrectly) / Convert.ToDouble(TotalQuestions)) * 100;
                studentScore.StudentScoreID = scoreID;
                studentScore.ScorePercentage = YourScore;
                studentScore.NWrongAns = TotalQuestions - questionsAnsweredCorrectly;
                studentScore.NCorrectAns = questionsAnsweredCorrectly;
                await studentScoresService.UpdateAsync("AcademicsCBT/UpdateCBTStudentScore/", 2, studentScore);

                StateHasChanged();
            }

            IsShow = true;



        }



        #endregion

        #region [Section - Click Events]
        async Task CBTQuestions()
        {
            toolBarMenuId = 1;
            question = new CBTQuestions();
            await AddQuestion();
            AddMoreQuestion = false;
        }

        void CBTQuestion()
        {
            toolBarMenuId = 2;
            //question = new CBTQuestions();
            //await AddQuestion();
            //AddMoreQuestion = false;
        }

        void EquationEditor()
        {
            toolBarMenuId = 3;
        }

        async Task PreviewQuestion()
        {
            toolBarMenuId = 4;
            question = new CBTQuestions();
            await AddQuestion();
            AddMoreQuestion = false;
        }

        void Cancel()
        {
            toolBarMenuId = 1;
        }

        void GoBack()
        {
            navManager.NavigateTo("/academics/cbtexams");
        }
        #endregion




    }
}
