﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Full.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using GitExtUtils.GitUI;

namespace NBug.Core.UI.WinForms
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using NBug.Core.Reporting.Info;
	using NBug.Core.Util.Serialization;
	using NBug.Properties;

	using Settings = NBug.Settings;

	internal partial class Full : Form
	{
		private UIDialogResult uiDialogResult;

		internal Full()
		{
			this.InitializeComponent();
			this.Icon = Resources.NBug_icon_16;
			this.warningLabel.Text = Settings.Resources.UI_Dialog_Full_Message;
			this.generalTabPage.Text = Settings.Resources.UI_Dialog_Full_General_Tab;
			this.exceptionTabPage.Text = Settings.Resources.UI_Dialog_Full_Exception_Tab;
			this.reportContentsTabPage.Text = Settings.Resources.UI_Dialog_Full_Report_Contents_Tab;
			this.errorDescriptionLabel.Text = Settings.Resources.UI_Dialog_Full_How_to_Reproduce_the_Error_Notification;
			this.quitButton.Text = Settings.Resources.UI_Dialog_Full_Quit_Button;

			// ToDo: Displaying report contents properly requires some more work.
			this.mainTabs.TabPages.Remove(this.mainTabs.TabPages["reportContentsTabPage"]);
		}

		internal UIDialogResult ShowDialog(SerializableException exception, Report report)
		{
			this.Text = string.Format("{0} {1}", report.GeneralInfo.HostApplication, Settings.Resources.UI_Dialog_Full_Title);

			// Scaling
			this.btnCopy.Image = DpiUtil.Scale(Resources.CopyToClipboard);
			this.exceptionTypeLabel.Image = DpiUtil.Scale(Resources.NBug_Icon_PNG_16);
			this.exceptionDetails.InformationColumnWidth = DpiUtil.Scale(350);
			this.exceptionDetails.PropertyColumnWidth = DpiUtil.Scale(101);

			// Fill in the 'General' tab
			this.warningPictureBox.Image = SystemIcons.Warning.ToBitmap();
			this.exceptionTextBox.Text = exception.Type;
			this.exceptionMessageTextBox.Text = exception.Message;
			this.targetSiteTextBox.Text = exception.TargetSite;
			this.applicationTextBox.Text = report.GeneralInfo.HostApplication + " [" + report.GeneralInfo.HostApplicationVersion + "]";
			this.nbugTextBox.Text = report.GeneralInfo.NBugVersion;
			this.dateTimeTextBox.Text = report.GeneralInfo.DateTime;
			this.clrTextBox.Text = report.GeneralInfo.CLRVersion;

			// Fill in the 'Exception' tab
			this.exceptionDetails.Initialize(exception);

			// ToDo: Fill in the 'Report Contents' tab);
			this.ShowDialog();

			// Write back the user description (as we passed 'report' as a reference since it is a refence object anyway)
			report.GeneralInfo.UserDescription = this.descriptionTextBox.Text;
			return this.uiDialogResult;
		}

		private void QuitButton_Click(object sender, EventArgs e)
		{
			this.uiDialogResult = new UIDialogResult(ExecutionFlow.BreakExecution, SendReport.DoNotSend);
			this.Close();
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			// TODO
		}
	}
}