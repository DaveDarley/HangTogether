using System.Collections.Generic;
using System.Timers;
using HangTogether.ServerManager;

namespace HangTogether
{
    public class ConversationBetweenTwoUserHelper
    {
        private static  User usertoCheck;
        private static  User userChecking;
        private static System.Timers.Timer aTimer;
        public static void test(DisplayMessages disp)
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(5000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += IsUserOnline;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        

        public static void IsUserOnline(object source, ElapsedEventArgs e)
        {
            if (usertoCheck.isUserReadMessage == "y")
            {
            }
            else
            {
            }
        }

    }
}