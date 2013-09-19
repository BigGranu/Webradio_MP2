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
using MediaPortal.Common;
using MediaPortal.UI.Presentation.Workflow;
using Webradio.Models;

namespace Webradio.Helper_Classes
{
  public class StreamlistUpdate
  {
    public static string StreamlistServerPath = "http://www.biggranu.de/StreamList.xml";

    public static int OnlineVersion()
    {
      WebRequest request = WebRequest.Create(StreamlistServerPath);
      request.Credentials = CredentialCache.DefaultCredentials;
      WebResponse response = null;
      StreamReader reader = null;

      try
      {
        response = request.GetResponse();
        reader = new StreamReader(response.GetResponseStream());
        char[] buffer = new char[200];
        reader.Read(buffer, 0, 200);

        string s = new string(buffer);
        int a = s.IndexOf("<Version>", System.StringComparison.Ordinal) + 9;
        int b = s.IndexOf("</Version>", System.StringComparison.Ordinal);
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
      if (!File.Exists(WebradioHome.StreamListFile))
      {
        return -1;
      }
      else
      {
        MyStreams ms = MyStreams.Read(WebradioHome.StreamListFile);
        return Convert.ToInt32(ms.Version);
      }
    }

    public static void CheckUpdate()
    {
      if (OfflineVersion() < OnlineVersion())
      {
        ServiceRegistration.Get<IWorkflowManager>().NavigatePushAsync(new Guid("7EB62BD5-3401-45B8-A622-C3A073D5BFDF"));
      }
    }

    public static void MakeUpdate()
    {
      ServiceRegistration.Get<IWorkflowManager>().NavigatePushAsync(new Guid("7EB62BD5-3401-45B8-A622-C3A073D5BFDF"));
    }

  }
}
