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
using System.Collections.Generic;
using MediaPortal.Common;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;
using Webradio.Helper;
using Webradio.Settings;

namespace Webradio.Dialogues
{
  public class WebradioDlgSettingsStreamlistUpdate : IWorkflowModel
  {
    public static ItemsList Items = new ItemsList();
    public string OfflineVersion = string.Empty;

    public string OnlineVersion = string.Empty;
    public WebradioSettings Settings = new WebradioSettings();

    public void Init()
    {
      var settingsManager = ServiceRegistration.Get<ISettingsManager>();
      Settings = settingsManager.Load<WebradioSettings>();

      OfflineVersion = ": " + Convert.ToString(StreamlistUpdate.OfflineVersion());
      OnlineVersion = " : " + Convert.ToString(StreamlistUpdate.OnlineVersion());

      Items.Clear();

      var item = new ListItem();
      item.AdditionalProperties[NAME] = "Automatically";
      item.SetLabel("Name", "[Webradio.Settings.Automatically]");
      if (Settings.StreamlistUpdateMode == "Automatically") item.Selected = true;
      Items.Add(item);

      item = new ListItem();
      item.AdditionalProperties[NAME] = "Manually";
      item.SetLabel("Name", "[Webradio.Settings.Manually]");
      if (Settings.StreamlistUpdateMode == "Manually") item.Selected = true;
      Items.Add(item);

      item = new ListItem();
      item.AdditionalProperties[NAME] = "Disabled";
      item.SetLabel("Name", "[Webradio.Settings.Disabled]");
      if (Settings.StreamlistUpdateMode == "Disabled") item.Selected = true;
      Items.Add(item);
    }

    public void Selected(ListItem item)
    {
      foreach (var li in Items) li.Selected = (string)item.AdditionalProperties[NAME] == (string)li.AdditionalProperties[NAME];
      Settings.StreamlistUpdateMode = (string)item.AdditionalProperties[NAME];
      Items.FireChange();
    }

    public void Update()
    {
      StreamlistUpdate.MakeUpdate();
    }

    #region Consts

    public const string MODEL_ID_STR = "A391ACBF-BEFE-4820-B17D-D06545CF9987";
    public const string NAME = "name";

    #endregion

    #region IWorkflowModel implementation

    public Guid ModelId => new Guid(MODEL_ID_STR);

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
      ServiceRegistration.Get<ISettingsManager>().Save(Settings);
    }

    public void ChangeModelContext(NavigationContext oldContext, NavigationContext newContext, bool push)
    {
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
