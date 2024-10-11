using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Build.Reporting;

using System;
using UnityEditor.SceneManagement;
using guiEnums;
using System.IO;

namespace guiEnums{
    public enum deployTargets{
        macOS, Windows, Linux, iOS, Android, tvOS, WebGL, WindowsUWP
    }
    public enum compression{
        LZ4HC, LZ4, none, 
    }
}

public class GenBuild
{

    //UI elements
     ScrollView Scrollview;
     Button startButton;
     Toggle devBuild, IDEbuild;
     UnityEngine.UIElements.ProgressBar progress;
     UnityEngine.UIElements.EnumField targetPlatformSelector, compressionSelector;

    //class variables
    static bool canceled = true;
    string[] scenesToBake;

    const string expiration_flag = "BUILD_EXPIRES";

    //map custom enum to build system enum
    static Dictionary<deployTargets,BuildTarget> buildMapping = new Dictionary<deployTargets,BuildTarget>{
        {deployTargets.macOS, BuildTarget.StandaloneOSX},
        {deployTargets.Windows, BuildTarget.StandaloneWindows64},
        {deployTargets.WindowsUWP, BuildTarget.WSAPlayer },
        {deployTargets.Linux, BuildTarget.StandaloneLinux64},
        {deployTargets.iOS, BuildTarget.iOS},
        {deployTargets.Android, BuildTarget.Android},
        {deployTargets.tvOS, BuildTarget.tvOS},
        {deployTargets.WebGL, BuildTarget.WebGL}
    };
    //map custom enum to compression options
    static Dictionary<compression,BuildOptions> compressionMapping = new Dictionary<compression, BuildOptions>{
        {compression.LZ4HC,BuildOptions.CompressWithLz4HC},
        {compression.LZ4,BuildOptions.CompressWithLz4},
        {compression.none, BuildOptions.None}
    };

    static string[] LoadScenes(){
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;     
        string[] scenes = new string[sceneCount];
        for( int i = 0; i < sceneCount; i++ )
        {
            scenes[i] = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
        }
        return scenes;
    }


    /// <summary>
    /// Adds the appropriate extension based on the platform
    /// </summary>
    /// <param name="str">Input path str</param>
    /// <param name="platform">Platform to add extension for</param>
    /// <returns></returns>
    static string AddFileExtension(string str, deployTargets platform)
    {
        if (platform == deployTargets.Windows)
        {
            str += ".exe";
        }
        else if (platform == deployTargets.macOS)
        {
            str += ".app";
        }
        return str;
    }

    /// <summary>
    /// Generates a build from the command line.
    /// Flags:
    ///     -outDir [path]: where to write the built game
    ///     -devBuild: make a debug build   (will make release if not present)
    ///     -ideBuild: make an Xcode or Visual Studio build (will generate an executable if not present)
    ///     -platform [str]: Set the target platform. Will not proceed if not present
    ///         possible values: mac, windows, linux, ios, android, tvos, webgl
    /// </summary>
    static void CliBuild(){
        //parse arguments into dictionary
        var args = new Dictionary<string,string>();
        {
            var cliargs = System.Text.RegularExpressions.Regex.Split(String.Join(" ",System.Environment.GetCommandLineArgs()),"-");
            foreach(string arg in cliargs){
                var part = System.Text.RegularExpressions.Regex.Split(arg," ");
                if (part.Length == 1){
                    args[part[0]] = "";
                }
                else{
                    args[part[0]] = part[1];
                }
            }
        }

        //platform flag must be present
        if (!args.ContainsKey("platform")){
            Console.WriteLine("ERROR: No platform specified! Build stopping...");
            return;
        }

        //get scenes to include
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;     
        string[] includeScenes = new string[sceneCount];
        for( int i = 0; i < sceneCount; i++ )
        {
            includeScenes[i] = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
        }

        //setup build arguments
        BuildArgs buildargs = new BuildArgs();
        buildargs.isDevBuild = args.ContainsKey("devBuild");
        buildargs.isIDEbuild = args.ContainsKey("ideBuild");
        buildargs.compressionType = compression.LZ4HC;          //make this configurable?
        buildargs.scenePaths = LoadScenes();

        //set target platform
        switch (args["platform"]){
            case "mac":
                buildargs.targetPlatform = deployTargets.macOS; break;
            case "windows":
                buildargs.targetPlatform = deployTargets.Windows; break;
            case "linux":
                buildargs.targetPlatform = deployTargets.Linux; break;
            case "ios":
                buildargs.targetPlatform = deployTargets.iOS; break;
            case "android":
                buildargs.targetPlatform = deployTargets.Android; break;
            case "tvos":
                buildargs.targetPlatform = deployTargets.tvOS; break;
            case "webgl":
                buildargs.targetPlatform = deployTargets.WebGL; break;

            default:
                Console.WriteLine("ERROR: Invalid platform " + args["platform"] + ". Build stopping..."); return;
        }

        if (!args.ContainsKey("outDir")){
            Console.WriteLine("ERROR: No build output directory provided. Use the -outDir flag.");
            return;
        }

        string buildDir = $"{args["outDir"]}/{args["platform"]}/{Application.productName}";

        System.IO.Directory.CreateDirectory(buildDir);

        var keydir = args["outDir"];
        buildDir = AddFileExtension(buildDir,buildargs.targetPlatform);

        MakeBuild(buildDir,buildargs,
        () => {  
            //when all scenes finished and preparing build
            Console.WriteLine("Beginning build to " + args["outDir"]);
        }, (report) => {
            //build ended
            Console.WriteLine("Build pipeline completed with status " + report.summary.result);
        });
    }

    //the class used to pass other details about the build
    class BuildArgs{
        public bool isDevBuild, isIDEbuild;
        public deployTargets targetPlatform;
        public compression compressionType;
        public string[] scenePaths;
    };

    /// <summary>
    /// Generate a build given arguments
    /// </summary>
    /// <param name="outputDirectory">the directory to write the build</param>
    /// <param name="buildargs">the build arguments object</param>
    /// <param name="sceneCompleteHook">the method to call with an int parameter when a scene has finished building</param>
    /// <param name="BuildBeginHook">the method to call when the scene bake has finished and the game build has started</param>
    /// <param name="buildEndHook">the method to call when the build has completed</param>
    static void MakeBuild(string outputDirectory, BuildArgs buildargs, Action BuildBeginHook, Action<BuildReport> buildEndHook){
        void on_finish_build(){
            //when all scenes finish
            BuildBeginHook();

            BuildPlayerOptions opt = new BuildPlayerOptions();
            opt.scenes = buildargs.scenePaths;
            opt.locationPathName = outputDirectory;
            opt.target = buildMapping[buildargs.targetPlatform];

            //load window settings into builder
            BuildOptions args = BuildOptions.ShowBuiltPlayer | BuildOptions.StrictMode;
            if (buildargs.isDevBuild){
                args = args | BuildOptions.Development;
            }
          
            if (buildargs.isIDEbuild){
                args = args | BuildOptions.AcceptExternalModificationsToPlayer;
            }

            //compression
            args = args | compressionMapping[buildargs.compressionType];

            //set options
            opt.options = args;

            BuildReport report = BuildPipeline.BuildPlayer(opt);
            if (report.summary.result == BuildResult.Succeeded ){
                Debug.Log("Build succeeded, wrote executable to " + outputDirectory);
            }
            else if (report.summary.result == BuildResult.Failed){
                Debug.LogError("Build failed!");
            }
            else{
                Debug.LogWarning("Build finished with unknown exit status");
            }
            buildEndHook(report);
        }

		on_finish_build();
    }
}
