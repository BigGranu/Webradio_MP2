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
using System.Linq;
using System.Xml.Serialization;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;

namespace Webradio.Models
{
  public class WebradioHome : IWorkflowModel
  {
    public const string MODEL_ID_STR = "EA3CC191-0BE5-4C8D-889F-E9C4616AB554";

    const string STREAM_ID = "StreamID";

    protected string _file = System.Windows.Forms.Application.StartupPath + "\\Plugins\\Webradio\\Data\\WebradioSender.xml";

    protected ItemsList _allRadioStreams;

    public ItemsList AllRadioStreams { get { return _allRadioStreams; } }

    public static List<MyStream> StreamList { get; set; }

    public WebradioHome()
    {
      StreamList = new List<MyStream>();
      // beim ersten Start alle Listen füllen-
      if (_allRadioStreams == null)
      {
        MyStreams streams = MyStreams.Read(_file);
        StreamList = streams.StreamList;
        _allRadioStreams = new ItemsList();

        //foreach (MyStream ms in Streams.StreamList) 
        //{ 
        //  AllRadioStreams.Add(ms); 
        //}

        foreach (MyStream ms in streams.StreamList)
        {
          ListItem item = new ListItem();
          item.SetLabel("Name", ms.Title);
          item.SetLabel("Country", ms.Country);
          item.SetLabel("City", ms.City);
          item.SetLabel("Genres", ms.Genres);
          item.SetLabel("Bitrate", ms.Bitrate);
          item.SetLabel("Logo", ms.Logo);
          item.AdditionalProperties[STREAM_ID] = ms.ID;
          item.SetLabel("ImageSrc", ms.Logo);
          _allRadioStreams.Add(item);
        }
      }

      //WebradioFilter.FilterList = Webradio.Models.MyFilters.Read(WebradioFilter._file);
    }

    /// <summary>
    /// Show Dialog to select the Favoritfunctions
    /// </summary>
    public void ShowFavorites(MyStream item)
    {
    }

    /// <summary>
    /// Play the Stream with the current StreamID and Set the Playcount +1
    /// </summary>
    private void Play(int id)
    {
      // Streamurl (GetStreamByID(_ID).URL) an den Player übergeben 
      // noch klären welcher Player dafür wie genutzt wird

      SetPlayCount(id);
    }

    public void SelectStream(ListItem item)
    {
      MyStream ms = GetStreamByID((int) item.AdditionalProperties[STREAM_ID]);
    }

    /// <summary>
    /// Set the Playcount of playing Stream +1
    /// </summary>
    private void SetPlayCount(int id)
    {
      foreach (MyStream f in StreamList.Where(f => f.ID == id))
      {
        f.PlayCount += 1;
      }
      MyStreams.Write(_file, _allRadioStreams);
    }

    /// <summary>
    /// Get the Stream of selected ID
    /// </summary>
    public MyStream GetStreamByID(int id)
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
  public class MyStreams
  {
    public List<MyStream> StreamList { get; set; }

    static readonly XmlSerializer SERIALIZER = new XmlSerializer(typeof(MyStreams));

    public MyStreams()
    {
      StreamList = new List<MyStream>();
    }

    public static MyStreams Read(string xmlFile)
    {
      using (FileStream stream = new FileStream(xmlFile, FileMode.Open))
        return (MyStreams) SERIALIZER.Deserialize(stream);
    }

    public static void Write(string xmlFile, Object obj)
    {
      using (FileStream stream = new FileStream(xmlFile, FileMode.Create))
        SERIALIZER.Serialize(stream, obj);
    }
  }

  public class MyStream
  {
    public int ID { get; set; }
    [XmlElement("Titel")]
    public string Title { get; set; }
    public string URL { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Genres { get; set; }
    public string Bitrate { get; set; }
    public string Description { get; set; }
    public string Home { get; set; }
    public string Logo { get; set; }
    public string Facebook { get; set; }
    public string Twitter { get; set; }
    public bool Love { get; set; }
    public bool Block { get; set; }
    public int PlayCount { get; set; }
    [XmlElement("tag1")]
    public string Tag1 { get; set; }
    [XmlElement("tag2")]
    public string Tag2 { get; set; }
    [XmlElement("tag3")]
    public string Tag3 { get; set; }
    [XmlElement("tag4")]
    public string Tag4 { get; set; }
    public override string ToString()
    {
      return Title;
    }
  }
  #endregion

}
