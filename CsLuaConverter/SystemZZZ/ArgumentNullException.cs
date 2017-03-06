﻿[assembly: CsLuaFramework.Attributes.CsLuaLibrary]

namespace SystemZZZ
{
    using System;

    public class ArgumentNullException : ArgumentException
    {
        public ArgumentNullException() : base("Value cannot be null.")
        {
            
        }

        public ArgumentNullException(string paramName) : base("Value cannot be null.", paramName)
        {

        }

        public ArgumentNullException(string paramName, Exception innerException) : base(paramName, innerException)
        {

        }

        public ArgumentNullException(string paramName, string message) : base(paramName, message)
        {

        }
    }
}
