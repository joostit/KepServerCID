using CidaRefImplCsharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib
{
    public class QuitThread
    {

        //This is the thread to loop and react to user pressing 'q' to quit 
        public void RuntimeThreadProc()
        {
            ConsoleKeyInfo cki;
            Console.TreatControlCAsInput = true;
            bool doContinue = true;
            while (doContinue)
            {
                cki = Console.ReadKey(true);
                if ((cki.Key != ConsoleKey.Q))
                {
                    Console.WriteLine("CIDA Reference Implementation is currently running.");
                    Console.WriteLine("Press 'q' to quit");
                }
                else
                {
                    //write something to main thread global to exit
                    MemInterface.exitFlag = true;
                    doContinue = false;
                }
            }
        } // RuntimeThreadProc ()

    } //public class QuitThread
}
