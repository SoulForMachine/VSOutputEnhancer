﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Balakin.VSOutputEnhancer.Parsers;
using Balakin.VSOutputEnhancer.Tests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;

namespace Balakin.VSOutputEnhancer.Tests.UnitTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ParsedDataTests
    {
        [TestMethod]
        public void Create()
        {
            var span = Utils.CreateSpan("Text");

            var match = Regex.Match(span.GetText(), "(?<Message>.*)");
            var parsedData = ParsedData.Create<ParsedDataStub>(match, span.Span);
            Assert.IsNotNull(parsedData);
            Assert.IsInstanceOfType(parsedData, typeof(ParsedDataStub));
            Assert.IsTrue(parsedData.Message.HasValue);
            Assert.AreEqual("Text", parsedData.Message);
            Assert.AreEqual(span.Span, parsedData.Message.Span);
            Assert.IsFalse(parsedData.Type.HasValue);

            span = Utils.CreateSpan("Text Error");

            match = Regex.Match(span.GetText(), "(?<Message>.*) (?<Type>.*)");
            parsedData = ParsedData.Create<ParsedDataStub>(match, span.Span);
            Assert.IsNotNull(parsedData);
            Assert.IsInstanceOfType(parsedData, typeof(ParsedDataStub));
            Assert.IsTrue(parsedData.Message.HasValue);
            Assert.AreEqual("Text", parsedData.Message);
            Assert.AreEqual(new Span(0, 4), parsedData.Message.Span);
            Assert.IsTrue(parsedData.Type.HasValue);
            Assert.AreEqual(TraceEventType.Error, parsedData.Type);
            Assert.AreEqual(new Span(5, 5), parsedData.Type.Span);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EmptyValueExceptionValueType()
        {
            var value = new ParsedValue<Int32>();
            // Trying to cast to Int32
            // ReSharper disable once UnusedVariable
            var i = (Int32) value;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EmptyValueExceptionReferenceType()
        {
            var value = new ParsedValue<String>();
            // Trying to cast to String
            // ReSharper disable once UnusedVariable
            var s = (String) value;
        }
    }
}