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
using MediaPortal.Common.General;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;
using Webradio.Models;

namespace Webradio.Dialogues
{
  internal class WebradioDlgSearchInStreams : IWorkflowModel
  {
    #region Consts

    public const string MODEL_ID_STR = "7AE86A07-DB55-4AA6-9FBF-B1888A4FF6DA";

    #endregion

    #region Propertys

    private static AbstractProperty _searchTextProperty = new WProperty(typeof(string), string.Empty);

    public AbstractProperty SearchTextProperty
    {
      get { return _searchTextProperty; }
    }

    public static string SearchText
    {
      get { return (string)_searchTextProperty.GetValue(); }
      set { _searchTextProperty.SetValue(value); }
    }

    #endregion

    public void SearchTitel()
    {
      var list = WebradioHome.StreamList.Where(ms => ms.Titel.ToUpper().IndexOf(SearchText.ToUpper(), StringComparison.Ordinal) >= 0).ToList();
      WebradioHome.FillItemList(list);
    }

    public void SearchDescription()
    {
      var list = WebradioHome.StreamList.Where(ms => ms.Description.ToUpper().IndexOf(SearchText.ToUpper(), StringComparison.Ordinal) >= 0).ToList();
      WebradioHome.FillItemList(list);
    }

    public void SearchAll()
    {
      var list = WebradioHome.StreamList.Where(ms => ms.Titel.ToUpper().IndexOf(SearchText.ToUpper(), StringComparison.Ordinal) >= 0 | ms.Description.ToUpper().IndexOf(SearchText.ToUpper(), StringComparison.Ordinal) >= 0).ToList();
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