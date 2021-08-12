// example script to convert LAS/LAZ file at runtime and then read it in regular viewer (as .ucpc format)

using System.Diagnostics;
using System.IO;
using unitycodercom_PointCloudBinaryViewer;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace unitycoder_examples
{
    public class RuntimeLASConvert : MonoBehaviour
    {
        public PointCloudViewerDX11 binaryViewerDX11;

        public string lasFile = "runtime.las";

        // inside streaming assets
        string commandlinePath = "PointCloudConverter173b/PointCloudConverter.exe";

        bool isConverting = false;

        void  Start()
        {
            isConverting = true;

            var sourceFile = lasFile;

            // check if full path or relative to streaming assets
            if (Path.IsPathRooted(sourceFile) == false)
            {
                sourceFile = Path.Combine(Application.streamingAssetsPath, sourceFile);
            }

            if (File.Exists(sourceFile))
            {
                Debug.Log("Converting file: " + sourceFile);
            }
            else
            {
                Debug.LogError("Input file missing: " + sourceFile);
                return;
            }

            var outputPath = Path.GetDirectoryName(sourceFile);
            outputPath = Path.Combine(outputPath, "runtime.ucpc");


            // start commandline tool with params https://github.com/unitycoder/UnityPointCloudViewer/wiki/Commandline-Tools
            var exePath = Path.Combine(Application.streamingAssetsPath, commandlinePath);
            ProcessStartInfo startInfo = new ProcessStartInfo(exePath);
            startInfo.Arguments = "-input=" + sourceFile + " -flip=true -output=" + outputPath;
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //startInfo.WindowStyle = ProcessWindowStyle.Minimized;
            var process = Process.Start(startInfo);
            process.WaitForExit();

            //Debug.Log(startInfo.Arguments);

            isConverting = false;

            // check if output exists
            if (File.Exists(outputPath))
            {
                Debug.Log("Reading output file: " + outputPath);
                binaryViewerDX11.CallReadPointCloudThreaded(outputPath);
            }
            else
            {
                Debug.LogError("File not found: " + outputPath);
            }

        }
    }
}
