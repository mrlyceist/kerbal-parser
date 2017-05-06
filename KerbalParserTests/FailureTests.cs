using System;
using KerbalParser;
using NUnit.Framework;

namespace KerbalParserTests
{
    [TestFixture]
    public class FailureTests
    {
        [Test]
        [ExpectedException(
            typeof (ParseErrorException),
            ExpectedMessage = "matching bracket",
            MatchType = MessageMatch.Contains,
            Handler = "HandlerMethod")]
        public void MismatchBrackets()
        {
            const string file = "..\\..\\testdata\\fail\\mismatch.cfg";
            var parser = new Parser();
            parser.ParseConfig(file);
        }

        public void HandlerMethod(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}