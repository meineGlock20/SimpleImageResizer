﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SimpleImageResizer.Localize {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class MainWindow {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MainWindow() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SimpleImageResizer.Localize.MainWindow", typeof(MainWindow).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified directory does not exist.
        ///However, it will be created when images are resized..
        /// </summary>
        internal static string ButtonDestinationNotFoundMessage {
            get {
                return ResourceManager.GetString("ButtonDestinationNotFoundMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Directory Does Not Exist.
        /// </summary>
        internal static string ButtonDestinationNotFoundTitle {
            get {
                return ResourceManager.GetString("ButtonDestinationNotFoundTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select the destination directory for your resized images..
        /// </summary>
        internal static string DestinationDialogTitle {
            get {
                return ResourceManager.GetString("DestinationDialogTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All Supported Images | *.bmp; *.gif; *.jfif; *.jpg; *.png; *.tif&quot;.
        /// </summary>
        internal static string SelectImagesSupportedTypes {
            get {
                return ResourceManager.GetString("SelectImagesSupportedTypes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Large.
        /// </summary>
        internal static string SimpleImageResizeLarge {
            get {
                return ResourceManager.GetString("SimpleImageResizeLarge", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Medium.
        /// </summary>
        internal static string SimpleImageResizeMedium {
            get {
                return ResourceManager.GetString("SimpleImageResizeMedium", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Small.
        /// </summary>
        internal static string SimpleImageResizeSmall {
            get {
                return ResourceManager.GetString("SimpleImageResizeSmall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thumbnail.
        /// </summary>
        internal static string SimpleImageResizeThumbnail {
            get {
                return ResourceManager.GetString("SimpleImageResizeThumbnail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown.
        /// </summary>
        internal static string SimpleImageResizeUnknown {
            get {
                return ResourceManager.GetString("SimpleImageResizeUnknown", resourceCulture);
            }
        }
    }
}
