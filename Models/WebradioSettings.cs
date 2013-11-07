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
using System.Globalization;
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.Localization;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Players.BassPlayer;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;
using Webradio.Helper_Classes;

namespace Webradio.Models
{
  public class WebradioSettings : IWorkflowModel
  {
    #region Consts

    public const string MODEL_ID_STR = "6378BD83-BF06-4AC3-ADD7-9A4B72DA878E";

    #endregion

    private static AbstractProperty _titelProperty = new WProperty(typeof(string), string.Empty);

    public AbstractProperty TitelProperty
    {
      get { return _titelProperty; }
    }

    public string SelectedTitel
    {
      get { return (string)_titelProperty.GetValue(); }
      set { _titelProperty.SetValue(value); }
    }

    public string OfflineStreamlistVersion = string.Empty;
    public string OnlineStreamlistVersion = string.Empty;


    public void Init()
    {
      OfflineStreamlistVersion = Convert.ToString(StreamlistUpdate.OfflineVersion());
      OnlineStreamlistVersion = Convert.ToString(StreamlistUpdate.OnlineVersion());
    }

    public void StreamlistInfos()
    {
    }

    public void StartTest()
    {
      RegionSettings settings = ServiceRegistration.Get<ISettingsManager>().Load<RegionSettings>();
      string reg = settings.Culture;
      var ci = new CultureInfo(reg).EnglishName;
    }

    public void ClearStreamList()
    {
      var t = "City";

      SelectedTitel = "[Webradio.Home." + t + "]";



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