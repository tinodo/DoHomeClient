//-----------------------------------------------------------------------
// <copyright file="DoHomeTimerType.cs" company="Company">
//    Copyright (c) Tino Donderwinkel. All rights reserved.
// </copyright>
// <author>Tino Donderwinkel</author>
//-----------------------------------------------------------------------

namespace DoHome
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The type of a <see cref="DoHomeTimer"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed.")]
    public enum DoHomeTimerType
    {
        TIMER_SHUTDOWN = 0,
        TIMER_CONSTANT = 1,
        TIMER_PRESET_MODE = 2,
        TIMER_CUSTOM_MODE = 3,
        TIMER_DELAY_SHUTDOWN = 4,
    }
}
