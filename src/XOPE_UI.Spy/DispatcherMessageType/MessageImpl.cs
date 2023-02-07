using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XOPE_UI.Model;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public abstract class MessageImpl
    {
        [JsonProperty]
        protected SpyMessageType Type { get; set; }

        /// <summary>
        /// Converts this object to JSON represented as a JObject. 
        /// Override if you need custom serialising for properties in derived class.
        /// </summary>
        /// <returns></returns>
        public virtual JObject ToJson()
        {
            //CBORObject.FromObject(this).EncodeToBytes
            return JObject.FromObject(this);
        }
    }
}
