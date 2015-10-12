using System;
using System.Reflection;
using log4net;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Services;

namespace Umbraco.Azure.MediaFtpSync
{
    public class EventHandler : ApplicationEventHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType
        );

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            
            var config = ConfigReader.FromAppSettings();
            if (!config.Enabled)
                return;

            var uploader = new Uploader(config);
            MediaService.Saved += (sender, args) =>
            {
                try
                {
                    args.SavedEntities.ForEach(uploader.UploadToAzure);
                }
                catch (Exception ex)
                {
                    Log.Error("Could not upload saved media item to the public website.", ex);
                    args.Messages.Add(new EventMessage(
                        "Image Upload",
                        "Could not upload saved media item to the public website. Please check the logs",
                        EventMessageType.Warning));
                }
            };
        }
    }
}