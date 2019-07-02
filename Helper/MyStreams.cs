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
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Webradio.Helper
{
  public class MyStreams
  {
    private static readonly XmlSerializer SERIALIZER = new XmlSerializer(typeof(MyStreams));
    private static FileStream _fileStream;
    public List<MyStream> Streams = new List<MyStream>();

    public string Version = "1";

    public MyStreams()
    {
    }

    public MyStreams(List<MyStream> streamsValue, string versionValue)
    {
      Version = versionValue;
      Streams = streamsValue;
    }

    public static void Write(string xmlFile, object obj)
    {
      try
      {
        _fileStream = new FileStream(xmlFile, FileMode.Create);
        SERIALIZER.Serialize(_fileStream, obj);
      }
      finally
      {
        _fileStream.Close();
      }
    }

    public static MyStreams Read(string xmlFile)
    {
      MyStreams ms;

      try
      {
        _fileStream = new FileStream(xmlFile, FileMode.Open);
        ms = (MyStreams)SERIALIZER.Deserialize(_fileStream);
      }
      finally
      {
        _fileStream.Close();
      }

      return ms;
    }

    public static List<MyStream> Filtered(FilterSetupInfo filter, List<MyStream> streams)
    {
      var list = new List<MyStream>();
      foreach (var ms in streams)
        if (Contains2(filter.Genres, ms.Genres) &&
            Contains(filter.Countrys, ms.Country) &&
            Contains(filter.Citys, ms.City) &&
            Contains(filter.Bitrate, ms.StreamUrls[0].Bitrate))
          list.Add(ms);

      return list;
    }

    private static bool Contains(List<string> l, string s)
    {
      return l.Count == 0 || l.Contains(s);
    }

    private static bool Contains2(ICollection<string> l, string s)
    {
      if (s == null) throw new ArgumentNullException("s");
      if (l.Count == 0) return true;

      var split = s.Split(',');
      return split.Any(part => l.Contains(part.Trim()));
    }
  }

  public class MyStream
  {
    [XmlAttribute("City")] public string City = string.Empty;

    [XmlAttribute("Country")] public string Country = string.Empty;

    public List<Description> Descriptions = new List<Description>();
    public string Genres = string.Empty;
    public string Homepage = string.Empty;
    public string Language = string.Empty;
    public string Logo = string.Empty;
    public List<Url> StreamUrls = new List<Url>();

    [XmlAttribute("Title")] public string Title = string.Empty;

    public MyStream()
    {
    }

    public MyStream(string titleValue, string countryValue, string cityValue, List<Url> streamUrlsValue, List<Description> descriptionsValue, string logoValue, string homepageValue, string languageValue, string genresValue)
    {
      Title = titleValue;
      Country = countryValue;
      City = cityValue;
      StreamUrls = streamUrlsValue;
      Descriptions = descriptionsValue;
      Logo = logoValue;
      Homepage = homepageValue;
      Language = languageValue;
      Genres = genresValue;
    }
  }

  public class Url
  {
    [XmlAttribute("Bitrate")] public string Bitrate = string.Empty;

    [XmlAttribute("Frequenz")] public string Frequenz = string.Empty;

    [XmlAttribute("Mode")] public string Mode = string.Empty;

    [XmlAttribute("Name")] public string Name = string.Empty;

    [XmlAttribute("Provider")] public string Provider = string.Empty;

    [XmlText] public string StreamUrl = string.Empty;

    [XmlAttribute("Typ")] public string Typ = string.Empty;

    public Url()
    {
    }

    public Url(string nameValue, string typValue, string bitrateValue, string modeValue, string frequenzValue, string urlValue, string providerValue)
    {
      Name = nameValue;
      Typ = typValue;
      Bitrate = bitrateValue;
      Mode = modeValue;
      Frequenz = frequenzValue;
      StreamUrl = urlValue;
      Provider = providerValue;
    }
  }

  public class Description
  {
    [XmlAttribute("Languagecode")] public string Languagecode = string.Empty;

    [XmlText] public string Txt = string.Empty;

    public Description()
    {
    }

    public Description(string languagecodeValue, string textValue)
    {
      Languagecode = languagecodeValue;
      Txt = textValue;
    }
  }
}
