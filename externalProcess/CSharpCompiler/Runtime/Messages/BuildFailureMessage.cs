﻿namespace CSharpCompiler.Runtime.Messages
{
    internal class BuildFailureMessage : BuildStatusMessage
    {
        public BuildFailureMessage(object value, string format) : base("FAILURE", value, format)
        {

        }
    }
}