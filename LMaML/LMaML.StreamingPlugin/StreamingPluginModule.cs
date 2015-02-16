using System;
using System.Collections.Generic;
using System.IO;
using iLynx.Common;
using iLynx.Networking.Interfaces;
using iLynx.Threading;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain.Concrete;
using Microsoft.Practices.Unity;

namespace LMaML.StreamingPlugin
{
    public enum MessageType
    {
        StreamCreate = 0,
        StreamDestroy = 1,
        StreamInfo = 2,
        StreamGetInfo = 3,
        StreamSegment = 4,
        StreamGetSegment = 5,
        StreamSeek = 6,
    }

    public class StreamInfo
    {
        public long Length { get; set; }
        public int SegmentSize { get; set; }
    }

    public class StreamSegment
    {
        public int Offset { get; set; }
        public byte[] Data { get; set; }
    }

    public class AudioMessage : IKeyedMessage<MessageType>
    {
        public MessageType Key { get; set; }
        public int StreamId { get; set; }
        public byte[] Data { get; set; }
    }

    public class StreamingPluginModule : AudioModuleBase
    {
        public StreamingPluginModule(IUnityContainer container)
            : base(container)
        {
        }

        protected override void RegisterTypes()
        {
            base.RegisterTypes();
        }

        protected override IAudioPlayer GetPlayer(out Guid storageType)
        {
            storageType = StorageTypes.StreamedFile;
            return Container.Resolve<StreamingAudioPlayer>();
        }
    }

    public abstract class ConnectionStreamBase : Stream
    {
        private readonly IConnection<AudioMessage, MessageType> connection;
        private readonly int streamId;
        private long length;
        private long streamPosition;
        private long nextOffset;
        private readonly SortedList<long, StreamSegment> segments = new SortedList<long, StreamSegment>();
        private int segmentSize;

        protected ConnectionStreamBase(IConnection<AudioMessage, MessageType> connection, int streamId)
        {
            this.connection = Guard.IsNull(() => connection);
            this.streamId = streamId;
        }

        public void HandleSegment(StreamSegment segment)
        {
            segments.Add(segment.Offset, segment);
        }

        public virtual void SetStreamInfo(StreamInfo info)
        {
            length = info.Length;
            segmentSize = info.SegmentSize;
        }

        public abstract StreamSegment GetSegment(int segment);

        public abstract StreamInfo GetStreamInfo();

        public int StreamId { get { return streamId; } }

        public override void Flush()
        {

        }

        /// <summary>
        /// When overridden in a derived class, sets the streamPosition within the current stream.
        /// </summary>
        /// <returns>
        /// The new streamPosition within the current stream.
        /// </returns>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter. </param><param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new streamPosition. </param><exception cref="T:System.IO.IOException">An I/O error occurs. </exception><exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output. </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>1</filterpriority>
        public override long Seek(long offset, SeekOrigin origin)
        {
            var absolutePosition = origin == SeekOrigin.Begin
                ? offset
                : (origin == SeekOrigin.End) ? length - offset : Position + offset;
            connection.Send(new AudioMessage());
            return absolutePosition;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return length; }
        }

        public override long Position
        {
            get { return streamPosition; }
            set { Seek(value, SeekOrigin.Begin); }
        }
    }

    public class LocalConnectionStream : ConnectionStreamBase
    {
        private readonly Stream sourceStream;
        private const int segmentSize = 128;

        public LocalConnectionStream(IConnection<AudioMessage, MessageType> connection, Stream sourceStream, int streamId)
            : base(connection, streamId)
        {
            this.sourceStream = sourceStream;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override StreamSegment GetSegment(int segment)
        {
            throw new NotImplementedException();
        }

        public override StreamInfo GetStreamInfo()
        {
            throw new NotImplementedException();
        }
    }

    public class RemoteConnectionStream : ConnectionStreamBase
    {
        public RemoteConnectionStream(IConnection<AudioMessage, MessageType> connection, int streamId) : base(connection, streamId)
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override StreamSegment GetSegment(int segment)
        {
            throw new NotImplementedException();
        }

        public override StreamInfo GetStreamInfo()
        {
            throw new NotImplementedException();
        }
    }

    public class StreamingAudioServer
    {
        private readonly IThreadManager threadManager;
        private readonly IConnectionListener<AudioMessage, MessageType> connectionListener;
        private readonly IWorker connectionWorker;
        private volatile bool isRunning;

        public StreamingAudioServer(IConnectionListener<AudioMessage, MessageType> connectionListener,
            IThreadManager threadManager)
        {
            this.threadManager = threadManager;
            this.connectionListener = Guard.IsNull(() => connectionListener);
            connectionWorker = this.threadManager.StartNew(AcceptConnections);
        }

        private void AcceptConnections()
        {
            while (isRunning)
            {
                var connection = connectionListener.AcceptNext();
                if (null == connection) continue;
                OnEstablished(connection);
            }
        }

        private void OnEstablished(IConnection<AudioMessage, MessageType> connection)
        {
            connection.Subscribe(MessageType.StreamCreate, OnStreamCreate);
            connection.Subscribe(MessageType.StreamDestroy, OnStreamDestroy);
        }

        private void OnStreamDestroy(AudioMessage keyedmessage, int totalsize)
        {

        }

        private void OnStreamCreate(AudioMessage keyedMessage, int totalSize)
        {

        }
    }

    public class StreamingAudioPlayer : IAudioPlayer
    {
        public ITrack CreateChannel(StorableTaggedFile file)
        {
            return null;
        }

        public void LoadPlugins(string dir)
        {

        }
    }
}
