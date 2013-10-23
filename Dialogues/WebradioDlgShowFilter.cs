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
using MediaPortal.Common;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;
using Webradio.Helper_Classes;
using Webradio.Models;
using Webradio.Settings;

namespace Webradio.Dialogues
{
  internal class WebradioDlgShowFilter : IWorkflowModel
  {
    #region Consts

    public const string MODEL_ID_STR = "63F1DA3E-E87F-4478-83E7-C13966447869";
    public const string KEY_FILTER = "filter";

    #endregion

    public static ItemsList FilterItems = new ItemsList();
    public static List<FilterSetupInfo> FilterList = new List<FilterSetupInfo>();
    private bool _quick = false;

    protected delegate Func<MyStream, bool> CreateFilterDelegate(string filter);

    public void Init()
    {
      ShowFilter();
    }

    public void ShowFilter()
    {
      _quick = false;
      FilterList = ServiceRegistration.Get<ISettingsManager>().Load<FilterSettings>().FilterSetupList;
      if (FilterList == null)
      {
        FilterList = new List<FilterSetupInfo> { new FilterSetupInfo("New Filter", "0", new List<string>(), new List<string>(), new List<string>(), new List<string>()) };
      }

      List<string> list = new List<string>();

      foreach (FilterSetupInfo f in FilterList.Where(f => !list.Contains(f.Titel)))
      {
        list.Add(f.Titel);
      }
      FillItems(list);
    }

    public void Selected(ListItem item)
    {
      if (_quick == true)
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
      foreach (IEnumerable<MyStream> query in from f in FilterList
        where f.Titel == (string)item.AdditionalProperties[KEY_FILTER]
        select (from r in WebradioHome.StreamList
          where
            Contains(f.Countrys, r.Country)
            && Contains(f.Citys, r.City)
            && Contains2(f.Genres, r.Genres)
            && Contains(f.Bitrate, r.Bitrate)
          select r))
      {
        foreach (MyStream ms in query.Where(ms => !list.Contains(ms)))
        {
          list.Add(ms);
        }
        break;
      }
      WebradioHome.FillItemList(list);
    }

    public void SelectedQuick(ListItem item)
    {
      Func<MyStream, bool> predicate = (Func<MyStream, bool>)item.AdditionalProperties[KEY_FILTER];
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
      _quick = true;
      CreateFilters(s => !string.IsNullOrWhiteSpace(s.Country), s => s.Country, filterValue => s => s.Country == filterValue);
    }

    public void QuickCity()
    {
      _quick = true;
      CreateFilters(s => !string.IsNullOrWhiteSpace(s.City), s => s.City, filterValue => s => s.City == filterValue);
    }

    public void QuickBitrate()
    {
      _quick = true;
      CreateFilters(s => !string.IsNullOrWhiteSpace(s.Bitrate), s => s.Bitrate, filterValue => s => s.Bitrate == filterValue);
    }

    public void QuickGenre()
    {
      _quick = true;
      CreateFiltersMulti(s => !string.IsNullOrWhiteSpace(s.Genres), s => s.Genres.Split(','), filterValue => s => Contains2(s.Genres.Split(','), filterValue));
    }

    private void FillItems(IEnumerable<string> list, CreateFilterDelegate createFilterDelegate)
    {
      FilterItems.Clear();
      var sorted = list.ToList();
      sorted.Sort();
      foreach (string s in sorted)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[KEY_FILTER] = createFilterDelegate(s); // Creates a dynamic filter like s => s.Titel="Radio 100"
        item.SetLabel("Name", s);
        FilterItems.Add(item);
      }
      FilterItems.FireChange();
    }

    private void FillItems(List<string> list)
    {
      FilterItems.Clear();
      list.Sort();
      foreach (string s in list)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[KEY_FILTER] = s;
        item.SetLabel("Name", s);
        FilterItems.Add(item);
      }
      FilterItems.FireChange();
    }

    private static bool Contains(List<string> l, string s)
    {
      return l.Count == 0 || l.Contains(s);
    }

    private static bool Contains2(ICollection<string> l, string s)
    {
      if (s == null) throw new ArgumentNullException("s");
      if (l.Count == 0)
      {
        return true;
      }

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