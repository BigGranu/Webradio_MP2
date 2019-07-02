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
using System.Linq;
using MediaPortal.Common;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Screens;
using MediaPortal.UI.Presentation.Workflow;
using Webradio.Helper;
using Webradio.Models;
using Webradio.Settings;

namespace Webradio.Dialogues
{
  internal class WebradioDlgShowFavorites : IWorkflowModel
  {
    public ItemsList AllFavoritItems = new ItemsList();

    public bool Changed;
    public ItemsList FavoritItems = new ItemsList();
    public List<FavoriteSetupInfo> FavoritList = new List<FavoriteSetupInfo>();
    public string SelectedStream = string.Empty;

    public void Init()
    {
      FavoritList = ServiceRegistration.Get<ISettingsManager>().Load<FavoritesSettings>().FavoritesSetupList ?? new List<FavoriteSetupInfo> { new FavoriteSetupInfo("New Favorite", true, new List<string>()) };
      if (WebradioHome.SelectedStream.Title != "")
      {
        ImportAllFavorits(WebradioHome.SelectedStream);
        SelectedStream = WebradioHome.SelectedStream.Title;
      }

      ImportFavorits();
    }

    public void ImportAllFavorits(MyStream stream)
    {
      AllFavoritItems.Clear();

      foreach (var f in FavoritList)
      {
        var item = new ListItem();
        item.AdditionalProperties[NAME] = f.Titel;
        item.SetLabel("Name", f.Titel);
        item.Selected = f.StreamUrls.Contains(stream.StreamUrls[0].StreamUrl);
        AllFavoritItems.Add(item);
      }

      AllFavoritItems.FireChange();
    }

    public void ImportFavorits()
    {
      FavoritItems.Clear();
      foreach (var f in FavoritList)
        if (f.StreamUrls.Count > 0)
        {
          var item = new ListItem();
          item.AdditionalProperties[NAME] = f.Titel;
          item.SetLabel("Name", f.Titel);
          item.SetLabel("Count", Convert.ToString(f.StreamUrls.Count));
          FavoritItems.Add(item);
        }

      FavoritItems.FireChange();
    }

    /// <summary>
    /// Import selected Filter
    /// </summary>
    public void SelectFavorite(ListItem item)
    {
      var list = new List<MyStream>();
      foreach (var query in from f in FavoritList where f.Titel == (string)item.AdditionalProperties[NAME] select from r in WebradioHome.StreamList where _contains(f.StreamUrls, r.StreamUrls[0].StreamUrl) select r)
      {
        foreach (var ms in query.Where(ms => !list.Contains(ms))) list.Add(ms);
        break;
      }

      WebradioHome.FillItemList(list);
      ServiceRegistration.Get<IScreenManager>().CloseTopmostDialog();
    }

    private static bool _contains(List<string> l, string s)
    {
      if (l.Count == 0) return true;
      var sp = s.Split(',');
      return sp.Any(l.Contains);
    }

    public void SetFavorite(ListItem item)
    {
      var s = (string)item.AdditionalProperties[NAME];

      var url = WebradioHome.SelectedStream.StreamUrls[0].StreamUrl;

      foreach (var f in FavoritList.Where(f => f.Titel == s))
      {
        if (item.Selected)
        {
          item.Selected = false;
          f.StreamUrls.Remove(url);
        }
        else
        {
          item.Selected = true;
          f.StreamUrls.Add(url);
        }

        Changed = true;
        ImportFavorits();
      }
    }

    #region Consts

    public const string MODEL_ID_STR = "9723DCC8-969D-470E-B156-F4E6E639DD18";
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
      if (Changed) ServiceRegistration.Get<ISettingsManager>().Save(new FavoritesSettings(FavoritList));
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
