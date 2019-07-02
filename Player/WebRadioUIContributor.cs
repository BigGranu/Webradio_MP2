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

using System.Timers;
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.Logging;
using MediaPortal.Extensions.UserServices.FanArtService.Client.Models;
using MediaPortal.UI.Players.BassPlayer.Interfaces;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.UI.Presentation.Workflow;
using MediaPortal.UI.SkinEngine.Controls.ImageSources;
using MediaPortal.UiComponents.Media.Models;
using Webradio.Helper;
using Webradio.Models;

namespace Webradio.Player
{
  public class WebRadioUIContributor : BaseTimerControlledModel, IPlayerUIContributor
  {
    public const string SCREEN_FULLSCREEN_AUDIO = "webradio_FullscreenContent";
    public const string SCREEN_CURRENTLY_PLAYING_AUDIO = "webradio_CurrentlyPlaying";

    protected const int UPDATE_INTERVAL_MS = 300;
    protected static AbstractProperty _listenersProperty;
    private readonly NavigationContext _context;
    private readonly Timer ATimer = new Timer();
    protected AbstractProperty _albumProperty;
    protected AbstractProperty _artistBioProperty;

    protected AbstractProperty _artistProperty;

    // only for Fanart    
    private string _cArtist = string.Empty;
    private string _cTitle = string.Empty;
    protected AbstractProperty _currentStreamLogoProperty;

    protected MediaWorkflowStateType _mediaWorkflowStateType;
    protected ITagSource _tagSource;
    protected AbstractProperty _titleProperty;
    protected bool _updating;
    private int FanartCount;
    private TrackInfo TInfo = new TrackInfo();

    #region Constructor & maintainance

    public WebRadioUIContributor() : base(false, UPDATE_INTERVAL_MS)
    {
      _artistProperty = new WProperty(typeof(string), string.Empty);
      _artistBioProperty = new WProperty(typeof(string), string.Empty);
      _titleProperty = new WProperty(typeof(string), string.Empty);
      _albumProperty = new WProperty(typeof(string), string.Empty);
      _currentStreamLogoProperty = new WProperty(typeof(string), string.Empty);
      _listenersProperty = new WProperty(typeof(string), string.Empty);

      _context = ServiceRegistration.Get<IWorkflowManager>().CurrentNavigationContext;

      StartTimer();
    }

    #endregion

    public AbstractProperty ArtistProperty => _artistProperty;

    public string Artist
    {
      get => (string)_artistProperty.GetValue();
      set => _artistProperty.SetValue(value);
    }

    public AbstractProperty ArtistBioProperty => _artistBioProperty;

    public string ArtistBio
    {
      get => (string)_artistBioProperty.GetValue();
      set => _artistBioProperty.SetValue(value);
    }

    public AbstractProperty TitleProperty => _titleProperty;

    public string Title
    {
      get => (string)_titleProperty.GetValue();
      set => _titleProperty.SetValue(value);
    }

    public AbstractProperty AlbumProperty => _albumProperty;

    public string Album
    {
      get => (string)_albumProperty.GetValue();
      set => _albumProperty.SetValue(value);
    }

    public AbstractProperty CurrentStreamLogoProperty => _currentStreamLogoProperty;

    public string CurrentStreamLogo
    {
      get => (string)_currentStreamLogoProperty.GetValue();
      set => _currentStreamLogoProperty.SetValue(value);
    }

    public AbstractProperty ListenersProperty => _listenersProperty;

    public string Listeners
    {
      get => (string)_listenersProperty.GetValue();
      set => _listenersProperty.SetValue(value);
    }

    public bool BackgroundDisabled => false;

    public string Screen
    {
      get
      {
        if (_mediaWorkflowStateType == MediaWorkflowStateType.CurrentlyPlaying)
          return SCREEN_CURRENTLY_PLAYING_AUDIO;
        if (_mediaWorkflowStateType == MediaWorkflowStateType.FullscreenContent)
          return SCREEN_FULLSCREEN_AUDIO;
        return null;
      }
    }

    public virtual MediaWorkflowStateType MediaWorkflowStateType => _mediaWorkflowStateType;

    public virtual void Initialize(MediaWorkflowStateType stateType, IPlayer player)
    {
      _mediaWorkflowStateType = stateType;
      _tagSource = player as ITagSource;
      CurrentStreamLogo = WebradioHome.CurrentStreamLogo;

      // Only for Fanart /////////////
      ATimer.Elapsed += OnTimedEvent;
      ATimer.Interval = 15000;
      ATimer.Start();
      ////////////////////////////////
    }

    // Update GUI properties
    protected override void Update()
    {
      if (_updating)
      {
        ServiceRegistration.Get<ILogger>().Warn("WebRadioUIContributor: last update cycle still not finished.");
        return;
      }

      _updating = true;
      try
      {
        if (_tagSource != null)
        {
          var tags = _tagSource.Tags;

          if (tags == null)
            return;

          Album = tags.album;

          Listeners = WebradioHome.CurrentListeners;

          // Only need for Fanrt ///////////////////////////////////
          Artist = tags.artist;
          Title = tags.title;

          if ((Artist != _cArtist) | (Title != _cTitle))
          {
            _cArtist = Artist;
            _cTitle = Title;

            if ((Title != "") & (Artist != ""))
            {
              TInfo = new TrackInfo(Artist, Title);

              if ((TInfo.FrontCover == null) | (TInfo.FrontCover == ""))
                CurrentStreamLogo = WebradioHome.CurrentStreamLogo;
              else
                CurrentStreamLogo = TInfo.FrontCover;

              if (TInfo.ArtistBackgrounds.Count > 0)
              {
                var fanArtBgModel = (FanArtBackgroundModel)ServiceRegistration.Get<IWorkflowManager>().GetModel(FanArtBackgroundModel.FANART_MODEL_ID);
                if (fanArtBgModel == null) return;
                var uriSource = TInfo.ArtistBackgrounds[0];
                fanArtBgModel.ImageSource = uriSource != "" ? new MultiImageSource { UriSource = uriSource } : new MultiImageSource { UriSource = null };
              }
              else
              {
                ClearFanart();
              }

              ArtistBio = TInfo.ArtistBio;
            }
          }

          //////////////////////////////////////////////////////////

          return;
        }

        Artist = string.Empty;
        Title = string.Empty;
        Album = string.Empty;
        CurrentStreamLogo = string.Empty;
        Listeners = string.Empty;
        ClearFanart();
      }
      finally
      {
        _updating = false;
      }
    }

    private void ClearFanart()
    {
      var fanArtBgModel = (FanArtBackgroundModel)ServiceRegistration.Get<IWorkflowManager>().GetModel(FanArtBackgroundModel.FANART_MODEL_ID);
      if (fanArtBgModel != null) fanArtBgModel.ImageSource = new MultiImageSource { UriSource = null };
    }

    private void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
      //var context = ServiceRegistration.Get<IWorkflowManager>().CurrentNavigationContext;

      //if (context.WorkflowModelId != _context.WorkflowModelId)
      //{
      //    ATimer.Stop();
      //    return;
      //}

      //if (TInfo.ArtistBackgrounds.Count > FanartCount)
      //{
      //    var fanArtBgModel = (FanArtBackgroundModel)ServiceRegistration.Get<IWorkflowManager>().GetModel(FanArtBackgroundModel.FANART_MODEL_ID);
      //    if (fanArtBgModel == null) return;
      //    var uriSource = TInfo.ArtistBackgrounds[FanartCount];
      //    fanArtBgModel.ImageSource = uriSource != "" ? new MultiImageSource { UriSource = uriSource } : new MultiImageSource { UriSource = null };
      //}

      //if (FanartCount == TInfo.ArtistBackgrounds.Count)
      //{
      //    FanartCount = 0;
      //}
      //else
      //{
      //    FanartCount += 1;
      //}
    }
  }
}
