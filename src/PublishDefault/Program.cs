using System;
using Altseed2;

namespace PublishDefault
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Engine.Initialize("PublishDefault", 800, 600);

            while(Engine.DoEvents())
            {
                Engine.Update();
            }

            Engine.Terminate();
        }
    }
}
