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

namespace Webradio.Helper_Classes
{
  public class MyStream
  {
    public int ID;
    public string Titel;
    public string URL;
    public string Country;
    public string City;
    public string Genres;
    public string Bitrate;
    public string Description;
    public string Home;
    public string Logo;
    public string Facebook;
    public string Twitter;
    public bool Love;
    public bool Block;
    public int PlayCount;
    public string tag1;
    public string tag2;
    public string tag3;
    public string tag4;

    public MyStream()
    {
      ID = 0;
      Titel = string.Empty;
      URL = string.Empty;
      Country = string.Empty;
      City = string.Empty;
      Genres = string.Empty;
      Bitrate = string.Empty;
      Home = string.Empty;
      Logo = string.Empty;
      Facebook = string.Empty;
      Twitter = string.Empty;
      Love = false;
      Block = false;
      PlayCount = 0;
      tag1 = string.Empty;
      tag2 = string.Empty;
      tag3 = string.Empty;
      tag4 = string.Empty;
    }
  }
}