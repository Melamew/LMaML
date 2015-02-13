using System;
using System.Collections.Generic;
using System.IO;
using iLynx.Common;
using iLynx.Networking.Interfaces;
using iLynx.Serialization;
using iLynx.Threading;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain.Concrete;
using Microsoft.Practices.Unity;

namespace LMaML.StreamingPlugin
{
    public enum MessageType
    {
        StreamSegment,
        StreamCreate,
        StreamDestroy,
        StreamGetInfo,
        StreamInfo,
    }

    public class StreamInfo
    {
        public long Length { get; set; }
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

    public class RemoteConnectionStream : Stream
    {
        private readonly IConnection<AudioMessage, MessageType> connection;
        private readonly int streamId;
        private long length;
        private long streamPosition;
        private long nextOffset;
        private readonly Dictionary<long, StreamSegment> segments = new Dictionary<long, StreamSegment>();

        public RemoteConnectionStream(IConnection<AudioMessage, MessageType> connection, int streamId)
        {
            this.connection = connection;
            this.streamId = streamId;
        }

        public void HandleSegment(StreamSegment segment)
        {
            segments.Add(segment.Offset, segment);
        }

        public void HandleInfo(StreamInfo info)
        {
            length = info.Length;
        }

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

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source. </param><param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream. </param><param name="count">The maximum number of bytes to be read from the current stream. </param><exception cref="T:System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length. </exception><exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative. </exception><exception cref="T:System.IO.IOException">An I/O error occurs. </exception><exception cref="T:System.NotSupportedException">The stream does not support reading. </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>1</filterpriority>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        private StreamSegment FindSegment(int position)
        {
            return new StreamSegment();
        }

        private byte[] ReadSegments(int byteLength)
        {
            var result = new byte[byteLength];
            while (byteLength > 0)
            {

            }
            return result;
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
