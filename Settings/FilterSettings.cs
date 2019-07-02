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
using Webradio.Helper;

namespace Webradio.Settings
{
  /// <summary>
  /// Filter settings class.
  /// </summary>
  public class FilterSettings
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public FilterSettings()
    {
      FilterSetupList = new List<FilterSetupInfo>();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public FilterSettings(List<FilterSetupInfo> list)
    {
      FilterSetupList = list;
    }

    /// <summary>
    /// The Active Filter
    /// </summary>
    [Setting(SettingScope.User, null)]
    public FilterSetupInfo ActiveFilter { get; set; } = null;

    /// <summary>
    /// List of all Filter
    /// </summary>
    [Setting(SettingScope.User, null)]
    public List<FilterSetupInfo> FilterSetupList { get; set; }
  }
}
