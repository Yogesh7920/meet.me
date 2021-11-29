/**
 * owned by: Pulavarti Vinay Kumar
 * created by: Pulavarti Vinay Kumar
 * date created: 16/10/2021
 * date modified: 28/10/2021
**/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ScreenSharing;

namespace Client.ViewModel
{
    public class ScreenShareViewModel : INotifyPropertyChanged, IScreenShare
    {
        private readonly string home;
        private readonly string ImagePath;

        private readonly ScreenShareClient model;
        private readonly string path;
        private bool testing;

        public ScreenShareViewModel(bool testing)
        {
            this.testing = testing;
        }

        public ScreenShareViewModel()
        {
            model = ScreenShareFactory.GetScreenShareClient();

            model.Subscribe(this);


            home = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = string.Join(@"\", home.Split('\\').Reverse().Skip(3).Reverse());
            ImagePath = path + "/Icons/screenshare.png";
            var image = new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
            ReceivedImage = image;

            ReceivedMessage = "No one is sharing the screen(Default)";
        }

        /// <summary>
        ///     The received image.
        /// </summary>
        public BitmapImage ReceivedImage { get; private set; }

        /// <summary>
        ///     The received message.
        /// </summary>
        public string ReceivedMessage { get; private set; }

        /// <summary>
        ///     Gets the dispatcher to the main thread. In case it is not available
        ///     (such as during unit testing) the dispatcher associated with the
        ///     current thread is returned.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
            Application.Current?.Dispatcher != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Handles an incoming screen.
        /// </summary>
        public void OnScreenRecieved(string uid, string uname, int mtype, Bitmap screen)
        {
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action<string, string, int, Bitmap>((uid, uname, mtype, screen) =>
                {
                    // Note that Bitmap cannot be automatically marshalled to the main thread
                    // if it were created on the worker thread. Hence the data model just passes
                    // the path to the image, and the main thread creates an image from it.

                    Trace.WriteLine("[UX] screen is received" + mtype);
                    Trace.WriteLine("[UX] path (Directory) " + ImagePath);
                    // Problem in the network connection
                    if (mtype == -2)
                    {
                        MainWindow.s_sharing = false;
                        var image = new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
                        ReceivedImage = image;


                        ReceivedMessage = "Network Problem";
                        Trace.WriteLine(ReceivedMessage);

                        OnPropertyChanged("ReceivedMessage");
                        OnPropertyChanged("ReceivedImage");
                    }
                    else if (mtype == -1) // some one else is sharing so u can't share ur screen
                    {
                        MainWindow.s_sharing = false;
                        var image = new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
                        ReceivedImage = image;

                        //File.Delete(path + "/Icons/screenshare.png");
                        //File.Copy(path + "/Icons/meet.me_logo_no_bg.png", path + "/Icons/screenshare.png");

                        ReceivedMessage = "some one else is sharing so u can't share ur screen";
                        Trace.WriteLine(ReceivedMessage);


                        OnPropertyChanged("ReceivedMessage");
                        OnPropertyChanged("ReceivedImage");
                    }
                    else if (mtype == 0) // Stop the screen share
                    {
                        MainWindow.s_sharing = false;
                        var image = new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
                        ReceivedImage = image;

                        ReceivedMessage = "No one is sharing the screen";
                        Trace.WriteLine(ReceivedMessage);


                        OnPropertyChanged("ReceivedMessage");
                        OnPropertyChanged("ReceivedImage");
                    }
                    else if (mtype == 1) // screen share is going now
                    {
                        MainWindow.s_sharing = true;
                        var image = ToBitmapImage(screen);
                        ReceivedImage = image;

                        ReceivedMessage = string.Empty;
                        Trace.WriteLine(ReceivedMessage);


                        OnPropertyChanged("ReceivedMessage");
                        OnPropertyChanged("ReceivedImage");
                    }
                    else
                    {
                        MainWindow.s_sharing = false;
                        var image = new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
                        ReceivedImage = image;

                        ReceivedMessage = "No one is sharing the screen";
                        Trace.WriteLine(ReceivedMessage);


                        OnPropertyChanged("ReceivedMessage");
                        OnPropertyChanged("ReceivedImage");
                    }
                }),
                uid, uname, mtype, screen);
        }

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        /// <summary>
        ///     Handles the property changed event raised on a component.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}