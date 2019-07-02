#region Copyright (C) 2007-2019 Team MediaPortal

/*
    Copyright (C) 2007-2019 Team MediaPortal
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
using MediaPortal.Common.General;
using MediaPortal.Common.Localization;
using MediaPortal.Common.Settings;
using MediaPortal.Extensions.UserServices.FanArtService.Client.Models;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;
using MediaPortal.UI.SkinEngine.Controls.ImageSources;
using Webradio.Dialogues;
using Webradio.Helper;
using Webradio.Player;
using Webradio.Settings;

namespace Webradio.Models
{
  public class WebradioHome : IWorkflowModel
  {
    public const string RES_MUSICINFO = "[MusikInfo]";

    public static Guid MODEL_ID = new Guid(MODEL_ID_STR);
    public static Guid MODEL_ID_MUSICINFO = new Guid("6518F915-7F7D-4E60-B24C-8BCA0681DDFE");
    public static Guid ACTION_ID_MUSICINFO = new Guid("269521C1-A9C1-4664-A923-055C30FF015F");

    public static string CurrentStreamLogo = string.Empty;
    public static string CurrentListeners = string.Empty;
    public static MyStream SelectedStream = new MyStream();
    public static ItemsList AllRadioStreams = new ItemsList();
    public static List<MyStream> StreamList = new List<MyStream>();

    private static List<FilterSetupInfo> FilterList = new List<FilterSetupInfo>();
    private static FilterSettings FilterSettings;

    private static readonly AbstractProperty _defaultImage = new WProperty(typeof(string), string.Empty);

    public int C = 0;

    public AbstractProperty DefaultImageProperty => _defaultImage;

    public string DefaultImage
    {
      get => (string)_defaultImage.GetValue();
      set => _defaultImage.SetValue(value);
    }

    public void Init()
    {
      ClearFanart();

      // if no Streamlist found, load a new List from Web
      if (!StreamlistUpdate.StreamListExists())
      {
        WebradioDlgLoadUpdate.LoadSenderListe();
        return; // Update runs async
      }

      DefaultImage = "DefaultLogo.png";
      StreamlistUpdate.CheckUpdate();
      var ms = MyStreams.Read(StreamlistUpdate.StreamListFile);
      StreamList = ms.Streams;

      FilterSettings = ServiceRegistration.Get<ISettingsManager>().Load<FilterSettings>();
      var af = FilterSettings.ActiveFilter;

      if (af != null)
      {
        var mst = MyStreams.Filtered(af, StreamList);
        FillItemList(mst);
      }
      else
      {
        FillItemList(StreamList);
      }
    }

    /// <summary>
    /// Fill the List and set the Labels
    /// </summary>
    public static void FillItemList(List<MyStream> list)
    {
      AllRadioStreams.Clear();
      var indx = 0;
      foreach (var ms in list)
      {
        indx += 1;
        SetFallbackValues(ms);
        var item = new ListItem { AdditionalProperties = { [STREAM_URL] = ms.StreamUrls[0].StreamUrl } };
        item.SetLabel("Name", ms.Title);
        item.SetLabel("Country", "[Country." + ms.Country + "]");
        item.SetLabel("CountryCode", ms.Country);
        item.SetLabel("City", ms.City);
        item.SetLabel("Genres", ms.Genres);
        item.SetLabel("Bitrate", ms.StreamUrls[0].Bitrate);
        item.SetLabel("StreamProvider", ms.StreamUrls[0].Provider);
        item.SetLabel("StreamFrequenz", ms.StreamUrls[0].Frequenz);
        item.SetLabel("StreamMode", ms.StreamUrls[0].Mode);
        item.SetLabel("StreamName", ms.StreamUrls[0].Name);
        item.SetLabel("StreamTyp", ms.StreamUrls[0].Typ);
        item.SetLabel("Logo", SetStreamLogo(ms));
        item.SetLabel("ImageSrc", SetStreamLogo(ms));
        item.SetLabel("Description", SetStreamDescription(ms));
        item.SetLabel("Language", "[Language." + ms.Language + "]");
        item.SetLabel("LanguageCode", ms.Language);
        item.SetLabel("Indx", indx + "/" + list.Count);

        AllRadioStreams.Add(item);
      }

      AllRadioStreams.FireChange();
    }

    /// <summary>
    /// Set the Description by Language
    /// </summary>
    public static string SetStreamDescription(MyStream ms)
    {
      var desc = "";
      var localization = ServiceRegistration.Get<ILocalization>().CurrentCulture.Name.Substring(0, 2);

      // is the original language available
      foreach (var d in ms.Descriptions)
        if (d.Languagecode.Contains(localization))
          return d.Txt;

      // is English available
      foreach (var d in ms.Descriptions)
        if (d.Languagecode.Contains("en") & (d.Txt != ""))
          return d.Txt;

      // is any language available
      foreach (var d in ms.Descriptions)
        if (d.Txt != "")
          return d.Txt;

      return desc;
    }

    /// <summary>
    /// Set the Logo of a Stream or use the DefaultLogo
    /// </summary>
    public static string SetStreamLogo(MyStream ms)
    {
      var s = "DefaultLogo.png";
      if (ms.Logo != "") s = ms.Logo;
      return s;
    }

    /// <summary>
    /// Set the FallbackValues of the current Stream
    /// </summary>
    public static void SetFallbackValues(MyStream ms)
    {
      if (ms.Country == "") ms.Country = "unknown";
      if (ms.City == "") ms.City = "unknown";
      if (ms.StreamUrls[0].Bitrate == "") ms.StreamUrls[0].Bitrate = "unknown";
      if (ms.Genres == "") ms.Genres = "unknown";
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
      if (WebRadioPlayerHelper.PlayStream(ms))
        StreamListeners.Listeners();
    }

    /// <summary>
    /// Get the selected Stream
    /// </summary>
    public void SelectStream(ListItem item)
    {
      SelectedStream = GetStream((string)item.AdditionalProperties[STREAM_URL]);
      Play(SelectedStream);
    }

    /// <summary>
    /// Get the Stream of selected ID
    /// </summary>
    public MyStream GetStream(string url)
    {
      return StreamList.FirstOrDefault(f => f.StreamUrls[0].StreamUrl == url);
    }

    private void ClearFanart()
    {
      var fanArtBgModel = (FanArtBackgroundModel)ServiceRegistration.Get<IWorkflowManager>().GetModel(FanArtBackgroundModel.FANART_MODEL_ID);
      if (fanArtBgModel != null) fanArtBgModel.ImageSource = new MultiImageSource { UriSource = null };
    }

    #region Consts

    public const string MODEL_ID_STR = "EA3CC191-0BE5-4C8D-889F-E9C4616AB554";

    public const string STREAM_URL = "StreamUrl";

    #endregion

    #region IWorkflowModel implementation

    public Guid ModelId => new Guid(MODEL_ID_STR);

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
      ClearFanart();
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
