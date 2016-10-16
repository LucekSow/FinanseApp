using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using AutofacExtension.Attributes;
using AutofacExtension.Entity;
using NLog;

namespace AutofacExtension
{
    public class ModuleExtension : Autofac.Module
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected void Register(Assembly assembly, ContainerBuilder builder)
        {
            logger.Info("Start Register Type in Assembly: {0}", assembly.FullName);
            IEnumerable<RegisterEntity> types = GetTypesToRegister(assembly);
            foreach (var registerEntity in types)
            {
                logger.Info("Register Type: {0}", registerEntity.OriginType.FullName);
                builder.RegisterType(registerEntity.OriginType);
            }
            IEnumerable<RegisterAsTypeEntity> registerAsType = GetTypesAsOtherTypeRegister(assembly);
            foreach (var registerAsTypeEntity in registerAsType)
            {
                logger.Info("Register Type: {0}", registerAsTypeEntity.OriginType.FullName);
                builder.RegisterType(registerAsTypeEntity.OriginType);
                foreach (var item in registerAsTypeEntity.RegisterType)
                {
                    logger.Info("Register Type: {0} as {1}", registerAsTypeEntity.OriginType.FullName, item.FullName);
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