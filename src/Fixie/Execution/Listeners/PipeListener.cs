﻿namespace Fixie.Execution.Listeners
{
    using System;
    using System.IO.Pipes;
    using Execution;

    public class PipeListener :
        Handler<MethodDiscovered>,
        Handler<CaseSkipped>,
        Handler<CasePassed>,
        Handler<CaseFailed>
    {
        readonly NamedPipeClientStream pipe;

        public PipeListener(NamedPipeClientStream pipe)
        {
            this.pipe = pipe;
        }

        public void Handle(MethodDiscovered message)
        {
            var testName = new TestName(message.Method);

            Write(new PipeMessage.Test
            {
                Class = testName.Class,
                Method = testName.Method,
                Name = testName.FullName
            });
        }

        public void Handle(CaseSkipped message)
        {
            Write<PipeMessage.SkipResult>(message, x =>
            {
                x.Reason = message.Reason;
            });
        }

        public void Handle(CasePassed message)
        {
            Write<PipeMessage.PassResult>(message);
        }

        public void Handle(CaseFailed message)
        {
            Write<PipeMessage.FailResult>(message, x =>
            {
                x.Exception = new PipeMessage.Exception(message.Exception);
            });
        }

        void Write<TTestResult>(CaseCompleted message, Action<TTestResult> customize = null)
            where TTestResult : PipeMessage.TestResult, new()
        {
            var testName = new TestName(message.Method);

            var result = new TTestResult
            {
                Class = testName.Class,
                Method = testName.Method,
                Name = message.Name,
                Duration = message.Duration,
                Output = message.Output
            };

            customize?.Invoke(result);

            Write(result);
        }

        void Write<T>(T message) => pipe.Send(message);
    }
}