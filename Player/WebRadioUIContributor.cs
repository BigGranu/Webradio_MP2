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

using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.Logging;
using MediaPortal.UI.Players.BassPlayer.Interfaces;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.UiComponents.Media.Models;
using Webradio.Models;
using Un4seen.Bass.AddOn.Tags;

namespace Webradio.Player
{
  public class WebRadioUIContributor : BaseTimerControlledModel, IPlayerUIContributor
  {
    public const string SCREEN_FULLSCREEN_AUDIO = "webradio_FullscreenContent";
    public const string SCREEN_CURRENTLY_PLAYING_AUDIO = "webradio_CurrentlyPlaying";

    protected const int UPDATE_INTERVAL_MS = 300;

    protected MediaWorkflowStateType _mediaWorkflowStateType;
    protected bool _updating = false;
    protected ITagSource _tagSource;

    protected AbstractProperty _artistProperty;
    protected AbstractProperty _titleProperty;
    protected AbstractProperty _albumProperty;
    protected AbstractProperty _CurrentStreamLogoProperty;

    #region Constructor & maintainance

    public WebRadioUIContributor()
      : base(false, UPDATE_INTERVAL_MS)
    {
      _artistProperty = new WProperty(typeof(string), string.Empty);
      _titleProperty = new WProperty(typeof(string), string.Empty);
      _albumProperty = new WProperty(typeof(string), string.Empty);
      _CurrentStreamLogoProperty = new WProperty(typeof(string), string.Empty);
      StartTimer();
    }

    #endregion

    public bool BackgroundDisabled
    {
      get { return false; }
    }

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

    public virtual MediaWorkflowStateType MediaWorkflowStateType
    {
      get { return _mediaWorkflowStateType; }
    }

    public AbstractProperty ArtistProperty
    {
      get { return _artistProperty; }
    }

    public string Artist
    {
      get { return (string) _artistProperty.GetValue(); }
      set { _artistProperty.SetValue(value); }
    }

    public AbstractProperty TitleProperty
    {
      get { return _titleProperty; }
    }

    public string Title
    {
      get { return (string) _titleProperty.GetValue(); }
      set { _titleProperty.SetValue(value); }
    }

    public AbstractProperty AlbumProperty
    {
      get { return _albumProperty; }
    }

    public string Album
    {
      get { return (string) _albumProperty.GetValue(); }
      set { _albumProperty.SetValue(value); }
    }

    public AbstractProperty CurrentStreamLogoProperty
    {
      get { return _CurrentStreamLogoProperty; }
    }

    public string CurrentStreamLogo
    {
      get { return (string)_CurrentStreamLogoProperty.GetValue(); }
      set { _CurrentStreamLogoProperty.SetValue(value); }
    }

    public virtual void Initialize(MediaWorkflowStateType stateType, IPlayer player)
    {
      _mediaWorkflowStateType = stateType;
      _tagSource = player as ITagSource;
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
          TAG_INFO tags = _tagSource.Tags;
          if (tags == null)
            return;

          Artist = tags.artist;
          Title = tags.title;
          Album = tags.album;
          CurrentStreamLogo = WebradioHome.CurrentStreamLogo;
          return;
        }
        Artist = string.Empty;
        Title = string.Empty;
        Album = string.Empty;
        CurrentStreamLogo = string.Empty;
      }
      finally
      {
        _updating = false;
      }
    }

  }
}
