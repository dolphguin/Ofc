namespace Ofc.Core.Algorithm.Raw
{
    using System;
    using System.IO;
    using Core;

    public class RawAlgorithm : IAlgorithm<string>
    {
        public string Id { get; }
        public string Name { get; }
        public Version Version { get; }
        public bool SupportsDimension(int width, int height)
        {
            return true;
        }

        public IReporter<string> Compress(IFile target, IConfiguaration configuaration, Stream output, int width, int height)
        {
            return new RawReporter(output);
        }

        public void Decompress(IFile target, IConfiguaration configuaration, Stream input, IReporter<string> reporter, int width)
        {

        }
    }
}
