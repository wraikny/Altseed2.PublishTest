using System;
using Altseed2;

namespace PublishSingleTrimmed
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Initialize("PublishSingleTrimmed", 800, 600);

            while(Engine.DoEvents())
            {
                Engine.Update();
            }

            Engine.Terminate();
        }
    }
}
