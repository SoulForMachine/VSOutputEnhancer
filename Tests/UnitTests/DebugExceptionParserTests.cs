﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Balakin.VSOutputEnhancer.Parsers.DebugException;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;

namespace Balakin.VSOutputEnhancer.Tests.UnitTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DebugExceptionParserTests
    {
        [TestMethod]
        public void NotParsed()
        {
            const String messageString = "Some message\r\n";
            const String messageString2 = "Exception thrown: 'blablabla\r\n";
            const String messageString3 = "A first chance exception of type 'blablabla\r\n";

            var span = Utils.CreateSpan(messageString);
            var parser = new DebugExceptionParser();
            DebugExceptionData parsedData;
            var parsed = parser.TryParse(span, out parsedData);
            Assert.IsFalse(parsed);
            Assert.IsNull(parsedData);

            span = Utils.CreateSpan(messageString2);
            parser = new DebugExceptionParser();
            parsed = parser.TryParse(span, out parsedData);
            Assert.IsFalse(parsed);
            Assert.IsNull(parsedData);

            span = Utils.CreateSpan(messageString3);
            parser = new DebugExceptionParser();
            parsed = parser.TryParse(span, out parsedData);
            Assert.IsFalse(parsed);
            Assert.IsNull(parsedData);
        }

        [TestMethod]
        public void Exception()
        {
            const String messageString = "Exception thrown: 'System.Exception' in VSOutputEnhancerDemo.exe\r\n";

            var span = Utils.CreateSpan(messageString);
            var parser = new DebugExceptionParser();
            DebugExceptionData data;
            var parsed = parser.TryParse(span, out data);
            Assert.IsTrue(parsed);
            Assert.IsNotNull(data);

            Assert.IsTrue(data.Exception.HasValue);
            Assert.IsTrue(data.Assembly.HasValue);

            Assert.AreEqual("System.Exception", data.Exception);
            Assert.AreEqual("VSOutputEnhancerDemo.exe", data.Assembly);

            Assert.AreEqual(new Span(19, 16), data.Exception.Span);
            Assert.AreEqual(new Span(40, 24), data.Assembly.Span);
        }

        [TestMethod]
        public void ExceptionVS2013()
        {
            const String messageString = "A first chance exception of type 'System.Exception' occurred in ConsoleDemo.exe\r\n";

            var span = Utils.CreateSpan(messageString);
            var parser = new DebugExceptionParser();
            DebugExceptionData data;
            var parsed = parser.TryParse(span, out data);
            Assert.IsTrue(parsed);
            Assert.IsNotNull(data);

            Assert.IsTrue(data.Exception.HasValue);
            Assert.IsTrue(data.Assembly.HasValue);

            Assert.AreEqual("System.Exception", data.Exception);
            Assert.AreEqual("ConsoleDemo.exe", data.Assembly);

            Assert.AreEqual(new Span(34, 16), data.Exception.Span);
            Assert.AreEqual(new Span(64, 15), data.Assembly.Span);
        }
    }
}