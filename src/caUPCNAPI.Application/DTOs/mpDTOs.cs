using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.DTOs
{
    public class MpWebhookDto
    {
        public string Action { get; set; } = string.Empty;
        [JsonPropertyName("api_version")]
        public string ApiVersion { get; set; } = string.Empty;
        public MpWebhookData Data { get; set; } = new();
        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }
        // Cambiar de string a long para que coincida con el JSON de MercadoPago
        public long Id { get; set; }
        [JsonPropertyName("live_mode")]
        public bool LiveMode { get; set; }
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("user_id")]
        public long UserId { get; set; }
    }

    public class MpWebhookData
    {
        public long Id { get; set; }
    }

}
