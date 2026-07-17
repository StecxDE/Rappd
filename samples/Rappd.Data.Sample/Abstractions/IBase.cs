using System;
using System.Collections.Generic;
using System.Text;

namespace Rappd.Data.Sample.Abstractions
{
    [BaseInterface(nameof(Type))]
    internal interface IBase
    {
        public string Type { get; }
        public string Id { get; }
    }
    [SubInterface<IBase>(TYPE)]
    internal interface ISub : IBase
    {
        public const string TYPE = "sub";

        public string Name { get; init; }
    }
}
