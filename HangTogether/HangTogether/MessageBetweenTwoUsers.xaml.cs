using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangTogether.ServerManager;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageBetweenTwoUsers : ContentPage
    {
        private User emetteurMessage;
        private User recepteurMessage;
        public MessageBetweenTwoUsers(User emetteurMessage, User recepteurMessage)
        {
            InitializeComponent();
            this.emetteurMessage = emetteurMessage;
            this.recepteurMessage = recepteurMessage;
        }
    }
}