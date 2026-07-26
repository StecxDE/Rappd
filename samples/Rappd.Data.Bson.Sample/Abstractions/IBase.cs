using System;
using System.Collections.Generic;
using System.Text;

namespace Rappd.Data.Bson.Sample.Abstractions
{
    [BaseInterface(nameof(Type))]
    internal interface IBase
    {
        public string Type { get; }
        public string Identifier { get; }
    }
    [SubInterface<IBase>(TYPE)]
    internal interface ISub : IBase
    {
        public const string TYPE = "sub";

        public string Name { get; init; }
    }
}
