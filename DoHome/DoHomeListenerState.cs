//-----------------------------------------------------------------------
// <copyright file="DoHomeListenerState.cs" company="Company">
//    Copyright (c) Tino Donderwinkel. All rights reserved.
// </copyright>
// <author>Tino Donderwinkel</author>
//-----------------------------------------------------------------------

namespace DoHome
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// State for the DoHome UDP listener.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed.")]
    public enum DoHomeListenerState
    {
        Stopped,
        Starting,
        Running,
        Stopping
    }
}
