# Polo Setup

#### 1) Install VS Extentions

Open Visual Studio Installer application.
Click modify on the most resent Visual Studio installation.
Install Azure development and Game Development with Unity

#### 2) Get the latest nuget packages of libraries:

Open the sample_unity_bodytracking project in Unity.
Either:
Open the Visual Studio Solution associated with this project.
If there is no Visual Studio Solution yet you can make one by opening the Unity Editor
and selecting one of the csharp files in the project and opening it for editing.
Or:
In the project tab under assets->scripts double click a file to open it in Visual Studio
If it does not open in Visual Studio go to Edit -> Preferences -> External Tools -> External Script Editor and click Visual Studio in the dropdown.
If there is no Visual Studio Solution yet you can make one by opening the Unity Editor
and selecting one of the csharp files in the project and opening it for editing.

In Visual Studio:
Select Tools->NuGet Package Manager-> Package Manager Console

On the command line of the console at type the following command:

`Install-Package Microsoft.Azure.Kinect.BodyTracking`

#### 3) Next Move the files:

**run the batch file MoveLibraryFile.bat** in the sample_unity_bodytracking

#### 4) Open the Unity Project and under Scenes/  select the Kinect4AzureSampleScene:

Connect the Azure Kinect Cameras to the Computer, 
Press play.

If you find `catching exception for background thread result = K4A_RESULT_FAILED` error in the console it means the kinects are not connected to the computer properly. Press play again and re-seat the kinect cables, then press play again.

### Finally if you Build a Standalone Executable:

You will need to put [required DLLs for ONNX Runtime execution](https://docs.microsoft.com/en-us/azure/kinect-dk/body-sdk-setup#required-dlls-for-onnx-runtime-execution-environments) in the same directory with the .exe:

You can copy ONNXRuntime and DirectML files from nuget package by hand or from sample_unity_bodytracking directory after running **the batch file MoveLibraryFile.bat** (Step #3)