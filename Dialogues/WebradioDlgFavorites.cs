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
using MediaPortal.Common.General;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;
using Webradio.Settings;

namespace Webradio.Dialogues
{
  public class WebradioDlgFavorites : IWorkflowModel
  {
    public static ItemsList FavoritItems = new ItemsList();
    public List<FavoriteSetupInfo> FavoritList = new List<FavoriteSetupInfo>();
    public string SelectedId = "";

    /// <summary>
    /// Remove a Entry
    /// </summary>
    public void Delete()
    {
      if (SelectedId == "") return;
      FavoritList.RemoveRange(Convert.ToInt32(SelectedId), 1);
      FillFavoritItems();
      SelectedTitel = "";
      SelectedId = "";
      SaveImage = "Unsaved.png";
    }

    /// <summary>
    /// Rename a Entry
    /// </summary>
    public void Rename()
    {
      if (SelectedId == "") return;
      var id = 0;
      foreach (var mf in FavoritList)
      {
        if (id == Convert.ToInt32(SelectedId))
        {
          mf.Titel = SelectedTitel;
          break;
        }

        id += 1;
      }

      ImportFavorits(false);
      SaveImage = "Unsaved.png";
    }

    /// <summary>
    /// Add a Entry
    /// </summary>
    public void Add()
    {
      FavoritList.Add(new FavoriteSetupInfo("New Favorite", true, new List<string>()));
      ImportFavorits(false);
      SelectedTitel = "New Favorite";
      SaveImage = "Unsaved.png";
    }

    /// <summary>
    /// Save all Changes on Site
    /// </summary>
    public void Save()
    {
      ServiceRegistration.Get<ISettingsManager>().Save(new FavoritesSettings(FavoritList));
      SaveImage = "Saved.png";
    }

    public void Selected(ListItem item)
    {
      SelectedTitel = (string)item.AdditionalProperties[NAME];
      SelectedId = (string)item.AdditionalProperties[ID];
    }

    private void ImportFavorits(bool read)
    {
      if (read) FavoritList = ServiceRegistration.Get<ISettingsManager>().Load<FavoritesSettings>().FavoritesSetupList;

      if (FavoritList == null) FavoritList = new List<FavoriteSetupInfo> { new FavoriteSetupInfo("New Favorite", true, new List<string>()) };
      FillFavoritItems();
    }

    private void FillFavoritItems()
    {
      FavoritItems.Clear();
      var id = 0;
      foreach (var f in FavoritList)
      {
        var item = new ListItem();
        item.AdditionalProperties[NAME] = f.Titel;
        item.AdditionalProperties[ID] = Convert.ToString(id);
        item.SetLabel("Name", f.Titel);
        id += 1;
        FavoritItems.Add(item);
      }

      FavoritItems.FireChange();
    }

    #region Consts

    public const string MODEL_ID_STR = "EC2F9DD4-C694-4C2D-9EFB-092AA1F4BD94";
    public const string NAME = "name";
    public const string ID = "id";

    #endregion

    #region Propertys

    private static readonly AbstractProperty _titelProperty = new WProperty(typeof(string), string.Empty);
    private static readonly AbstractProperty _saveImage = new WProperty(typeof(string), string.Empty);

    public AbstractProperty TitelProperty => _titelProperty;

    public string SelectedTitel
    {
      get => (string)_titelProperty.GetValue();
      set => _titelProperty.SetValue(value);
    }

    public AbstractProperty SaveImageProperty => _saveImage;

    public string SaveImage
    {
      get => (string)_saveImage.GetValue();
      set => _saveImage.SetValue(value);
    }

    #endregion

    #region IWorkflowModel implementation

    public Guid ModelId => new Guid(MODEL_ID_STR);

    public bool CanEnterState(NavigationContext oldContext, NavigationContext newContext)
    {
      return true;
    }

    public void EnterModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
      ImportFavorits(true);
      SaveImage = "Saved.png";
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
