﻿#region Copyright (C) 2007-2013 Team MediaPortal

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
using System.ComponentModel;
using System.Net;
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.Logging;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Screens;
using MediaPortal.UI.Presentation.Workflow;
using Webradio.Helper;
using Webradio.Models;

namespace Webradio.Dialogues
{
    public class WebradioDlgLoadUpdate : IWorkflowModel
    {
        #region Consts

        public const string MODEL_ID_STR = "028ABECD-9885-48F6-B39F-F252EC0115EF";
        public const string DOWNLOAD_COMPLETE = "[Webradio.Dialog.LoadUpdate.DownloadComplete]";
        public const string DOWNLOAD_ERROR = "[Webradio.Dialog.LoadUpdate.DownloadError]";

        #endregion

        #region Propertys

        protected static AbstractProperty UpdateProgressProperty = new WProperty(typeof(int), 0);
        public static int UpdateProgress
        {
            get => (int)UpdateProgressProperty.GetValue();
            set => UpdateProgressProperty.SetValue(value);
        }

        protected static AbstractProperty InfoProperty = new WProperty(typeof(string), string.Empty);
        public static string Info
        {
            get => (string)InfoProperty.GetValue();
            set => InfoProperty.SetValue(value);
        }

        #endregion

        public static void LoadSenderListe()
        {
            try
            {
                WebClient webclient1 = new WebClient();
                webclient1.DownloadFileCompleted += DownloadCompleted;
                webclient1.DownloadProgressChanged += DownloadStatusChanged;
                webclient1.DownloadFileAsync(new Uri(StreamlistUpdate.StreamlistServerPath), StreamlistUpdate.StreamListFile);
            }
            catch (Exception ex)
            {
                ServiceRegistration.Get<ILogger>().Error("Webradio: Error read Online Stationslist '{0}'", ex);
            }
        }

        public static void Finish()
        {
            WebradioHome homeModel = ServiceRegistration.Get<IWorkflowManager>().GetModel(WebradioHome.MODEL_ID) as WebradioHome;
            if (homeModel == null)
                return;

            ServiceRegistration.Get<IScreenManager>().CloseTopmostDialog();
            homeModel.Init();
        }

        private static void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Info = e.Error == null ? DOWNLOAD_COMPLETE : DOWNLOAD_ERROR;
            Finish();
        }

        private static void DownloadStatusChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            UpdateProgress = e.ProgressPercentage;
        }

        #region IWorkflowModel implementation

        public Guid ModelId => new Guid(MODEL_ID_STR);

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
}