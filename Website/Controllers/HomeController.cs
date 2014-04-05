using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Sql;
using Microsoft.WindowsAzure.Management.Sql.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var sqlManagementClient = createSqlManagementClient();
            return View(GetSqlFirewallRuleList().Where(f => f.Name == ConfigurationManager.AppSettings["FirewallRuleName"]).SingleOrDefault());
        }
        [HttpPost]
        public ActionResult Index(string IPAddress)
        {
            var sqlManagementClient = createSqlManagementClient();
            FirewallRuleUpdateParameters parameters = new FirewallRuleUpdateParameters();
            parameters.StartIPAddress = IPAddress;
            parameters.EndIPAddress = IPAddress;
            parameters.Name = ConfigurationManager.AppSettings["FirewallRuleName"];
            sqlManagementClient.FirewallRules.Update(ConfigurationManager.AppSettings["SQLServerName"], ConfigurationManager.AppSettings["FirewallRuleName"], parameters);
            return View(GetSqlFirewallRuleList().Where(f => f.Name == ConfigurationManager.AppSettings["FirewallRuleName"]).SingleOrDefault());
        }
        
        private SqlManagementClient createSqlManagementClient()
        {
            //To use this, create and upload your own management certificate for Azure.  Then export the pfx file from Certificates MMC with the private key.
            //Save that file in the root directory called azure-mgt-cert.pfx and add the password to the web.config file
            //The file in this solution is empty and just a placeholder
            var cert = new X509Certificate2(Server.MapPath("/azure-mgt-cert.pfx"), ConfigurationManager.AppSettings["CertificatePassword"], X509KeyStorageFlags.MachineKeySet);
            SqlManagementClient sqlManagementClient = new SqlManagementClient(new CertificateCloudCredentials(ConfigurationManager.AppSettings["SubscriptionID"], cert));
            return sqlManagementClient;
        }
        
        private IList<FirewallRule> GetSqlFirewallRuleList()
        {
            var sqlManagementClient = createSqlManagementClient();
            var firewallRuleList = sqlManagementClient.FirewallRules.List(ConfigurationManager.AppSettings["SQLServerName"]);
            return firewallRuleList.FirewallRules;            
        }        
    }
}