﻿namespace Ofc.Actions
{
    using System;
    using System.IO;
    using System.Text;
    using Ofc.Algorithm.Blocky.Integration;
    using Ofc.Algorithm.Integration;
    using Ofc.CLI;
    using Ofc.IO;
    using Ofc.LZMA.Helper;

    internal class DecompressAction : IOfcAction // todo
    {
        public string Code => "DCOM";

        public string Message { get; set; }

        public bool Faulty => _faulty;

        public string Path => _relativePath;

        public int Status { get; set; } = -1;

        public OfcActionResult Result => _result;

        public bool Force { get; set; }


        private string _metaPath;
        private string _dataPath;
        private bool _isLzma;
        private string _destination;
        private string _relativePath;

        private bool _hasData;

        private bool _faulty;
        private OfcActionResult _result = OfcActionResult.Done;


        public DecompressAction(string basePath, string metaPath, string dataPath, bool isLzma, string destination)
        {
            _metaPath = metaPath;
            _dataPath = dataPath;
            _isLzma = isLzma;
            _destination = destination;
            _relativePath = basePath != null && _metaPath.StartsWith(basePath) ? _metaPath.Substring(basePath.Length) : _metaPath;
        }


        public void Preperation()
        {
            if (!File.Exists(_metaPath)) Throw<FileNotFoundException>("Could not find the meta file.");
            _hasData = File.Exists(_dataPath);
            if (!Force && File.Exists(_destination)) Throw<Exception>("The destination file does already exist.");
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_destination));
        }

        public void Conduction()
        {
            try
            {
                if (_isLzma)
                {
                    Status = 1;
                    using (var source = File.OpenRead(_metaPath))
                    {
                        Status = 2;
                        using (var destination = File.OpenWrite(_destination))
                        {
                            Status = 3;
                            LzmaHelper.DecompressFileLzma(source, destination);
                            Status = 4;
                        }
                    }
                    Status = 5;
                }
                else
                {
                    Status = 100;
                    using (var source = File.OpenRead(_metaPath))
                    {
                        Status = 101;
                        using (var destination = File.OpenWrite(_metaPath + ActionUtils.TempFileExtention))
                        {
                            Status = 102;
                            LzmaHelper.DecompressFileLzma(source, destination);
                            Status = 103;
                        }
                    }

                    Status = 104;
                    if (_hasData)
                    {
                        Status = 105;
                        using (var source = File.OpenRead(_dataPath))
                        {
                            Status = 106;
                            using (var destination = File.OpenWrite(_dataPath + ActionUtils.TempFileExtention))
                            {
                                Status = 107;
                                LzmaHelper.DecompressFileLzma(source, destination);
                                Status = 108;
                            }
                        }
                    }

                    Status = 109;
                    using (var output = File.OpenWrite(_destination) )
                    {
                        Status = 110;
                        using (var outputText = new StreamWriter(output))
                        {
                            Status = 111;
                            var algorithm = new BlockyAlgorithm();
                            var converter = new CompressionDataConverter();

                            Status = 112;
                            using (var meta = File.OpenText(_metaPath + ActionUtils.TempFileExtention))
                            {
                                Status = 113;
                                if (_hasData)
                                {
                                    Status = 114;
                                    using (var data = File.OpenRead(_dataPath + ActionUtils.TempFileExtention))
                                    {
                                        Status = 115;
                                        using (var reader = new MarerReader<OfcNumber>(meta, outputText, algorithm, converter, data))
                                        {
                                            Status = 116;
                                            reader.Do();
                                            Status = 117;
                                        }
                                        Status = 118;
                                    }
                                    Status = 119;
                                }
                                else
                                {
                                    Status = 130;
                                    using (var reader = new MarerReader<OfcNumber>(meta, outputText, algorithm, converter, null))
                                    {
                                        Status = 131;
                                        reader.Do();
                                        Status = 132;
                                    }
                                    Status = 133;
                                }
                            }
                            Status = 140;
                        }
                        Status = 141;
                    }
                    Status = 142;
                }
            }
            catch (Exception)
            {
                Message = "?";
                throw;
            }
        }

        public void Cleanup()
        {
            if (Status >= 102 && File.Exists(_metaPath + ActionUtils.TempFileExtention)) File.Delete(_metaPath + ActionUtils.TempFileExtention);
            if (_hasData && Status >= 107 && File.Exists(_dataPath + ActionUtils.TempFileExtention)) File.Delete(_dataPath + ActionUtils.TempFileExtention);
        }


        private void Throw<T>(string message) where T : Exception, new()
        {
            Message = message;
            _faulty = true;
            throw new T();
        }
    }
}