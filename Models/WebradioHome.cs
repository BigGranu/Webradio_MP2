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
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;
using Webradio.Helper_Classes;
using Webradio.Player;

namespace Webradio.Models
{
  public class WebradioHome : IWorkflowModel
  {
    #region Consts

    public const string MODEL_ID_STR = "EA3CC191-0BE5-4C8D-889F-E9C4616AB554";
    public static Guid MODEL_ID = new Guid(MODEL_ID_STR);
    public const string STREAM_ID = "StreamID";

    #endregion

    public static string CurrentStreamLogo = string.Empty;
    public static MyStream SelectedStream = new MyStream();
    public static ItemsList AllRadioStreams = new ItemsList();
    public static List<MyStream> StreamList = new List<MyStream>();

    public void Init()
    {
      // if no Streamlist found, load a new List from Web
      if (!StreamlistUpdate.StreamListExists())
      {
        StreamlistUpdate.MakeUpdate();
        return; // Update runs async
      }

      StreamlistUpdate.CheckUpdate();
      MyStreams ms = MyStreams.Read(StreamlistUpdate.StreamListFile);
      StreamList = ms.StreamList;
      FillItemList(StreamList);
    }

    /// <summary>
    /// Fill the List and set the Labels
    /// </summary>
    public static void FillItemList(List<MyStream> list)
    {
      AllRadioStreams.Clear();
      int indx = 0;
      foreach (MyStream ms in list)
      {
        indx += 1;
        var item = new ListItem();
        item.AdditionalProperties[STREAM_ID] = ms.ID;
        item.SetLabel("Name", ms.Titel);
        item.SetLabel("Country", ms.Country);
        item.SetLabel("City", ms.City);
        item.SetLabel("Genres", ms.Genres);
        item.SetLabel("Bitrate", ms.Bitrate);
        item.SetLabel("Logo", SetStreamLogo(ms));
        item.SetLabel("ImageSrc", SetStreamLogo(ms));
        item.SetLabel("Description", ms.Description);
        item.SetLabel("Indx", indx + "/" + list.Count );

        AllRadioStreams.Add(item);
      }
      AllRadioStreams.FireChange();
    }

    /// <summary>
    /// Set the Logo of a Stream or use the DefaultLogo
    /// </summary>
    public static string SetStreamLogo(MyStream ms)
    {
      string s = "DefaultLogo.png";
      if (ms.Logo != "")
      {
        s = ms.Logo;
      }
      return s;
    }

    /// <summary>
    /// Reset the List and show all Streams
    /// </summary>
    public void ShowAllStreams()
    {
      FillItemList(StreamList);
    }

    /// <summary>
    /// Play the Stream with the current StreamID and Set the Playcount +1
    /// </summary>
    private void Play(MyStream ms)
    {
      CurrentStreamLogo = SetStreamLogo(ms);
      WebRadioPlayerHelper.PlayStream(ms);
      SetPlayCount(ms.ID);
    }

    /// <summary>
    /// Get the selected Stream
    /// </summary>
    public void SelectStream(ListItem item)
    {
      SelectedStream = GetStreamById((int) item.AdditionalProperties[STREAM_ID]);
      Play(SelectedStream);
    }

    /// <summary>
    /// Set the Playcount of playing Stream +1
    /// </summary>
    private void SetPlayCount(int id)
    {
      foreach (var f in StreamList.Where(f => f.ID == id))
      {
        f.PlayCount += 1;
      }
      MyStreams.Write(StreamlistUpdate.StreamListFile, new MyStreams(StreamList));
    }

    /// <summary>
    /// Get the Stream of selected ID
    /// </summary>
    public MyStream GetStreamById(int id)
    {
      return StreamList.FirstOrDefault(f => f.ID == id);
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
     // Todo: select any or the Last ListItem
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
  public class MyStreams
  {
    public string Version = "1";
    public List<MyStream> StreamList = new List<MyStream>();

    static readonly XmlSerializer Serializer = new XmlSerializer(typeof(MyStreams));
    static FileStream _stream;

    public MyStreams()
    {
    }

    public MyStreams(List<MyStream> streams)
    {
      StreamList = streams;
    }

    public MyStreams(List<MyStream> streams, string version)
    {
      Version = version;
      StreamList = streams;
    }

    public static MyStreams Read(string xmlFile)
    {
      _stream = new FileStream(xmlFile, FileMode.Open);
      MyStreams s = (MyStreams) Serializer.Deserialize(_stream);
      _stream.Close();
      return s;
    }

    public static void Write(string xmlFile, Object obj)
    {
      _stream = new FileStream(xmlFile, FileMode.Create);
      Serializer.Serialize(_stream, obj);
      _stream.Close();
    }
  }
  #endregion

}