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

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Webradio.Helper
{
  public class MyStreams
  {
    private static readonly XmlSerializer SERIALIZER = new XmlSerializer(typeof(MyStreams));
    private static FileStream _fileStream;

    public string Version = "1";
    public List<MyStream> Streams = new List<MyStream>();

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
  }

  public class MyStream
  {
    [XmlAttribute("City")]
    public string City = string.Empty;

    [XmlAttribute("Country")]
    public string Country = string.Empty;

    public List<Description> Descriptions = new List<Description>();
    public string Genres = String.Empty;
    public string Homepage = String.Empty;
    public string Language = String.Empty;
    public string Logo = String.Empty;
    public List<Url> StreamUrls = new List<Url>();

    [XmlAttribute("Title")]
    public string Title = String.Empty;

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
    [XmlAttribute("Provider")]
    public string Provider = String.Empty;
    [XmlAttribute("Bitrate")]
    public string Bitrate = String.Empty;
    [XmlAttribute("Frequenz")]
    public string Frequenz = String.Empty;
    [XmlAttribute("Mode")]
    public string Mode = String.Empty;
    [XmlAttribute("Name")]
    public string Name = String.Empty;
    [XmlText]
    public string StreamUrl = String.Empty;
    [XmlAttribute("Typ")]
    public string Typ = String.Empty;

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
    [XmlAttribute("Languagecode")]
    public string Languagecode = String.Empty;
    [XmlText]
    public string Txt = String.Empty;

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