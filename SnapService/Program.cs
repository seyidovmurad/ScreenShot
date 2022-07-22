using System;
using System.Windows;
using System.Drawing;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SnapService
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Listen();
        }


        static Bitmap TakeScreenShot()
        {
            Bitmap memoryImage;
            memoryImage = new Bitmap(500, 400);
            Size s = new Size(memoryImage.Width, memoryImage.Height);

            Graphics memoryGraphics = Graphics.FromImage(memoryImage);

            memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);


            return memoryImage;
        }


        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }


        static void Listen()
        {
            var listener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);

            var ip = IPAddress.Parse("127.0.0.1");
            var port = 45678;
            var listenerEP = new IPEndPoint(ip, port);
            listener.Bind(listenerEP);


            var buffer = new byte[ushort.MaxValue - 30];
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            var len = 0;
            var msg = "";

            while (true)
            {
                len = listener.ReceiveFrom(buffer, ref remoteEP);
                msg = Encoding.Default.GetString(buffer, 0, len);
                Console.WriteLine(remoteEP.ToString());
                if(msg == "ss")
                {
                    var img = TakeScreenShot();
                    buffer = ImageToByte(img);

                    listener.SendTo(buffer, new IPEndPoint(ip, 27000));
                }
            }
        }


    }
}
