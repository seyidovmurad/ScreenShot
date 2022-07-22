using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace ScreenShoter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Send();
            Listen();
        }

        private void Listen()
        {
            Task.Run(() =>
            {
                var listener = new Socket(
               AddressFamily.InterNetwork,
               SocketType.Dgram,
               ProtocolType.Udp);

                var ip = IPAddress.Parse("127.0.0.1");
                var port = 27000;
                var listenerEP = new IPEndPoint(ip, port);
                listener.Bind(listenerEP);


                var buffer = new byte[ushort.MaxValue - 30];
                EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                var len = 0;

                while (true)
                {
                    len = listener.ReceiveFrom(buffer, ref remoteEP);
                    var img = LoadImage(buffer);
                    Dispatcher.Invoke(() => { Img.Source = img; });
                }
            });
        }

        private void Send()
        {
            using var client = new Socket(
               AddressFamily.InterNetwork,
               SocketType.Dgram,
               ProtocolType.Udp);
            
            var ip = IPAddress.Parse("127.0.0.1");
            var port = 45678;
            var remoteEP = new IPEndPoint(ip, port);
            
            var msg = "ss";
            byte[] buffer = null;
            buffer = Encoding.Default.GetBytes(msg);
            client.SendTo(buffer, remoteEP);
                
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

    }
}
