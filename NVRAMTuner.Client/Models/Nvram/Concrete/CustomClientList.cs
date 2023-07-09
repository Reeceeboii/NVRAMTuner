namespace NVRAMTuner.Client.Models.Nvram.Concrete
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Model representing a special case variable: "custom_clientlist".
    /// This variable is very hard to decipher as plaintext so further formatting is required before it can be displayed nicely.
    ///
    /// This variable is a series of values enclosed in angle brackets:
    ///                 1. Device name
    ///                 2. Device MAC address
    ///                 3. Unknown third value
    ///                 4. Unknown fourth value
    ///                 5. Unknown fifth value
    ///                 6. Unknown sixth value
    ///                 // TODO some more detail on what these *actually* are would be good
    ///
    /// This is represented in this variable as a <see cref="List{T}"/> of six tuples: <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/>
    /// </summary>
    public class CustomClientList : VariableBase<List<Tuple<string, string, string, string, string, string>>>
    {
    }
}
