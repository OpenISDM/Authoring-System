using System;
using System.Linq;
using HistoricalRepository.Upload;

namespace HistoricalRepository
{




    public partial class Index : System.Web.UI.Page
    {
        protected global::System.Web.UI.HtmlControls.HtmlInputText uploaddatepicker;
        protected global::System.Web.UI.HtmlControls.HtmlInputText uploadtimepicker;
        protected global::System.Web.UI.WebControls.TextBox uploadName;
        protected global::System.Web.UI.WebControls.FileUpload FileUp;
        protected global::System.Web.UI.WebControls.DropDownList DropDownList2;
        protected global::System.Web.UI.WebControls.Button UploadBtn;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void UploadBtn_Click(object sender, EventArgs e)
        {

            // Check if there is no date
            if (uploaddatepicker.Value == "")
                return;

            // Check if there is no time
            if (uploadtimepicker.Value == "")
                return;

            // Check if there is no file name
            if (uploadName.Text == "")
                return;

            // Check if there is no file
            if (!FileUp.HasFile)
                return;

            //  uploaddatepicker Example : 2016-04-01
            string[] date_ = uploaddatepicker.Value.Split('-');
            string date = date_[0] + date_[1] + date_[2];

            // uploadtimepicker Example : 14 : 06
            string time = uploadtimepicker.Value.ElementAt(0).ToString() + uploadtimepicker.Value.ElementAt(1).ToString() +
                        uploadtimepicker.Value.ElementAt(3).ToString() + uploadtimepicker.Value.ElementAt(4).ToString();


            string datasetName = date + time;
            if (FileUp.HasFile)
            {
                FileUploader.Upload(FileUp.FileContent, FileUp.FileName, datasetName, uploadName.Text);
            }
        }


    }
}