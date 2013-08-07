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
    public static string xmlFilter = System.Windows.Forms.Application.StartupPath + "\\Plugins\\Webradio\\Data\\WebradioFilters.xml";
    public static string xmlFavorites = System.Windows.Forms.Application.StartupPath + "\\Plugins\\Webradio\\Data\\WebradioFavorites.xml";

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
      
      static XmlSerializer serializer; 
      static FileStream stream;

      public MyFilters()
      {
      }

      public MyFilters(List<MyFilter> _filters)
      {
        FilterList = _filters;
      }

      public static MyFilters Read()
      {
        string XmlFile = WebradioData.xmlFilter;
        if (!File.Exists(XmlFile)) { File.Create(XmlFile); }

        stream = new FileStream(XmlFile, FileMode.Open);
        serializer = new XmlSerializer(typeof(MyFilters));
        MyFilters _s = new MyFilters();
        _s = (MyFilters)serializer.Deserialize(stream);
        stream.Close();
        serializer = null;
        return _s;
      }

      public static bool Write(Object obj)
      {
        string XmlFile = WebradioData.xmlFilter;
        try
        {
          stream = new FileStream(XmlFile, FileMode.Create);
          serializer = new XmlSerializer(typeof(MyFilters));
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

    public class MyFilter
    {
      public string Titel;
      public List<string> fCountrys;
      public List<string> fCitys;
      public List<string> fGenres;
      public List<string> fBitrate;

      public MyFilter()
      {
        Titel = "";
        fCountrys = new List<string>();
        fCitys = new List<string>();
        fGenres = new List<string>();
        fBitrate = new List<string>();
      }

      public MyFilter(String _Titel, List<string> _Countrys, List<string> _Citys, List<string> _Genres, List<string> _Bitrate)
      {
        Titel = _Titel;
        fCountrys = _Countrys;
        fCitys = _Citys;
        fGenres = _Genres;
        fBitrate = _Bitrate;
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

      public static MyFavorits Read()
      {
        string XmlFile = WebradioData.xmlFavorites;
        if (!File.Exists(XmlFile)) { File.Create(XmlFile); }

        stream = new FileStream(XmlFile, FileMode.Open);
        serializer = new XmlSerializer(typeof(MyFavorits));
        MyFavorits _s = new MyFavorits();
        _s = (MyFavorits)serializer.Deserialize(stream);
        stream.Close();
        serializer = null;
        return _s;
      }

      public static bool Write(string XmlFile, MyFavorits mliste)
      {
        XmlSerializer serializer = new XmlSerializer(typeof(MyFavorits));
        StreamWriter writer = new StreamWriter(XmlFile, false);

        try
        {
          serializer.Serialize(writer, mliste);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.StackTrace);
        }
        finally
        {
          writer.Close();
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
