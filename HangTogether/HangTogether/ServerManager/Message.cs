namespace HangTogether
{
    public class Message
    {
        public Message(string fromEmail, string toEmail, string message, string Key, string timeStamp)
        {
            this.fromEmail = fromEmail;
            this.toEmail = toEmail;
            this.message = message;
            this.Key = Key;
            this.timeStamp = timeStamp;
        }
        public string timeStamp { get; set; }

        
        public string fromEmail { get; set; }
        
        
        public string toEmail { get; set; }
      
        public string message { get; set; }
        
        public string Key { get; set; }


    }
}