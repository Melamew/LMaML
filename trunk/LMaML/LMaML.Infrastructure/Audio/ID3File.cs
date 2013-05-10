using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using iLynx.Common;
using File = TagLib.File;

namespace LMaML.Infrastructure.Audio
{
    //public delegate void SetTrackInfoDelegate(ID3File file);

    /// <summary>
    ///  ID3File
    /// </summary>
    public class ID3File
    {
        //private FileInfo fInfo;
        private File file;
        private string filename;
        //private ID3Format format;
        private bool isValid;

        public ID3File(string filename)//, IAudioFormat format)
        {
            filename.GuardString("filename");
            LoadFile(filename);
        }

        /// <summary>
        ///     Gets the filename of the current mp3 file
        /// </summary>
        public string Filename
        {
            get { return filename; }
            set { LoadFile(value); }
        }

        /// <summary>
        ///     Gets a value indicating whether or not this file is considered as bein "valid"
        /// </summary>
        public bool IsValid
        {
            get { return isValid; }
        }

        /// <summary>
        ///     Gets or Sets the Artist of the underlying mp3 file
        /// </summary>
        public string Artist
        {
            get
            {
                var artist = file.Tag.FirstPerformer ?? "";
                return artist.Replace("\0", "");
            }
            set
            {
                if (file.Tag.Performers == null)
                    file.Tag.Performers = new[] { value };
                else if (file.Tag.Performers.Length < 1)
                    file.Tag.Performers = new[] { value };
                else
                {
                    var preList = new List<string> { value };
                    preList.AddRange(file.Tag.Performers);
                    file.Tag.Performers = preList.ToArray();
                }
            }
        }

        /// <summary>
        ///     Gets or Sets the Title of the underlying mp3 file
        /// </summary>
        public string Title
        {
            get
            {
                string title = file.Tag.Title ?? "";
                return title.Replace("\0", "");
            }
            set { file.Tag.Title = value; }
        }

        /// <summary>
        ///     Gets or Sets the Album of the underlying mp3 file
        /// </summary>
        public string Album
        {
            get
            {
                var album = file.Tag.Album ?? "";
                return album.Replace("\0","");
            }
            set { file.Tag.Album = value; }
        }

        /// <summary>
        ///     Gets or Sets the Genre of the underlying mp3 file
        /// </summary>
        public string Genre
        {
            get
            {
                var genre = file.Tag.FirstGenre ?? "";
                return genre.Replace("\0","");
            }
            set
            {
                if (file.Tag.Genres == null)
                    file.Tag.Genres = new[] { value };
                else if (file.Tag.Genres.Length < 1)
                    file.Tag.Genres = new[] { value };
                else
                {
                    var preList = new ArrayList();
                    preList.AddRange(file.Tag.Genres);
                    preList.Add(value);
                    file.Tag.Genres = (string[])preList.ToArray(typeof(string));
                }
            }
        }

        /// <summary>
        ///     Gets or Sets the Year of the underlying mp3 file
        /// </summary>
        public uint Year
        {
            get { return file.Tag.Year; }
            set { file.Tag.Year = value; }
        }

        /// <summary>
        ///     Gets or Sets the comment of the underlying mp3 file
        /// </summary>
        public string Comment
        {
            get
            {
                var comment = file.Tag.Comment ?? "";
                return comment.Replace("\0","");
            }
            set { file.Tag.Comment = value; }
        }

        /// <summary>
        ///     Gets or Sets the Track Number of the underlying mp3 file
        /// </summary>
        public uint TrackNo
        {
            get { return file.Tag.Track; }
            set { file.Tag.Track = value; }
        }

        private void LoadFile(string fName)
        {
            try
            {
                filename = fName;
                file = File.Create(fName);
                isValid = true;
                return;
            }
            catch (Exception)
            {
                isValid = false; // Pfft, whatever
                //Trace.WriteLine(string.Format("{0},{1}", e.GetType(), e));
            }
            isValid = false; // Just making sure...
        }

        /// <summary>
        ///     Physically moves this file to the specified destination filename
        ///     <para />
        ///     Overwrites if <paramref name="overWrite" /> is true
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="overWrite"></param>
        public void MoveTo(string destination, bool overWrite)
        {
            if (!isValid) return;
            if (System.IO.File.Exists(destination) && overWrite)
                System.IO.File.Move(filename, destination);
            else if (!System.IO.File.Exists(destination))
                System.IO.File.Move(filename, destination);
        }

        /// <summary>
        ///     Physically deletes the file from disk
        /// </summary>
        public void Delete()
        {
            if (isValid)
                System.IO.File.Delete(filename);
        }

        /// <summary>
        ///     Override .ToString to return ([Artist] - [Title])
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (isValid)
            {
                if (Artist.Trim() == "" && Title.Trim() == "")
                    return filename;
                return Artist + " - " + Title;
            }
            return "Unkown format or file not found";
        }

        /// <summary>
        ///     Saves the modifications made to the ID3v* Tag to the mp3 file
        /// </summary>
        public void Save()
        {
            file.Save();
        }

        /// <summary>
        ///     Attempts to fix the tag of this file according to the filename
        /// </summary>
        public void FixTag()
        {
            var fileSplit = Filename.Split('\\');

            Album = fileSplit[fileSplit.Length - 2].Replace('_', ' ').Replace('.', ' ');
            Artist = "";
            Title = "";

            var fileName = fileSplit[fileSplit.Length - 1].Split('.');

            var preName = "";

            for (var i = 0; i < fileName.Length - 1; i++)
                preName += fileName[i] + "-";
            var nameSplit = preName.Split('-');

            if (nameSplit.Length <= 1) return;
            var pos = 0;
            var destination = 0;
            try
            {
                foreach (var str in nameSplit)
                {
                    uint num;
                    if (!uint.TryParse(str, out num))
                    {
                        if (pos <= destination)
                        {
                            var filesplit2 = str.Split('.');
                            var artist = filesplit2[0].Replace('_', ' ').Trim();
                            if (artist.Length > 0)
                            {
                                string sub = artist.Substring(0, 1);
                                artist = artist.Remove(0, 1);
                                artist = sub.ToUpper() + artist;
                                Artist += artist;
                            }
                        }
                        else if (pos <= destination + 1)
                        {
                            var filesplit2 = str.Split('.');
                            var title = filesplit2[0].Replace('_', ' ').Trim();
                            if (title.Length > 0)
                            {
                                var sub = title.Substring(0, 1);
                                title = title.Remove(0, 1);
                                title = sub.ToUpper() + title;
                                Title += title;
                            }
                        }
                        else
                            Title += str == ".mp3" ? "" : " " + str;
                    }
                    else
                    {
                        if (pos == 0)
                        {
                            TrackNo = num;
                            destination++;
                        }
                        else
                            Title += num.ToString(CultureInfo.InvariantCulture);
                    }
                    pos++;
                }
            }
            catch (Exception e)
            {
                RuntimeCommon.DefaultLogger.Log(LoggingType.Error, this, string.Format("FixTag() Caught: {0}", e));
            }
        }

        /// <summary>
        ///     Copies the MP3 file
        ///     to a given destination
        /// </summary>
        /// <param name="destination">The destination of the copied file</param>
        /// <param name="overwrite">Indicates wether or not the target should be overwritten if it already exists</param>
        public void Copy(string destination, bool overwrite)
        {
            if (!IsValid) return;
            if (System.IO.File.Exists(destination) && overwrite)
                System.IO.File.Copy(filename, destination);
            else if (!System.IO.File.Exists(destination))
                System.IO.File.Copy(filename, destination);
        }
    }
}