/*
 * 
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Android.Media;

namespace ServerConsole
{
    public class ChatRoomHandler
    {
        public Stack<ClientHandlerForChat> recordsHandler{ get; set; }
        //public MediaRecorder record;

        static int howManyMess = 0;
        public string str = "no messages$ ";
        public ChatRoomHandler()
        {
            recordsHandler = new Stack<ClientHandlerForChat>();
        }
        public void addToList(ClientHandlerForChat clhfc)
        {
            recordsHandler.Push(clhfc);
            howManyMess++;
        }
        public void sendTO(int n)
        {
            Stack<ClientHandlerForChat> temp;
            if (n > howManyMess)
            {
                n = n - howManyMess;
            }
            else
            {
                if (howManyMess >= 1)
                {
                    n = 1;
                }
                else
                    n = 0;
            }
            while (true)
            {

            }
        }
        public override string ToString()
        {            

            if (howManyMess != 0)
            {
                if (howManyMess==1)
                    str ="";
                foreach(ClientHandlerForChat chlfc in recordsHandler)
                {
                    str += chlfc.ToString() + "|";
                }
            }

            return str;
        }
    }
}
