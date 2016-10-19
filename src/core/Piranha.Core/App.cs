﻿/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Piranha
{
    /// <summary>
    /// The main application object.
    /// </summary>
    public sealed class App
    {
        #region Members
        private static readonly App instance = new App();
        private readonly object mutex = new object();
        private bool isInitialized = false;
        private Extend.FieldInfoList fields;
        private List<Extend.IModule> modules;
        private IList<Extend.BlockType> blockTypes;
        private IList<Extend.PageType> pageTypes;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the currently registered field types.
        /// </summary>
        public static Extend.FieldInfoList Fields {
            get { return instance.fields; }
        }

        /// <summary>
        /// Gets the currently registred modules.
        /// </summary>
        public static IList<Extend.IModule> Modules {
            get { return instance.modules; }
        }

        /// <summary>
        /// Gets the currently registered block types.
        /// </summary>
        public static IList<Extend.BlockType> BlockTypes {
            get { return instance.blockTypes; }
        }

        /// <summary>
        /// Gets the currently registered page types.
        /// </summary>
        public static IList<Extend.PageType> PageTypes {
            get { return instance.pageTypes; }
        }

        /// <summary>
        /// Gets the binding flags for retrieving a region from a
        /// strongly typed model.
        /// </summary>
        public static BindingFlags PropertyBindings {
            get { return BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance; }
        }
        #endregion

        /// <summary>
        /// Default private constructor.
        /// </summary>
        private App() {
            fields = new Extend.FieldInfoList();
            modules = new List<Extend.IModule>();
            blockTypes = new List<Extend.BlockType>();
            pageTypes = new List<Extend.PageType>();
        }

        /// <summary>
        /// Initializes the application object.
        /// </summary>
        /// <param name="modules">The modules to use</param>
        public static void Init(IApi api, params Extend.IModule[] modules) {
            instance.Initialize(api, modules);
        }

        /// <summary>
        /// Initializes the application object.
        /// </summary>
        /// <param name="modules">The modules to use</param>
        private void Initialize(IApi api, Extend.IModule[] modules = null) {
            if (!isInitialized) {
                lock (mutex) {
                    if (!isInitialized) {
                        // Compose field types
                        fields.Register<Extend.Fields.HtmlField>();
                        fields.Register<Extend.Fields.StringField>();
                        fields.Register<Extend.Fields.TextField>();

                        // Get page types
                        pageTypes = api.PageTypes.Get();

                        // Compose & initialize modules
                        foreach (var module in modules) {
                            module.Init();
                            this.modules.Add(module);
                        }

                        isInitialized = true;
                    }
                }
            }
        }
    }
}
