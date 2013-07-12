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
using System.Collections.ObjectModel;
using System.Collections.Generic;
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
  public class WebradioFavorites : IWorkflowModel
  {
    public const string MODEL_ID_STR = "EC2F9DD4-C694-4C2D-9EFB-092AA1F4BD94";
    public static string _file = System.Windows.Forms.Application.StartupPath + "\\Plugins\\Webradio\\Data\\WebradioFavorites.xml";

    // List of all Favorites in Xmlfile
    public static Favorits FavoritList = new Favorits();

    //test
    public  ObservableCollection<Favorit> Liste = new ObservableCollection<Favorit>();

    public WebradioFavorites()
    {
    }

    /// <summary>
    /// Remove a Entry
    /// </summary>
    public void Delete()
    {
    }

    /// <summary>
    /// Rename a Entry
    /// </summary>
    public void Rename()
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

  #region Read/Write Xml
  public class Favorits
  {
      public List<Favorit> FavoritList = new List<Favorit>();

      public Favorits()
      {
      }

      public static Favorits Read(string XmlFile)
      {
          if (!File.Exists(XmlFile)) { File.Create(XmlFile); }
          Favorits _list = new Favorits();
          XmlSerializer serializer = new XmlSerializer(typeof(Favorits));
          FileStream fs = new FileStream(XmlFile, FileMode.Open);

          try
          {
              _list = (Favorits)serializer.Deserialize(fs);

          }
          catch (Exception ex)
          {
              Console.WriteLine(ex.StackTrace);
          }
          finally
          {
              fs.Close();
              serializer = null;
          }

          return _list;
      }

      public static bool Write(string XmlFile, Favorits mliste)
      {
          XmlSerializer serializer = new XmlSerializer(typeof(Favorits));
          StreamWriter writer = new StreamWriter(XmlFile, false);

          try
          {
              serializer.Serialize(writer, mliste);
          }
          catch (Exception ex)
          {
              Console.WriteLine(ex.StackTrace);
          }
          finally
          {
              writer.Close();
              serializer = null;
          }

          return true;
      }
  }

  public class Favorit
  {
      public string Titel;
      public bool Active;
      public List<string> IDs;

      public Favorit()
      {
          Titel = "";
          Active = true;
          IDs = new List<string>();
      }

      public Favorit(String _Titel, bool _Active, List<string> _IDs)
      {
          Titel = _Titel;
          Active = _Active;
          IDs = _IDs;
      }
  }
    #endregion

}
