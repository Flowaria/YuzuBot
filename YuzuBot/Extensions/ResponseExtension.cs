using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YuzuBot;
internal static class ResponseExtension
{
    public static bool IsSuccessOrRedirectCode(this HttpResponseMessage msg)
    {
        if (msg == null)
            return false;

        if (msg.IsSuccessStatusCode)
            return true;

        return (int)msg.StatusCode switch
        {
            301 or 302 or 303 or 307 or 308 => true,
            _ => false,
        };
    }
}
