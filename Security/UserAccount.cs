using ispProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace ispProject.Security
{
    public class UserAccount
    {
        public static string HashSHA1(string value)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var inputBytes = Encoding.ASCII.GetBytes(value);
            var hash = sha1.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static Int32 GetUserID()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            return Convert.ToInt32(ticket.Name);
        }

        public static bool isAuthorized(int? roleId)
        {
            bool ans = false;

            using (HITProjectData_Fall17Entities1 db = new HITProjectData_Fall17Entities1())
            {
                user u = db.users.Find(GetUserID());
                if (u.roleId <= roleId)
                    ans = true;
            }
            return ans;
        }
    }
}