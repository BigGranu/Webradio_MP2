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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Globalization;
using System.Linq;
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;

namespace Webradio.Models
{
  class WebradioFilter : IWorkflowModel  
  {
    public const string MODEL_ID_STR = "FF29E03E-F4A9-4E21-A299-349E79010430";
    const string NAME = "name";

    public static List<MyFilter> FilterList = new List<MyFilter>();
    
    #region Lists

    // Lists with all Items from Streamlist
    public static ItemsList Countrys = new ItemsList();
    public static ItemsList Citys = new ItemsList();
    public static ItemsList Bitrate = new ItemsList();
    public static ItemsList Genres = new ItemsList();

    // Lists with Selected Items 
    private static List<string> selectedCountrys = new List<string>();
    private static List<string> selectedCitys = new List<string>();
    private static List<string> selectedBitrate = new List<string>();
    private static List<string> selectedGenres = new List<string>();

    // Lists with all Entrys in Streamlist
    private static List<string> Coun_List = new List<string>();
    private static List<string> City_List = new List<string>();
    private static List<string> Bitr_List = new List<string>();
    private static List<string> Genr_List = new List<string>();
    #endregion

    #region Propertys
    private static AbstractProperty _FilterTitelProperty = null;
    public AbstractProperty FilterTitelProperty
    {
      get { return _FilterTitelProperty; }
    }
    public static string FilterTitel
    {
      get { return (string)_FilterTitelProperty.GetValue(); }
      set { 
          _FilterTitelProperty.SetValue(value);
          }
    }

    private static AbstractProperty _SelectedStreamsCountProperty = null;
    public AbstractProperty SelectedStreamsCountProperty
    {
      get { return _SelectedStreamsCountProperty; }
    }
    public static string SelectedStreamsCount
    {
      get { return (string)_SelectedStreamsCountProperty.GetValue(); }
      set
      {
        _SelectedStreamsCountProperty.SetValue(value);
      }
    }

    private static AbstractProperty _CountryStateProperty = null;
    public AbstractProperty CountryStateProperty
    {
      get { return _CountryStateProperty; }
    }
    public static string CountryState
    {
      get { return (string)_CountryStateProperty.GetValue(); }
      set
      {
        _CountryStateProperty.SetValue(value);
      }
    }

    private static AbstractProperty _CityStateProperty = null;
    public AbstractProperty CityStateProperty
    {
      get { return _CityStateProperty; }
    }
    public static string CityState
    {
      get { return (string)_CityStateProperty.GetValue(); }
      set
      {
        _CityStateProperty.SetValue(value);
      }
    }

    private static AbstractProperty _BitrateStateProperty = null;
    public AbstractProperty BitrateStateProperty
    {
      get { return _BitrateStateProperty; }
    }
    public static string BitrateState
    {
      get { return (string)_BitrateStateProperty.GetValue(); }
      set
      {
        _BitrateStateProperty.SetValue(value);
      }
    }

    private static AbstractProperty _GenreStateProperty = null;
    public AbstractProperty GenreStateProperty
    {
      get { return _GenreStateProperty; }
    }
    public static string GenreState
    {
      get { return (string)_GenreStateProperty.GetValue(); }
      set
      {
        _GenreStateProperty.SetValue(value);
      }
    }

    private static AbstractProperty _SaveImage = null;
    public AbstractProperty SaveImageProperty
    {
      get { return _SaveImage; }
    }
    public static string SaveImage
    {
      get { return (string)_SaveImage.GetValue(); }
      set
      {
        _SaveImage.SetValue(value);
      }
    }

    #endregion

    public WebradioFilter()
    {
      Init();
    }

    private void Init()
    {
      _FilterTitelProperty = new WProperty(typeof(string), string.Empty);
      _SelectedStreamsCountProperty = new WProperty(typeof(string), string.Empty);
      _CountryStateProperty = new WProperty(typeof(string), string.Empty);
      _CityStateProperty = new WProperty(typeof(string), string.Empty);
      _BitrateStateProperty = new WProperty(typeof(string), string.Empty);
      _GenreStateProperty = new WProperty(typeof(string), string.Empty);
      _SaveImage = new WProperty(typeof(string), string.Empty);
      SaveImage = "Saved.png";
      FillAllLists();
    }

    #region from Menu

    /// <summary>
    /// Import selected Filter
    /// </summary>
    public static void SetFilter(MyFilter filter)
    {
      ClearSelected();
      FilterTitel = filter.Titel;

      foreach (string s in filter.fCountrys)
      {
        selectedCountrys.Add(s);
      }

      foreach (string s in filter.fCitys)
      {
        selectedCitys.Add(s);
      }

      foreach (string s in filter.fBitrate)
      {
        selectedBitrate.Add(s);
      }

      foreach (string s in filter.fGenres)
      {
        selectedGenres.Add(s);
      }
      FillAllItemsList();
    }

    /// <summary>
    /// Rename a Entry
    /// </summary>
    public void Clear( )
    {
      //FilterTitel = string.Empty;
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
      FilterList.Add(new MyFilter("New Filter", Convert.ToString(FilterList.Count + 1), selectedCountrys, selectedCitys, selectedGenres, selectedBitrate));
      FilterTitel = "New Filter";
      SaveImage = "Unsaved.png";
    }

    /// <summary>
    /// Save all Changes on Site
    /// </summary>
    public void Save()
    {
      bool find = false;
      foreach (MyFilter f in FilterList)
      {
        if (f.Titel == FilterTitel)
        {
          find = true;
          f.fCountrys = selectedCountrys;
          f.fCitys = selectedCitys;
          f.fBitrate = selectedBitrate;
          f.fGenres = selectedGenres;
        }
      }

      if (find == false)
      {
        FilterList.Add(new MyFilter(FilterTitel,"1", selectedCountrys, selectedCitys, selectedGenres, selectedBitrate));
      }
      MyFilters.Write(new MyFilters(FilterList));
      SaveImage = "Saved.png";
    }
    #endregion

    #region Change SelectedItem
    public void ChangeCountry(ListItem item)
    {
      string s = (string)item.AdditionalProperties[NAME];
      if (selectedCountrys.Contains(s))
      {
        selectedCountrys.Remove(s);
        item.Selected = false;
      }
      else
      {
        selectedCountrys.Add(s);
        item.Selected = true;
      }
      
      // Autofill Citys in selected Country
      if (selectedCountrys.Count > 0)
      { 
        IEnumerable<MyStream> query = from r in WebradioHome.StreamList where _contains(selectedCountrys, r.Country) select r;
        foreach (MyStream ms in query)
        {
          if (!selectedCitys.Contains(ms.City)) {selectedCitys.Add(ms.City);}
        }

        foreach (ListItem i in Citys)
        {
          string si = (string)i.AdditionalProperties[NAME];
          if (selectedCitys.Contains(si))
          {
            i.Selected = true;
            Refresh(Citys,i);
          }
        }
      }
      SaveImage = "Unsaved.png";
      Refresh(Countrys,item);
    }

    public void ChangeCity(ListItem item)   
    {
      string s = (string)item.AdditionalProperties[NAME];
      if (selectedCitys.Contains(s))
      {
        selectedCitys.Remove(s);
        item.Selected = false;
      }
      else
      {
        selectedCitys.Add(s);
        item.Selected = true;
      }

      // Autofill Country by selected City
      if (selectedCitys.Count > 0)
      { 
        IEnumerable<MyStream> query = from r in WebradioHome.StreamList where _contains(selectedCitys, r.City) select r;
        foreach (MyStream ms in query)
        {
          if (!selectedCountrys.Contains(ms.Country)) {selectedCountrys.Add(ms.Country);}
        }

        foreach (ListItem i in Countrys)
        {
          string si = (string)i.AdditionalProperties[NAME];
          if (selectedCountrys.Contains(si))
          {
            i.Selected = true;
            Refresh(Countrys, i);
          }
        }
      }
      SaveImage = "Unsaved.png";
      Refresh(Citys, item);
    }

    public void ChangeBitrate(ListItem item)
    {
      string s = (string)item.AdditionalProperties[NAME];
      if (selectedBitrate.Contains(s))
      {
        selectedBitrate.Remove(s);
        item.Selected = false;
      }
      else
      {
        selectedBitrate.Add(s);
        item.Selected = true;
      }
      SaveImage = "Unsaved.png";
      Refresh(Bitrate, item);
    }

    public void ChangeGenre(ListItem item)
    {
      string s = (string)item.AdditionalProperties[NAME];
      if (selectedGenres.Contains(s))
      {
        selectedGenres.Remove(s);
        item.Selected = false;
      }
      else
      {
        selectedGenres.Add(s);
        item.Selected = true;
      }
      SaveImage = "Unsaved.png";
      Refresh(Genres, item);
    }
    #endregion

    /// <summary>
    /// Import all Details (Countrys, Citys ...)
    /// </summary>
    private static void FillAllLists()
    {
      foreach (MyStream ms in WebradioHome.StreamList)
      {
        // Add Countrys
        if (ms.Country != "" & !Coun_List.Contains(ms.Country))
        {
          Coun_List.Add(ms.Country);
        }

        // Add Citys
        if (ms.City != "" & !City_List.Contains(ms.City))
        {      
          City_List.Add(ms.City);
        }

        // Add Bitrate
        if (ms.Bitrate != "")
        {
          string br = ms.Bitrate.Replace(" kbps", "").PadLeft(3, '0');
          if (!Bitr_List.Contains(br))
          { 
            Bitr_List.Add(br);
          }           
        }

        // Add Genres
        string[] split = ms.Genres.Split(new Char[] {','});
        foreach (string s in split)
        {
          if (s.Trim() != "" & !Genr_List.Contains(s.Trim()))
          {
            Genr_List.Add(s.Trim());
          }
        }
      }

      Coun_List.Sort();
      City_List.Sort();
      Bitr_List.Sort();
      Genr_List.Sort();
      FillAllItemsList();
    }

    private static void FillAllItemsList()
    {
      ClearItemsList();

      foreach (string s in Coun_List)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = s;
        item.SetLabel("Name", s);
        if (selectedCountrys.Contains(s)) 
        {
          item.Selected = true;
        }
        Countrys.Add(item);
      }
      Refresh(Countrys);

      foreach (string s in City_List)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = s;
        item.SetLabel("Name", s);
        if (selectedCitys.Contains(s))
        {
          item.Selected = true;
        }
        Citys.Add(item);
      }
      Refresh(Citys);

      foreach (string s in Bitr_List)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = Convert.ToInt32(s) + " kbps";
        item.SetLabel("Name", Convert.ToInt32(s) + " kbps");
        if (selectedBitrate.Contains(Convert.ToInt32(s) + " kbps"))
        {
          item.Selected = true;
        }
        Bitrate.Add(item);
      }
      Refresh(Bitrate);

      foreach (string s in Genr_List)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = s;
        item.SetLabel("Name", s);
        if (selectedGenres.Contains(s))
        {
          item.Selected = true;
        }
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
      selectedCountrys.Clear();
      selectedCitys.Clear();
      selectedBitrate.Clear();
      selectedGenres.Clear();
    }

    private static void Refresh(ItemsList List, ListItem item)
    {
      RefreshState(List);
      item.FireChange();
    }

    private static void Refresh(ItemsList List)
    {
      RefreshState(List);
      List.FireChange();
    }

    private static void RefreshState(ItemsList List)
    {
      if (List == Countrys) { CountryState = Convert.ToString(selectedCountrys.Count) + "/" + Convert.ToString(Countrys.Count);}
      if (List == Citys) { CityState = Convert.ToString(selectedCitys.Count) + "/" + Convert.ToString(Citys.Count); }
      if (List == Bitrate) { BitrateState = Convert.ToString(selectedBitrate.Count) + "/" + Convert.ToString(Bitrate.Count); }
      if (List == Genres) { GenreState = Convert.ToString(selectedGenres.Count) + "/" + Convert.ToString(Genres.Count); }
      RefreshState();
    }

    private static void RefreshState()
    {
      int x = 0;
      if (selectedCountrys.Count + selectedCitys.Count + selectedBitrate.Count + selectedGenres.Count > 0)
      {
        IEnumerable<MyStream> query = from r in WebradioHome.StreamList where
                                      _contains(selectedCountrys, r.Country)
                                      && _contains(selectedCitys, r.City)
                                      && _contains2(selectedGenres, r.Genres)
                                      && _contains(selectedBitrate, r.Bitrate) select r;
        x = query.Count<MyStream>();
      }
      SelectedStreamsCount = Convert.ToString(x) + "/" + Convert.ToString(WebradioHome.StreamList.Count);
    }

    private static bool _contains(List<string> L, string S)
    {
      if (L.Count == 0)
        return true;
      return L.Contains(S);
    }

    private static bool _contains2(List<string> L, string S)
    {
      if (L.Count == 0) { return true;}

      string[] split = S.Split(new Char[] { ',' });
      foreach (string s in split)
      {
        if (L.Contains(s))
        {
          return true;
        }
      }
      return false;
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
      FilterList = MyFilters.Read().FilterList;
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
