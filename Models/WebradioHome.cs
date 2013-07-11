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
        
        public static MyStreams Streams = new MyStreams();
        // Diese Collection sollte in der Liste im Skin angezeigt werden
        public static ObservableCollection<MyStream> AllRadioStreams = new ObservableCollection<MyStream>();
        public ObservableCollection<MyStream> AAllRadioStreams
        {
            get { return AllRadioStreams; }
        }

        public WebradioHome()
        {
            // beim ersten Start alle Listen füllen
            Streams = MyStreams.Read(_file);
          //  WebradioFavorites.FavoritList = Webradio.Models.Favorits.Read(WebradioFavorites._file);
            WebradioFilter.FilterList = Webradio.Models.MyFilters.Read(WebradioFilter._file);
        }

        public void ShowFavorites()
        {

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
            foreach (MyStream f in Streams.StreamList)
            {
                if (f.ID == _ID) { f.PlayCount  += 1; }
            }
            MyStreams.Write(_file, Streams);
        }

        public MyStream GetStreamByID(int _ID)
        {
            foreach (MyStream f in Streams.StreamList )
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

        static XmlSerializer serializer = new XmlSerializer(typeof(MyStreams));
        static FileStream stream;

        public static MyStreams Read(string XmlFile)
        {
            stream = new FileStream(XmlFile , FileMode.Open);
            MyStreams _s = (MyStreams)serializer.Deserialize(stream);
            stream.Close();
            return _s;
        }

        public static void Write(string XmlFile, Object obj)
        {
            stream = new FileStream(XmlFile, FileMode.Create);
            serializer.Serialize(stream,obj);
            stream.Close();
        }
    }

    public class MyStream
    {
        public int ID { get; set; }
        private string _Titel;
        public string Titel { get { return _Titel; } set { _Titel = value; } }
        private string _URL;
        public string URL { get { return _URL; } set { _URL = value; } }
        private string _Country;
        public string Country { get { return _Country; } set { _Country = value; } }
        private string _City;
        public string City { get { return _City; } set { _City = value; } }
        private string _Genres;
        public string Genres { get { return _Genres; } set { _Genres = value; } }
        private string _Bitrate;
        public string Bitrate { get { return _Bitrate; } set { _Bitrate = value; } }
        private string _Description;
        public string Description { get { return _Description; } set { _Description = value; } }
        private string _Home;
        public string Home { get { return _Home; } set { _Home = value; } }
        private string _Logo;
        public string Logo { get { return _Logo; } set { _Logo = value; } }
        private string _Facebook;
        public string Facebook { get { return _Facebook; } set { _Facebook = value; } }
        private string _Twitter;
        public string Twitter { get { return _Twitter; } set { _Twitter = value; } }
        public bool Love { get; set; }
        public bool Block { get; set; }
        public int PlayCount { get; set; }
        private string _tag1;
        public string tag1 { get { return _tag1; } set { _tag1 = value; } }
        private string _tag2;
        public string tag2 { get { return _tag2; } set { _tag2 = value; } }
        private string _tag3;
        public string tag3 { get { return _tag3; } set { _tag3 = value; } }
        private string _tag4;
        public string tag4 { get { return _tag4; } set { _tag4 = value; } }
    }

    #endregion

}
