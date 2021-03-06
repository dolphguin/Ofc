﻿namespace Ofc.Core.Algorithm.Raw
{
    using System.IO;

    public class RawReporter : IReporter<string>
    {
        private readonly StreamWriter _streamWriter;

        public RawReporter(Stream stream)
        {
            _streamWriter = new StreamWriter(stream);
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
        }

        public IConfiguaration Configuaration { get; }
        public void Finish()
        {
            _streamWriter.Flush();
        }

        public void Flush()
        {
            _streamWriter.Flush();
        }

        public void Report(string value)
        {
            _streamWriter.WriteLine(value);
        }

        public void Report(string[] values, int offset, int amount)
        {
            for (var i = offset; i < amount + offset; i++)
            {
                Report(values[i]);
            }
        }
    }
}
