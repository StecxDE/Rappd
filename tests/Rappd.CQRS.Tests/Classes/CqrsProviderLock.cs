using Rappd.CQRS.Tests.Classes;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: AssemblyFixture(typeof(CqrsProviderFixture))]

namespace Rappd.CQRS.Tests.Classes;

public class CqrsProviderFixture
{
    private readonly ReaderWriterLockSlim _lock = new();

    public void Lock()
    {
        _lock.EnterWriteLock();
    }

    public void Release()
    {
        _lock.ExitWriteLock();
    }
}