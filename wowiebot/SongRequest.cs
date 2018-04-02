﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace wowiebot
{
    public class SongRequest
    {
        static private Regex youtubeIDRegex = new Regex("v=(?<id>[A-Za-z0-9_-]{11})");
        private String youtubeURL;

        public SongRequest(String youtubeURL)
        {
            this.youtubeURL = youtubeURL;
        }

        public String getYoutubeID()
        {
            Match match = youtubeIDRegex.Match(youtubeURL);
            if (match.Success)
            {
                return match.Groups["id"].Value;
            }
            else
            {
                return "";
            }
        }
    }
}
