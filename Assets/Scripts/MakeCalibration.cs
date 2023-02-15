// // Copyright (c) Microsoft Corporation. All rights reserved.
// // Licensed under the MIT License.
// using UnityEngine;
// using Microsoft.Azure.Kinect.BodyTracking;
// using Microsoft.Azure.Kinect.Sensor;
// using System;
// using System.IO;
// using System.Diagnostics;
// using System.Runtime.InteropServices;
// using System.Threading;
// using OpenCvSharp;

// using System.Linq;

// // Allowing at least 160 microseconds between depth cameras should ensure they do not interfere with one another.
// const uint32_t MIN_TIME_BETWEEN_DEPTH_CAMERA_PICTURES_USEC = 160;

// public class MakeCalibration
// {
//     public MakeCalibration() { }

//     public int calibrate()
//     {
//         float chessboard_square_length = 0; // must be included in the input params
//         Int32 color_exposure_usec = 8000;  // somewhat reasonable default exposure time
//         Int32 powerline_freq = 2;          // default to a 60 Hz powerline
//         Size chessboard_pattern = (0, 0);   // height, width. Both need to be set.
//         UInt16 depth_threshold = 1000;     // default to 1 meter
//         UInt32 num_devices = 0;
//         double calibration_timeout = 60.0; // default to timing out after 60s of trying to get calibrated

//         List<UInt32> device_indices = new List<UInt32> { 0 }; // Set up a MultiDeviceCapturer to handle getting many synchronous captures
//                                                               // Note that the order of indices in device_indices is not necessarily
//                                                               // preserved because MultiDeviceCapturer tries to find the master device based
//                                                               // on which one has sync out plugged in. Start with just { 0 }, and add
//                                                               // another if needed



//         MultiDeviceCapturer capturer = new MultiDeviceCapturer(device_indices, color_exposure_usec, powerline_freq);

//         // Create configurations for devices
//         var main_config = new DeviceConfiguration()
//         {
//             CameraFPS = FPS.FPS15,
//             ColorFormat = ImageFormat.ColorBGRA32,
//             ColorResolution = ColorResolution.R720p,
//             DepthMode = DepthMode.NFOV_Unbinned,
//             SynchronizedImagesOnly = true,
//             WiredSyncMode = WiredSyncMode.Master,
//         };
//         var secondary_config = new DeviceConfiguration()
//         {
//             CameraFPS = FPS.FPS15,
//             ColorResolution = ColorResolution.R720p,
//             // SubordinateDelayOffMaster = TimeSpan.
//             ColorFormat = ImageFormat.ColorBGRA32,
//             DepthMode = DepthMode.NFOV_Unbinned,
//             SynchronizedImagesOnly = true,
//             WiredSyncMode = WiredSyncMode.Subordinate,
//         };

//         // Construct all the things that we'll need whether or not we are running with 1 or 2 cameras
//         Calibration main_calibration = capturer.get_master_device().GetCalibration(main_config.DepthMode,
//                                                                                         main_config.ColorResolution);

//         // Set up a transformation. DO THIS OUTSIDE OF YOUR MAIN LOOP! Constructing transformations involves time-intensive
//         // hardware setup and should not change once you have a rigid setup, so only call it once or it will run very
//         // slowly.
//         Transformation main_depth_to_main_color = new Transformation(main_calibration);

//         capturer.start_devices(main_config, secondary_config);
//         // get an image to be the background
//         List<Capture> background_captures = capturer.get_synchronized_captures(secondary_config);
//         Mat background_image = color_to_opencv(background_captures[0].Color());
//         Mat output_image = background_image.clone(); // allocated outside the loop to avoid re-creating every time

//         if (num_devices == 1)
//         {
//             UnityEngine.Debug.Log("ONLY 1 DEVICE");
//         }
//         else if (num_devices == 2)
//         {
//             // This wraps all the device-to-device details
//             TranslationAndRotation tr_secondary_color_to_main_color = calibrate_devices(capturer,
//                                                                                 main_config,
//                                                                                 secondary_config,
//                                                                                 chessboard_pattern,
//                                                                                 chessboard_square_length,
//                                                                                 calibration_timeout);

//             Calibration secondary_calibration =
//                 capturer.get_subordinate_device_by_index().GetCalibration(secondary_config.DepthMode,
//                                                                             secondary_config.ColorResolution);
//             // Get the transformation from secondary depth to secondary color using its calibration object
//             TranslationAndRotation tr_secondary_depth_to_secondary_color = get_depth_to_color_transformation_from_calibration(
//                 secondary_calibration);

//             // We now have the secondary depth to secondary color transform. We also have the transformation from the
//             // secondary color perspective to the main color perspective from the calibration earlier. Now let's compose the
//             // depth secondary -> color secondary, color secondary -> color main into depth secondary -> color main
//             TranslationAndRotation tr_secondary_depth_to_main_color = tr_secondary_depth_to_secondary_color.compose_with(
//                 tr_secondary_color_to_main_color); //WHAT IS COMPOSEWITH in C#

//             // Construct a new calibration object to transform from the secondary depth camera to the main color camera
//             Calibration secondary_depth_to_main_color_cal =
//                 construct_device_to_device_calibration(main_calibration,
//                                                     secondary_calibration,
//                                                     tr_secondary_depth_to_main_color);
//             Transformation secondary_depth_to_main_color = new Transformation(secondary_depth_to_main_color_cal);

//             var start_time = new Stopwatch();
//             start_time.Start();
//             while (start_time.Elapsed() <
//                 greenscreen_duration)
//             {
//                 List<Capture> captures;
//                 captures = capturer.get_synchronized_captures(secondary_config, true);
//                 Image main_color_image = captures[0].get_color_image();
//                 Image main_depth_image = captures[0].get_depth_image();

//                 // let's green screen out things that are far away.
//                 // first: let's get the main depth image into the color camera space
//                 Image main_depth_in_main_color = create_depth_image_like(main_color_image);
//                 main_depth_to_main_color.DepthImageToColorCamera(main_depth_image, main_depth_in_main_color);
//                 var cv_main_depth_in_main_color = depth_to_opencv(main_depth_in_main_color);
//                 var cv_main_color_image = color_to_opencv(main_color_image);

//                 Image secondary_depth_image = captures[1].get_depth_image();

//                 // Get the depth image in the main color perspective
//                 Image secondary_depth_in_main_color = create_depth_image_like(main_color_image);
//                 secondary_depth_to_main_color.DepthImageToColorCamera(secondary_depth_image,
//                                                                         secondary_depth_in_main_color);
//                 var cv_secondary_depth_in_main_color = depth_to_opencv(secondary_depth_in_main_color);

//                 // Now it's time to actually construct the green screen. Where the depth is 0, the camera doesn't know how
//                 // far away the object is because it didn't get a response at that point. That's where we'll try to fill in
//                 // the gaps with the other camera.
//                 var main_valid_mask = cv_main_depth_in_main_color != 0;
//                 var secondary_valid_mask = cv_secondary_depth_in_main_color != 0;
//                 // build depth mask. If the main camera depth for a pixel is valid and the depth is within the threshold,
//                 // then set the mask to display that pixel. If the main camera depth for a pixel is invalid but the
//                 // secondary depth for a pixel is valid and within the threshold, then set the mask to display that pixel.
//                 var within_threshold_range = (main_valid_mask & (cv_main_depth_in_main_color < depth_threshold)) |
//                                                 (main_valid_mask & secondary_valid_mask &
//                                                 (cv_secondary_depth_in_main_color < depth_threshold));
//                 // copy main color image to output image only where the mask within_threshold_range is true
//                 cv_main_color_image.CopyTo(output_image, within_threshold_range);
//                 // fill the rest with the background image
//                 background_image.CopyTo(output_image, within_threshold_range);

//                 Cv2.ImShow("Green Screen", output_image);
//                 Cv2.WaitKey(1);
//             }
//         }
//         else
//         {
//             UnityEngine.Debug.Log("Invalid number of devices!");
//         }
//         return 0;
//     }

//     static Mat color_to_opencv(Image im)
//     {
//         Mat cv_image_with_alpha = new Mat(im.HeightPixels, im.WidthPixels, CV_8UC4, im.get_buffer());
//         Mat cv_image_no_alpha;
//         CvtColor(cv_image_with_alpha, cv_image_no_alpha, COLOR_BGRA2BGR);
//         return cv_image_no_alpha;
//     }

//     static Mat depth_to_opencv(Image im)
//     {
//         return new Mat(im.HeightPixels(),
//                     im.WidthPixels(),
//                     CV_16U,
//                     im.get_buffer(),
//                     im.get_stride_bytes());
//     }

//     static Mat calibration_to_color_camera_matrix(Calibration cal)
//     {
//         var i = cal.ColorCameraCalibration.Intrinsics.Parameters;
//         Mat camera_matrix = Mat.Eye(3, MatchType.CV_32F);
//         camera_matrix(0, 0) = i.fx;
//         camera_matrix(1, 1) = i.fy;
//         camera_matrix(0, 2) = i.cx;
//         camera_matrix(1, 2) = i.cy;
//         return camera_matrix;
//     }

//     static TranslationAndRotation get_depth_to_color_transformation_from_calibration(Calibration cal)
//     {
//         Extrinsics ex = cal.Extrinsics[K4A_CALIBRATION_TYPE_DEPTH][K4A_CALIBRATION_TYPE_COLOR];
//         TranslationAndRotation tr;
//         for (int i = 0; i < 3; ++i)
//         {
//             for (int j = 0; j < 3; ++j)
//             { //check tr.R in c# - cant clearly find
//                 tr.R(i, j) = ex.Rotation[i * 3 + j];
//             }
//         }
//         tr.t = Vec3d(ex.Translation[0], ex.Translation[1], ex.Translation[2]);
//         return tr;
//     }

//     // This function constructs a calibration that operates as a transformation between the secondary device's depth camera
//     // and the main camera's color camera. IT WILL NOT GENERALIZE TO OTHER TRANSFORMS. Under the hood, the transformation
//     // depth_image_to_color_camera method can be thought of as converting each depth pixel to a 3d point using the
//     // intrinsics of the depth camera, then using the calibration's extrinsics to convert between depth and color, then
//     // using the color intrinsics to produce the point in the color camera perspective.
//     static Calibration construct_device_to_device_calibration(Calibration main_cal,
//                                                                     Calibration secondary_cal,
//                                                                     TranslationAndRotation secondary_to_main)
//     {
//         Calibration cal = secondary_cal;
//         //what does this indexing mean
//         Extrinsics ex = cal.Extrinsics[K4A_CALIBRATION_TYPE_DEPTH][K4A_CALIBRATION_TYPE_COLOR];
//         for (int i = 0; i < 3; ++i)
//         {
//             for (int j = 0; j < 3; ++j)
//             {
//                 ex.Rotation[i * 3 + j] = (float)(secondary_to_main.R(i, j));
//             }
//         }
//         for (int i = 0; i < 3; ++i)
//         {
//             ex.Translation[i] = (float)(secondary_to_main.t[i]);
//         }
//         cal.ColorCameraCalibration = main_cal.ColorCameraCalibration;
//         return cal;
//     }

//     static List<float> calibration_to_color_camera_dist_coeffs(Calibration cal)
//     {
//         Intrinsics.Parameters i = cal.ColorCameraCalibration.Intrinsics.Parameters;
//         float[] l = { i.k1, i.k2, i.p1, i.p2, i.k3, i.k4, i.k5, i.k6 };
//         List<float> param = new List<float>(l);
//         return param;
//     }

//     bool find_chessboard_corners_helper(Mat main_color_image,
//                                         Mat secondary_color_image,
//                                         Size chessboard_pattern,
//                                         List<Point2f> main_chessboard_corners,
//                                         List<Point2f> secondary_chessboard_corners)
//     {
//         bool found_chessboard_main = Cv2.FindChessboardCorners(main_color_image,
//                                                             chessboard_pattern,
//                                                             main_chessboard_corners);
//         bool found_chessboard_secondary = Cv2.FindChessboardCorners(secondary_color_image,
//                                                                     chessboard_pattern,
//                                                                     secondary_chessboard_corners);

//         // Cover the failure cases where chessboards were not found in one or both images.
//         if (!found_chessboard_main || !found_chessboard_secondary)
//         {
//             if (found_chessboard_main)
//             {
//                 Console.WriteLine("Could not find the chessboard corners in the secondary image. Trying again...\n");
//             }
//             // Likewise, if the chessboard was found in the secondary image, it was not found in the main image.
//             else if (found_chessboard_secondary)
//             {
//                 Console.WriteLine("Could not find the chessboard corners in the main image. Trying again...\n");
//             }
//             // The only remaining case is the corners were in neither image.
//             else
//             {
//                 Console.WriteLine("Could not find the chessboard corners in either image. Trying again...\n");
//             }
//             return false;
//         }
//         // Before we go on, there's a quick problem with calibration to address.  Because the chessboard looks the same when
//         // rotated 180 degrees, it is possible that the chessboard corner finder may find the correct points, but in the
//         // wrong order.

//         // A visual:
//         //        Image 1                  Image 2
//         // .....................    .....................
//         // .....................    .....................
//         // .........xxxxx2......    .....xxxxx1..........
//         // .........xxxxxx......    .....xxxxxx..........
//         // .........xxxxxx......    .....xxxxxx..........
//         // .........1xxxxx......    .....2xxxxx..........
//         // .....................    .....................
//         // .....................    .....................

//         // The problem occurs when this case happens: the find_chessboard() function correctly identifies the points on the
//         // chessboard (shown as 'x's) but the order of those points differs between images taken by the two cameras.
//         // Specifically, the first point in the list of points found for the first image (1) is the *last* point in the list
//         // of points found for the second image (2), though they correspond to the same physical point on the chessboard.

//         // To avoid this problem, we can make the assumption that both of the cameras will be oriented in a similar manner
//         // (e.g. turning one of the cameras upside down will break this assumption) and enforce that the vector between the
//         // first and last points found in pixel space (which will be at opposite ends of the chessboard) are pointing the
//         // same direction- so, the dot product of the two vectors is positive.

//         //  WHAT ARE .BACK AND .FRONT
//         Vec2f main_image_corners_vec = main_chessboard_corners.Last() - main_chessboard_corners.First();
//         Vec2f secondary_image_corners_vec = secondary_chessboard_corners.Last() - secondary_chessboard_corners.First();
//         if (main_image_corners_vec.dot(secondary_image_corners_vec) <= 0.0)
//         {
//             secondary_chessboard_corners = secondary_chessboard_corners.Reverse();
//         }
//         return true;
//     }

//     TranslationAndRotation stereo_calibration(Calibration main_calib,
//                                     Calibration secondary_calib,
//                                     List<List<Point2f>> main_chessboard_corners_list,
//                                     List<List<Point2f>> secondary_chessboard_corners_list,
//                                     Size image_size,
//                                     Size chessboard_pattern,
//                                     float chessboard_square_length)
//     {
//         // We have points in each image that correspond to the corners that the findChessboardCorners function found.
//         // However, we still need the points in 3 dimensions that these points correspond to. Because we are ultimately only
//         // interested in find a transformation between two cameras, these points don't have to correspond to an external
//         // "origin" point. The only important thing is that the relative distances between points are accurate. As a result,
//         // we can simply make the first corresponding point (0, 0) and construct the remaining points based on that one. The
//         // order of points inserted into the vector here matches the ordering of findChessboardCorners. The units of these
//         // points are in millimeters, mostly because the depth provided by the depth cameras is also provided in
//         // millimeters, which makes for easy comparison.
//         List<Point3f> chessboard_corners_world;
//         for (int h = 0; h < chessboard_pattern.height; ++h)
//         {
//             for (int w = 0; w < chessboard_pattern.width; ++w)
//             {
//                 Point3f p = new Point3f { w * chessboard_square_length, h * chessboard_square_length, 0.0 };
//                 chessboard_corners_world.Add(p);
//             }
//         }

//         // Calibrating the cameras requires a lot of data. OpenCV's stereoCalibrate function requires:
//         // - a list of points in real 3d space that will be used to calibrate*
//         // - a corresponding list of pixel coordinates as seen by the first camera*
//         // - a corresponding list of pixel coordinates as seen by the second camera*
//         // - the camera matrix of the first camera
//         // - the distortion coefficients of the first camera
//         // - the camera matrix of the second camera
//         // - the distortion coefficients of the second camera
//         // - the size (in pixels) of the images
//         // - R: stereoCalibrate stores the rotation matrix from the first camera to the second here
//         // - t: stereoCalibrate stores the translation vector from the first camera to the second here
//         // - E: stereoCalibrate stores the essential matrix here (we don't use this)
//         // - F: stereoCalibrate stores the fundamental matrix here (we don't use this)
//         //
//         // * note: OpenCV's stereoCalibrate actually requires as input an array of arrays of points for these arguments,
//         // allowing a caller to provide multiple frames from the same camera with corresponding points. For example, if
//         // extremely high precision was required, many images could be taken with each camera, and findChessboardCorners
//         // applied to each of those images, and OpenCV can jointly solve for all of the pairs of corresponding images.
//         // However, to keep things simple, we use only one image from each device to calibrate.  This is also why each of
//         // the vectors of corners is placed into another vector.
//         //
//         // A function in OpenCV's calibration function also requires that these points be F32 types, so we use those.
//         // However, OpenCV still provides doubles as output, strangely enough.
//         List<List<Point3f>> chessboard_corners_world_nested_for_cv = new List<List<Point3f>>(main_chessboard_corners_list.Count,
//                                                                         chessboard_corners_world);

//         Matx33f main_camera_matrix = calibration_to_color_camera_matrix(main_calib);
//         Matx33f secondary_camera_matrix = calibration_to_color_camera_matrix(secondary_calib);
//         List<float> main_dist_coeff = calibration_to_color_camera_dist_coeffs(main_calib);
//         List<float> secondary_dist_coeff = calibration_to_color_camera_dist_coeffs(secondary_calib);

//         // Finally, we'll actually calibrate the cameras.
//         // Pass secondary first, then main, because we want a transform from secondary to main.
//         TranslationAndRotation tr;
//         double error = Cv2.StereoCalibrate(chessboard_corners_world_nested_for_cv,
//                                         secondary_chessboard_corners_list,
//                                         main_chessboard_corners_list,
//                                         secondary_camera_matrix,
//                                         secondary_dist_coeff,
//                                         main_camera_matrix,
//                                         main_dist_coeff,
//                                         image_size,
//                                         tr.R, // output
//                                         tr.t, // output
//                                         cv::noArray(),
//                                         cv::noArray(),
//                                         cv::CALIB_FIX_INTRINSIC | cv::CALIB_RATIONAL_MODEL | cv::CALIB_CB_FAST_CHECK);
//         Console.WriteLine("Finished calibrating!\n");
//         Console.WriteLine("Got error of ", error, "\n");
//         return tr;
//     }


//     static TranslationAndRotation calibrate_devices(MultiDeviceCapturer capturer,
//                                             DeviceConfiguration main_config,
//                                             DeviceConfiguration secondary_config,
//                                             Size chessboard_pattern,
//                                             float chessboard_square_length,
//                                             double calibration_timeout)
//     {
//         Calibration main_calibration = capturer.get_master_device().GetCalibration(main_config.depth_mode,
//                                                                                         main_config.color_resolution);

//         Calibration secondary_calibration =
//             capturer.get_subordinate_device_by_index(0).GetCalibration(secondary_config.depth_mode,
//                                                                         secondary_config.color_resolution);
//         List<List<Point2f>> main_chessboard_corners_list = new List<List<Point2f>>();
//         List<List<Point2f>> secondary_chessboard_corners_list = new List<List<Point2f>>();
//         var start_time = new Stopwatch();
//         while (start_time.Elapsed() < calibration_timeout)
//         {
//             List<Capture> captures = capturer.get_synchronized_captures(secondary_config);
//             Capture main_capture = captures[0];
//             Capture secondary_capture = captures[1];
//             // get_color_image is guaranteed to be non-null because we use get_synchronized_captures for color
//             // (get_synchronized_captures also offers a flag to use depth for the secondary camera instead of color).
//             Image main_color_image = main_capture.Color();
//             Image secondary_color_image = secondary_capture.Color();
//             Mat cv_main_color_image = color_to_opencv(main_color_image);
//             Mat cv_secondary_color_image = color_to_opencv(secondary_color_image);

//             List<Point2f> main_chessboard_corners;
//             List<Point2f> secondary_chessboard_corners;
//             bool got_corners = find_chessboard_corners_helper(cv_main_color_image,
//                                                             cv_secondary_color_image,
//                                                             chessboard_pattern,
//                                                             main_chessboard_corners,
//                                                             secondary_chessboard_corners);
//             if (got_corners)
//             {
//                 main_chessboard_corners_list.Add(main_chessboard_corners);
//                 secondary_chessboard_corners_list.Add(secondary_chessboard_corners);
//                 DrawChessboardCorners(cv_main_color_image, chessboard_pattern, main_chessboard_corners, true);
//                 DrawChessboardCorners(cv_secondary_color_image, chessboard_pattern, secondary_chessboard_corners, true);
//             }

//             Cv2.ImShow("Chessboard view from main camera", cv_main_color_image);
//             Cv2.WaitKey(1);
//             Cv2.ImShow("Chessboard view from secondary camera", cv_secondary_color_image);
//             Cv2.WaitKey(1);

//             // Get 20 frames before doing calibration.
//             if (main_chessboard_corners_list.Size() >= 20)
//             {
//                 UnityEngine.Debug.Log("Calculating calibration...");
//                 return stereo_calibration(main_calibration,
//                                         secondary_calibration,
//                                         main_chessboard_corners_list,
//                                         secondary_chessboard_corners_list,
//                                         cv_main_color_image.size(),
//                                         chessboard_pattern,
//                                         chessboard_square_length);
//             }
//         }
//         UnityEngine.Debug.Log("Calibration timed out !\n ");
//     }

//     static Image create_depth_image_like(Image im)
//     {
//         return Image.Create(K4A_IMAGE_FORMAT_DEPTH16,
//                                 im.get_width_pixels(),
//                                 im.get_height_pixels(),
//                                 im.get_width_pixels() * 2);
//     }




// }
