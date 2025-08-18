using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class ChattigoLoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class ChattigoLoginResponse
    {
        public string access_token { get; set; }
        public bool access { get; set; }
    }

    // Clase principal para la solicitud
    public class HsmMessageRequest
    {
        public string did { get; set; }
        public string type { get; set; } = "HSM";
        public string channel { get; set; } = "WHATSAPP";
        public HsmContent hsm { get; set; }
    }

    public class HsmContent
    {
        public List<HsmDestination> destinations { get; set; }
        public string @namespace { get; set; } = "";
        public string template { get; set; } = "avisogeneraciondeuda";
        public bool botAttention { get; set; } = true;
        public string languageCode { get; set; } = "es";
    }

    public class HsmDestination
    {
        public string id { get; set; }
        public string destination { get; set; }
        public List<string> parameters { get; set; }
    }

}
