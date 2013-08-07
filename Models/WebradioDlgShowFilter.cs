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
  internal class WebradioDlgShowFilter : IWorkflowModel
  {
    public const string MODEL_ID_STR = "63F1DA3E-E87F-4478-83E7-C13966447869";
    const string NAME = "name";

    public static ItemsList Items = new ItemsList();
    public static List<MyFilter> FilterList;

    public WebradioDlgShowFilter()
    {
    }

    public void Init()
    {
      FilterList = new List<MyFilter>();
      FilterList = MyFilters.Read().FilterList;
      List<string> list = new List<string>();

      foreach (MyFilter f in FilterList)
      {
        if (!list.Contains(f.Titel)) { list.Add(f.Titel); }
      }
      FillItems(list);
    }

    public void Selected(ListItem item)
    {
      List<MyStream> list = new List<MyStream>();
      foreach (MyFilter f in FilterList)
      {
        if (f.Titel == (string)item.AdditionalProperties[NAME])
        {
          IEnumerable<MyStream> query = from r in WebradioHome.StreamList where
                                          _contains(f.fCountrys, r.Country)
                                          && _contains(f.fCitys, r.City)
                                          && _contains2(f.fGenres, r.Genres)
                                          && _contains(f.fBitrate, r.Bitrate)
                                        select r;

          foreach (MyStream ms in query)
          {
            if (!list.Contains(ms)) { list.Add(ms); }
          }
          break;
        }
      }
      WebradioHome.FillItemList(list);
      //int x = 0;
      //if (selectedCountrys.Count + selectedCitys.Count + selectedBitrate.Count + selectedGenres.Count > 0)
      //{
     
      //  x = query.Count<MyStream>();
      //}
      //SelectedStreamsCount = Convert.ToString(x) + "/" + Convert.ToString(WebradioHome.StreamList.Count);
    }



    public void QuickCountry()
    {
      List<string> list = new List<string>();
      foreach (MyStream ms in WebradioHome.StreamList)
      {
        if (!list.Contains(ms.Country) & ms.Country != "") { list.Add(ms.Country); }
      }
      FillItems(list);
    }

    public void QuickCity()
    {
      List<string> list = new List<string>();
      foreach (MyStream ms in WebradioHome.StreamList)
      {
        if (!list.Contains(ms.City) & ms.City != "") { list.Add(ms.City); }
      }
      FillItems(list);
    }

    public void QuickBitrate()
    {
      List<string> list = new List<string>();
      foreach (MyStream ms in WebradioHome.StreamList)
      {
        if (!list.Contains(ms.Bitrate) & ms.Bitrate != "") { list.Add(ms.Bitrate); }
      }
      FillItems(list);
    }

    public void QuickGenre()
    {
      List<string> list = new List<string>();
      foreach (MyStream ms in WebradioHome.StreamList)
      {
        string[] split = ms.Genres.Split(new Char[] { ',' });
        foreach (string s in split)
        {
          if (s.Trim() != "" & !list.Contains(s.Trim()))
          {
            list.Add(s.Trim());
          }
        }
      }
      FillItems(list);
    }

    private void FillItems(List<string> list)
    {
      Items.Clear();
      list.Sort();
      foreach (string s in list)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = s;
        item.SetLabel("Name", s);
        Items.Add(item);
      }
      Items.FireChange();
    }

    private static bool _contains(List<string> L, string S)
    {
      if (L.Count == 0)
        return true;
      return L.Contains(S);
    }

    private static bool _contains2(List<string> L, string S)
    {
      if (L.Count == 0) { return true; }

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
