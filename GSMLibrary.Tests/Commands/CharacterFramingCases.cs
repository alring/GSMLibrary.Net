using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using GSMLibrary.Commands;

namespace GSMLibrary.Tests.Commands.CommandsCases
{
    public class CharacterFramingCases : WritableBaseCommand, IReadableCommandCases    
    {
        private CharacterFramingCommand _CharFrCommand;
        
        public CharacterFramingCases()
        {
            _CharFrCommand = new CharacterFramingCommand();
            _Command = _CharFrCommand;
        }

        [Fact]
        public override void WriteCommandTest()
        {
            _CharFrCommand.DataBits = 8;
            _CharFrCommand.ParityValue = System.IO.Ports.Parity.None;
            _CharFrCommand.StopBitsValue = System.IO.Ports.StopBits.One;

            Assert.Equal("AT+ICF=3,4", _CharFrCommand.WriteCommand());
        }

        [Fact]
        public void ParseCorrect()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("AT+ICF?");
            zList.Add("");
            zList.Add("+ICF: 2,3");
            zList.Add("OK");
            Assert.True(_CharFrCommand.Parse(zList));

            Assert.Equal(8, _CharFrCommand.DataBits);
            Assert.Equal(System.IO.Ports.Parity.Space, _CharFrCommand.ParityValue);
            Assert.Equal(System.IO.Ports.StopBits.One, _CharFrCommand.StopBitsValue);
        }

        [Fact]
        public void WrongPrefix()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("+ICdF: 3,4");
            zList.Add("OK");
            Assert.False(_CharFrCommand.Parse(zList));
        }

        [Fact]
        public void WrongParamsCount()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("+ICF: 3");
            zList.Add("OK");
            Assert.False(_CharFrCommand.Parse(zList));
        }

        [Fact]
        public void WrongParams()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("+ICF: 8,4");
            zList.Add("OK");
            Assert.False(_CharFrCommand.Parse(zList));

            zList.Clear();
            zList.Add("+ICF: 3,10");
            zList.Add("OK");
            Assert.False(_CharFrCommand.Parse(zList));
        }

        [Fact]
        public void CorrectBitsDecode()
        {
            Assert.Equal(1, CharacterFramingCommand.StopBitsDecode(8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.Two));
            Assert.Equal(2, CharacterFramingCommand.StopBitsDecode(8, System.IO.Ports.Parity.Mark, System.IO.Ports.StopBits.One));
            Assert.Equal(3, CharacterFramingCommand.StopBitsDecode(8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One));
            Assert.Equal(4, CharacterFramingCommand.StopBitsDecode(7, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.Two));
            Assert.Equal(5, CharacterFramingCommand.StopBitsDecode(7, System.IO.Ports.Parity.Mark, System.IO.Ports.StopBits.One));
            Assert.Equal(6, CharacterFramingCommand.StopBitsDecode(7, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One));
            

            Assert.Throws<System.ArgumentOutOfRangeException>(
               delegate
               {
                   CharacterFramingCommand.StopBitsDecode(8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.OnePointFive);
               });

            Assert.Throws<System.ArgumentOutOfRangeException>(
               delegate
               {
                   CharacterFramingCommand.StopBitsDecode(7, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.OnePointFive);
               });

            Assert.Throws<System.ArgumentOutOfRangeException>(
               delegate
               {
                   CharacterFramingCommand.StopBitsDecode(6, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.OnePointFive);
               });
        }

        [Fact]
        public void CorrectBitsEncode()
        {
            ushort zDataBits;
            System.IO.Ports.StopBits zStopBits;

            Assert.True(CharacterFramingCommand.StopBitsEncode(1, out zDataBits, out zStopBits));
            Assert.Equal(8, zDataBits);
            Assert.Equal(System.IO.Ports.StopBits.Two, zStopBits);

            Assert.True(CharacterFramingCommand.StopBitsEncode(2, out zDataBits, out zStopBits));
            Assert.Equal(8, zDataBits);
            Assert.Equal(System.IO.Ports.StopBits.One, zStopBits);

            Assert.True(CharacterFramingCommand.StopBitsEncode(3, out zDataBits, out zStopBits));
            Assert.Equal(8, zDataBits);
            Assert.Equal(System.IO.Ports.StopBits.One, zStopBits);

            Assert.True(CharacterFramingCommand.StopBitsEncode(4, out zDataBits, out zStopBits));
            Assert.Equal(7, zDataBits);
            Assert.Equal(System.IO.Ports.StopBits.Two, zStopBits);

            Assert.True(CharacterFramingCommand.StopBitsEncode(5, out zDataBits, out zStopBits));
            Assert.Equal(7, zDataBits);
            Assert.Equal(System.IO.Ports.StopBits.One, zStopBits);

            Assert.True(CharacterFramingCommand.StopBitsEncode(6, out zDataBits, out zStopBits));
            Assert.Equal(7, zDataBits);
            Assert.Equal(System.IO.Ports.StopBits.One, zStopBits);

            Assert.False(CharacterFramingCommand.StopBitsEncode(7, out zDataBits, out zStopBits));
        }

        [Fact]
        public void CorrectParityDecode()
        {
            Assert.Equal(0, CharacterFramingCommand.ParityDecode(System.IO.Ports.Parity.Odd));
            Assert.Equal(1, CharacterFramingCommand.ParityDecode(System.IO.Ports.Parity.Even));
            Assert.Equal(2, CharacterFramingCommand.ParityDecode(System.IO.Ports.Parity.Mark));
            Assert.Equal(3, CharacterFramingCommand.ParityDecode(System.IO.Ports.Parity.Space));
            Assert.Equal(4, CharacterFramingCommand.ParityDecode(System.IO.Ports.Parity.None));
        }

        [Fact]
        public void CorrectParityEncode()
        {
            Assert.Equal(System.IO.Ports.Parity.Odd, CharacterFramingCommand.ParityEncode(0));
            Assert.Equal(System.IO.Ports.Parity.Even, CharacterFramingCommand.ParityEncode(1));
            Assert.Equal(System.IO.Ports.Parity.Mark, CharacterFramingCommand.ParityEncode(2));
            Assert.Equal(System.IO.Ports.Parity.Space, CharacterFramingCommand.ParityEncode(3));
            Assert.Equal(System.IO.Ports.Parity.None, CharacterFramingCommand.ParityEncode(4));

            Assert.Throws<System.ArgumentOutOfRangeException>(
               delegate
               {
                   CharacterFramingCommand.ParityEncode(5);
               });  
        }

        [Fact]
        public void ReadCommandTest()
        {
            Assert.Equal("AT+ICF?", _CharFrCommand.ReadParamsCommand());
        }
    }
}
