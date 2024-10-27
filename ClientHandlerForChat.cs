using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Android.Media;
namespace ServerConsole
{
    public class ClientHandlerForChat
    {
        public string name { get; set; }
        public string mess { get; set; }
        public ClientHandlerForChat(string name, string mess)
        {
            this.name = name;
            this.mess = mess;
        }
        public override string ToString()
        {
            return "s"+name+": "+mess;
        }
    }
}
