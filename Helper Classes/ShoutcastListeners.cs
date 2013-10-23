#region Copyright (C) 2007-2013 Team MediaPortal

/*
    Copyright (C) 2007-2013 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.IO;
using System.Net;
using System.Timers;
using Webradio.Models;

namespace Webradio.Helper_Classes
{
  public class ShoutcastListeners
  {
    public static Timer ATimer = new Timer();

    public static void Listeners()
    {
      try
      {
        var request = (HttpWebRequest)WebRequest.Create(WebradioHome.SelectedStream.URL);
        request.UserAgent = "Mozilla";
        request.Credentials = CredentialCache.DefaultCredentials;
        WebResponse response = null;
        StreamReader reader = null;
        response = request.GetResponse();
        if (response.ContentType != "text/html")
        {
          WebradioHome.CurrentListeners = "unknown";
          return;
        }

        reader = new StreamReader(response.GetResponseStream());
        string s = reader.ReadToEnd();

        if (!s.Contains(">SHOUTcast Administrator<"))
        {
          ATimer.Stop();
          WebradioHome.CurrentListeners = "unknown";
          return;
        }

        var i = s.LastIndexOf(">", s.LastIndexOf("listeners", StringComparison.Ordinal), StringComparison.Ordinal) + 1;
        WebradioHome.CurrentListeners = s.Substring(i, s.IndexOf(" ", i, StringComparison.Ordinal) - i);

        ATimer.Elapsed += OnTimedEvent;
        ATimer.Interval = 60000;
        ATimer.Start();
      }
      catch (Exception)
      {
        ATimer.Stop();
        WebradioHome.CurrentListeners = "unknown";
      }
    }

    private static void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
      Listeners();
    }
  }
}