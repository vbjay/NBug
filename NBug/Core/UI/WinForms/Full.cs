// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Full.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// <copyright file="Full.cs" company="Git Extensions">
//   Copyright (c) 2019 Igor Velikorossov. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using GitExtUtils.GitUI;
using NBug.Core.Reporting.Info;
using NBug.Core.Util;
using NBug.Core.Util.Serialization;
using NBug.Properties;

namespace NBug.Core.UI.WinForms
{
    internal partial class Full : Form
    {
        private static readonly IErrorReportMarkDownBodyBuilder ErrorReportBodyBuilder;
        private static readonly GitHubUrlBuilder UrlBuilder;
        private UIDialogResult _uiDialogResult;
        private SerializableException _lastException;

        static Full()
        {
            ErrorReportBodyBuilder = new ErrorReportMarkDownBodyBuilder();
            UrlBuilder = new GitHubUrlBuilder(ErrorReportBodyBuilder);
        }

        internal Full()
        {
            InitializeComponent();
            Icon = Resources.NBug_icon_16;
            warningLabel.Text = Settings.Resources.UI_Dialog_Full_Message;
            generalTabPage.Text = Settings.Resources.UI_Dialog_Full_General_Tab;
            exceptionTabPage.Text = Settings.Resources.UI_Dialog_Full_Exception_Tab;
            reportContentsTabPage.Text = Settings.Resources.UI_Dialog_Full_Report_Contents_Tab;
            errorDescriptionLabel.Text = Settings.Resources.UI_Dialog_Full_How_to_Reproduce_the_Error_Notification;
            quitButton.Text = Settings.Resources.UI_Dialog_Full_Quit_Button;
            sendAndQuitButton.Text = Settings.Resources.UI_Dialog_Full_Send_and_Quit_Button;

            // ToDo: Displaying report contents properly requires some more work.
            mainTabs.TabPages.Remove(mainTabs.TabPages["reportContentsTabPage"]);
        }

        internal UIDialogResult ShowDialog(SerializableException exception, Report report)
        {
            Text = string.Format("{0} {1}", report.GeneralInfo.HostApplication, Settings.Resources.UI_Dialog_Full_Title);

            _lastException = exception;

            // Scaling
            sendAndQuitButton.Image = DpiUtil.Scale(Resources.Send);
            btnCopy.Image = DpiUtil.Scale(Resources.CopyToClipboard);
            exceptionTypeLabel.Image = DpiUtil.Scale(Resources.NBug_Icon_PNG_16);
            exceptionDetails.InformationColumnWidth = DpiUtil.Scale(350);
            exceptionDetails.PropertyColumnWidth = DpiUtil.Scale(101);

            // Fill in the 'General' tab
            warningPictureBox.Image = SystemIcons.Warning.ToBitmap();
            exceptionTextBox.Text = exception.Type;
            exceptionMessageTextBox.Text = exception.Message;
            targetSiteTextBox.Text = exception.TargetSite;
            applicationTextBox.Text = $@"{report.GeneralInfo.HostApplication} [{report.GeneralInfo.HostApplicationVersion}]";
            nbugTextBox.Text = report.GeneralInfo.NBugVersion;
            dateTimeTextBox.Text = report.GeneralInfo.DateTime;
            clrTextBox.Text = report.GeneralInfo.CLRVersion;

            // Fill in the 'Exception' tab
            exceptionDetails.Initialize(exception);

            // ToDo: Fill in the 'Report Contents' tab);
            ShowDialog();

            // Write back the user description (as we passed 'report' as a reference since it is a reference object anyway)
            report.GeneralInfo.UserDescription = descriptionTextBox.Text;
            return _uiDialogResult;
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            _uiDialogResult = new UIDialogResult(ExecutionFlow.BreakExecution, SendReport.DoNotSend);
            Close();
        }

        private void SendAndQuitButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show(this,
                    @"Give as much as information as possible please to help the developers solve this issue. Otherwise, your issue ticket may be closed without any follow-up from the developers.

Because of this, make sure to fill in all the fields in the report template please.

Send report anyway?",
                    "Error Report",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
            {
                return;
            }

            string url = UrlBuilder.Build("https://github.com/gitextensions/gitextensions/issues/new", _lastException.OriginalException, descriptionTextBox.Text);
            Process.Start(url);

            _uiDialogResult = new UIDialogResult(ExecutionFlow.BreakExecution, SendReport.DoNotSend);
            Close();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var report = ErrorReportBodyBuilder.Build(_lastException.OriginalException, descriptionTextBox.Text);
            if (string.IsNullOrWhiteSpace(report))
            {
                return;
            }

            Clipboard.SetDataObject(report, true, 5, 100);
        }
    }
}