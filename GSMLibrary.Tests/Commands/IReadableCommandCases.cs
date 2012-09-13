using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace GSMLibrary.Tests.Commands
{
    interface IReadableCommandCases
    {
        [Fact]
        void ParseCorrect();

        [Fact]
        void WrongPrefix();

        [Fact]
        void WrongParamsCount();

        [Fact]
        void WrongParams();

        [Fact]
        void ReadCommandTest();
    }
}
