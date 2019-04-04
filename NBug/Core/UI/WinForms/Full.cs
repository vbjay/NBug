// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Full.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// <copyright file="Full.cs" company="Git Extensions">
//   Copyright (c) 2019 Igor Velikorossov. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GitExtUtils.GitUI;
using NBug.Core.Reporting.Info;
using NBug.Core.Util.Serialization;
using NBug.Properties;

namespace NBug.Core.UI.WinForms
{
	internal partial class Full : Form
	{
		private UIDialogResult _uiDialogResult;
		private SerializableException _lastException;

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

			// ToDo: Displaying report contents properly requires some more work.
			mainTabs.TabPages.Remove(mainTabs.TabPages["reportContentsTabPage"]);
		}

		internal UIDialogResult ShowDialog(SerializableException exception, Report report)
		{
			Text = string.Format("{0} {1}", report.GeneralInfo.HostApplication, Settings.Resources.UI_Dialog_Full_Title);

			_lastException = exception;

			// Scaling
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

			// Write back the user description (as we passed 'report' as a reference since it is a refence object anyway)
			report.GeneralInfo.UserDescription = descriptionTextBox.Text;
			return _uiDialogResult;
		}

		private void QuitButton_Click(object sender, EventArgs e)
		{
			_uiDialogResult = new UIDialogResult(ExecutionFlow.BreakExecution, SendReport.DoNotSend);
			Close();
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("## Current behaviour");
			sb.AppendLine();
			sb.AppendLine("```");
			sb.AppendLine(_lastException.OriginalException.ToString());
			sb.AppendLine("```");
			sb.AppendLine();
			sb.AppendLine();

			if (!string.IsNullOrWhiteSpace(descriptionTextBox.Text))
			{
				sb.AppendLine("## Additional information");
				sb.AppendLine();
				sb.AppendLine(descriptionTextBox.Text.Trim());
				sb.AppendLine();
				sb.AppendLine();
			}

			try
			{
				sb.AppendLine("## Environment");
				sb.AppendLine();

				var systemInfo = Settings.GetSystemInfo?.Invoke();
				if (!string.IsNullOrWhiteSpace(systemInfo))
				{
					sb.AppendLine(systemInfo);
				}
				else
				{
					sb.AppendLine("System information is not supplied");
				}
			}
			catch (Exception ex)
			{
				sb.AppendLine("Failed to retrieve system information.");
				sb.AppendLine("Exception:");
				sb.AppendLine("```");
				sb.AppendLine(ex.ToString());
				sb.AppendLine("```");
			}

			Clipboard.SetDataObject(sb.ToString(), true, 5, 100);
		}
	}
}