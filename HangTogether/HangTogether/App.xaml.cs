﻿using System;
using HangTogether.ServerManager;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace HangTogether
{
    public partial class App : Application
    {
        private DataBaseManager dbManager;
        public App()
        {
           /* dbManager = new DataBaseManager(); 
            MainPage =  new NavigationPage(new LogInSignUp());*/
           User A = new User("", "", "", "", "", "", "");
           MainPage = new MessageBetweenTwoUsers(A, A);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
