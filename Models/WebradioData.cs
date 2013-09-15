using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;

namespace Webradio.Models
{
  class WebradioData : IWorkflowModel 
  {
    public const string DATA_ID_STR = "BD1BA004-1BC0-49F5-9107-AD8FFD07BAAE";

    public static string XmlFilter = WebradioHome.DataPath + "WebradioFilters.xml";
    public static string XmlFavorites = WebradioHome.DataPath + "WebradioFavorites.xml";

    public WebradioData()
    {
    }

    #region IWorkflowModel implementation

    public Guid ModelId
    {
      get { return new Guid(DATA_ID_STR); }
    }

    public bool CanEnterState(NavigationContext oldContext, NavigationContext newContext)
    {
      return true;
    }

    public void EnterModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
    }

    public void ExitModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
    }

    public void ChangeModelContext(NavigationContext oldContext, NavigationContext newContext, bool push)
    {
      // We could initialize some data here when changing the media navigation state
    }

    public void Deactivate(NavigationContext oldContext, NavigationContext newContext)
    {
    }

    public void Reactivate(NavigationContext oldContext, NavigationContext newContext)
    {
    }

    public void UpdateMenuActions(NavigationContext context, IDictionary<Guid, WorkflowAction> actions)
    {
    }

    public ScreenUpdateMode UpdateScreen(NavigationContext context, ref string screen)
    {
      return ScreenUpdateMode.AutoWorkflowManager;
    }

    #endregion
  }

    public class MyFilters
    {
      public List<MyFilter> FilterList = new List<MyFilter>();
      
      static XmlSerializer _serializer; 
      static FileStream _stream;

      public MyFilters()
      {
      }

      public MyFilters(List<MyFilter> filters)
      {
        FilterList = filters;
      }

      public static MyFilters Read()
      {
        MyFilters mfs = new MyFilters();
        string xmlFile = WebradioData.XmlFilter;

        try
        { 
          if (!File.Exists(xmlFile)) 
          { 
            MyFilter mf = new MyFilter("New Filter", "1", new List<string>(), new List<string>(), new List<string>(), new List<string>());
            mfs.FilterList.Add(mf);
            MyFilters.Write(mfs);
          }
          _stream = new FileStream(xmlFile, FileMode.Open);
          _serializer = new XmlSerializer(typeof(MyFilters));
          mfs = (MyFilters)_serializer.Deserialize(_stream);
        }
        finally
        {
          _stream.Close();
          _serializer = null;
        }
        return mfs;
      }

      public static bool Write(Object obj)
      {
        string xmlFile = WebradioData.XmlFilter;
        try
        {
          _stream = new FileStream(xmlFile, FileMode.Create);
          _serializer = new XmlSerializer(typeof(MyFilters));
          _serializer.Serialize(_stream, obj);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.StackTrace);
        }
        finally
        {
          _stream.Close();
          _serializer = null;
        }
        return true;
      }

    }

    public class MyFilter
    {
      public string Titel;
      public string ID;
      public List<string> fCountrys;
      public List<string> fCitys;
      public List<string> fGenres;
      public List<string> fBitrate;

      public MyFilter()
      {
        Titel = "";
        ID = "";
        fCountrys = new List<string>();
        fCitys = new List<string>();
        fGenres = new List<string>();
        fBitrate = new List<string>();
      }

      public MyFilter(String titel, String id, List<string> countrys, List<string> citys, List<string> genres, List<string> bitrate)
      {
        Titel = titel;
        ID = id;
        fCountrys = countrys;
        fCitys = citys;
        fGenres = genres;
        fBitrate = bitrate;
      }
    }

    #region Read/Write Favorites
    public class MyFavorits
    {
      public List<MyFavorit> FavoritList = new List<MyFavorit>();

      static XmlSerializer serializer;
      static FileStream stream;

      public MyFavorits()
      {
      }

      public MyFavorits(List<MyFavorit> _favorites)
      {
        FavoritList = _favorites ;
      }

      public static MyFavorits Read()
      {
        MyFavorits _s = new MyFavorits();
        string XmlFile = WebradioData.XmlFavorites;

        try
        {
          if (!File.Exists(XmlFile))
          {
            MyFavorit mf = new MyFavorit("Favorites", true, new List<string>());
            _s.FavoritList.Add(mf);
            MyFavorits.Write(_s);
          }

          stream = new FileStream(XmlFile, FileMode.Open);
          serializer = new XmlSerializer(typeof(MyFavorits));
          _s = (MyFavorits)serializer.Deserialize(stream);
        }
        finally
        {
          stream.Close();
          serializer = null;
        }
        return _s;
      }

      public static bool Write(Object obj)
      {
        string XmlFile = WebradioData.XmlFavorites;
        try
        {
          stream = new FileStream(XmlFile, FileMode.Create);
          serializer = new XmlSerializer(typeof(MyFavorits));
          serializer.Serialize(stream, obj);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.StackTrace);
        }
        finally
        {
          stream.Close();
          serializer = null;
        }
        return true;
      }

    }

    public class MyFavorit
    {
      public string Titel;
      public bool Active;
      public List<string> IDs;

      public MyFavorit()
      {
        Titel = "";
        Active = true;
        IDs = new List<string>();
      }

      public MyFavorit(String _Titel, bool _Active, List<string> _IDs)
      {
        Titel = _Titel;
        Active = _Active;
        IDs = _IDs;
      }
    }
    #endregion

}
