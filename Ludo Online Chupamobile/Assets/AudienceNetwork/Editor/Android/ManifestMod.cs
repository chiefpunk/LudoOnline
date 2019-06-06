/**
 * Copyright (c) 2014-present, Facebook, Inc. All rights reserved.
 *
 * You are hereby granted a non-exclusive, worldwide, royalty-free license to use,
 * copy, modify, and distribute this software in source code or binary form for use
 * in connection with the web services and APIs provided by Facebook.
 *
 * As with any software that integrates with the Facebook platform, your use of
 * this software is subject to the Facebook Developer Principles and Policies
 * [http://developers.facebook.com/policy/]. This copyright notice shall be
 * included in all copies or substantial portions of the software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace AudienceNetwork.Editor
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using AudienceNetwork;
    using UnityEditor;
    using UnityEngine;

    public class ManifestMod
    {
        public static string AudienceNetworkPath = Path.Combine(Application.dataPath, "AudienceNetwork/");
        public static string AudienceNetworkPluginsPath = Path.Combine(AudienceNetworkPath, "Plugins/");

        public static string InterstitialActivityName = "com.facebook.ads.AudienceNetworkActivity";
        public static string AndroidPluginPath = Path.Combine(AudienceNetworkPluginsPath, "Android/");
        public static string AndroidManifestName = "AndroidManifest.xml";
        public static string AndroidManifestPath = Path.Combine(AndroidPluginPath, AndroidManifestName);
        public static string FacebookDefaultAndroidManifestPath = Path.Combine(Application.dataPath, "AudienceNetwork/Editor/Android/DefaultAndroidManifest.xml");

        public static void GenerateManifest ()
        {
            var outputFile = ManifestMod.AndroidManifestPath;
            // Create containing directory if it does not exist
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

            // only copy over a fresh copy of the AndroidManifest if one does not exist
            if (!File.Exists (outputFile)) {
                ManifestMod.CreateDefaultAndroidManifest(outputFile);
            }

            UpdateManifest (outputFile);
        }

        public static bool CheckManifest()
        {
            bool result = true;
            var outputFile = ManifestMod.AndroidManifestPath;
            if (!File.Exists(outputFile))
            {
                Debug.LogError("An android manifest must be generated for the Audience Network SDK to work.  " +
                    "Go to Tools->Audience Network and press \"Regenerate Android Manifest\"");
                return false;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(outputFile);

            if (doc == null)
            {
                Debug.LogError("Couldn't load " + outputFile);
                return false;
            }

            XmlNode manNode = FindChildNode(doc, "manifest");
            XmlNode dict = FindChildNode(manNode, "application");

            if (dict == null)
            {
                Debug.LogError("Error parsing " + outputFile);
                return false;
            }

            XmlElement loginElement;
            if (!ManifestMod.TryFindElementWithAndroidName(dict, InterstitialActivityName, out loginElement))
            {
                Debug.LogError(string.Format("{0} is missing from your android manifest.  " +
                    "Go to Tools->Audience Network and press \"Regenerate Android Manifest\"", InterstitialActivityName));
                result = false;
            }

            return result;
        }

        public static void UpdateManifest (string fullPath)
        {
            XmlDocument doc = new XmlDocument ();
            doc.Load (fullPath);

            if (doc == null) {
                Debug.LogError ("Couldn't load " + fullPath);
                return;
            }

            XmlNode manNode = FindChildNode (doc, "manifest");

            XmlNode dict = FindChildNode (manNode, "application");

            if (dict == null) {
                Debug.LogError ("Error parsing " + fullPath);
                return;
            }

            string ns = dict.GetNamespaceOfPrefix ("android");

            ManifestMod.AddPermission (doc, manNode, ns, "android.permission.INTERNET");
            ManifestMod.AddPermission (doc, manNode, ns, "android.permission.ACCESS_NETWORK_STATE");

            var configOptions = new Dictionary<string, string> ();
            configOptions.Add ("configChanges", "keyboardHidden|orientation|screenSize");
            ManifestMod.AddSimpleActivity (doc, dict, ns, InterstitialActivityName, configOptions);

            // Save the document formatted
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };

            using (XmlWriter xmlWriter = XmlWriter.Create(fullPath, settings)) {
                doc.Save (xmlWriter);
            }
        }

        private static XmlNode FindChildNode (XmlNode parent, string name)
        {
            XmlNode curr = parent.FirstChild;
            while (curr != null) {
                if (curr.Name.Equals (name)) {
                    return curr;
                }

                curr = curr.NextSibling;
            }

            return null;
        }

        private static void SetOrReplaceXmlElement (
            XmlNode parent,
            XmlElement newElement)
        {
            string attrNameValue = newElement.GetAttribute ("name");
            string elementType = newElement.Name;

            XmlElement existingElment;
            if (TryFindElementWithAndroidName (parent, attrNameValue, out existingElment, elementType)) {
                parent.ReplaceChild (newElement, existingElment);
            } else {
                parent.AppendChild (newElement);
            }
        }

        private static bool TryFindElementWithAndroidName (
            XmlNode parent,
            string attrNameValue,
            out XmlElement element,
            string elementType = "activity")
        {
            string ns = parent.GetNamespaceOfPrefix ("android");
            var curr = parent.FirstChild;
            while (curr != null) {
                var currXmlElement = curr as XmlElement;
                if (currXmlElement != null &&
                    currXmlElement.Name == elementType &&
                    currXmlElement.GetAttribute ("name", ns) == attrNameValue) {
                    element = currXmlElement;
                    return true;
                }

                curr = curr.NextSibling;
            }

            element = null;
            return false;
        }

        private static void AddSimpleActivity (XmlDocument doc,
                                               XmlNode xmlNode,
                                               string ns,
                                               string className,
                                               Dictionary<string, string> customOptions = null,
                                               bool export = false)
        {
            XmlElement element = CreateActivityElement (doc, ns, className, customOptions, export);
            ManifestMod.SetOrReplaceXmlElement (xmlNode, element);
        }

        private static XmlElement CreateActivityElement (XmlDocument doc,
                                                         string ns,
                                                         string activityName,
                                                         Dictionary<string, string> customOptions = null,
                                                         bool exported = false)
        {
            // <activity android:name="activityName" android:exported="true">
            // </activity>
            XmlElement activityElement = doc.CreateElement ("activity");
            activityElement.SetAttribute ("name", ns, activityName);
            if (exported) {
                activityElement.SetAttribute ("exported", ns, "true");
            }

            if (customOptions != null) {
                foreach (var key in customOptions.Keys) {
                    var value = customOptions [key];
                    activityElement.SetAttribute (key, ns, value);
                }
            }

            return activityElement;
        }

        private static void AddPermission (XmlDocument doc,
                                           XmlNode xmlNode,
                                           string ns,
                                           string permissionName)
        {
            XmlElement element = CreatePermissionElement (doc, ns, permissionName);
            ManifestMod.SetOrReplaceXmlElement (xmlNode, element);
        }

        private static XmlElement CreatePermissionElement (XmlDocument doc,
                                                           string ns,
                                                           string permissionName)
        {
            // <uses-permission android:name="android.permission.INTERNET" />
            // <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
            XmlElement activityElement = doc.CreateElement ("uses-permission");
            activityElement.SetAttribute ("name", ns, permissionName);

            return activityElement;
        }

        private static void CreateDefaultAndroidManifest(string outputFile)
        {
            var inputFile = Path.Combine(
                EditorApplication.applicationContentsPath,
                "PlaybackEngines/androidplayer/AndroidManifest.xml");
            if (!File.Exists(inputFile))
            {
                // Unity moved this file. Try to get it at its new location
                inputFile = Path.Combine(
                    EditorApplication.applicationContentsPath,
                    "PlaybackEngines/AndroidPlayer/Apk/AndroidManifest.xml");

                if (!File.Exists(inputFile))
                {
                    // On Unity 5.3+ we don't have default manifest so use our own
                    // manifest and warn the user that they may need to modify it manually
                    inputFile = FacebookDefaultAndroidManifestPath;
                    Debug.LogWarning(
                        string.Format(
                        "No existing android manifest found at '{0}'. Creating a default manifest file",
                        outputFile));
                }
            }

            File.Copy(inputFile, outputFile);
        }

    }
}
