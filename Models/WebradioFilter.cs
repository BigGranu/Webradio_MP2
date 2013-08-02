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

    public static string _file = System.Windows.Forms.Application.StartupPath + "\\Plugins\\Webradio\\Data\\WebradioFilters.xml";

    const string NAME = "name";

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

    // Filter
    private AbstractProperty _filterTitelProperty = null;
    public AbstractProperty FilterTitelProperty
    {
      get { return _filterTitelProperty; }
    }

    public string FilterTitel
    {
      get { return (string)_filterTitelProperty.GetValue(); }
      set { _filterTitelProperty.SetValue(value); }
    }

    public WebradioFilter()
    {
      FillAllLists();
    }

    /// <summary>
    /// Import all Details (Countrys, Citys ...)
    /// </summary>
    public void FillAllLists()
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

      // Sort Countrys
      Coun_List.Sort();
      foreach (string s in Coun_List)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = s;
        item.SetLabel("Name", s);
        Countrys.Add(item);
      }

      // Sort Citys
      City_List.Sort();
      foreach (string s in City_List)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = s;
        item.SetLabel("Name", s);
        Citys.Add(item);
      }

      // Sort Bitrate
      Bitr_List.Sort();
      foreach (string s in Bitr_List)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = Convert.ToInt32(s) + " kbps";
        item.SetLabel("Name", Convert.ToInt32(s) + " kbps");
        Bitrate.Add(item);
      }

      // Sort Genres
      Genr_List.Sort();
      foreach (string s in Genr_List)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = s;
        item.SetLabel("Name", s); 
        Genres.Add(item);
      }
    }

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

    public void SetImportFilter(ListItem item)
    {
      FilterTitel = (string)item.AdditionalProperties[NAME];
      foreach (MyFilter f in FilterList)
      {
        if (f.Titel == FilterTitel)
        {
          // Countrys
          foreach (ListItem i in Countrys)
          {
            if (f.fCountrys.Contains(i.AdditionalProperties[NAME]))
            {
              i.Selected = true;
            }
            else
            {
              i.Selected = false;
            }
          }
          Countrys.FireChange();

          // Citys
          foreach (ListItem i in Citys)
          {
            if (f.fCitys.Contains(i.AdditionalProperties[NAME]))
            {
              i.Selected = true;
            }
            else
            {
              i.Selected = false;
            }
          }
          Citys.FireChange();

          // Bitrate
          foreach (ListItem i in Bitrate)
          {
            if (f.fBitrate.Contains(i.AdditionalProperties[NAME]))
            {
              i.Selected = true;
            }
            else
            {
              i.Selected = false;
            }
          }
          Bitrate.FireChange();

          // Genres
          foreach (ListItem i in Genres)
          {
            if (f.fGenres.Contains(i.AdditionalProperties[NAME]))
            {
              i.Selected = true;
            }
            else
            {
              i.Selected = false;
            }
          }
          Genres.FireChange();

        }
      }
    }

    /// <summary>
    /// Rename a Entry
    /// </summary>
    public void Rename(ListItem item)
    {
    }

    /// <summary>
    /// Added a Entry
    /// </summary>
    public void Add()
    {
    }

    /// <summary>
    /// Save all Changes on Site
    /// </summary>
    public void Save()
    {
      FilterList.Add(new MyFilter("test1", selectedCountrys, selectedCitys, selectedGenres, selectedBitrate));
      MyFilters.Write(_file, new MyFilters(FilterList));
    }

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
      item.FireChange();
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
      item.FireChange();
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
      item.FireChange();
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
     item.FireChange();
    }
    #endregion


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
      _filterTitelProperty = new WProperty(typeof(string), string.Empty);
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
