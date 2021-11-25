﻿using System;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Samples.DependencyInjection.Shared
{
    public interface IService
    {
        void SayHello();
    }
    public interface ISingleton : IService {}
    public interface ITransient : IService {}
    public interface IScoped : IService {}

    public class Singleton : ISingleton
    {
        public void SayHello()
            => Console.WriteLine($"Hello from Singleton");
    }

    public class Transient : ITransient
    {
        public void SayHello()
            => Console.WriteLine($"Hello from Transient");
    }

    public class Scoped : IScoped, IDisposable
    {
        public Scoped()
            => Console.WriteLine("Creating scoped service");
        public void SayHello()
            => Console.WriteLine($"Hello from Scoped");

        public void Dispose()
            => Console.WriteLine($"Disposing scoped service");
    }

    public interface ITenantSpecific : IService { }

    public class TenantSpecific : ITenantSpecific
    {
        readonly TenantId _tenant;

        public TenantSpecific(TenantId tenant)
        {
            _tenant = tenant;
        }

        public void SayHello()
            => Console.WriteLine($"Hello from tenant {_tenant}");
    }
    
    public interface ITenantSpecificScoped : IService { }

    public class TenantSpecificScoped : ITenantSpecificScoped, IDisposable
    {
        readonly TenantId _tenant;

        public TenantSpecificScoped(TenantId tenant)
        {
            _tenant = tenant;
        }
        
        public void SayHello()
            => Console.WriteLine($"Hello from tenant scoped {_tenant}");

        public void Dispose()
            => Console.WriteLine($"Disposing scoped service for {_tenant}");
    }
}

