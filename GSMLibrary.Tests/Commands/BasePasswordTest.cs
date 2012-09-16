using System;
using GSMLibrary.Commands.TrspSpecific;
using Xunit;
using GSMLibrary.Commands;

namespace GSMLibrary.Tests.Commands
{
    public class BasePasswordTest : BaseATCommandCase, IWritableCaseCommand
    {
        protected BasePasswordCommand _PasswordCommand;

        public BasePasswordTest()
        {
            _PasswordCommand = new BasePasswordCommand();
            _Command = _PasswordCommand;
        }

        [Fact]
        public void OldPasswordLengthTest()
        {
            try
            {
                _PasswordCommand.OldPassword = "1234567890" + "1234567890" + "1234567890" + "1234567890" + "1234567890" + "n";
            }
            catch (ArgumentOutOfRangeException zExcept)
            {
                Assert.Contains("диапазон", zExcept.Message);
                return;
            }
            Assert.True(false, "Удалось задать слишком длинную строку со старым паролем");
        }

        [Fact]
        public void OldPasswordEmptyTest()
        {
            try
            {
                _PasswordCommand.OldPassword = "";
            }
            catch (ArgumentNullException zExcept)
            {
                Assert.Contains("неопределенным", zExcept.Message);
                return;
            }

            Assert.True(false, "Удалось задать пустой старый пароль");
        }

        [Fact]
        public void NewPasswordLengthTest()
        {
            try
            {
                _PasswordCommand.NewPassword = "1234567890" + "1234567890" + "1234567890" + "1234567890" + "1234567890" + "n";
            }
            catch (ArgumentOutOfRangeException zExcept)
            {
                Assert.Contains("диапазон", zExcept.Message);
                return;
            }
            Assert.True(false, "Удалось задать слишком длинную строку с новым паролем");
        }

        [Fact]
        public void NewPasswordEmptyTest()
        {
            try
            {
                _PasswordCommand.NewPassword = "";
            }
            catch (ArgumentNullException zExcept)
            {
                Assert.Contains("неопределенным", zExcept.Message);
                return;
            }
            Assert.True(false, "Удалось задать пустой новый пароль");
        }

        [Fact]
        public void CheckInterfaces()
        {
            Assert.True(_Command is IWritableCommand);
        }

        [Fact]
        public virtual void WriteCommandTest()
        {
            // dummy
            _PasswordCommand.OldPassword = "as";
            _PasswordCommand.NewPassword = "as";
            _PasswordCommand.WriteCommand();            
        }
    }
}
