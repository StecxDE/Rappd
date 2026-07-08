using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Rappd.Data.Generators.Tests
{
    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifySourceGenerators.Initialize();
        }
    }
}
