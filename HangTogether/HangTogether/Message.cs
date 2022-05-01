using Google.Cloud.Firestore;

namespace HangTogether
{
    [FirestoreData]
    public class Message
    {
        public Message(string fromEmail, string toEmail, string message, string Key, string timeStamp, string lu)
        {
            this.fromEmail = fromEmail;
            this.toEmail = toEmail;
            this.message = message;
            this.Key = Key;
            this.timeStamp = timeStamp;
            this.lu = lu;
        }
        [FirestoreProperty]
        public string timeStamp { get; set; }

        [FirestoreProperty]
        public string fromEmail { get; set; }
        
        [FirestoreProperty]
        public string toEmail { get; set; }
        
        [FirestoreProperty]
        public string message { get; set; }
        
        [FirestoreProperty]
        public string Key { get; set; }

        [FirestoreProperty] 
        public string lu { get; set; }

    }
}