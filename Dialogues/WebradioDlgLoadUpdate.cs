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
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.Logging;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;
using Webradio.Models;

namespace Webradio.Dialogues
{
  class WebradioDlgLoadUpdate : IWorkflowModel
  {
    #region Consts

    public const string MODEL_ID_STR = "028ABECD-9885-48F6-B39F-F252EC0115EF";

    protected const string DOWNLOAD_COMPLETE = "[Webradio.Dialog.LoadUpdate.DownloadComplete]";
    protected const string DOWNLOAD_ERROR = "[Webradio.Dialog.LoadUpdate.DownloadError]";

    #endregion

    #region Propertys

    private AbstractProperty _updateProgressProperty = new WProperty(typeof(int),0);
    public AbstractProperty UpdateProgressProperty { get { return _updateProgressProperty; } }
    public int UpdateProgress
    {
      get { return (int)_updateProgressProperty.GetValue(); }
      set { _updateProgressProperty.SetValue(value); }
    }

    private static AbstractProperty _infoProperty = new WProperty(typeof(string), string.Empty);
    public AbstractProperty InfoProperty { get { return _infoProperty; } }
    public static string Info
    {
      get { return (string)_infoProperty.GetValue(); }
      set { _infoProperty.SetValue(value); }
    }

    #endregion

    public void Init()
    {
    }

    public void LoadSenderListe()
    {
      try
      {
        WebClient webclient1 = new WebClient();
        webclient1.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
        webclient1.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadStatusChanged);
        webclient1.DownloadFileAsync(new Uri("http://www.zum-fliegenden-bauern.de/StreamList.xml"), WebradioHome.StreamListFile);
      }
      catch (Exception ex)
      {
        ServiceRegistration.Get<ILogger>().Error("Webradio: Error read Online Stationslist '{0}'", ex);
      }
    }

    private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
    {
      Info = e.Error == null ? DOWNLOAD_COMPLETE : DOWNLOAD_ERROR;
    }

    private void DownloadStatusChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      UpdateProgress = e.ProgressPercentage;
    }

    #region IWorkflowModel implementation

    public Guid ModelId
    {
      get { return new Guid(MODEL_ID_STR); }
    }

    public bool CanEnterState(NavigationContext oldContext, NavigationContext newContext)
    {
      return true;
    }

    public void EnterModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
      Init();
    }

    public void ExitModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
    }

    public void ChangeModelContext(NavigationContext oldContext, NavigationContext newContext, bool push)
    {
      // We could initialize some data here when changing the media navigation state
    }

    public void Deactivate(NavigationContext oldContext, NavigationContext newContext)
    {
    }

    public void Reactivate(NavigationContext oldContext, NavigationContext newContext)
    {
    }

    public void UpdateMenuActions(NavigationContext context, IDictionary<Guid, WorkflowAction> actions)
    {
    }

    public ScreenUpdateMode UpdateScreen(NavigationContext context, ref string screen)
    {
      return ScreenUpdateMode.AutoWorkflowManager;
    }

    #endregion
  }
}
