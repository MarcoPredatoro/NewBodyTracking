// // Copyright (c) Microsoft Corporation. All rights reserved.
// // Licensed under the MIT License.
// using System.Collections;
// using UnityEngine;
// using OpenCvSharp;
// using System;

// public class TranslationAndRotation
// {
//     public Mat R;
//     public Vec3d t;

//     // Construct an identity transformation.
//     public TranslationAndRotation()
//     {
//         R = Mat.Eye(3, MatType.CV_32F);
//         t = new Vec3d(0, 0, 0);
//     }

//     // Construct from H
//     public TranslationAndRotation(Mat H)
//     {
//         R = H.SubMat(0, 3, 0, 3);
//         t = new Vec3d(H(0, 3), H(1, 3), H(2, 3));
//     }

//     // Create homogeneous matrix from this transformation
//     Mat to_homogeneous()
//     {
//         return new Mat(
//             // row 1
//             R(0, 0),
//             R(0, 1),
//             R(0, 2),
//             t(0),
//             // row 2
//             R(1, 0),
//             R(1, 1),
//             R(1, 2),
//             t(1),
//             // row 3
//             R(2, 0),
//             R(2, 1),
//             R(2, 2),
//             t(2),
//             // row 4
//             0,
//             0,
//             0,
//             1);
//     }

//     // Construct a transformation equivalent to this transformation followed by the second transformation
//     TranslationAndRotation compose_with(TranslationAndRotation second_transformation)
//     {
//         // get this transform
//         Matx H_1 = to_homogeneous();
//         // get the transform to be composed with this one
//         Matx H_2 = second_transformation.to_homogeneous();
//         // get the combined transform
//         Matx H_3 = H_1 * H_2;
//         return new TranslationAndRotation(H_3);
//     }
// };