using System;
using System.Text;
using System.Windows.Forms;

using Test.Business;
using Test.Business.Factories;
using Test.Core;

namespace FileCopier
{
    public partial class frmFileCopier : Form
    {
        private readonly IFileServer _fileServer;

        public frmFileCopier()
        {
            _fileServer = FileServerFactory.Create(typeof(LocalFileServer).AssemblyQualifiedName);
            InitializeComponent();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            ToggleControlEnability(false);

            if (ValidateInput())
            {
                txtStatus.Clear();

                var messageBuilder = new StringBuilder();

                try
                {
                    var parm = new CopyFilesParam()
                    {
                        DestinationDirectoryPath = txtDestination.Text,
                        SourceDirectoryPath = txtSource.Text,
                        FileSearchQuery = txtQuery.Text,
                        MaxFileAgeDays = Int32.Parse(txtAge.Text),
                        MaxFilesPerFolder = Int32.Parse(txtMaxFiles.Text)
                    };

                    var result = _fileServer.CopyFiles(parm);

                    messageBuilder.AppendLine("SUCCESSFUL FILES:");

                    foreach (var goodFile in result.CompletedFilePaths)
                    {
                        messageBuilder.AppendFormat("{0}{1}{2}", Environment.NewLine, "\t", goodFile);
                    }

                    messageBuilder.AppendFormat("{0}{0}{1}", Environment.NewLine, "UNSUCCESSFUL FILES:");

                    foreach (var failedFile in result.FailedLocalFilePaths)
                    {
                        messageBuilder.AppendFormat("{0}{1}{2}", Environment.NewLine, "\t", failedFile);
                    }


                }
                catch (Exception ex)
                {
                    messageBuilder.Clear();
                    messageBuilder.AppendFormat("ERROR: {0}", ex);
                }

                txtStatus.Text = messageBuilder.ToString();
            }

            ToggleControlEnability(true);
        }

        private bool ValidateInput()
        {
            var message = new StringBuilder("ERROR: ");
            var result = true;
            int val;

            if (string.IsNullOrWhiteSpace(txtDestination.Text))
            {
                message.AppendLine("Destination input missing");
                result = false;
            }
            else if (string.IsNullOrWhiteSpace(txtSource.Text))
            {
                message.AppendLine("Source input missing");
                result = false;
            }
            else if (string.IsNullOrWhiteSpace(txtQuery.Text))
            {
                message.AppendLine("Query input missing");
                result = false;
            }
            else if (string.IsNullOrWhiteSpace(txtAge.Text) || !Int32.TryParse(txtAge.Text, out val))
            {
                message.AppendLine("File Age input incorrect");
                result = false;
            }
            else if (string.IsNullOrWhiteSpace(txtMaxFiles.Text) || !Int32.TryParse(txtMaxFiles.Text, out val))
            {
                message.AppendLine("Max Files input incorrect");
                result = false;
            }

            if (!result)
            {
                txtStatus.Clear();
                txtStatus.Text = message.ToString();
            }

            return result;
        }

        private void ToggleControlEnability(bool enable)
        {
            txtSource.Enabled = enable;
            txtDestination.Enabled = enable;
            txtAge.Enabled = enable;
            txtMaxFiles.Enabled = enable;
            txtQuery.Enabled = enable;
            btnCopy.Enabled = enable;
        }
    }
}
