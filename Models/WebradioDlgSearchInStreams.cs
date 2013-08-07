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
  internal class WebradioDlgSearchInStreams : IWorkflowModel
  {
    public const string MODEL_ID_STR = "7AE86A07-DB55-4AA6-9FBF-B1888A4FF6DA";

    private static AbstractProperty _SearchTextProperty = null;
    public AbstractProperty SearchTextProperty
    {
      get { return _SearchTextProperty; }
    }
    public static string SearchText
    {
      get { return (string)_SearchTextProperty.GetValue(); }
      set
      {
        _SearchTextProperty.SetValue(value);
      }
    }

    public WebradioDlgSearchInStreams()
    {
    }

    public void Init()
    {
      _SearchTextProperty = new WProperty(typeof(string), string.Empty);
    }

    public void SearchTitel()
    {
      List<MyStream> list = new List<MyStream>();
      foreach (MyStream ms in WebradioHome.StreamList)
      {
        if (ms.Titel.IndexOf(SearchText) > 0)
        {
          list.Add(ms);
        }
      }
      WebradioHome.FillItemList(list);
    }

    public void SearchDescription()
    {
      List<MyStream> list = new List<MyStream>();
      foreach (MyStream ms in WebradioHome.StreamList)
      {
        if (ms.Description.IndexOf(SearchText) > 0)
        {
          list.Add(ms);
        }
      }
      WebradioHome.FillItemList(list);
    }

    public void SearchAll()
    {
      List<MyStream> list = new List<MyStream>();
      foreach (MyStream ms in WebradioHome.StreamList)
      {
        if (ms.Titel.IndexOf(SearchText) > 0)
        {
          list.Add(ms);
        }

        if (ms.Description.IndexOf(SearchText) > 0)
        {
          list.Add(ms);
        }
      }
      WebradioHome.FillItemList(list);
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
