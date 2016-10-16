using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using AutofacExtension.Attributes;
using AutofacExtension.Entity;

namespace AutofacExtension
{
    public class ModuleExtension : Autofac.Module
    {
        protected void Register(Assembly assembly, ContainerBuilder builder)
        {
            IEnumerable<RegisterEntity> types = GetTypesToRegister(assembly);
            foreach (var registerEntity in types)
            {
                builder.RegisterType(registerEntity.OriginType);
            }
            IEnumerable<RegisterAsTypeEntity> registerAsType = GetTypesAsOtherTypeRegister(assembly);
            foreach (var registerAsTypeEntity in registerAsType)
            {
                builder.RegisterType(registerAsTypeEntity.OriginType);
                foreach (var item in registerAsTypeEntity.RegisterType)
                {
                    builder.RegisterType(registerAsTypeEntity.OriginType).As(item);
                }
            }
        }

        private static IEnumerable<RegisterAsTypeEntity> GetTypesAsOtherTypeRegister(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(RegisterAsType), true).Length > 0)
                {
                    var customAttributes = (RegisterAsType[])type.GetCustomAttributes(typeof(RegisterAsType), true);
                    yield return new RegisterAsTypeEntity
                    {
                        OriginType = type,
                        RegisterType = (customAttributes.Length == 0) ? new List<Type>() : customAttributes.Select(x => x.Type).Where(x => x != null)
                    };
                }
            }
        }

        private static IEnumerable<RegisterEntity> GetTypesToRegister(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(Register), true).Length > 0)
                {
                    yield return new RegisterEntity
                    {
                        OriginType = type
                    };
                }
            }
        }
    }
}