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
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.Logging;
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
    protected List<MyFilter> _filterList = new List<MyFilter>();

    public ItemsList FilterItems { get; protected set; }

    // Lists with all Items from Streamlist
    public ItemsList Countrys { get; protected set; }
    public ItemsList Citys { get; protected set; }
    public ItemsList Bitrate { get; protected set; }
    public ItemsList Genres { get; protected set; }

    // Lists with Selected Items 
    private readonly List<string> _selectedCountrys = new List<string>();
    private readonly List<string> _selectedCitys = new List<string>();
    private readonly List<string> _selectedBitrate = new List<string>();
    private readonly List<string> _selectedGenres = new List<string>();

    // Lists with all Entrys in Streamlist
    private readonly List<string> _countries = new List<string>();
    private readonly List<string> _cities = new List<string>();
    private readonly List<string> _bitrates = new List<string>();
    private readonly List<string> _genres = new List<string>();
    #endregion

    #region Labels
    private AbstractProperty _filterTitelProperty = null;
    public AbstractProperty FilterTitelProperty
    {
      get { return _filterTitelProperty; }
    }
    public string FilterTitel
    {
      get { return (string) _filterTitelProperty.GetValue(); }
      set
      {
        _filterTitelProperty.SetValue(value);
      }
    }

    private AbstractProperty _selectedStreamsCountProperty = null;
    public AbstractProperty SelectedStreamsCountProperty
    {
      get { return _selectedStreamsCountProperty; }
    }
    public string SelectedStreamsCount
    {
      get { return (string) _selectedStreamsCountProperty.GetValue(); }
      set
      {
        _selectedStreamsCountProperty.SetValue(value);
      }
    }

    private AbstractProperty _countryStateProperty = null;
    public AbstractProperty CountryStateProperty
    {
      get { return _countryStateProperty; }
    }
    public string CountryState
    {
      get { return (string) _countryStateProperty.GetValue(); }
      set
      {
        _countryStateProperty.SetValue(value);
      }
    }

    private AbstractProperty _cityStateProperty = null;
    public AbstractProperty CityStateProperty
    {
      get { return _cityStateProperty; }
    }
    public string CityState
    {
      get { return (string) _cityStateProperty.GetValue(); }
      set
      {
        _cityStateProperty.SetValue(value);
      }
    }

    private AbstractProperty _bitrateStateProperty = null;
    public AbstractProperty BitrateStateProperty
    {
      get { return _bitrateStateProperty; }
    }
    public string BitrateState
    {
      get { return (string) _bitrateStateProperty.GetValue(); }
      set
      {
        _bitrateStateProperty.SetValue(value);
      }
    }

    private AbstractProperty _genreStateProperty = null;
    public AbstractProperty GenreStateProperty
    {
      get { return _genreStateProperty; }
    }
    public string GenreState
    {
      get { return (string) _genreStateProperty.GetValue(); }
      set
      {
        _genreStateProperty.SetValue(value);
      }
    }
    #endregion

    public WebradioFilter()
    {
      Init();
    }

    private void Init()
    {
      _filterTitelProperty = new WProperty(typeof(string), string.Empty);
      _selectedStreamsCountProperty = new WProperty(typeof(string), string.Empty);
      _countryStateProperty = new WProperty(typeof(string), string.Empty);
      _cityStateProperty = new WProperty(typeof(string), string.Empty);
      _bitrateStateProperty = new WProperty(typeof(string), string.Empty);
      _genreStateProperty = new WProperty(typeof(string), string.Empty);
      Countrys = new ItemsList();
      Citys = new ItemsList();
      Bitrate = new ItemsList();
      Genres = new ItemsList();
      FilterItems = new ItemsList();
      FillAllLists();
    }

    #region from Menu
    /// <summary>
    /// Import all Filter
    /// </summary>
    public void ImportFilter()
    {
      MyFilters filters = MyFilters.Read(_file);
      _filterList = filters.FilterList;

      foreach (MyFilter f in _filterList)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = f.Title;
        item.SetLabel("Name", f.Title);
        FilterItems.Add(item);
      }
    }

    /// <summary>
    /// Import selected Filter
    /// </summary>
    public void SetImportFilter(ListItem item)
    {
      FilterTitel = (string) item.AdditionalProperties[NAME];
      ClearSelected();

      foreach (MyFilter f in _filterList.Where(f => f.Title == FilterTitel))
      {
        _selectedCountrys.AddRange(f.Countries);
        _selectedCitys.AddRange(f.Cities);
        _selectedBitrate.AddRange(f.Bitrates);
        _selectedGenres.AddRange(f.Genres);
        FillAllItemsList();
        break;
      }
    }

    /// <summary>
    /// Rename a Entry
    /// </summary>
    public void Clear()
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
      _filterList.Add(new MyFilter(FilterTitel, _selectedCountrys, _selectedCitys, _selectedGenres, _selectedBitrate));
    }

    /// <summary>
    /// Save all Changes on Site
    /// </summary>
    public void Save()
    {
      bool find = false;
      foreach (MyFilter f in _filterList.Where(f => f.Title == FilterTitel))
      {
        find = true;
        f.Countries = _selectedCountrys;
        f.Cities = _selectedCitys;
        f.Bitrates = _selectedBitrate;
        f.Genres = _selectedGenres;
      }

      if (find == false)
      {
        _filterList.Add(new MyFilter(FilterTitel, _selectedCountrys, _selectedCitys, _selectedGenres, _selectedBitrate));
      }
      MyFilters.Write(_file, new MyFilters(_filterList));
    }
    #endregion

    #region Change SelectedItem
    public void ChangeCountry(ListItem item)
    {
      string s = (string) item.AdditionalProperties[NAME];
      if (_selectedCountrys.Contains(s))
      {
        _selectedCountrys.Remove(s);
        item.Selected = false;
      }
      else
      {
        _selectedCountrys.Add(s);
        item.Selected = true;
      }

      // Autofill Citys in selected Country
      if (_selectedCountrys.Count > 0)
      {
        IEnumerable<MyStream> query = from r in WebradioHome.StreamList where EmptyOrContains(_selectedCountrys, r.Country) select r;
        foreach (MyStream ms in query)
        {
          if (!_selectedCitys.Contains(ms.City)) { _selectedCitys.Add(ms.City); }
        }

        foreach (ListItem i in Citys)
        {
          string si = (string) i.AdditionalProperties[NAME];
          if (_selectedCitys.Contains(si))
          {
            i.Selected = true;
            Refresh(Citys, i);
          }
        }
      }

      Refresh(Countrys, item);
    }

    public void ChangeCity(ListItem item)
    {
      string s = (string) item.AdditionalProperties[NAME];
      if (_selectedCitys.Contains(s))
      {
        _selectedCitys.Remove(s);
        item.Selected = false;
      }
      else
      {
        _selectedCitys.Add(s);
        item.Selected = true;
      }

      // Autofill Country by selected City
      if (_selectedCitys.Count > 0)
      {
        IEnumerable<MyStream> query = from r in WebradioHome.StreamList where EmptyOrContains(_selectedCitys, r.City) select r;
        foreach (MyStream ms in query)
        {
          if (!_selectedCountrys.Contains(ms.Country)) { _selectedCountrys.Add(ms.Country); }
        }

        foreach (ListItem i in Countrys)
        {
          string si = (string) i.AdditionalProperties[NAME];
          if (_selectedCountrys.Contains(si))
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
      string s = (string) item.AdditionalProperties[NAME];
      if (_selectedBitrate.Contains(s))
      {
        _selectedBitrate.Remove(s);
        item.Selected = false;
      }
      else
      {
        _selectedBitrate.Add(s);
        item.Selected = true;
      }
      Refresh(Bitrate, item);
    }

    public void ChangeGenre(ListItem item)
    {
      string s = (string) item.AdditionalProperties[NAME];
      if (_selectedGenres.Contains(s))
      {
        _selectedGenres.Remove(s);
        item.Selected = false;
      }
      else
      {
        _selectedGenres.Add(s);
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
        if (ms.Country != "" & !_countries.Contains(ms.Country))
        {
          _countries.Add(ms.Country);
        }

        // Add Citys
        if (ms.City != "" & !_cities.Contains(ms.City))
        {
          _cities.Add(ms.City);
        }

        // Add Bitrate
        if (ms.Bitrate != "")
        {
          string br = ms.Bitrate.Replace(" kbps", "").PadLeft(3, '0');
          if (!_bitrates.Contains(br))
          {
            _bitrates.Add(br);
          }
        }

        // Add Genres
        string[] split = ms.Genres.Split(new[] { ',' });
        foreach (string s in split)
        {
          if (s.Trim() != "" & !_genres.Contains(s.Trim()))
          {
            _genres.Add(s.Trim());
          }
        }
      }

      _countries.Sort();
      _cities.Sort();
      _bitrates.Sort();
      _genres.Sort();
      FillAllItemsList();
    }

    private void FillAllItemsList()
    {
      ClearItemsList();

      foreach (string s in _countries)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = s;
        item.SetLabel("Name", s);
        if (_selectedCountrys.Contains(s))
        {
          item.Selected = true;
        }
        Countrys.Add(item);
      }
      Refresh(Countrys);

      foreach (string s in _cities)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = s;
        item.SetLabel("Name", s);
        if (_selectedCitys.Contains(s))
        {
          item.Selected = true;
        }
        Citys.Add(item);
      }
      Refresh(Citys);

      foreach (string s in _bitrates)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = Convert.ToInt32(s) + " kbps";
        item.SetLabel("Name", Convert.ToInt32(s) + " kbps");
        if (_selectedBitrate.Contains(Convert.ToInt32(s) + " kbps"))
        {
          item.Selected = true;
        }
        Bitrate.Add(item);
      }
      Refresh(Bitrate);

      foreach (string s in _genres)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = s;
        item.SetLabel("Name", s);
        if (_selectedGenres.Contains(s))
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
      _selectedCountrys.Clear();
      _selectedCitys.Clear();
      _selectedBitrate.Clear();
      _selectedGenres.Clear();
    }

    private void Refresh(ItemsList list, ListItem item)
    {
      RefreshState(list);
      item.FireChange();
    }

    private void Refresh(ItemsList list)
    {
      RefreshState(list);
      list.FireChange();
    }

    private void RefreshState(ItemsList list)
    {
      if (list == Countrys) { CountryState = Convert.ToString(_selectedCountrys.Count) + "/" + Convert.ToString(Countrys.Count); }
      if (list == Citys) { CityState = Convert.ToString(_selectedCitys.Count) + "/" + Convert.ToString(Citys.Count); }
      if (list == Bitrate) { BitrateState = Convert.ToString(_selectedBitrate.Count) + "/" + Convert.ToString(Bitrate.Count); }
      if (list == Genres) { GenreState = Convert.ToString(_selectedGenres.Count) + "/" + Convert.ToString(Genres.Count); }
      RefreshState();
    }

    private void RefreshState()
    {
      int x = 0;
      if (_selectedCountrys.Count + _selectedCitys.Count + _selectedBitrate.Count + _selectedGenres.Count > 0)
      {
        IEnumerable<MyStream> query = from r in WebradioHome.StreamList
                                      where
                                        EmptyOrContains(_selectedCountrys, r.Country)
                                        && EmptyOrContains(_selectedCitys, r.City)
                                        && ContainsAny(_selectedGenres, r.Genres)
                                        && EmptyOrContains(_selectedBitrate, r.Bitrate)
                                      select r;
        x = query.Count();
      }
      SelectedStreamsCount = Convert.ToString(x) + "/" + Convert.ToString(WebradioHome.StreamList.Count);
    }

    private bool EmptyOrContains(List<string> l, string s)
    {
      return l.Count == 0 || l.Contains(s);
    }

    private bool ContainsAny(List<string> l, string s)
    {
      if (l.Count == 0) { return true; }

      string[] split = s.Split(new[] { ',' });
      return split.Any(l.Contains);
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
    public List<MyFilter> FilterList { get; set; }
    static XmlSerializer _serializer = new XmlSerializer(typeof(MyFilters));
    static FileStream _stream;

    public MyFilters()
    {
      FilterList = new List<MyFilter>();
    }

    public MyFilters(List<MyFilter> filters)
    {
      FilterList = new List<MyFilter>();
      FilterList = filters;
    }

    public static MyFilters Read(string xmlFile)
    {
      MyFilters s = new MyFilters();
      try
      {
        _stream = new FileStream(xmlFile, FileMode.Open);
        s = (MyFilters) _serializer.Deserialize(_stream);
      }
      catch (Exception ex)
      {
        ServiceRegistration.Get<ILogger>().Error("WebRadio: Error reading filters", ex);
      }
      finally
      {
        _stream.Close();
        _serializer = null;
      }
      return s;
    }

    public static bool Write(string xmlFile, Object obj)
    {
      try
      {
        _stream = new FileStream(xmlFile, FileMode.Create);
        _serializer = new XmlSerializer(typeof(MyFilters));
        _serializer.Serialize(_stream, obj);
      }
      catch (Exception ex)
      {
        ServiceRegistration.Get<ILogger>().Error("WebRadio: Error writing favorites", ex);
      }
      finally
      {
        _stream.Close();
        _serializer = null;
      }
      return true;
    }

  }

  public class MyFilter
  {
    [XmlElement("Titel")]
    public string Title { get; set; }
    [XmlElement("fCountrys")]
    public List<string> Countries { get; set; }
    [XmlElement("fCitys")]
    public List<string> Cities { get; set; }
    [XmlElement("fGenres")]
    public List<string> Genres { get; set; }
    [XmlElement("fBitrate")]
    public List<string> Bitrates { get; set; }

    public MyFilter()
    {
      Title = "";
      Countries = new List<string>();
      Cities = new List<string>();
      Genres = new List<string>();
      Bitrates = new List<string>();
    }

    public MyFilter(String titel, List<string> countrys, List<string> citys, List<string> genres, List<string> bitrate)
    {
      Title = titel;
      Countries = countrys;
      Cities = citys;
      Genres = genres;
      Bitrates = bitrate;
    }
  }

  #endregion

}
