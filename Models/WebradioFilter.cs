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
using MediaPortal.Common.General;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;
using Webradio.Helper;
using Webradio.Settings;

namespace Webradio.Models
{
  internal class WebradioFilter : IWorkflowModel
  {
    public static List<FilterSetupInfo> FilterList = new List<FilterSetupInfo>();
    public static List<MyStream> StreamList = new List<MyStream>();

    public WebradioFilter()
    {
      Init();
    }

    private void Init()
    {
      FilterList = ServiceRegistration.Get<ISettingsManager>().Load<FilterSettings>().FilterSetupList ?? new List<FilterSetupInfo> { new FilterSetupInfo("New Filter", "0", new List<string>(), new List<string>(), new List<string>(), new List<string>()) };
      SaveImage = "Saved.png";
      FillAllLists();
    }

    /// <summary>
    /// Import all Details (Countrys, Citys ...)
    /// </summary>
    private static void FillAllLists()
    {
      StreamList = WebradioHome.StreamList;

      if (StreamList.Count == 0) StreamList = MyStreams.Read(StreamlistUpdate.StreamListFile).Streams;

      foreach (var ms in StreamList)
      {
        // Add Countrys
        if ((ms.Country != "") & !CounList.Contains(ms.Country)) CounList.Add(ms.Country);

        // Add Citys
        if ((ms.City != "") & !CityList.Contains(ms.City)) CityList.Add(ms.City);

        // Add Bitrate
        if (ms.StreamUrls[0].Bitrate != "")
        {
          var br = ms.StreamUrls[0].Bitrate.Replace(" kbps", "").PadLeft(3, '0');
          if (!BitrList.Contains(br)) BitrList.Add(br);
        }

        // Add Genres
        var split = ms.Genres.Split(',');
        foreach (var s in split.Where(s => (s.Trim() != "") & !GenrList.Contains(s.Trim()))) GenrList.Add(s.Trim());
      }

      CounList.Sort();
      CityList.Sort();
      BitrList.Sort();
      GenrList.Sort();
      FillAllItemsList();
    }

    private static void FillAllItemsList()
    {
      ClearItemsList();

      foreach (var s in CounList)
      {
        var item = new ListItem { AdditionalProperties = { [NAME] = s } };
        item.SetLabel("Name", "[Country." + s + "]");
        if (SelectedCountrys.Contains(s)) item.Selected = true;
        Countrys.Add(item);
      }

      Refresh(Countrys);

      foreach (var s in CityList)
      {
        var item = new ListItem { AdditionalProperties = { [NAME] = s } };
        item.SetLabel("Name", s);
        if (SelectedCitys.Contains(s)) item.Selected = true;
        Citys.Add(item);
      }

      Refresh(Citys);

      foreach (var s in BitrList)
      {
        var item = new ListItem { AdditionalProperties = { [NAME] = Convert.ToInt32(s) + " kbps" } };
        item.SetLabel("Name", Convert.ToInt32(s) + " kbps");
        if (SelectedBitrate.Contains(Convert.ToInt32(s) + " kbps")) item.Selected = true;
        Bitrate.Add(item);
      }

      Refresh(Bitrate);

      foreach (var s in GenrList)
      {
        var item = new ListItem { AdditionalProperties = { [NAME] = s } };
        item.SetLabel("Name", s);
        if (SelectedGenres.Contains(s)) item.Selected = true;
        Genres.Add(item);
      }

      Refresh(Genres);
    }

    private static void ClearItemsList()
    {
      Countrys.Clear();
      Citys.Clear();
      Bitrate.Clear();
      Genres.Clear();
    }

    private static void ClearSelected()
    {
      SelectedCountrys.Clear();
      SelectedCitys.Clear();
      SelectedBitrate.Clear();
      SelectedGenres.Clear();
    }

    private static void Refresh(ItemsList list, ListItem item)
    {
      RefreshState(list);
      item.FireChange();
    }

    private static void Refresh(ItemsList list)
    {
      RefreshState(list);
      list.FireChange();
    }

    private static void RefreshState(ItemsList list)
    {
      if (list == Countrys) CountryState = Convert.ToString(SelectedCountrys.Count) + "/" + Convert.ToString(Countrys.Count);
      if (list == Citys) CityState = Convert.ToString(SelectedCitys.Count) + "/" + Convert.ToString(Citys.Count);
      if (list == Bitrate) BitrateState = Convert.ToString(SelectedBitrate.Count) + "/" + Convert.ToString(Bitrate.Count);
      if (list == Genres) GenreState = Convert.ToString(SelectedGenres.Count) + "/" + Convert.ToString(Genres.Count);
      RefreshState();
    }

    private static void RefreshState()
    {
      var x = 0;
      if (SelectedCountrys.Count + SelectedCitys.Count + SelectedBitrate.Count + SelectedGenres.Count > 0)
      {
        var query = from r in StreamList
          where
            _contains(SelectedCountrys, r.Country)
            && _contains(SelectedCitys, r.City)
            && _contains2(SelectedGenres, r.Genres)
            && _contains(SelectedBitrate, r.StreamUrls[0].Bitrate)
          select r;
        x = query.Count();
      }

      SelectedStreamsCount = Convert.ToString(x) + "/" + Convert.ToString(StreamList.Count);
    }

    private static bool _contains(ICollection<string> l, string s)
    {
      return l.Count == 0 || l.Contains(s);
    }

    private static bool _contains2(ICollection<string> l, string S)
    {
      if (l.Count == 0) return true;

      var split = S.Split(',');
      return split.Any(l.Contains);
    }

    #region Consts

    public const string MODEL_ID_STR = "FF29E03E-F4A9-4E21-A299-349E79010430";
    public const string NAME = "name";

    #endregion

    #region Lists

    // Lists with all Items from Streamlist
    public static ItemsList Countrys = new ItemsList();
    public static ItemsList Citys = new ItemsList();
    public static ItemsList Bitrate = new ItemsList();
    public static ItemsList Genres = new ItemsList();

    // Lists with Selected Items 
    private static readonly List<string> SelectedCountrys = new List<string>();
    private static readonly List<string> SelectedCitys = new List<string>();
    private static readonly List<string> SelectedBitrate = new List<string>();
    private static readonly List<string> SelectedGenres = new List<string>();

    // Lists with all Entrys in Streamlist
    private static readonly List<string> CounList = new List<string>();
    private static readonly List<string> CityList = new List<string>();
    private static readonly List<string> BitrList = new List<string>();
    private static readonly List<string> GenrList = new List<string>();

    #endregion

    #region Propertys

    private static readonly AbstractProperty _filterTitelProperty = new WProperty(typeof(string), string.Empty);

    public AbstractProperty FilterTitelProperty => _filterTitelProperty;

    public static string FilterTitel
    {
      get => (string)_filterTitelProperty.GetValue();
      set => _filterTitelProperty.SetValue(value);
    }

    private static readonly AbstractProperty _selectedStreamsCountProperty = new WProperty(typeof(string), string.Empty);

    public AbstractProperty SelectedStreamsCountProperty => _selectedStreamsCountProperty;

    public static string SelectedStreamsCount
    {
      get => (string)_selectedStreamsCountProperty.GetValue();
      set => _selectedStreamsCountProperty.SetValue(value);
    }

    private static readonly AbstractProperty _countryStateProperty = new WProperty(typeof(string), string.Empty);

    public AbstractProperty CountryStateProperty => _countryStateProperty;

    public static string CountryState
    {
      get => (string)_countryStateProperty.GetValue();
      set => _countryStateProperty.SetValue(value);
    }

    private static readonly AbstractProperty _cityStateProperty = new WProperty(typeof(string), string.Empty);

    public AbstractProperty CityStateProperty => _cityStateProperty;

    public static string CityState
    {
      get => (string)_cityStateProperty.GetValue();
      set => _cityStateProperty.SetValue(value);
    }

    private static readonly AbstractProperty _bitrateStateProperty = new WProperty(typeof(string), string.Empty);

    public AbstractProperty BitrateStateProperty => _bitrateStateProperty;

    public static string BitrateState
    {
      get => (string)_bitrateStateProperty.GetValue();
      set => _bitrateStateProperty.SetValue(value);
    }

    private static readonly AbstractProperty _genreStateProperty = new WProperty(typeof(string), string.Empty);

    public AbstractProperty GenreStateProperty => _genreStateProperty;

    public static string GenreState
    {
      get => (string)_genreStateProperty.GetValue();
      set => _genreStateProperty.SetValue(value);
    }

    private static readonly AbstractProperty _saveImage = new WProperty(typeof(string), string.Empty);

    public AbstractProperty SaveImageProperty => _saveImage;

    public static string SaveImage
    {
      get => (string)_saveImage.GetValue();
      set => _saveImage.SetValue(value);
    }

    #endregion

    #region from Menu

    /// <summary>
    /// Import selected Filter
    /// </summary>
    public static void SetFilter(FilterSetupInfo filter)
    {
      ClearSelected();
      FilterTitel = filter.Titel;

      foreach (var s in filter.Countrys) SelectedCountrys.Add(s);

      foreach (var s in filter.Citys) SelectedCitys.Add(s);

      foreach (var s in filter.Bitrate) SelectedBitrate.Add(s);

      foreach (var s in filter.Genres) SelectedGenres.Add(s);
      FillAllItemsList();
    }

    /// <summary>
    /// Rename a Entry
    /// </summary>
    public void Clear()
    {
      ClearSelected();
      FillAllItemsList();
      FilterTitel = "";
      SaveImage = "Unsaved.png";
    }

    /// <summary>
    /// Added a Entry
    /// </summary>
    public void Add()
    {
      Clear();
      FilterList.Add(new FilterSetupInfo("New Filter", Convert.ToString(FilterList.Count + 1), SelectedCountrys, SelectedCitys, SelectedGenres, SelectedBitrate));
      FilterTitel = "New Filter";
      SaveImage = "Unsaved.png";
    }

    /// <summary>
    /// Save all Changes on Site
    /// </summary>
    public void Save()
    {
      var find = false;
      foreach (var f in FilterList.Where(f => f.Titel == FilterTitel))
      {
        find = true;
        f.Countrys = SelectedCountrys;
        f.Citys = SelectedCitys;
        f.Bitrate = SelectedBitrate;
        f.Genres = SelectedGenres;
      }

      if (find == false) FilterList.Add(new FilterSetupInfo(FilterTitel, "1", SelectedCountrys, SelectedCitys, SelectedGenres, SelectedBitrate));
      ServiceRegistration.Get<ISettingsManager>().Save(new FilterSettings(FilterList));
      SaveImage = "Saved.png";
    }

    #endregion

    #region Change SelectedItem

    public void ChangeCountry(ListItem item)
    {
      var s = (string)item.AdditionalProperties[NAME];
      if (SelectedCountrys.Contains(s))
      {
        SelectedCountrys.Remove(s);
        item.Selected = false;
      }
      else
      {
        SelectedCountrys.Add(s);
        item.Selected = true;
      }

      // Autofill Citys in selected Country
      if (SelectedCountrys.Count > 0)
      {
        var query = from r in StreamList where _contains(SelectedCountrys, r.Country) select r;
        foreach (var ms in query.Where(ms => !SelectedCitys.Contains(ms.City))) SelectedCitys.Add(ms.City);

        foreach (var i in from i in Citys let si = (string)i.AdditionalProperties[NAME] where SelectedCitys.Contains(si) select i)
        {
          i.Selected = true;
          Refresh(Citys, i);
        }
      }

      SaveImage = "Unsaved.png";
      Refresh(Countrys, item);
    }

    public void ChangeCity(ListItem item)
    {
      var s = (string)item.AdditionalProperties[NAME];
      if (SelectedCitys.Contains(s))
      {
        SelectedCitys.Remove(s);
        item.Selected = false;
      }
      else
      {
        SelectedCitys.Add(s);
        item.Selected = true;
      }

      // Autofill Country by selected City
      if (SelectedCitys.Count > 0)
      {
        var query = from r in StreamList where _contains(SelectedCitys, r.City) select r;
        foreach (var ms in query.Where(ms => !SelectedCountrys.Contains(ms.Country))) SelectedCountrys.Add(ms.Country);

        foreach (var i in from i in Countrys let si = (string)i.AdditionalProperties[NAME] where SelectedCountrys.Contains(si) select i)
        {
          i.Selected = true;
          Refresh(Countrys, i);
        }
      }

      SaveImage = "Unsaved.png";
      Refresh(Citys, item);
    }

    public void ChangeBitrate(ListItem item)
    {
      var s = (string)item.AdditionalProperties[NAME];
      if (SelectedBitrate.Contains(s))
      {
        SelectedBitrate.Remove(s);
        item.Selected = false;
      }
      else
      {
        SelectedBitrate.Add(s);
        item.Selected = true;
      }

      SaveImage = "Unsaved.png";
      Refresh(Bitrate, item);
    }

    public void ChangeGenre(ListItem item)
    {
      var s = (string)item.AdditionalProperties[NAME];
      if (SelectedGenres.Contains(s))
      {
        SelectedGenres.Remove(s);
        item.Selected = false;
      }
      else
      {
        SelectedGenres.Add(s);
        item.Selected = true;
      }

      SaveImage = "Unsaved.png";
      Refresh(Genres, item);
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
