// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitHubUrlBuilderTests.cs" company="Git Extensions">
//   Copyright (c) 2019 Igor Velikorossov. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using ApprovalTests;
using FluentAssertions;
using NBug.Core.Util;
using Xunit;

namespace NBug.Tests.Unit.Util
{
    public class ErrorReportMarkDownBodyBuilderTests
    {
        private readonly ErrorReportMarkDownBodyBuilder _errorReportMarkDownBodyBuilder;

        public ErrorReportMarkDownBodyBuilderTests()
        {
            Settings.GetSystemInfo = null;
            _errorReportMarkDownBodyBuilder = new ErrorReportMarkDownBodyBuilder();
        }

        [Fact]
        public void Build_should_throw_ArgumentNullException_if_exception_null()
        {
            ((Action)(() => _errorReportMarkDownBodyBuilder.Build(null, null))).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Build_should_build_report_without_system_or_additional_info()
        {
            try
            {
                Mock.DoWork();

                Assert.True(false, "Expected an exception to be thrown");
            }
            catch (Exception ex)
            {
                var report = _errorReportMarkDownBodyBuilder.Build(ex, null);
                Approvals.Verify(report);
            }
        }

        [Fact]
        public void Build_should_build_report_with_system_without_additional_info()
        {
            Settings.GetSystemInfo = () => @"
- Git Extensions 0.0.2.5232
- Build 23b6f51905006ccdde8701591df706284b4155dc
- Git 2.19.0.windows.1
- Microsoft Windows NT 10.0.17134.0
- .NET Framework 4.7.3324.0
- DPI 144dpi (150% scaling)
";

            try
            {
                Mock.DoWork();

                Assert.True(false, "Expected an exception to be thrown");
            }
            catch (Exception ex)
            {
                var report = _errorReportMarkDownBodyBuilder.Build(ex, null);
                Approvals.Verify(report);
            }
        }

        [Fact]
        public void Build_should_build_report_with_system_and_additional_info()
        {
            try
            {
                Mock.DoWork();

                Assert.True(false, "Expected an exception to be thrown");
            }
            catch (Exception ex)
            {
                var report = _errorReportMarkDownBodyBuilder.Build(ex, "It happened when I was arguing with someone on the Internet.\r\nI was right and they were absolutely wrong!\r\nI win! But the app crashed!\r\nPLEASE HELP!!!");
                Approvals.Verify(report);
            }
        }
    }
}