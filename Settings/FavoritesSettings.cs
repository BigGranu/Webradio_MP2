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

using System.Collections.Generic;
using MediaPortal.Common.Settings;

namespace Webradio.Settings
{
  /// <summary>
  /// Favorites settings class.
  /// </summary>
  internal class FavoritesSettings
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public FavoritesSettings()
    {
      FavoritesSetupList = new List<FavoriteSetupInfo>();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public FavoritesSettings(List<FavoriteSetupInfo> list)
    {
      FavoritesSetupList = list;
    }

    /// <summary>
    /// List of all Favorites
    /// </summary>
    [Setting(SettingScope.User, null)]
    public List<FavoriteSetupInfo> FavoritesSetupList { get; set; }
  }

  public class FavoriteSetupInfo
  {
    public bool Active;
    public List<string> StreamUrls;
    public string Titel;

    public FavoriteSetupInfo()
    {
      Titel = "";
      Active = true;
      StreamUrls = new List<string>();
    }

    public FavoriteSetupInfo(string titel, bool active, List<string> streamUrls)
    {
      Titel = titel;
      Active = active;
      StreamUrls = streamUrls;
    }
  }
}
