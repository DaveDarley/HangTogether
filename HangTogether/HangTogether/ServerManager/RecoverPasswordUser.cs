using System;
using Xamarin.Forms;

namespace HangTogether.ServerManager
{
    public class RecoverPasswordUser
    {
        public RecoverPasswordUser(string email, string verifCode)
        {
            this.email = email;
            this.verifCode = verifCode;
        }

        public string email { get; set; }
        public string verifCode { get; set; }
    }
}