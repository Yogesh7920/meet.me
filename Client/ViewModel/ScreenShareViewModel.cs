/**
 * owned by: Pulavarti Vinay Kumar
 * created by: Pulavarti Vinay Kumar
 * date created: 16/10/2021
 * date modified: 28/10/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenSharing;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace Client.ViewModel
{
    public class ScreenShareViewModel : INotifyPropertyChanged, IScreenShare
    {
        string home;
        string path;
        string ImagePath;
        /// <summary>
        /// The received image.
        /// </summary>
        public BitmapImage ReceivedImage
        {
            get; private set;
        }

        /// <summary>
        /// The received message.
        /// </summary>
        public string ReceivedMessage
        {
            get; private set;
        }

        public ScreenShareViewModel(bool testing)
        {
            this.testing = testing;
        }

        public ScreenShareViewModel()
        {

            model = ScreenShareFactory.GetScreenShareClient();

            model.Subscribe(this);


            //home = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //path = String.Join(@"\", home.Split('\\').Reverse().Skip(3).Reverse());
            //ImagePath = path + "/Icons/screenshare.png";
            BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/"
             + Assembly.GetExecutingAssembly().GetName().Name
             + ";component/"
             + "Icons/screenshare.png", UriKind.Absolute));
            //BitmapImage image = new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
            this.ReceivedImage = image;

            this.ReceivedMessage = "No one is sharing the screen(Default)";
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
        /// Handles an incoming screen.
        /// </summary>
        public void OnScreenRecieved(string uid, string uname, int mtype, Bitmap screen)
        {
            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
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
                                BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/"
                                 + Assembly.GetExecutingAssembly().GetName().Name
                                 + ";component/"
                                 + "Icons/screenshare.png", UriKind.Absolute));

                                this.ReceivedImage = image;


                                this.ReceivedMessage = "Network Problem";
                                Trace.WriteLine(ReceivedMessage);

                                this.OnPropertyChanged("ReceivedMessage");
                                this.OnPropertyChanged("ReceivedImage");
                            }
                            else if (mtype == -1) // some one else is sharing so u can't share ur screen
                            {
                                MainWindow.s_sharing = false;
                                BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/"
                                 + Assembly.GetExecutingAssembly().GetName().Name
                                 + ";component/"
                                 + "Icons/screenshare.png", UriKind.Absolute));

                                this.ReceivedImage = image;

                                //File.Delete(path + "/Icons/screenshare.png");
                                //File.Copy(path + "/Icons/meet.me_logo_no_bg.png", path + "/Icons/screenshare.png");

                                this.ReceivedMessage = "some one else is sharing so u can't share ur screen";
                                Trace.WriteLine(ReceivedMessage);


                                this.OnPropertyChanged("ReceivedMessage");
                                this.OnPropertyChanged("ReceivedImage");
                            }
                            else if (mtype == 0)  // Stop the screen share
                            {
                                MainWindow.s_sharing = false;
                                BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/"
                                 + Assembly.GetExecutingAssembly().GetName().Name
                                 + ";component/"
                                 + "Icons/screenshare.png", UriKind.Absolute));

                                this.ReceivedImage = image;

                                this.ReceivedMessage = "No one is sharing the screen";
                                Trace.WriteLine(ReceivedMessage);


                                this.OnPropertyChanged("ReceivedMessage");
                                this.OnPropertyChanged("ReceivedImage");
                            }
                            else if (mtype == 1)  // screen share is going now
                            {
                                MainWindow.s_sharing = true;
                                BitmapImage image = ToBitmapImage(screen);
                                this.ReceivedImage = image;

                                this.ReceivedMessage = string.Empty;
                                Trace.WriteLine(ReceivedMessage);


                                this.OnPropertyChanged("ReceivedMessage");
                                this.OnPropertyChanged("ReceivedImage");
                            }
                            else
                            {
                                MainWindow.s_sharing = false;
                                BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/"
                                 + Assembly.GetExecutingAssembly().GetName().Name
                                 + ";component/"
                                 + "Icons/screenshare.png", UriKind.Absolute));

                                this.ReceivedImage = image;

                                this.ReceivedMessage = "No one is sharing the screen";
                                Trace.WriteLine(ReceivedMessage);


                                this.OnPropertyChanged("ReceivedMessage");
                                this.OnPropertyChanged("ReceivedImage");
                            }
                        }),
                        uid, uname, mtype, screen);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handles the property changed event raised on a component.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Gets the dispatcher to the main thread. In case it is not available
        /// (such as during unit testing) the dispatcher associated with the
        /// current thread is returned.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ?
                    Application.Current.Dispatcher :
                    Dispatcher.CurrentDispatcher;

        private ScreenShareClient model;
        private bool testing;
    }
}