﻿#region Copyright (C) 2007-2013 Team MediaPortal

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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using MediaPortal.Common;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;
using MediaPortal.Common.PathManager;
using MediaPortal.Common.Services.ResourceAccess.RawUrlResourceProvider;
using MediaPortal.Common.SystemResolver;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.UiComponents.Media.Models;
using Webradio.Helper_Classes;

namespace Webradio.Player
{
  internal class WebRadioPlayerHelper
  {
    public const string WEBRADIO_MIMETYPE = "webradio/stream";

    /// <summary>
    /// Constructs a dynamic <see cref="MediaItem"/> that contains the URL for the given <paramref name="stream"/> and starts the playback.
    /// </summary>
    /// <param name="stream">Stream.</param>
    public static void PlayStream(MyStream stream)
    {
      var mediaItem = CreateStreamMediaItem(stream);
      if (ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive)
      {
        PlayItemsModel.CheckQueryPlayAction(mediaItem);
      }
      else
      {
        PlayItemsModel.PlayItem(mediaItem);
      }
    }

    /// <summary>
    /// Constructs a dynamic <see cref="MediaItem"/> that contains the URL for the given <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">Stream.</param>
    private static MediaItem CreateStreamMediaItem(MyStream stream)
    {
      IDictionary<Guid, MediaItemAspect> aspects = new Dictionary<Guid, MediaItemAspect>();

      MediaItemAspect providerResourceAspect;
      aspects[ProviderResourceAspect.ASPECT_ID] = providerResourceAspect = new MediaItemAspect(ProviderResourceAspect.Metadata);
      MediaItemAspect mediaAspect;
      aspects[MediaAspect.ASPECT_ID] = mediaAspect = new MediaItemAspect(MediaAspect.Metadata);
      aspects[AudioAspect.ASPECT_ID] = new MediaItemAspect(AudioAspect.Metadata); // AudioAspect needs to be contained for player mapping

      providerResourceAspect.SetAttribute(ProviderResourceAspect.ATTR_RESOURCE_ACCESSOR_PATH, RawUrlResourceProvider.ToProviderResourcePath(stream.URL).Serialize());
      providerResourceAspect.SetAttribute(ProviderResourceAspect.ATTR_SYSTEM_ID, ServiceRegistration.Get<ISystemResolver>().LocalSystemId);

      mediaAspect.SetAttribute(MediaAspect.ATTR_MIME_TYPE, WEBRADIO_MIMETYPE);
      mediaAspect.SetAttribute(MediaAspect.ATTR_TITLE, stream.Titel);

      MediaItemAspect.SetAttribute(aspects, ThumbnailLargeAspect.ATTR_THUMBNAIL, ImageFromLogo(stream.Logo));

      var mediaItem = new MediaItem(Guid.Empty, aspects);
      return mediaItem;
    }

    /// <summary>
    /// Convert the Webstreamlogo (Online or Default) to byte[]
    /// </summary>
    public static byte[] ImageFromLogo(string path)
    {
      var ms = new MemoryStream();

      if (path == "")
      {
        var s = ServiceRegistration.Get<IPathManager>().GetPath(@"<PLUGINS>\Webradio\Skin\default\images\DefaultLogo.png");
        Image.FromFile(s).Save(ms, ImageFormat.Png);
      }
      else
      {
        var imageRequest = (HttpWebRequest)WebRequest.Create(path);
        imageRequest.Credentials = CredentialCache.DefaultCredentials;

        using (var imageReponse = (HttpWebResponse)imageRequest.GetResponse())
        {
          using (var imageStream = imageReponse.GetResponseStream())
          {
            if (imageStream != null) Image.FromStream(imageStream).Save(ms, ImageFormat.Png);
          }
        }
      }
      return ms.ToArray();
    }
  }
}