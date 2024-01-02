# Face Detection HAAR

This software has been developed for specific testing of Object Detection Algorithm.
It is a winform application using OpenCvSharp version 4.8.

the software used Haar Cascade, It is an Object Detection Algorithm used to identify faces in an image or a real time video. The algorithm uses edge or line detection features proposed by Viola and Jones in their research paper “Rapid Object Detection using a Boosted Cascade of Simple Features” published in 2001. The algorithm is given a lot of positive images consisting of faces, and a lot of negative images not consisting of any face to train on them. The model created from this training is available at the OpenCV GitHub repository

## Features
* displays stream from USB cameras
* detect a face and draw a rectangal around it.


## Made with
* Net Framework 4.8, C# and WinForms
* OpenCvSharp: https://github.com/shimat/opencvsharp
* Visual Studio 2022 
