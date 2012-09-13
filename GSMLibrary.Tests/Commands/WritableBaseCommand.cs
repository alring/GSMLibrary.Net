using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSMLibrary.Commands;
using Xunit;

namespace GSMLibrary.Tests.Commands
{
    interface IWritableCaseCommand
    {
        void WriteCommandTest();        
    }

    public abstract class WritableBaseCommand : BaseReadableATCommand, IWritableCaseCommand
    {
        [Fact]
        public override void CheckInterfaces()
        {
            base.CheckInterfaces();

            Assert.True(_Command is IWritableCommand);
        }

        public abstract void WriteCommandTest();        
    }
}
