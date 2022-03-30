namespace HangTogether
{
    public class Message
    {
        public Message(string fromEmail, string toEmail, string firstMessage, string sndMessage, string Key)
        {
            this.fromEmail = fromEmail;
            this.toEmail = toEmail;
            this.firstMessage = firstMessage;
            this.sndMessage = sndMessage;
            this.Key = Key;
        }

        public string fromEmail { get; set; }
        public string toEmail { get; set; }
        public string firstMessage { get; set; }
        public string sndMessage { get; set; }
        public string Key { get; set; }
        
    }
}