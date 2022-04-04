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
        
        /*
         * Qd quelqu'un click sur send; on stoque le message dans la BD
         */
        public void sendMessage(object sender, EventArgs args)
        {
            var textToSend = this.textMessage.ToString();
            var emetteurEmail = this.emetteurMessage.email;
            var recepteurEmail = this.recepteurMessage.email;
            var timeStamp = "101010100101"; // test
            Message message = new Message(emetteurEmail, recepteurEmail, textToSend, "", timeStamp);
            DataBaseMessagesManager dataBaseManager = new DataBaseMessagesManager();
            dataBaseManager.addNewConversation(message);
        }

      
    }
}