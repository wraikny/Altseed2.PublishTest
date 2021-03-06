﻿using System;
using Altseed2;

namespace PublishSingleTrimmedExcludeGitHub
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Engine.Initialize("PublishSingleTrimmedExcludeNuget", 800, 600);

            while(Engine.DoEvents())
            {
                Engine.Update();
            }

            Engine.Terminate();
        }
    }
}
