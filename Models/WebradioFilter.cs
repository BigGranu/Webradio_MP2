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
using System.Xml.Serialization;
using System.Linq;
using MediaPortal.Common.General;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;

namespace Webradio.Models
{
  class WebradioFilter : IWorkflowModel  
  {
    public const string MODEL_ID_STR = "FF29E03E-F4A9-4E21-A299-349E79010430";

    public static string _file = System.Windows.Forms.Application.StartupPath + "\\Plugins\\Webradio\\Data\\WebradioFilters.xml";

    const string NAME = "name";

    #region Lists
    // List of all Filters in Xmlfile
    public static List<MyFilter> FilterList = new List<MyFilter>();
    public ItemsList FilterItems = new ItemsList();

    // Lists with all Items from Streamlist
    public ItemsList Countrys = new ItemsList();
    public ItemsList Citys = new ItemsList();
    public ItemsList Bitrate = new ItemsList();
    public ItemsList Genres = new ItemsList();

    // Lists with Selected Items 
    private List<string> selectedCountrys = new List<string>();
    private List<string> selectedCitys = new List<string>();
    private List<string> selectedBitrate = new List<string>();
    private List<string> selectedGenres = new List<string>();

    // Lists with all Entrys in Streamlist
    private List<string> Coun_List = new List<string>();
    private List<string> City_List = new List<string>();
    private List<string> Bitr_List = new List<string>();
    private List<string> Genr_List = new List<string>();
    #endregion

    #region Labels
    private AbstractProperty _FilterTitelProperty = null;
    public AbstractProperty FilterTitelProperty
    {
      get { return _FilterTitelProperty; }
    }
    public string FilterTitel
    {
      get { return (string)_FilterTitelProperty.GetValue(); }
      set { 
          _FilterTitelProperty.SetValue(value);
          }
    }

    private AbstractProperty _SelectedStreamsCountProperty = null;
    public AbstractProperty SelectedStreamsCountProperty
    {
      get { return _SelectedStreamsCountProperty; }
    }
    public string SelectedStreamsCount
    {
      get { return (string)_SelectedStreamsCountProperty.GetValue(); }
      set
      {
        _SelectedStreamsCountProperty.SetValue(value);
      }
    }

    private AbstractProperty _CountryStateProperty = null;
    public AbstractProperty CountryStateProperty
    {
      get { return _CountryStateProperty; }
    }
    public string CountryState
    {
      get { return (string)_CountryStateProperty.GetValue(); }
      set
      {
        _CountryStateProperty.SetValue(value);
      }
    }

    private AbstractProperty _CityStateProperty = null;
    public AbstractProperty CityStateProperty
    {
      get { return _CityStateProperty; }
    }
    public string CityState
    {
      get { return (string)_CityStateProperty.GetValue(); }
      set
      {
        _CityStateProperty.SetValue(value);
      }
    }

    private AbstractProperty _BitrateStateProperty = null;
    public AbstractProperty BitrateStateProperty
    {
      get { return _BitrateStateProperty; }
    }
    public string BitrateState
    {
      get { return (string)_BitrateStateProperty.GetValue(); }
      set
      {
        _BitrateStateProperty.SetValue(value);
      }
    }

    private AbstractProperty _GenreStateProperty = null;
    public AbstractProperty GenreStateProperty
    {
      get { return _GenreStateProperty; }
    }
    public string GenreState
    {
      get { return (string)_GenreStateProperty.GetValue(); }
      set
      {
        _GenreStateProperty.SetValue(value);
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
      FillAllLists();
    }

    #region from Menu
    /// <summary>
    /// Import all Filter
    /// </summary>
    public void ImportFilter()
    {
      MyFilters filters = MyFilters.Read(_file);
      FilterList = filters.FilterList;

      foreach (MyFilter f in FilterList)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = f.Titel;
        item.SetLabel("Name", f.Titel);
        FilterItems.Add(item);
      }
    }

    /// <summary>
    /// Import selected Filter
    /// </summary>
    public void SetImportFilter(ListItem item)
    {
      FilterTitel = (string)item.AdditionalProperties[NAME];
      ClearSelected();

      foreach (MyFilter f in FilterList)
      {
        if (f.Titel == FilterTitel)
        {
          foreach (string s in f.fCountrys)
          {
            selectedCountrys.Add(s);
          }

          foreach (string s in f.fCitys)
          {
            selectedCitys.Add(s);
          }

          foreach (string s in f.fBitrate)
          {
            selectedBitrate.Add(s);
          }

          foreach (string s in f.fGenres)
          {
            selectedGenres.Add(s);
          }

          FillAllItemsList();
          break;
        }
      }
    }

    /// <summary>
    /// Rename a Entry
    /// </summary>
    public void Clear( )
    {
      //FilterTitel = string.Empty;
      ClearSelected();
      FillAllItemsList();
    }

    /// <summary>
    /// Added a Entry
    /// </summary>
    public void Add()
    {
      Clear();
      FilterList.Add(new MyFilter(FilterTitel, selectedCountrys, selectedCitys, selectedGenres, selectedBitrate));
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
        FilterList.Add(new MyFilter(FilterTitel, selectedCountrys, selectedCitys, selectedGenres, selectedBitrate));
      }
      MyFilters.Write(_file, new MyFilters(FilterList));
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
      Refresh(Genres, item);
    }
    #endregion

    /// <summary>
    /// Import all Details (Countrys, Citys ...)
    /// </summary>
    private void FillAllLists()
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

    private void FillAllItemsList()
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

    private void ClearItemsList()
    {
      Countrys.Clear();
      Citys.Clear();
      Bitrate.Clear();
      Genres.Clear();
    }

    private void ClearSelected() 
    {
      selectedCountrys.Clear();
      selectedCitys.Clear();
      selectedBitrate.Clear();
      selectedGenres.Clear();
    }

    private void Refresh(ItemsList List, ListItem item)
    {
      RefreshState(List);
      item.FireChange();
    }

    private void Refresh(ItemsList List)
    {
      RefreshState(List);
      List.FireChange();
    }

    private void RefreshState(ItemsList List)
    {
      if (List == Countrys) { CountryState = Convert.ToString(selectedCountrys.Count) + "/" + Convert.ToString(Countrys.Count);}
      if (List == Citys) { CityState = Convert.ToString(selectedCitys.Count) + "/" + Convert.ToString(Citys.Count); }
      if (List == Bitrate) { BitrateState = Convert.ToString(selectedBitrate.Count) + "/" + Convert.ToString(Bitrate.Count); }
      if (List == Genres) { GenreState = Convert.ToString(selectedGenres.Count) + "/" + Convert.ToString(Genres.Count); }
      RefreshState();
    }

    private void RefreshState()
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

    private bool _contains(List<string> L, string S)
    {
      if (L.Count == 0)
        return true;
      return L.Contains(S);
    }

    private bool _contains2(List<string> L, string S)
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
      ImportFilter();
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

  #region Read/Write
  public class MyFilters
  {

      public List<MyFilter> FilterList = new List<MyFilter>();

      static XmlSerializer serializer = new XmlSerializer(typeof(MyFilters));
      static FileStream stream;

      public MyFilters()
      {
      }

      public MyFilters(List<MyFilter> _filters)
      {
        FilterList = _filters;
      }

      public static MyFilters Read(string XmlFile)
      {
        MyFilters _s = new MyFilters();
        try
        { 
          stream = new FileStream(XmlFile, FileMode.Open);
          _s = (MyFilters)serializer.Deserialize(stream);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.StackTrace);
        }
        finally
        {
          stream.Close();
          serializer = null;
        }
        return _s;
      }

      public static bool Write(string XmlFile, Object obj)
      {
        try
        {
          stream = new FileStream(XmlFile, FileMode.Create);
          serializer = new XmlSerializer(typeof(MyFilters));
          serializer.Serialize(stream, obj);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.StackTrace);
        }
        finally
        {
          stream.Close();
          serializer = null;
        }
        return true;
      }

    }

  public class MyFilter
  {
      public string Titel;
      public List<string> fCountrys;
      public List<string> fCitys;
      public List<string> fGenres;
      public List<string> fBitrate;

      public MyFilter()
      {
          Titel = "";
          fCountrys = new List<string>();
          fCitys = new List<string>();
          fGenres = new List<string>();
          fBitrate = new List<string>();
      }

      public MyFilter(String _Titel, List<string> _Countrys, List<string> _Citys, List<string> _Genres, List<string> _Bitrate)
      {
          Titel = _Titel;
          fCountrys = _Countrys;
          fCitys = _Citys;
          fGenres = _Genres;
          fBitrate = _Bitrate;
      }
  }

  #endregion

}
