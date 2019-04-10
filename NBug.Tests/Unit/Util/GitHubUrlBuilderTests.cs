// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BugReportTests.cs" company="NBug Project">
//   Copyright (c) 2019 Igor Velikorossov. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using ApprovalTests;
using FluentAssertions;
using NBug.Core.Util;
using NSubstitute;
using Xunit;
using Xunit.Extensions;

namespace NBug.Tests.Unit.Util
{
    public class GitHubUrlBuilderTests
    {
        private readonly IErrorReportMarkDownBodyBuilder _errorReportMarkDownBodyBuilder;
        private readonly GitHubUrlBuilder _gitHubUrlBuilder;

        public GitHubUrlBuilderTests()
        {
            _errorReportMarkDownBodyBuilder = Substitute.For<IErrorReportMarkDownBodyBuilder>();
            _gitHubUrlBuilder = new GitHubUrlBuilder(_errorReportMarkDownBodyBuilder);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public void Build_should_return_null_if_url_unset(string url)
        {
            _gitHubUrlBuilder.Build(url, null, null).Should().BeNull();
        }

        [Fact]
        public void Build_should_return_null_if_exception_unset()
        {
            _gitHubUrlBuilder.Build("http://", null, null).Should().BeNull();
        }

        [Theory]
        [InlineData("./page/new")]
        [InlineData("ftp://api.github.com/issues/new")]
        [InlineData("file:////c:/development/file.txt")]
        public void Build_should_return_null_if_url_not_absolute_or_http_https(string url)
        {
            _gitHubUrlBuilder.Build(url, null, null).Should().BeNull();
        }

        [Fact]
        public void Build_should_trim_subject_if_longer_70_char()
        {
            _errorReportMarkDownBodyBuilder.Build(Arg.Any<Exception>(), Arg.Any<string>()).Returns(x => @"## Current behaviour");
            var ex = new ApplicationException("BAU-863 adding settings and organisations if they are not already exist in the set");

            var url = _gitHubUrlBuilder.Build("http://localhost/issues/new?foo=bar", ex, null);

            var subject = url.Split('&', '?').First(x => x.StartsWith("title=")).Substring("title=".Length);
            subject.Should().Be(Uri.EscapeDataString("[NBug] BAU-863 adding settings and organisations if they are not a..."));
        }

        [Fact]
        public void Build_should_return_preserve_url_querystring()
        {
            _errorReportMarkDownBodyBuilder.Build(Arg.Any<Exception>(), Arg.Any<string>()).Returns(x => @"## Current behaviour");
            var ex = new ApplicationException("BAU-863 adding settings and organisations if they are not already exist in the set");

            var url = _gitHubUrlBuilder.Build("http://localhost/issues/new?foo=bar", ex, null);

            Approvals.Verify(url);
        }

        [Fact]
        public void Build_should_return_full_url()
        {
            _errorReportMarkDownBodyBuilder.Build(Arg.Any<Exception>(), Arg.Any<string>()).Returns(x => @"## Current behaviour

```
System.ApplicationException: Failed ---> System.ArgumentOutOfRangeException: Opps ---> System.DivideByZeroException: Boom!
   --- End of inner exception stack trace ---
   --- End of inner exception stack trace ---
   at NBug.Tests.Unit.Util.Mock.Method1() in C:\Development\gitextensions\Externals\NBug\NBug.Tests\Unit\Util\Mock.cs:line 13
   at NBug.Tests.Unit.Util.Mock.Method2() in C:\Development\gitextensions\Externals\NBug\NBug.Tests\Unit\Util\Mock.cs:line 16
   at NBug.Tests.Unit.Util.Mock.Method3() in C:\Development\gitextensions\Externals\NBug\NBug.Tests\Unit\Util\Mock.cs:line 17
   at NBug.Tests.Unit.Util.Mock.Method4() in C:\Development\gitextensions\Externals\NBug\NBug.Tests\Unit\Util\Mock.cs:line 18
   at NBug.Tests.Unit.Util.Mock.DoWork() in C:\Development\gitextensions\Externals\NBug\NBug.Tests\Unit\Util\Mock.cs:line 7
   at NBug.Tests.Unit.Util.ErrorReportMarkDownBodyBuilderTests.Build_should_build_report_without_system_or_additional_info() in C:\Development\gitextensions\Externals\NBug\NBug.Tests\Unit\Util\ErrorReportMarkDownBodyBuilderTests.cs:line 36
```

## Additional information

It happened when I was arguing with someone on the Internet.
I was right and they were absolutely wrong!
I win!But the app crashed!
PLEASE HELP!!!


## Environment

- Git Extensions 0.0.2.5232
- Build 23b6f51905006ccdde8701591df706284b4155dc
- Git 2.19.0.windows.1
- Microsoft Windows NT 10.0.17134.0
- .NET Framework 4.7.3324.0
- DPI 144dpi (150% scaling)
");
            var ex = new ApplicationException("BAU-863 adding settings and organisations if they are not already exist in the set");

            var url = _gitHubUrlBuilder.Build("http://localhost/issues/new", ex, null);

            Approvals.Verify(url);
        }
    }
}