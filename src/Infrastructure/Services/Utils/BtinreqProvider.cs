using Application.Contracts.BtServices.Common;
using Application.Interfaces.Utils;
using Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using System.Net;

namespace Infrastructure.Services.Utils
{
    public class BtinreqProvider : IBtinreqProvider
    {
        private readonly IOptions<BtServicesConfig> _options;
        public BtinreqProvider(IOptions<BtServicesConfig> options)
        {
            _options = options;
        }
        public Btinreq GetBtinreqAsync(string token)
        {
            string deviceIp = GetLocalIpAddress();
            return new Btinreq
            {
                Canal = _options.Value.Channel,
                Requerimiento = "0",
                Usuario = _options.Value.Username,
                Token = token,
                Device = deviceIp
            };
        }
        private static string GetLocalIpAddress()
        {
            string localIP = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
    }
}
