using System;
using System.IO;
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
    class WebradioFilter
    {
        public const string MODEL_ID_STR = "FF29E03E-F4A9-4E21-A299-349E79010430";

        public static string _file = System.Windows.Forms.Application.StartupPath + "\\Plugins\\Webradio\\Data\\WebradioFilters.xml";

        // List of all Filters in Xmlfile
        public static MyFilters FilterList = new MyFilters();

        public WebradioFilter()
        {          
        }

        // Rename a Entry
        public void Rename()
        {
        }

        // Added a Entry
        public void Add()
        {
        }

        // Save all Changes on Site
        public void Save()
        {
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
    public class MyFilters
    {
        public List<MyFilter> FilterList = new List<MyFilter>();

        public MyFilters()
        {
        }

        public static MyFilters Read(string XmlFile)
        {
            if (!File.Exists(XmlFile)) { File.Create(XmlFile); }
            MyFilters _list = new MyFilters();
            XmlSerializer serializer = new XmlSerializer(typeof(MyFilters));
            FileStream fs = new FileStream(XmlFile, FileMode.Open);

            try
            {
                _list = (MyFilters)serializer.Deserialize(fs);

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

        public static bool Write(string XmlFile, MyFilters mliste)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MyFilters));
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

    #endregion

}
