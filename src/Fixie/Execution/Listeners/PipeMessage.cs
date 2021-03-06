﻿namespace Fixie.Execution.Listeners
{
    using System;

    public static class PipeMessage
    {
        public class DiscoverTests { }

        public class ExecuteTests
        {
            public Test[] Filter { get; set; }
        }

        public class Test
        {
            public string Class { get; set; }
            public string Method { get; set; }
            public string Name { get; set; }
        }

        public abstract class TestResult
        {
            public string Class { get; set; }
            public string Method { get; set; }
            public string Name { get; set; }
            public TimeSpan Duration { get; set; }
            public string Output { get; set; }
        }

        public class SkipResult : TestResult
        {
            public string Reason { get; set; }
        }

        public class PassResult : TestResult
        {
        }

        public class FailResult : TestResult
        {
            public Exception Exception { get; set; }
        }

        public class Exception
        {
            public Exception()
            {
            }

            public Exception(System.Exception exception)
            {
                Type = exception.TypeName();
                Message = exception.Message;
                StackTrace = exception.LiterateStackTrace();
            }

            public string Type { get; set; }
            public string Message { get; set; }
            public string StackTrace { get; set; }
        }

        public class Completed { }
    }
}