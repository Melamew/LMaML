//using System;
//using LMaML.Infrastructure;
//using LMaML.Infrastructure.Audio;
//using LMaML.Infrastructure.Domain;
//using LMaML.Infrastructure.Domain.Concrete;
//using LMaML.Infrastructure.Util;
//using iLynx.Common;

//namespace LMaML.NHibernate
//{
//    /// <summary>
//    /// TaggedFileProxy
//    /// </summary>
//    internal class TaggedFileProxy : StorableTaggedFile
//    {
//        private readonly StorableTaggedFile source;
//        public TaggedFileProxy(StorableTaggedFile source)
//        {
//            source.Guard("source");
//            this.source = source;
//        }

//        /// <summary>
//        /// Gets the source.
//        /// </summary>
//        /// <value>
//        /// The source.
//        /// </value>
//        public StorableTaggedFile Source { get { return source; } }

//        public Guid Id { get { return source.Id; } set { source.Id = value; } }
//        public uint TrackNo { get { return source.TrackNo; } set { source.TrackNo = value; } }
//        public string Comment { get { return source.Comment; } set { source.Comment = value; } }
//        public IYear Year
//        {
//            get { return source.Year; }
//            set
//            {

//            }
//        }
//        public IGenre Genre
//        {
//            get { return source.Genre; }
//            set
//            {

//            }
//        }
//        public IAlbum Album
//        {
//            get { return source.Album; }
//            set
//            {

//            }
//        }
//        public IArtist Artist
//        {
//            get { return source.Artist; }
//            set
//            {

//            }
//        }
//        public ITitle Title { get { return source.Title; } set { source.Title = value; } }
//        public string Filename { get { return source.Filename; } set { source.Filename = value; } }
//        public ID3Format TagFormat { get { return source.TagFormat; } set { source.TagFormat = value; } }
//    }
//}