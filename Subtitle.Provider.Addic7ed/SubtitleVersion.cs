﻿using System;
using System.Collections.Generic;

namespace Subtitle.Provider.Addic7ed
{
    public class SubtitleVersion
    {
        public SubtitleVersion()
        {
            Subtitles = new List<Subtitle>();
        }

        public string Release { get; set; }
        public List<Subtitle> Subtitles { get; set; }
        public TimeSpan Age { get; set; }
        public string Uploader { get; set; }
    }
}