using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileGenerator.Domain;
using FileGenerator.IO;

namespace FileGenerator.Generation
{
    ///<inheritdoc/>
    internal sealed class ChunkGenerator : IChunkGenerator
    {
        private readonly Func<IRandomNumberGenerator> _randomNumberGeneratorFactory;
        private readonly Func<IRandomStringGenerator> _randomStringGeneratorFactory;
        private readonly Func<string, IFileWriter> _fileWriterFactory;
        private readonly Func<IChunkInfoBuilder> _chunkInfoBuilderFactory;
        private readonly IFileNameProvider _fileNameProvider;

        private readonly BlockingCollection<long> _generateQueue;
        private readonly BlockingCollection<string> _writeQueue;
        private readonly CancellationTokenSource _generateCancellationTokenSource;
        private readonly CancellationTokenSource _writeCancellationTokenSource;

        private readonly Task _writeTask;
        private readonly Task[] _generateTasks;

        public ChunkGenerator(
            IFileNameProvider fileNameProvider,
            Func<IChunkInfoBuilder> chunkInfoBuilderFactory,
            Func<string, IFileWriter> fileWriterFactory,
            Func<IRandomNumberGenerator> randomNumberGeneratorFactory,
            Func<IRandomStringGenerator> randomStringGeneratorFactory)
        {
            _randomNumberGeneratorFactory = randomNumberGeneratorFactory;
            _randomStringGeneratorFactory = randomStringGeneratorFactory;
            _chunkInfoBuilderFactory = chunkInfoBuilderFactory;
            _fileWriterFactory = fileWriterFactory;
            _fileNameProvider = fileNameProvider;

            var threadsCount = Environment.ProcessorCount - 2;
            
            _generateQueue = new BlockingCollection<long>(new ConcurrentBag<long>(), threadsCount);
            _writeQueue = new BlockingCollection<string>(new ConcurrentBag<string>(), threadsCount);

            _generateCancellationTokenSource = new CancellationTokenSource();
            _writeCancellationTokenSource = new CancellationTokenSource();
            
            _generateTasks = new Task[threadsCount];
            for (var i = 0; i < threadsCount; i++)
            {
                _generateTasks[i] = Task.Run(() => GenerateNextChunk(_generateCancellationTokenSource.Token));
            }

            _writeTask = Task.Run(() => WriteToFile(_writeCancellationTokenSource.Token));
        }

        ///<inheritdoc/>
        public void GenerateNext(long chunkSize) => _generateQueue.Add(chunkSize);

        private void GenerateNextChunk(CancellationToken token)
        {
            var numberGenerator = _randomNumberGeneratorFactory.Invoke();
            var stringGenerator = _randomStringGeneratorFactory.Invoke();
            var chunkInfoBuilder = _chunkInfoBuilderFactory.Invoke();
            var builder = new StringBuilder();

            static int GenerateNumber(int length, IRandomNumberGenerator generator)
                => generator.Generate((int) Math.Pow(10, length - 1), (int) Math.Pow(10, length));

            while (!token.IsCancellationRequested || _generateQueue.Any())
            {
                if (!_generateQueue.TryTake(out var chunkSize, 1000))
                    continue;

                var chunkInfo = chunkInfoBuilder.Build(chunkSize);
                builder.Clear();

                // Generate first entry that string will be repeated
                var firstNumber = GenerateNumber(chunkInfo.RepeatedEntry.NumberLength, numberGenerator);
                var lineToRepeat = stringGenerator.Generate(chunkInfo.RepeatedEntry.LineLength);
                builder.AppendLine(EntryInfo.BuildEntry(firstNumber, lineToRepeat));

                foreach (var entryInfo in chunkInfo.EntryInfos)
                {
                    var number = GenerateNumber(entryInfo.NumberLength, numberGenerator);
                    var line = stringGenerator.Generate(entryInfo.LineLength);
                    builder.AppendLine(EntryInfo.BuildEntry(number, line));
                }

                // Generate last entry with repeated string
                var lastNumber = GenerateNumber(chunkInfo.RepeatedEntry.NumberLength, numberGenerator);
                builder.AppendLine(EntryInfo.BuildEntry(lastNumber, lineToRepeat));

                _writeQueue.Add(builder.ToString());
            }
        }

        private void WriteToFile(CancellationToken token)
        {
            using var writer = _fileWriterFactory.Invoke(_fileNameProvider.GetPath());

            while (!token.IsCancellationRequested || _writeQueue.Any())
            {
                if (!_writeQueue.TryTake(out var nextChunk, 1000))
                    continue;
                writer.Write(nextChunk);
            }
        }

        /// <summary>
        /// Stop and wait generation and writing jobs
        /// </summary>
        public void Dispose()
        {
            _generateCancellationTokenSource.Cancel();
            Task.WaitAll(_generateTasks);

            _writeCancellationTokenSource.Cancel();
            _writeTask.Wait();
        }
    }
}