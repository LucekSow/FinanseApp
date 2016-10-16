using System;

namespace AutofacExtension.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterAsType : Attribute
    {
        public RegisterAsType(Type registerType)
        {
            Type = registerType;
        }

        public Type Type { get; }
    }
}