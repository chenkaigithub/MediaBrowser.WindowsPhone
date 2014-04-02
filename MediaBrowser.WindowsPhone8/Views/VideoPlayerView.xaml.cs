﻿using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.PlayerFramework;
using MediaBrowser.WindowsPhone.ViewModel;
using System;
using System.Threading.Tasks;

namespace MediaBrowser.WindowsPhone.Views
{
    public partial class VideoPlayerView
    {
        private bool _seeking = false;
        // Constructor
        public VideoPlayerView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
                Messenger.Default.Send(new NotificationMessage(Constants.Messages.SendVideoTimeToServerMsg));
            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                if (!this.NavigationContext.QueryString.ContainsKey("id") || !this.NavigationContext.QueryString.ContainsKey("type"))
                    return;
                var itemId = this.NavigationContext.QueryString["id"];
                var type = this.NavigationContext.QueryString["type"];
                var model = this.DataContext as VideoPlayerViewModel;
                if (model != null)
                {
                    model.Recover = true;
                    model.ItemId = itemId;
                    model.ItemType = type;
                }

            }
            base.OnNavigatedTo(e);
        }

        protected override async void InitialiseOnBack()
        {
            base.InitialiseOnBack();
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.SetResumeMsg));
            var model = this.DataContext as VideoPlayerViewModel;
            if (model != null && model.Recover)
            {
                model.RecoverState();
            }
        }

        private void ThePlayerMediaEnded(object sender, MediaPlayerActionEventArgs e)
        {
            if (!_seeking)
                Messenger.Default.Send(new NotificationMessage(Constants.Messages.SendVideoTimeToServerMsg));
        }

        private void ThePlayerMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Log.ErrorException("Error playing media: " + e.ErrorException.Message, e.ErrorException);
        }

        private void ThePlayer_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            
        }

        private void ThePlayer_OnMediaStarting(object sender, MediaPlayerDeferrableEventArgs e)
        {

        }

        private void ThePlayer_OnMediaStarted(object sender, RoutedEventArgs e)
        {
            _seeking = false;
            var model = this.DataContext as VideoPlayerViewModel;
            if (model != null)
            {                
                thePlayer.StartTime = model.StartTime;
                thePlayer.EndTime = model.EndTime;
                thePlayer.Position = TimeSpan.FromTicks(model.StartTime.Ticks * -1);                         
                model.StartUpdateTimer();
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            var result = MessageBox.Show("Are you sure you want to exit the video player?", "Are you sure?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Messenger.Default.Send(new NotificationMessage(Constants.Messages.SendVideoTimeToServerMsg));
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void ThePlayer_OnCurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (thePlayer.CurrentState == MediaElementState.Playing || thePlayer.CurrentState == MediaElementState.Paused)
            {
                var isPaused = thePlayer.CurrentState == MediaElementState.Paused;
                Messenger.Default.Send(new NotificationMessage(isPaused, Constants.Messages.VideoStateChangedMsg));
            }
        }

        private async void thePlayer_ScrubbingCompleted(object sender, ScrubProgressRoutedEventArgs e)
        {
            e.Canceled = true;
            var model = this.DataContext as VideoPlayerViewModel;
            if (model != null)
            {
                _seeking = true;
                model.Seek(e.Position.Ticks);
            }
        }

        private async void thePlayer_Seeked(object sender, SeekRoutedEventArgs e)
        {
            e.Canceled = true;
            var model = this.DataContext as VideoPlayerViewModel;
            if (model != null)
            {                
                _seeking = true;
                model.Seek(e.Position.Ticks);
            }
        }


    }
}