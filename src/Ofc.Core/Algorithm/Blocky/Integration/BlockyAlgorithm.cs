﻿namespace Ofc.Core.Algorithm.Blocky.Integration
{
    using System;
    using System.IO;
    using Algorithm.Integration;
    using Core;

    /// <summary>
    /// Implemented the IAlgorithm so that the parser can access algorithm functions
    /// </summary>
    public class BlockyAlgorithm : IAlgorithm<OfcNumber>
    {
        public string Id => "BLKY";

        public string Name => "Blocky";

        public Version Version => new Version(0, 1);

        public IReporter<OfcNumber> Compress(IFile file, IConfiguaration config, Stream writer, int width, int elements)
        {
            if (width == 1)
                return new BlockyCompression(elements, writer, config);

            var compressions = new IReporter<OfcNumber>[width];
            for (var i = 0; i < compressions.Length; i++)
                compressions[i] = new BlockyCompression(elements, writer, config);
            return new CompressionSplitter(compressions);
        }


        public void Decompress(IFile file, IConfiguaration config, Stream reader, IReporter<OfcNumber> reporter, int width)
        {
            if (width == 0)
            {
                new BlockyDecompression(reader, reporter).Decompress();
                return;
            }

            var decomp = new BlockyNumberSaver[width];

            for (var i = 0; i < decomp.Length; i++)
            {
                decomp[i] = new BlockyNumberSaver();
                var blockyDecomp = new BlockyDecompression(reader, decomp[i]);
                decomp[i].Initialize(blockyDecomp.Metadata.ValueCount);
                blockyDecomp.Decompress();
            }

            var len = decomp[0].Values.Length;

            for (var i = 0; i < len; i++)
                for (var j = 0; j < decomp.Length; j++)
                    reporter.Report(decomp[j].Values[i]);

        }

        public bool SupportsDimension(int width, int elements)
        {
            return width > 0;
        }

        public static void SetBlockfindingDebugConsoleEnabled(bool enabled)
        {
#if DEBUG //Todo: Also compile on release, make debug console an option in the args parser
            Blockfinding.Blockfinding.SetDebugEnabled(enabled);
#endif
        }
    }
}