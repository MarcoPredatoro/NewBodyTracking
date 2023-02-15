// // Copyright (c) Microsoft Corporation. All rights reserved.
// // Licensed under the MIT License.
// using Microsoft.Azure.Kinect.BodyTracking;
// using Microsoft.Azure.Kinect.Sensor;
// using System;
// using System.Diagnostics;
// using System.IO;
// using System.Runtime.InteropServices;
// using System.Threading;
// using OpenCvSharp;


// public class MultiDeviceCapturer
// {
//     // Once the constuctor finishes, devices[0] will always be the master
//     Device master_device;
//     Device subordinate_device;
//     // Set up all the devices. Note that the index order isn't necessarily preserved, because we might swap with master
//     public MultiDeviceCapturer(List<UInt32> device_indices, UInt32 color_exposure_usec, UInt32 powerline_freq)
//     {
//         bool master_found = false;
//         bool subordinate_found = false;
//         if (device_indices.size() == 0)
//         {
//             Debug.Log("Not enough Cameras");
//         }
//         foreach (int i in device_indices)
//         {
//             Device next_device = Device.open(i); // construct a device using this index
//             // If you want to synchronize cameras, you need to manually set both their exposures
//             next_device.setColorControl(ColorControlCommand.ExposureTimeAbsolute,
//                                           ColorControlMode.Manual,
//                                           color_exposure_usec);
//             // This setting compensates for the flicker of lights due to the frequency of AC power in your region. If
//             // you are in an area with 50 Hz power, this may need to be updated (check the docs for
//             // k4a_color_control_command_t)
//             next_device.setColorControl(ColourControlCommand.PowerlineFrequenct,
//                                           ColorControlMode.Manual,
//                                           powerline_freq);
//             // We treat the first device found with a sync out cable attached as the master. If it's not supposed to be,
//             // unplug the cable from it. Also, if there's only one device, just use it
//             if ((next_device.SyncInJackConnected && !master_found) || device_indices.size() == 1)
//             {
//                 master_device = next_device;
//                 master_found = true;
//             }
//             else if (!next_device.SyncInJackConnected && !next_device.SyncOutJackConnected)
//             {
//                 Debug.log("Sync in and out aren't connected");
//             }
//             else if (!next_device.SyncInJackConnected)
//             {
//                 Debug.log("Sync In cable not connected");
//             }
//             else
//             {
//                 subordinate_devices = next_device;
//                 subordinate_found = false;
//             }
//         }
//         if (!master_found || !subordinate_found)
//         {
//             Debug.log("Master or Subordinate not found");
//         }
//     }

//     // configs[0] should be the master, the rest subordinate
//     public void start_devices(DeviceConfiguration master_config, DeviceConfiguration sub_config)
//     {
//         // Start by starting all of the subordinate devices. They must be started before the master!
//         subordinate_device.StartCameras(sub_config);
//         // Lastly, start the master device
//         master_device.StartCameras(master_config);
//     }

//     // Blocks until we have synchronized captures stored in the output. First is master, rest are subordinates
//     List<Capture> get_synchronized_captures(DeviceConfiguration sub_config, bool compare_sub_depth_instead_of_color = false)
//     {
//         // Dealing with the synchronized cameras is complex. The Azure Kinect DK:
//         //      (a) does not guarantee exactly equal timestamps between depth and color or between cameras (delays can
//         //      be configured but timestamps will only be approximately the same)
//         //      (b) does not guarantee that, if the two most recent images were synchronized, that calling get_capture
//         //      just once on each camera will still be synchronized.
//         // There are several reasons for all of this. Internally, devices keep a queue of a few of the captured images
//         // and serve those images as requested by get_capture(). However, images can also be dropped at any moment, and
//         // one device may have more images ready than another device at a given moment, et cetera.
//         //
//         // Also, the process of synchronizing is complex. The cameras are not guaranteed to exactly match in all of
//         // their timestamps when synchronized (though they should be very close). All delays are relative to the master
//         // camera's color camera. To deal with these complexities, we employ a fairly straightforward algorithm. Start
//         // by reading in two captures, then if the camera images were not taken at roughly the same time read a new one
//         // from the device that had the older capture until the timestamps roughly match.

//         // The captures used in the loop are outside of it so that they can persist across loop iterations. This is
//         // necessary because each time this loop runs we'll only update the older capture.
//         // The captures are stored in a vector where the first element of the vector is the master capture and
//         // subsequent elements are subordinate captures
//         List<Capture> captures = new List<Capture>(2); //size 2?

//         //takes TimeSpan - work out what this is equiv.
//         captures[0] = master_device.GetCapture();
//         captures[1] = subordinate_device.GetCapture();

//         bool have_synced_images = false;
//         var start = new Stopwatch();
//         start.Start();
//         while (!have_synced_images)
//         {
//             // Timeout if this is taking too long
//             int duration_ms = start.Elapsed();
//             if (duration_ms > WAIT_FOR_SYNCHRONIZED_CAPTURE_TIMEOUT)
//             {
//                 Debug.Log("To long to connect");
//             }

//             Image master_color_image = captures[0].Image;
//             var master_color_image_time = master_color_image.DeviceTimestamp;

//             Image sub_image;
//             if (compare_sub_depth_instead_of_color)
//             {
//                 sub_image = captures[1].Depth; // offset of 1 because master capture is at front
//             }
//             else
//             {
//                 sub_image = captures[1].Image; // offset of 1 because master capture is at front
//             }

//             if (master_color_image && sub_image)
//             {
//                 var sub_image_time = sub_image.DeviceTimestamp;
//                 // The subordinate's color image timestamp, ideally, is the master's color image timestamp plus the
//                 // delay we configured between the master device color camera and subordinate device color camera
//                 var expected_sub_image_time = master_color_image_time + sub_config.SubordinateDelayOffMaster + sub_config.DepthDelayOffColour;
//                 var sub_image_time_error = sub_image_time - expected_sub_image_time;
//                 // The time error's absolute value must be within the permissible range. So, for example, if
//                 // MAX_ALLOWABLE_TIME_OFFSET_ERROR_FOR_IMAGE_TIMESTAMP is 2, offsets of -2, -1, 0, 1, and -2 are
//                 // permitted
//                 if (sub_image_time_error < -MAX_ALLOWABLE_TIME_OFFSET_ERROR_FOR_IMAGE_TIMESTAMP)
//                 {
//                     // Example, where MAX_ALLOWABLE_TIME_OFFSET_ERROR_FOR_IMAGE_TIMESTAMP is 1
//                     // time                    t=1  t=2  t=3
//                     // actual timestamp        x    .    .
//                     // expected timestamp      .    .    x
//                     // error: 1 - 3 = -2, which is less than the worst-case-allowable offset of -1
//                     // the subordinate camera image timestamp was earlier than it is allowed to be. This means the
//                     // subordinate is lagging and we need to update the subordinate to get the subordinate caught up
//                     Debug.Log("sub", captures[0], captures[1]);
//                     //We only have 1 subord so this unnecessary? What are we trying to do here
//                     captures[1] = subordinate_device.GetCapture();
//                     break;
//                 }
//                 else if (sub_image_time_error > MAX_ALLOWABLE_TIME_OFFSET_ERROR_FOR_IMAGE_TIMESTAMP)
//                 {
//                     // Example, where MAX_ALLOWABLE_TIME_OFFSET_ERROR_FOR_IMAGE_TIMESTAMP is 1
//                     // time                    t=1  t=2  t=3
//                     // actual timestamp        .    .    x
//                     // expected timestamp      x    .    .
//                     // error: 3 - 1 = 2, which is more than the worst-case-allowable offset of 1
//                     // the subordinate camera image timestamp was later than it is allowed to be. This means the
//                     // subordinate is ahead and we need to update the master to get the master caught up
//                     Debug.Log("master", captures[0], captures[1]);
//                     captures[0] = master_device.GetCapture();
//                     break;
//                 }
//                 else
//                 {
//                     have_synced_images = true;
//                 }
//             }
//             else if (!master_color_image)
//             {
//                 Console.WriteLine("Master image was bad!\n");
//                 captures[0] = master_device.GetCapture();
//                 break;
//             }
//             else if (!sub_image)
//             {
//                 Console.WriteLine("Subordinate image was bad!\n");
//                 captures[1] = subordinate_device.GetCapture();
//                 break;
//             }
//         }
//         // if we've made it to here, it means that we have synchronized captures.
//         return captures;
//     }

//     Device get_master_device()
//     {
//         return master_device;
//     }

//     Device get_subordinate_device_by_index()
//     {
//         return subordinate_device;
//     }


// }