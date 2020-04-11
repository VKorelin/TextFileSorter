using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileGenerator.Configuration;
using FileGenerator.IO;

namespace FileGenerator.Generation
{
    internal sealed class ChunkGenerationJob : IChunkGenerationJob
    {
        private readonly IFileNameProvider _fileNameProvider;
        private readonly Func<string, IFileWriter> _fileWriterFactory;
        private readonly Func<IChunkGenerator> _chunkGeneratorFactory;

        private readonly BlockingCollection<long> _generateQueue;
        private readonly BlockingCollection<string> _writeQueue;
        private readonly CancellationTokenSource _generateCancellationTokenSource;
        private readonly CancellationTokenSource _writeCancellationTokenSource;
        
        private readonly Task _writeTask;
        private readonly Task[] _generateTasks;

        private bool isStarted;
        
        public ChunkGenerationJob(
            IFileNameProvider fileNameProvider,
            IConfigurationProvider configurationProvider,
            Func<string, IFileWriter> fileWriterFactory,
            Func<IChunkGenerator> chunkGeneratorFactory)
        {
            _fileNameProvider = fileNameProvider;
            _fileWriterFactory = fileWriterFactory;
            _chunkGeneratorFactory = chunkGeneratorFactory;

            var threadsCount = configurationProvider.ThreadsCount - 2;
            
            _generateQueue = new BlockingCollection<long>(new ConcurrentBag<long>(), threadsCount);
            _writeQueue = new BlockingCollection<string>(new ConcurrentBag<string>(), threadsCount);

            _generateCancellationTokenSource = new CancellationTokenSource();
            _writeCancellationTokenSource = new CancellationTokenSource();
            
            _generateTasks = new Task[threadsCount];
            for (var i = 0; i < threadsCount; i++)
            {
                _generateTasks[i] = new Task(() => GenerateChunk(_generateCancellationTokenSource.Token));
            }

            _writeTask = new Task(() => WriteToFile(_writeCancellationTokenSource.Token));
        }
        
        ///<inheritdoc/>
        public void Start()
        {
            foreach (var generateTask in _generateTasks)
            {
                generateTask.Start();
            }
            
            _writeTask.Start();

            isStarted = true;
        }

        ///<inheritdoc/>
        public void AddNext(long chunkSize)
        {
            if (!isStarted)
                throw new InvalidOperationException("Job should be started");
            
            _generateQueue.Add(chunkSize);
        }

        ///<inheritdoc/>
        public void Stop()
        {
            if (!isStarted)
                throw new InvalidOperationException("Job should be started");
            
            _generateCancellationTokenSource.Cancel();
            Task.WaitAll(_generateTasks);

            _writeCancellationTokenSource.Cancel();
            _writeTask.Wait();

            isStarted = false;
        }

        private void GenerateChunk(CancellationToken token)
        {
            var chunkGenerator = _chunkGeneratorFactory.Invoke();

            while (!token.IsCancellationRequested || _generateQueue.Any())
            {
                if (!_generateQueue.TryTake(out var chunkSize, 1000))
                    continue;
                
                var chunk = chunkGenerator.GenerateNext(chunkSize);
                _writeQueue.Add(chunk);
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
    }
}