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
using MediaPortal.Common;
using MediaPortal.Common.PathManager;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.Workflow;
using Webradio.Dialogues;
using Webradio.Settings;

namespace Webradio.Helper
{
  public class StreamlistUpdate
  {
    public static string WebradioDataFolder = ServiceRegistration.Get<IPathManager>().GetPath(@"<DATA>\Webradio");
    public static string StreamListFile = Path.Combine(WebradioDataFolder, "StreamList.xml");
    public static string StreamlistServerPath = "http://install.team-mediaportal.com/MP2/Webradio/Streamlist/StreamList.xml";

    public static bool StreamListExists()
    {
      if (!Directory.Exists(WebradioDataFolder))
        Directory.CreateDirectory(WebradioDataFolder);
      return File.Exists(StreamListFile);
    }

    public static int OnlineVersion()
    {
      var request = WebRequest.Create(StreamlistServerPath);
      request.Credentials = CredentialCache.DefaultCredentials;
      WebResponse response = null;
      StreamReader reader = null;

      try
      {
        response = request.GetResponse();
        reader = new StreamReader(response.GetResponseStream());
        var buffer = new char[200];
        reader.Read(buffer, 0, 200);

        var s = new string(buffer);
        var a = s.IndexOf("<Version>", StringComparison.Ordinal) + 9;
        var b = s.IndexOf("</Version>", StringComparison.Ordinal);
        return Convert.ToInt32(s.Substring(a, b - a));
      }
      catch (Exception ex)
      {
        return -1;
      }
      finally
      {
        if (reader != null) reader.Close();
        if (response != null) response.Close();
      }
    }

    public static int OfflineVersion()
    {
      if (!StreamListExists()) return -1;

      try
      {
        var ms = MyStreams.Read(StreamListFile);
        return Convert.ToInt32(ms.Version);
      }
      catch (Exception)
      {
        return -1;
      }
    }

    public static void CheckUpdate()
    {
      if (OfflineVersion() >= OnlineVersion()) return;
      var mode = ServiceRegistration.Get<ISettingsManager>().Load<WebradioSettings>().StreamlistUpdateMode;
      if (mode == "Manually") MakeUpdate();

      if (mode == "Automatically") WebradioDlgLoadUpdate.LoadSenderListe();
    }

    public static void MakeUpdate()
    {
      ServiceRegistration.Get<IWorkflowManager>().NavigatePushAsync(new Guid("7EB62BD5-3401-45B8-A622-C3A073D5BFDF"));
    }
  }
}
