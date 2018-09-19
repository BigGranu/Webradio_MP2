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
using MediaPortal.Extensions.UserServices.FanArtService.Client.Models;
using MediaPortal.UiComponents.Media.Models;
using MediaPortal.UI.Players.BassPlayer.Interfaces;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.UI.Presentation.Workflow;
using MediaPortal.UI.SkinEngine.Controls.ImageSources;
using Webradio.Helper;
using Webradio.Models;

namespace Webradio.Player
{
    public class WebRadioUIContributor : BaseTimerControlledModel, IPlayerUIContributor
    {
        public const string SCREEN_FULLSCREEN_AUDIO = "webradio_FullscreenContent";
        public const string SCREEN_CURRENTLY_PLAYING_AUDIO = "webradio_CurrentlyPlaying";
        protected const int UPDATE_INTERVAL_MS = 300;

        private TrackInfo _info = new TrackInfo();
        private string _cArtist = string.Empty;
        private string _cTitle = string.Empty;

        protected MediaWorkflowStateType _mediaWorkflowStateType;
        protected ITagSource _tagSource;
        protected bool _updating;

        #region Constructor & maintainance

        public WebRadioUIContributor() : base(false, UPDATE_INTERVAL_MS)
        {
            _artistProperty = new WProperty(typeof(string), string.Empty);
            _titleProperty = new WProperty(typeof(string), string.Empty);
            _artistBioProperty = new WProperty(typeof(string), string.Empty);
            _albumProperty = new WProperty(typeof(string), string.Empty);
            _listenersProperty = new WProperty(typeof(string), string.Empty);

            _currentStreamLogoProperty = new WProperty(typeof(string), string.Empty);
            _currentStreamTitleProperty = new WProperty(typeof(string), string.Empty);
            _currentStreamCityProperty = new WProperty(typeof(string), string.Empty);
            _currentStreamCountryProperty = new WProperty(typeof(string), string.Empty);
            _currentStreamCountryCodeProperty = new WProperty(typeof(string), string.Empty);
            _currentStreamGenresProperty = new WProperty(typeof(string), string.Empty);
            _currentStreamHomepageProperty = new WProperty(typeof(string), string.Empty);
            _currentStreamLanguageProperty = new WProperty(typeof(string), string.Empty);
            _currentStreamLanguageCodeProperty = new WProperty(typeof(string), string.Empty);
            _currentStreamDescriptionProperty = new WProperty(typeof(string), string.Empty);
        }

        #endregion

        #region Current Stream Infos

        protected AbstractProperty _currentStreamLogoProperty;
        public AbstractProperty CurrentStreamLogoProperty => _currentStreamLogoProperty;
        public string CurrentStreamLogo
        {
            get => (string)_currentStreamLogoProperty.GetValue();
            set => _currentStreamLogoProperty.SetValue(value);
        }

        protected AbstractProperty _currentStreamTitleProperty;
        public AbstractProperty CurrentStreamTitleProperty => _currentStreamTitleProperty;
        public string CurrentStreamTitle
        {
            get => (string)_currentStreamTitleProperty.GetValue();
            set => _currentStreamTitleProperty.SetValue(value);
        }

        protected AbstractProperty _currentStreamCityProperty;
        public AbstractProperty CurrentStreamCityProperty => _currentStreamCityProperty;
        public string CurrentStreamCity
        {
            get => (string)_currentStreamCityProperty.GetValue();
            set => _currentStreamCityProperty.SetValue(value);
        }

        protected AbstractProperty _currentStreamCountryProperty;
        public AbstractProperty CurrentStreamCountryProperty => _currentStreamCountryProperty;
        public string CurrentStreamCountry
        {
            get => (string)_currentStreamCountryProperty.GetValue();
            set => _currentStreamCountryProperty.SetValue(value);
        }

        protected AbstractProperty _currentStreamCountryCodeProperty;
        public AbstractProperty CurrentStreamCountryCodeProperty => _currentStreamCountryCodeProperty;
        public string CurrentStreamCountryCode
        {
            get => (string)_currentStreamCountryCodeProperty.GetValue();
            set => _currentStreamCountryCodeProperty.SetValue(value);
        }

        protected AbstractProperty _currentStreamGenresProperty;
        public AbstractProperty CurrentStreamGenresProperty => _currentStreamGenresProperty;
        public string CurrentStreamGenres
        {
            get => (string)_currentStreamGenresProperty.GetValue();
            set => _currentStreamGenresProperty.SetValue(value);
        }

        protected AbstractProperty _currentStreamHomepageProperty;
        public AbstractProperty CurrentStreamHomepageProperty => _currentStreamHomepageProperty;
        public string CurrentStreamHomepage
        {
            get => (string)_currentStreamHomepageProperty.GetValue();
            set => _currentStreamHomepageProperty.SetValue(value);
        }

        protected AbstractProperty _currentStreamLanguageProperty;
        public AbstractProperty CurrentStreamLanguageProperty => _currentStreamLanguageProperty;
        public string CurrentStreamLanguage
        {
            get => (string)_currentStreamLanguageProperty.GetValue();
            set => _currentStreamLanguageProperty.SetValue(value);
        }

        protected AbstractProperty _currentStreamLanguageCodeProperty;
        public AbstractProperty CurrentStreamLanguageCodeProperty => _currentStreamLanguageCodeProperty;
        public string CurrentStreamLanguageCode
        {
            get => (string)_currentStreamLanguageCodeProperty.GetValue();
            set => _currentStreamLanguageCodeProperty.SetValue(value);
        }

        protected AbstractProperty _currentStreamDescriptionProperty;
        public AbstractProperty CurrentStreamDescriptionProperty => _currentStreamDescriptionProperty;
        public string CurrentStreamDescription
        {
            get => (string)_currentStreamDescriptionProperty.GetValue();
            set => _currentStreamDescriptionProperty.SetValue(value);
        }

        #endregion

        #region Current Title Infos

        protected AbstractProperty _artistProperty;
        public AbstractProperty ArtistProperty => _artistProperty;
        public string Artist
        {
            get => (string)_artistProperty.GetValue();
            set => _artistProperty.SetValue(value);
        }

        protected AbstractProperty _artistBioProperty;
        public AbstractProperty ArtistBioProperty => _artistBioProperty;
        public string ArtistBio
        {
            get => (string)_artistBioProperty.GetValue();
            set => _artistBioProperty.SetValue(value);
        }

        protected AbstractProperty _titleProperty;
        public AbstractProperty TitleProperty => _titleProperty;
        public string Title
        {
            get => (string)_titleProperty.GetValue();
            set => _titleProperty.SetValue(value);
        }

        protected AbstractProperty _albumProperty;
        public AbstractProperty AlbumProperty => _albumProperty;
        public string Album
        {
            get => (string)_albumProperty.GetValue();
            set => _albumProperty.SetValue(value);
        }

        protected static AbstractProperty _listenersProperty;
        public AbstractProperty ListenersProperty => _listenersProperty;
        public string Listeners
        {
            get => (string)_listenersProperty.GetValue();
            set => _listenersProperty.SetValue(value);
        }

        #endregion

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
            SetChannelInfos();

            StartTimer();
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
                SetChannelInfos();

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
                            _info = new TrackInfo(Artist, Title);

                            if ((_info.FrontCover == null) | (_info.FrontCover == ""))
                                CurrentStreamLogo = WebradioHome.CurrentStreamLogo;
                            else
                                CurrentStreamLogo = _info.FrontCover;

                            if (_info.ArtistBackgrounds.Count > 0)
                            {
                                var fanArtBgModel = (FanArtBackgroundModel)ServiceRegistration.Get<IWorkflowManager>().GetModel(FanArtBackgroundModel.FANART_MODEL_ID);
                                if (fanArtBgModel == null) return;
                                var uriSource = _info.ArtistBackgrounds[0];
                               // var us = new MultiImageSource { UriSource = uriSource };
                                
                                var d = new BitmapImageSource();
                                d.UriSource = uriSource;
                                fanArtBgModel.ImageSource = d;

                                // fanArtBgModel.ImageSource = uriSource != "" ? new MultiImageSource { UriSource = uriSource } : new MultiImageSource { UriSource = null };
                            }
                            else
                            {
                                ClearFanart();
                            }

                            ArtistBio = _info.ArtistBio;
                        }
                    }

                    return;
                }

                Artist = string.Empty;
                Title = string.Empty;
                Album = string.Empty;
                Listeners = string.Empty;
                ArtistBio = string.Empty;

                CurrentStreamLogo = string.Empty;
                CurrentStreamTitle = string.Empty;
                CurrentStreamCity = string.Empty;
                CurrentStreamCountry = string.Empty;
                CurrentStreamGenres = string.Empty;
                CurrentStreamHomepage = string.Empty;
                CurrentStreamLanguage = string.Empty;
                CurrentStreamCountryCode = string.Empty;
                CurrentStreamLanguageCode = string.Empty;
                CurrentStreamDescription = string.Empty;

                ClearFanart();
            }
            finally
            {
                _updating = false;
            }
        }

        private void SetChannelInfos()
        {
            var s = WebradioHome.SelectedStream;
            CurrentStreamTitle = s.Title;
            CurrentStreamCity = s.City;
            CurrentStreamCountry = "[Country." + s.Country + "]";
            CurrentStreamGenres = s.Genres;
            CurrentStreamHomepage = s.Homepage;
            CurrentStreamLanguage = "[Language." + s.Language + "]";
            CurrentStreamCountryCode = s.Country;
            CurrentStreamLanguageCode = s.Language;
            CurrentStreamDescription = WebradioHome.SetStreamDescription(s);
            CurrentStreamLogo = WebradioHome.CurrentStreamLogo;
        }

        private void ClearFanart()
        {
            var fanArtBgModel = (FanArtBackgroundModel)ServiceRegistration.Get<IWorkflowManager>().GetModel(FanArtBackgroundModel.FANART_MODEL_ID);
            if (fanArtBgModel != null) fanArtBgModel.ImageSource = new MultiImageSource { UriSource = null };
        }
    }
}