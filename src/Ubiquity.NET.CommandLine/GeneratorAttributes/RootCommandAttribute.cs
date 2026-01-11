// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine.GeneratorAttributes
{
    /// <summary>Attribute to mark a command for generation of the backing implementation</summary>
    [AttributeUsage( AttributeTargets.Class, Inherited = false, AllowMultiple = false )]
    public sealed class RootCommandAttribute
        : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="RootCommandAttribute"/> class.</summary>
        public RootCommandAttribute( )
        {
        }

        /// <summary>Gets or sets the Description for this command (Default: null)</summary>
        public string? Description { get; set; } = null;

        /// <summary>Gets a value indicating whether errors reported should also show the help message [Default: <see langword="false"/>]</summary>
        /// <remarks>This has a <see langword="false"/> default, which is the opposite of the default from <see cref="ParserConfiguration"/></remarks>
        public bool ShowHelpOnErrors { get; init; } = false;

        /// <summary>Gets a value indicating whether errors reported should also show Typo corrections [Default: <see langword="false"/>]</summary>
        /// <remarks>This has a <see langword="false"/> default, which is the opposite of the default from <see cref="ParserConfiguration"/></remarks>
        public bool ShowTypoCorrections { get; init; } = false;

        /// <inheritdoc cref="ParserConfiguration.EnablePosixBundling"/>
        public bool EnablePosixBundling { get; init; } = true;

        /// <summary>Gets a value that indicates the default options to include</summary>
        /// <remarks>
        /// Default handling includes <see cref="DefaultOption.Help"/> and <see cref="DefaultOption.Version"/>.
        /// This allows overriding that to specify behavior as desired.
        /// </remarks>
        public DefaultOption DefaultOptions { get; init; } = DefaultOption.Help | DefaultOption.Version;

        /// <summary>Gets a value that indicates the default Directives to include</summary>
        /// <remarks>
        /// Default handling includes <see cref="DefaultDirective.Suggest"/>.
        /// This allows overriding that to specify behavior as needed.
        /// </remarks>
        public DefaultDirective DefaultDirectives { get; init; } = DefaultDirective.Suggest;
    }
}
