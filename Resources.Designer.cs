﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CLARiNET {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CLARiNET.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;bsvc:Put_Drive_Document_Content_Request bsvc:version=&quot;v37.0&quot; xmlns:bsvc=&quot;urn:com.workday/bsvc&quot;&gt;
        ///	&lt;bsvc:Add_Only&gt;true&lt;/bsvc:Add_Only&gt;
        ///	&lt;bsvc:Drive_Document_Content_Data&gt;
        ///		&lt;bsvc:Drive_Document_Content_File_Name&gt;{filename}&lt;/bsvc:Drive_Document_Content_File_Name&gt;
        ///		&lt;bsvc:Workdrive_Item_Generic_Data&gt;
        ///			&lt;bsvc:Drive_Item_Name&gt;{filename}&lt;/bsvc:Drive_Item_Name&gt;
        ///			&lt;bsvc:Owned_By_Reference&gt;
        ///				&lt;bsvc:ID bsvc:type=&quot;WorkdayUserName&quot;&gt;{username}&lt;/bsvc:ID&gt;
        ///			&lt;/bsvc:Owned [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Put_Drive_Document_Content_Request {
            get {
                return ResourceManager.GetString("Put_Drive_Document_Content_Request", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;bsvc:Put_Drive_Document_Content_Request bsvc:version=&quot;v37.0&quot; xmlns:bsvc=&quot;urn:com.workday/bsvc&quot;&gt;
        ///	&lt;bsvc:Drive_Document_Content_Reference&gt;
        ///		&lt;bsvc:ID bsvc:type=&quot;WID&quot;&gt;{document_wid}&lt;/bsvc:ID&gt;
        ///	&lt;/bsvc:Drive_Document_Content_Reference&gt;
        ///	&lt;bsvc:Drive_Document_Content_Data&gt;
        ///		&lt;bsvc:Drive_Document_Content_File_Name&gt;{filename}&lt;/bsvc:Drive_Document_Content_File_Name&gt;
        ///		&lt;bsvc:Workdrive_Item_Generic_Data&gt;
        ///			&lt;bsvc:Drive_Item_Name&gt;{filename}&lt;/bsvc:Drive_Item_Name&gt;
        ///			&lt;bsvc [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Put_Drive_Document_Content_Trash_Request {
            get {
                return ResourceManager.GetString("Put_Drive_Document_Content_Trash_Request", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot;?&gt;
        ///&lt;envs&gt;
        ///  &lt;env name=&quot;Production (Ashburn)&quot; type=&quot;prod&quot;&gt;
        ///    &lt;e2-endpoint&gt;e2-enterprise-services1.myworkday.com&lt;/e2-endpoint&gt;
        ///    &lt;app-endpoint&gt;&lt;/app-endpoint&gt;
        ///  &lt;/env&gt;
        ///  &lt;env name=&quot;Production (Portland)&quot; type=&quot;prod&quot;&gt;
        ///    &lt;e2-endpoint&gt;wd5-e2.myworkday.com&lt;/e2-endpoint&gt;
        ///    &lt;app-endpoint&gt;&lt;/app-endpoint&gt;
        ///  &lt;/env&gt;
        ///  &lt;env name=&quot;Production (WD3)&quot; type=&quot;prod&quot;&gt;
        ///    &lt;e2-endpoint&gt;wd3-e2.myworkday.com&lt;/e2-endpoint&gt;
        ///    &lt;app-endpoint&gt;&lt;/app-endpoint&gt;
        ///  &lt;/env&gt;
        ///  &lt;env name=&quot;Production (W [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string WDEnvironments {
            get {
                return ResourceManager.GetString("WDEnvironments", resourceCulture);
            }
        }
    }
}
