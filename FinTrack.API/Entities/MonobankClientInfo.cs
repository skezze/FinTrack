using System.Security.Principal;

namespace FinTrack.API.Entities
{
    public class MonobankClientInfo
    {
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string WebHookUrl { get; set; }
        public string Permissions { get; set; }
        public List<MonobankAccount> Accounts { get; set; }
        public List<MonobankJar> Jars { get; set; }
    }
}
