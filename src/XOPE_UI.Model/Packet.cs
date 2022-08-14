using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Newtonsoft.Json.Linq;

namespace XOPE_UI.Model
{
    public class Packet
    {
        public Guid Id { get; set; }
        public int Socket { get; set; }
        public byte[] Data { get; set; }
        public int Length { get; set; }
        public HookedFuncType Type { get; set; }
        public bool Modified { get; set; } = false;
        public bool Tunneled { get; set; } = false;
        public JObject UnderlyingEvent { get; set; }

        public static byte[] ConvertB64CompressedToByteArray(string b64String)
        {
            byte[] compressedData = Convert.FromBase64String(b64String);

            MemoryStream outputStream = new MemoryStream();
            using (MemoryStream memoryStream = new MemoryStream(compressedData))
            using (var inflater = new InflaterInputStream(memoryStream))
            {
                inflater.CopyTo(outputStream);
            }

            return outputStream.ToArray();
        }
    }
}
