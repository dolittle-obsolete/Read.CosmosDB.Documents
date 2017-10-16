﻿/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using doLittle.Assemblies;
using doLittle.Assemblies.Configuration;
using doLittle.Assemblies.Rules;

namespace doLittle.Read.DocumentDB
{
    /// <summary>
    /// Reperesents an <see cref="ICanSpecifyAssemblies">assembly specifier</see> for client aspects
    /// </summary>
    public class AssemblySpecifier : ICanSpecifyAssemblies
    {
#pragma warning disable 1591 // Xml Comments
        public void Specify(IAssemblyRuleBuilder builder)
        {
            builder.ExcludeAssembliesStartingWith(
                "Microsoft.Azure.DocumentDB.Core"
            );
        }
#pragma warning disable 1591 // Xml Comments
    }
}
