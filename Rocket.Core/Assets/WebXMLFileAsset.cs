using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Utils;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Rocket.Core.Assets
{
    public class WebXMLFileAsset<T> : Asset<T> where T : class
    {
        private XmlSerializer serializer;
        private Uri url;
        RocketWebClient webclient = new RocketWebClient();
        private bool waiting = false;

        public WebXMLFileAsset(Uri url = null, XmlRootAttribute attr = null, AssetLoaded<T> callback = null)
        {
            serializer = new XmlSerializer(typeof(T), attr);
            this.url = url;
            Load(callback);

            webclient.DownloadStringCompleted += (object sender, System.Net.DownloadStringCompletedEventArgs e) =>
            {
                if (e.Error != null)
                {
                    Logger.LogError("Error retrieving WebXMLFileAsset: " + e.Error.Message);
                }
                else
                {
                    try
                    {
                        using (StringReader reader = new StringReader(e.Result))
                        {
                            T result = (T)serializer.Deserialize(reader);
                            if (result != null)
                                instance = result;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error retrieving WebXMLFileAsset: " + ex.Message);
                    }
                }

                TaskDispatcher.QueueOnMainThread(() =>
                {
                    if (callback != null)
                        callback(this);
                    waiting = false;
                });

            };
        }

        public override void Load(AssetLoaded<T> callback = null)
        {
            try
            {
                if (!waiting)
                {
                    Logger.Log(String.Format("Updating WebXMLFileAsset {0} from {1}", typeof(T).Name, url));
                    waiting = true;
                    webclient.DownloadStringAsync(url);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Failed to deserialize WebXMLFileAsset: {0}", url), ex);
            }
        }
    }
}