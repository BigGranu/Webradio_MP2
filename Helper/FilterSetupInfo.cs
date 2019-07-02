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

namespace Webradio.Helper
{
  public class FilterSetupInfo
  {
    public List<string> Bitrate;
    public List<string> Citys;
    public List<string> Countrys;
    public List<string> Genres;
    public string Id;
    public string Titel;

    public FilterSetupInfo()
    {
      Titel = "";
      Id = "";
      Countrys = new List<string>();
      Citys = new List<string>();
      Genres = new List<string>();
      Bitrate = new List<string>();
    }

    public FilterSetupInfo(string titel, string id, List<string> countrys, List<string> citys, List<string> genres, List<string> bitrate)
    {
      Titel = titel;
      Id = id;
      Countrys = countrys;
      Citys = citys;
      Genres = genres;
      Bitrate = bitrate;
    }
  }
}
