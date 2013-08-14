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
using System.Linq;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;

namespace Webradio.Models
{
  internal class WebradioDlgShowFilter : IWorkflowModel
  {
    public const string MODEL_ID_STR = "63F1DA3E-E87F-4478-83E7-C13966447869";
    const string KEY_FILTER = "filter";

    protected ItemsList _items = new ItemsList();
    protected List<MyFilter> _filterList;

    private bool Quick = false;

    protected delegate Func<MyStream, bool> CreateFilterDelegate(string filter);

    public ItemsList Items
    {
      get { return _items; }
    }

    public void Init()
    {    
      ShowFilter();
    }

    public void ShowFilter()
    { 
      Quick = false;
      _filterList = new List<MyFilter>();
      _filterList = MyFilters.Read().FilterList;
      List<string> list = new List<string>();

      foreach (MyFilter f in _filterList)
      {
        if (!list.Contains(f.Titel)) { list.Add(f.Titel); }
      }
      FillItems(list);
    }

    public void Selected(ListItem item)
    {
      if (Quick == true)
      {
        SelectedQuick(item);
      }
      else
      {
        SelectedFilter(item);
      }
    }

    public void SelectedFilter(ListItem item)
    {
      List<MyStream> list = new List<MyStream>();
      foreach (MyFilter f in _filterList)
      {
        if (f.Titel == (string)item.AdditionalProperties[KEY_FILTER])
        {
          IEnumerable<MyStream> query = from r in WebradioHome.StreamList
                                        where
                                          Contains(f.fCountrys, r.Country)
                                          && Contains(f.fCitys, r.City)
                                          && Contains2(f.fGenres, r.Genres)
                                          && Contains(f.fBitrate, r.Bitrate)
                                        select r;

          foreach (MyStream ms in query)
          {
            if (!list.Contains(ms)) { list.Add(ms); }
          }
          break;
        }
      }
      WebradioHome.FillItemList(list);
    }

    public void SelectedQuick(ListItem item)
    {
      Func<MyStream, bool> predicate = (Func<MyStream, bool>) item.AdditionalProperties[KEY_FILTER];
      var filtered = WebradioHome.StreamList.Where(predicate).ToList();
      WebradioHome.FillItemList(filtered);
    }

    private void CreateFilters(Func<MyStream, bool> predicate, Func<MyStream, string> selector, CreateFilterDelegate filter)
    {
      var list = WebradioHome.StreamList.Where(predicate).Select(selector).Distinct();
      FillItems(list, filter);
    }

    private void CreateFiltersMulti(Func<MyStream, bool> predicate, Func<MyStream, string[]> selector, CreateFilterDelegate filter)
    {
      var list = WebradioHome.StreamList.Where(predicate).SelectMany(selector).Select(s => s.Trim()).Distinct();
      FillItems(list, filter);
    }

    public void QuickCountry()
    {
      Quick = true;
      CreateFilters(s => !string.IsNullOrWhiteSpace(s.Country), s => s.Country, filterValue => s => s.Country == filterValue);
    }

    public void QuickCity()
    {
      Quick = true;
      CreateFilters(s => !string.IsNullOrWhiteSpace(s.City), s => s.City, filterValue => s => s.City == filterValue);
    }

    public void QuickBitrate()
    {
      Quick = true;
      CreateFilters(s => !string.IsNullOrWhiteSpace(s.Bitrate), s => s.Bitrate, filterValue => s => s.Bitrate == filterValue);
    }

    public void QuickGenre()
    {
      Quick = true;
      CreateFiltersMulti(s => !string.IsNullOrWhiteSpace(s.Genres), s => s.Genres.Split(','), filterValue => s => Contains2(s.Genres.Split(','), filterValue));
    }

    private void FillItems(IEnumerable<string> list, CreateFilterDelegate createFilterDelegate)
    {
      _items.Clear();
      var sorted = list.ToList();
      sorted.Sort();
      foreach (string s in sorted)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[KEY_FILTER] = createFilterDelegate(s); // Creates a dynamic filter like s => s.Titel="Radio 100"
        item.SetLabel("Name", s);
        _items.Add(item);
      }
      _items.FireChange();
    }

    private void FillItems(List<string> list)
    {
      Items.Clear();
      list.Sort();
      foreach (string s in list)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[KEY_FILTER] = s;
        item.SetLabel("Name", s);
        Items.Add(item);
      }
      Items.FireChange();
    }

    private static bool Contains(List<string> L, string S)
    {
      if (L.Count == 0)
        return true;
      return L.Contains(S);
    }

    private static bool Contains2(ICollection<string> l, string s)
    {
      if (s == null) throw new ArgumentNullException("s");
      if (l.Count == 0) { return true; }

      string[] split = s.Split(',');
      return split.Any(part => l.Contains(part.Trim()));
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
