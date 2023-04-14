using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace WebAppAcademics.Client.Shared
{
    public partial class DialogEnlargeQuestionImage
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter] public byte[] _QImage { get; set; }

        string imgSrc { get; set; } = "";
        public bool SetHeight { get; set; } = false;
        public bool SetWidth { get; set; } = true;

        public int ImageHeight { get; set; } = 300;
        public int ImageWidth { get; set; } = 300;

        void Close() => MudDialog.Close();
    }
}
