using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Globalization;
using System.Linq;
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;

namespace Webradio.Models
{
    public class WebradioHome : IWorkflowModel
    {
        public const string MODEL_ID_STR = "5726DA5A-70D5-458f-AF67-611293D97912";
        public static string _file = System.Windows.Forms.Application.StartupPath + "\\Plugins\\Webradio\\Data\\WebradioSender.xml";

        public static MyStreams StreamList = new MyStreams();

        public WebradioHome()
        {
            WebradioHome.StreamList = Webradio.Models.MyStreams.Read(WebradioHome._file);
            WebradioFavorites.FavoritList = Webradio.Models.Favorits.Read(WebradioFavorites._file);
            WebradioFilter.FilterList = Webradio.Models.MyFilters.Read(WebradioFilter._file);
        }

        public static ObservableCollection<MyStream> Liste { get; set; }

        public void InitStreams()
        {
            Liste = new ObservableCollection<MyStream>();
            foreach (MyStream f in StreamList.StreamList)
            {
                ListItem item = new ListItem("Titel", f.Titel);
                Liste.Add(f);
            }
        }

        #region IWorkflowModel implementation

        public Guid ModelId
        {
            get { return new Guid(MODEL_ID_STR); }
        }

        public bool CanEnterState(NavigationContext oldContext, NavigationContext newContext)
        {
            return true;
        }

        public void EnterModelContext(NavigationContext oldContext, NavigationContext newContext)
        {
            InitStreams();
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

    #region Read/Write
    public class MyStreams
    {
        public List<MyStream> StreamList = new List<MyStream>();

        public MyStreams()
        {
        }

        public static MyStreams Read(string XmlFile)
        {
            if (!File.Exists(XmlFile)) { File.Create(XmlFile); }
            MyStreams _list = new MyStreams();
            XmlSerializer serializer = new XmlSerializer(typeof(MyStreams));
            FileStream fs = new FileStream(XmlFile, FileMode.Open);

            try
            {
                _list = (MyStreams)serializer.Deserialize(fs);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                fs.Close();
                serializer = null;
            }

            return _list;
        }

        public static bool Write(string XmlFile, MyStreams mliste)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MyStreams));
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

    public class MyStream
    {
        public int ID = 0;
        public string Titel = "";
        public string URL = "";
        public string Country = "";
        public string City = "";
        public string Genres = "";
        public string Bitrate = "";
        
        public string Description = "";
        public string Home = "";
        public string Logo = "";
        public string Facebook = "";
        public string Twitter  = "";

        public bool Love = false;
        public bool Block = false;

        public int PlayCount = 0;

        public string tag1 = "";
        public string tag2 = "";
        public string tag3 = "";
        public string tag4 = "";

        public MyStream() 
        {
        }
    }

    #endregion

}
