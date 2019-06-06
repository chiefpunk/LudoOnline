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
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using AudienceNetwork;

    public class AudienceNetworkSettingsEditor : UnityEditor.Editor
    {
        private static string title = "Audience Network SDK";

        [MenuItem("Tools/Audience Network/About")]
        private static void AboutGUI ()
        {
            string aboutString = System.String.Format ("Facebook Audience Network Unity SDK Version {0}",
                                               AudienceNetwork.SdkVersion.Build);
            EditorUtility.DisplayDialog (title,
                                         aboutString,
                                         "Okay");
        }

        [MenuItem("Tools/Audience Network/Regenerate Android Manifest")]
        private static void RegenerateManifest ()
        {
            bool updateManifest = EditorUtility.DisplayDialog (title,
                                                               "Are you sure you want to regenerate your Android Manifest.xml?",
                                                               "Okay",
                                                               "Cancel");

            if (updateManifest) {
                AudienceNetwork.Editor.ManifestMod.GenerateManifest ();
                EditorUtility.DisplayDialog (title, "Android Manifest updated. \n \n If interstitial ads still throw ActivityNotFoundException, " +
                    "you may need to copy the generated manifest at " + ManifestMod.AndroidManifestPath + " to /Assets/Plugins/Android.", "Okay");
            }
        }

        [MenuItem("Tools/Audience Network/Build SDK Package")]
        private static void BuildGUI ()
        {
            try {
                string exportedPath = AudienceNetworkBuild.ExportPackage ();
                EditorUtility.DisplayDialog (title, "Exported to " + exportedPath, "Okay");

            } catch (System.Exception e) {
                EditorUtility.DisplayDialog (title, e.Message, "Okay");
            }
        }
    }
}
