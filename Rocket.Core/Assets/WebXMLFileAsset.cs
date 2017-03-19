using Rocket.API.Assets;
using Rocket.Core.Utils.Web;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Rocket.Core.Assets
{
    public class WebXMLFileAsset<T> : Asset<T> where T : class
    {
        private XmlSerializer serializer;
        private Uri url;
        private RocketWebClient webclient = new RocketWebClient();
        private bool waiting = false;

        public WebXMLFileAsset(Uri url = null, XmlRootAttribute attr = null, AssetLoaded<T> callback = null)
        {
            serializer = new XmlSerializer(typeof(T), attr);
            this.url = url;
            Load(callback);
        }

        public override void Load(AssetLoaded<T> callback = null)
        {
            try
            {
                if (!waiting)
                {
                    waiting = true;
                    webclient.DownloadStringCompleted += (object sender, System.Net.DownloadStringCompletedEventArgs e) =>
                    {
                        using (StringReader reader = new StringReader(e.Result))
                        {
                            try
                            {
                                T result = (T)serializer.Deserialize(reader);

                                instance = result;

                                if (callback != null)
                                    callback(this);
                            }
                            catch (Exception ex)
                            {
                                throw (ex);
                            }
                            finally
                            {
                                callback(this);
                            }
                        }
                        waiting = false;
                    };
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