#region Copyright (C) 2007-2019 Team MediaPortal

/*
    Copyright (C) 2007-2019 Team MediaPortal
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
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.UI.Presentation.Players;
using Webradio.Models;

namespace Webradio.Helper
{
  public class StreamListeners
  {
    public static Timer ATimer = new Timer();

    public static void Listeners()
    {
      var listeners = "unknown";

      if (!ServiceRegistration.Get<IPlayerContextManager>().IsAudioContextActive & ATimer.Enabled)
      {
        ATimer.Enabled = false;
        return;
      }

      if (WebradioHome.SelectedStream.StreamUrls[0].StreamUrl != "")
      {
        var link = WebradioHome.SelectedStream.StreamUrls[0].StreamUrl;

        if (WebradioHome.SelectedStream.StreamUrls[0].Provider == "Ru") link = WebradioHome.SelectedStream.StreamUrls[0].StreamUrl.Substring(0, WebradioHome.SelectedStream.StreamUrls[0].StreamUrl.LastIndexOf("/", StringComparison.Ordinal));

        var request = (HttpWebRequest)WebRequest.Create(link);
        request.UserAgent = "Mozilla";
        request.Credentials = CredentialCache.DefaultCredentials;

        try
        {
          using (var response = request.GetResponse())
          {
            if ((response.ContentType == "text/html") | (response.ContentType == "text/xml"))
              using (var stream = response.GetResponseStream())
              {
                if (stream == null) return;

                using (var reader = new StreamReader(stream))
                {
                  var s = reader.ReadToEnd();

                  //SHOUTcast
                  if ((WebradioHome.SelectedStream.StreamUrls[0].Provider == "ShC") & s.Contains("Server is currently up"))
                    try
                    {
                      var i = s.LastIndexOf(">", s.LastIndexOf("listeners", StringComparison.Ordinal), StringComparison.Ordinal) + 1;
                      listeners = s.Substring(i, s.IndexOf(" ", i, StringComparison.Ordinal) - i);
                    }
                    catch (Exception)
                    {
                      listeners = "";
                    }

                  //Icecast
                  if (WebradioHome.SelectedStream.StreamUrls[0].Provider == "ScC")
                  {
                  }

                  //Ru
                  if (WebradioHome.SelectedStream.StreamUrls[0].Provider == "Ru")
                    try
                    {
                      var search = WebradioHome.SelectedStream.StreamUrls[0].StreamUrl.Substring(WebradioHome.SelectedStream.StreamUrls[0].StreamUrl.LastIndexOf("/", StringComparison.Ordinal)) + ",";
                      var i = s.IndexOf(",,,", s.LastIndexOf(search, StringComparison.Ordinal), StringComparison.Ordinal) + 3;
                      listeners = s.Substring(i, s.IndexOf(",", i, StringComparison.Ordinal) - i);
                    }
                    catch (Exception)
                    {
                      listeners = "";
                    }

                  //Streamerspanel
                  if ((WebradioHome.SelectedStream.StreamUrls[0].StreamUrl == "StP") & s.Contains("Server is currently up"))
                    try
                    {
                      var i = s.LastIndexOf(">", s.LastIndexOf("listeners", StringComparison.Ordinal), StringComparison.Ordinal) + 1;
                      listeners = s.Substring(i, s.IndexOf(" ", i, StringComparison.Ordinal) - i);
                    }
                    catch (Exception)
                    {
                      listeners = "";
                    }

                  //Steamcast
                  if (WebradioHome.SelectedStream.StreamUrls[0].StreamUrl == "StC")
                    try
                    {
                      var i = s.LastIndexOf(">", s.LastIndexOf("listeners", StringComparison.Ordinal), StringComparison.Ordinal) + 1;
                      listeners = s.Substring(i, s.IndexOf(" ", i, StringComparison.Ordinal) - i);
                    }
                    catch (Exception)
                    {
                      listeners = "";
                    }

                  if (listeners == "unknown")
                  {
                    ATimer.Stop();
                    WebradioHome.CurrentListeners = "unknown";
                    return;
                  }

                  WebradioHome.CurrentListeners = listeners;
                  ATimer.Elapsed += OnTimedEvent;
                  ATimer.Interval = 10000;
                  ATimer.Start();
                }
              }
          }
        }
        catch (WebException ex)
        {
          ATimer.Stop();
          WebradioHome.CurrentListeners = "unknown";
          ServiceRegistration.Get<ILogger>().Warn("WebradioStreamListeners: Error reading Streamdata '{0}'", ex);
        }
      }
    }

    private static void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
      Listeners();
    }
  }
}
