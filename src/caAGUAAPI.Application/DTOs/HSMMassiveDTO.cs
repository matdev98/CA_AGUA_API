using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caAGUAAPI.Application.DTOs
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

    public class ChattigoMessageRequest
    {
        public string Id { get; set; }
        public string Did { get; set; }
        public string Msisdn { get; set; }
        public string Type { get; set; } = "text";
        public string Channel { get; set; } = "WHATSAPP";
        public string Content { get; set; }
        public string Name { get; set; }
        public bool IsAttachment { get; set; } = false;
    }

}
