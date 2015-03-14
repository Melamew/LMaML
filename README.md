# LMaML

A Music Library, Made by a Lynx ;)
Please check back in a couple of years for a proper readme, I've too much to do at this point.

In simple terms, it's a music player / library, that supports a variety of file formats.

Supported File Formats:
* See audio plugin for details (Configuration file)
* Currently there are three audio systems available (Implemented):
  * BASS.Net
  * FMODex
  * NAudio (.Net audio player)

Support for Storage Back-ends (For the library itself):
* Please see the configuration file of LMaML,
  * MongoDB is the default storage back-end.
  * NHibernate (With SQLite) is supported, though may be a bit slower.
  * Do note that the BPlusTree storage back-end is an experiment, and is by no means usable.
