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
        public const string MODEL_ID_STR = "EA3CC191-0BE5-4C8D-889F-E9C4616AB554";
        public static string _file = System.Windows.Forms.Application.StartupPath + "\\Plugins\\Webradio\\Data\\WebradioSender.xml";

        public static MyStreams StreamList = new MyStreams();
        // Diese Collection sollte in der Liste im Skin angezeigt werden
        public static ObservableCollection<MyStream> AllRadioStreams = new ObservableCollection<MyStream>();

        public WebradioHome()
        {
            // beim ersten Start alle Listen füllen
            WebradioHome.StreamList = Webradio.Models.MyStreams.Read(WebradioHome._file);
            WebradioFavorites.FavoritList = Webradio.Models.Favorits.Read(WebradioFavorites._file);
            WebradioFilter.FilterList = Webradio.Models.MyFilters.Read(WebradioFilter._file);
        }

        public void Init_AllRadioStreams()
        {
            StreamList.Streams.ToList().ForEach(AllRadioStreams.Add);
        }

        public void Item_selected()
        {
            //Play( selected Item ID)
        }

        public void Play(int _ID)
        {
           
            // Streamurl (GetStreamByID(_ID).URL) an den Player übergeben 
            // noch klären welcher Player dafür wie genutzt wird

            // Playcount des Sender hochzählen
           SetPlayCount(_ID);
        }

        public void SetPlayCount(int _ID)
        {
            foreach (MyStream f in StreamList.Streams)
            {
                if (f.ID == _ID) { f.PlayCount  += 1; }
            }
            MyStreams.Write(_file, StreamList);
        }

        public MyStream GetStreamByID(int _ID)
        {
            foreach (MyStream f in StreamList.Streams)
            {
                if (f.ID == _ID ){return f;}
            }
            return null;
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
            Init_AllRadioStreams();
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
        public List<MyStream> Streams = new List<MyStream>();

        public MyStreams(){}

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

        public MyStream() {}
    }

    #endregion

}
