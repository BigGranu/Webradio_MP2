using System;
using System.Collections.Generic;
using System.Linq;
using CoverArtArchive.Release;
using FanartTv;
using FanartTv.Music;
using MediaPortal.Common;
using MediaPortal.Common.Localization;
using MusicBrainz;
using ParkSquare.Gracenote;
using TheAudioDB.Data;

namespace Webradio.Helper
{
  public class TrackInfo
  {
    public string Artist;

    public List<string> ArtistBackgrounds = new List<string>();
    public string ArtistBio;

    public string ArtistId = string.Empty;
    public string FrontCover;
    public string ReleaseId = string.Empty;
    public string Title;

    public TrackInfo()
    {
    }

    public TrackInfo(string artist, string title)
    {
      Artist = artist;
      Title = title;
      FrontCover = "";
      ArtistBio = "";
      ArtistId = "";
      ReleaseId = "";
      ArtistBackgrounds = new List<string>();

      GetMusicBrainzId();
      FrontCover = GetCover(Title, Artist);

      if (FrontCover == "") FrontCover = GetFrontCoverByCoverArtArchive(ReleaseId);

      if ((ArtistId == null) | (ArtistId == "")) return;

      GetFanartBackgrounds(ArtistId);
      GetTAudioDbInfos(ArtistId);
    }

    private void GetMusicBrainzId()
    {
      var rec = Search.Release(Title, artistname: Artist);
      if (rec?.Data.Count > 0)
      {
        ArtistId = rec.Data[0].Artistcredit.Count > 0 ? rec.Data[0].Artistcredit[0].Artist.Id : "";
        ReleaseId = rec.Data[0].Artistcredit.Count > 0 ? rec.Data[0].Id : "";
      }
    }

    private string GetFrontCoverByCoverArtArchive(string releaseId)
    {
      var c = Get.Cover(releaseId);
      foreach (var s in c.Images.Where(s => s.Front)) return s.Image;
      return "";
    }

    private void GetFanartBackgrounds(string artistId)
    {
      API.cKey = "52c813aa7b8c8b3bb87f4797532a2f8c";
      var ims = new Artist(artistId);

      if (ims.List.AImagesrtistbackground != null)
        foreach (var i in ims.List.AImagesrtistbackground)
          if (!ArtistBackgrounds.Contains(i.Url))
            ArtistBackgrounds.Add(i.Url);
    }

    private void GetTAudioDbInfos(string artistId)
    {
      var b = new TheAudioDB.Artist(new Guid(artistId), "912057237373f620001833");

      if (b.List == null) return;

      foreach (var s in b.List)
      {
        if ((s.ArtistFanart1 != "") & !ArtistBackgrounds.Contains(s.ArtistFanart1)) ArtistBackgrounds.Add(s.ArtistFanart1);
        if ((s.ArtistFanart2 != "") & !ArtistBackgrounds.Contains(s.ArtistFanart2)) ArtistBackgrounds.Add(s.ArtistFanart2);
        if ((s.ArtistFanart3 != "") & !ArtistBackgrounds.Contains(s.ArtistFanart3)) ArtistBackgrounds.Add(s.ArtistFanart3);

        ArtistBio = GetBio(s);
      }
    }

    private string GetBio(ArtistData value)
    {
      var localization = ServiceRegistration.Get<ILocalization>();
      var re = localization.CurrentCulture.Name;

      if (re.Contains("en-") & (value.BiographyEn != "")) return value.BiographyEn;
      if (re.Contains("de-") & (value.BiographyDe != "")) return value.BiographyDe;
      if (re.Contains("fr-") & (value.BiographyFr != "")) return value.BiographyFr;
      if (re.Contains("cn-") & (value.BiographyCn != "")) return value.BiographyCn;
      if (re.Contains("it-") & (value.BiographyIt != "")) return value.BiographyIt;
      if (re.Contains("jp-") & (value.BiographyJp != "")) return value.BiographyJp;
      if (re.Contains("ru-") & (value.BiographyRu != "")) return value.BiographyRu;
      if (re.Contains("es-") & (value.BiographyEs != "")) return value.BiographyEs;
      if (re.Contains("pt-") & (value.BiographyPt != "")) return value.BiographyPt;
      if (re.Contains("se-") & (value.BiographySe != "")) return value.BiographySe;
      if (re.Contains("nl-") & (value.BiographyNl != "")) return value.BiographyNl;
      if (re.Contains("hu-") & (value.BiographyHu != "")) return value.BiographyHu;
      if (re.Contains("no-") & (value.BiographyNo != "")) return value.BiographyNo;
      if (re.Contains("il-") & (value.BiographyIl != "")) return value.BiographyIl;
      if (re.Contains("pl-") & (value.BiographyPl != "")) return value.BiographyPl;

      return value.BiographyEn;
    }

    private string GetCover(string title, string artist)
    {
      // Search with GraceNote
      try
      {
        var cover2 = new GracenoteClient("8038400-D6D9583FE60EA28CE621BC0EAD6ED8A0").Search(new SearchCriteria
        {
          Artist = artist,
          TrackTitle = title,
          SearchMode = SearchMode.BestMatchWithCoverArt,
          SearchOptions = SearchOptions.Cover
        });

        foreach (var a in cover2.Albums.SelectMany(c => c.Artwork.Where(a => a.Uri.AbsoluteUri != string.Empty))) return a.Uri.AbsoluteUri;
      }
      catch (Exception ex)
      {
        return string.Empty;
      }

      return string.Empty;
    }
  }
}
