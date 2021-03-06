﻿namespace Ofc.Core.Algorithm.Blocky
{
    using System;
    using System.IO;
    using Algorithm.Integration;
    using JetBrains.Annotations;
    using Method;
    using Method.FloatSimmilar;
    using Method.NumbersNoExp;
    using Method.PatternOffset;
    using Method.PatternPingPong;
    using Method.PatternSame;
    using Util;

    class BlockyDecompression : IOfcNumberWriter
    {
        private readonly StreamBitReader _bitReader;
        private readonly IReporter<OfcNumber> _writer;
        public readonly BlockyMetadata Metadata;
        private readonly DecompressionMethod[] _decompressionMethods = new DecompressionMethod[(int)Blockfinding.Blockfinding.Methods.Count];
        private readonly IOfcNumberWriter _numberWriter;

        public BlockyDecompression([NotNull]Stream reader, [NotNull]IReporter<OfcNumber> target) : this(reader)
        {
            _writer = target;
            _numberWriter = this;
        }

        public BlockyDecompression([NotNull]Stream reader, [NotNull]IOfcNumberWriter writer) : this(reader)
        {
            _numberWriter = writer;
        }

        protected BlockyDecompression([NotNull]Stream reader)
        {
            _bitReader = new StreamBitReader(reader);
            Metadata = BlockyMetadata.FromBitStream(_bitReader);

            _decompressionMethods[(int)Blockfinding.Blockfinding.Methods.PatternSame] = new PatternSameDecompression(Metadata);
            _decompressionMethods[(int)Blockfinding.Blockfinding.Methods.PatternPingPong] = new PatternPingPongDecompression(Metadata);
            _decompressionMethods[(int)Blockfinding.Blockfinding.Methods.FloatSimmilar] = new FloatSimmilarDecompression(Metadata);
            _decompressionMethods[(int)Blockfinding.Blockfinding.Methods.NumbersNoExp] = new NumbersNoExpDecompression(Metadata);
            _decompressionMethods[(int)Blockfinding.Blockfinding.Methods.PatternOffset] = new PatternOffsetDecompression(Metadata);

            if (_bitReader.ReadByte(1) > 0) // use huffman
            {
                throw new NotImplementedException();
            }
        }


        public void Decompress()
        {
            var valueCount = 0;
            while (valueCount < Metadata.ValueCount)
            {
                if (_bitReader.ReadByte(1) > 0) // isBlock
                {
                    var block = DecompressionMethod.ReadDefaultBlockHeader(_bitReader, Metadata);
                    var method = GetMethodForBlock(block); // Get decompressor class for block type
                    valueCount += method.Read(_numberWriter, block, _bitReader);
                }
                else
                {
                    _numberWriter.Write(DecompressionMethod.ReadSingleValueWithoutControlBit(_bitReader, Metadata));
                    valueCount++;
                }
            }
            _writer?.Flush();
        }

        private DecompressionMethod GetMethodForBlock(Block block)
        {
            if (!block.HasPattern) return block.HasExponent ? _decompressionMethods[(int)Blockfinding.Blockfinding.Methods.FloatSimmilar] : _decompressionMethods[(int)Blockfinding.Blockfinding.Methods.NumbersNoExp];
            switch (block.Pattern)
            {
                case Block.PatternType.Same:
                    return _decompressionMethods[(int)Blockfinding.Blockfinding.Methods.PatternSame];
                case Block.PatternType.Offset:
                    return _decompressionMethods[(int)Blockfinding.Blockfinding.Methods.PatternOffset];
                case Block.PatternType.Pingpong:
                    return _decompressionMethods[(int)Blockfinding.Blockfinding.Methods.PatternPingPong];
                case Block.PatternType.Reserved:
                    throw new NotImplementedException("Invalid pattern type: " + block.Pattern);
                default:
                    throw new NotImplementedException("Pattern type not implemented!");
            }
        }

        //public bool CheckIntegrity()
        //{

        //}

        public void Write(OfcNumber value)
        {
            _writer.Report(value);
        }
    }
}
