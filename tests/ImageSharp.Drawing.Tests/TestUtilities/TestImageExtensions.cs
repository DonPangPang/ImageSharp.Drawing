// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Drawing.Tests.TestUtilities.ImageComparison;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors;
using Xunit;

namespace SixLabors.ImageSharp.Drawing.Tests
{
    public static class TestImageExtensions
    {
        /// <summary>
        /// TODO: Consider adding this private processor to the library
        /// </summary>
        public static void MakeOpaque(this IImageProcessingContext ctx) =>
            ctx.ApplyProcessor(new MakeOpaqueProcessor());

        public static void DebugSave(
            this Image image,
            ITestImageProvider provider,
            FormattableString testOutputDetails,
            string extension = "png",
            bool appendPixelTypeToFileName = true,
            bool appendSourceFileOrDescription = true,
            IImageEncoder encoder = null) => image.DebugSave(
                provider,
                (object)testOutputDetails,
                extension,
                appendPixelTypeToFileName,
                appendSourceFileOrDescription,
                encoder);

        /// <summary>
        /// Saves the image only when not running in the CI server.
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="provider">The image provider</param>
        /// <param name="testOutputDetails">Details to be concatenated to the test output file, describing the parameters of the test.</param>
        /// <param name="extension">The extension</param>
        /// <param name="appendPixelTypeToFileName">A boolean indicating whether to append the pixel type to the  output file name.</param>
        /// <param name="appendSourceFileOrDescription">A boolean indicating whether to append <see cref="ITestImageProvider.SourceFileOrDescription"/> to the test output file name.</param>
        /// <param name="encoder">Custom encoder to use.</param>
        /// <returns>The <see cref="Image"/>.</returns>
        public static Image DebugSave(
            this Image image,
            ITestImageProvider provider,
            object testOutputDetails = null,
            string extension = "png",
            bool appendPixelTypeToFileName = true,
            bool appendSourceFileOrDescription = true,
            IImageEncoder encoder = null)
        {
            if (TestEnvironment.RunsWithCodeCoverage)
            {
                return image;
            }

            // We are running locally then we want to save it out
            provider.Utility.SaveTestOutputFile(
                image,
                extension,
                testOutputDetails: testOutputDetails,
                appendPixelTypeToFileName: appendPixelTypeToFileName,
                appendSourceFileOrDescription: appendSourceFileOrDescription,
                encoder: encoder);
            return image;
        }

        public static void DebugSave(
            this Image image,
            ITestImageProvider provider,
            IImageEncoder encoder,
            FormattableString testOutputDetails,
            bool appendPixelTypeToFileName = true)
            => image.DebugSave(provider, encoder, (object)testOutputDetails, appendPixelTypeToFileName);

        /// <summary>
        /// Saves the image only when not running in the CI server.
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="provider">The image provider</param>
        /// <param name="encoder">The image encoder</param>
        /// <param name="testOutputDetails">Details to be concatenated to the test output file, describing the parameters of the test.</param>
        /// <param name="appendPixelTypeToFileName">A boolean indicating whether to append the pixel type to the  output file name.</param>
        public static void DebugSave(
            this Image image,
            ITestImageProvider provider,
            IImageEncoder encoder,
            object testOutputDetails = null,
            bool appendPixelTypeToFileName = true)
        {
            if (TestEnvironment.RunsWithCodeCoverage)
            {
                return;
            }

            // We are running locally then we want to save it out
            provider.Utility.SaveTestOutputFile(
                image,
                encoder: encoder,
                testOutputDetails: testOutputDetails,
                appendPixelTypeToFileName: appendPixelTypeToFileName);
        }

        public static Image<TPixel> DebugSaveMultiFrame<TPixel>(
            this Image<TPixel> image,
            ITestImageProvider provider,
            object testOutputDetails = null,
            string extension = "png",
            bool appendPixelTypeToFileName = true)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            if (TestEnvironment.RunsWithCodeCoverage)
            {
                return image;
            }

            // We are running locally then we want to save it out
            provider.Utility.SaveTestOutputFileMultiFrame(
                image,
                extension,
                testOutputDetails: testOutputDetails,
                appendPixelTypeToFileName: appendPixelTypeToFileName);
            return image;
        }

        public static Image<TPixel> CompareToReferenceOutput<TPixel>(
            this Image<TPixel> image,
            ITestImageProvider provider,
            FormattableString testOutputDetails,
            string extension = "png",
            bool appendPixelTypeToFileName = true,
            bool appendSourceFileOrDescription = true)
            where TPixel : unmanaged, IPixel<TPixel> => image.CompareToReferenceOutput(
                provider,
                (object)testOutputDetails,
                extension,
                appendPixelTypeToFileName,
                appendSourceFileOrDescription);

        /// <summary>
        /// Compares the image against the expected Reference output, throws an exception if the images are not similar enough.
        /// The output file should be named identically to the output produced by <see cref="DebugSave{TPixel}(Image{TPixel}, ITestImageProvider, object, string, bool)"/>.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format</typeparam>
        /// <param name="image">The image</param>
        /// <param name="provider">The image provider</param>
        /// <param name="testOutputDetails">Details to be concatenated to the test output file, describing the parameters of the test.</param>
        /// <param name="extension">The extension</param>
        /// <param name="appendPixelTypeToFileName">A boolean indicating whether to append the pixel type to the  output file name.</param>
        /// <param name="appendSourceFileOrDescription">A boolean indicating whether to append <see cref="ITestImageProvider.SourceFileOrDescription"/> to the test output file name.</param>
        /// <returns>The <see cref="Image{TPixel}"/>.</returns>
        public static Image<TPixel> CompareToReferenceOutput<TPixel>(
            this Image<TPixel> image,
            ITestImageProvider provider,
            object testOutputDetails = null,
            string extension = "png",
            bool appendPixelTypeToFileName = true,
            bool appendSourceFileOrDescription = true)
            where TPixel : unmanaged, IPixel<TPixel> => CompareToReferenceOutput(
                image,
                ImageComparer.Tolerant(),
                provider,
                testOutputDetails,
                extension,
                appendPixelTypeToFileName,
                appendSourceFileOrDescription);

        public static Image<TPixel> CompareToReferenceOutput<TPixel>(
            this Image<TPixel> image,
            ImageComparer comparer,
            ITestImageProvider provider,
            FormattableString testOutputDetails,
            string extension = "png",
            bool grayscale = false,
            bool appendPixelTypeToFileName = true)
            where TPixel : unmanaged, IPixel<TPixel> => image.CompareToReferenceOutput(
                comparer,
                provider,
                (object)testOutputDetails,
                extension,
                grayscale,
                appendPixelTypeToFileName);

        /// <summary>
        /// Compares the image against the expected Reference output, throws an exception if the images are not similar enough.
        /// The output file should be named identically to the output produced by <see cref="DebugSave{TPixel}(Image{TPixel}, ITestImageProvider, object, string, bool)"/>.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format</typeparam>
        /// <param name="image">The image</param>
        /// <param name="comparer">The <see cref="ImageComparer"/> to use</param>
        /// <param name="provider">The image provider</param>
        /// <param name="testOutputDetails">Details to be concatenated to the test output file, describing the parameters of the test.</param>
        /// <param name="extension">The extension</param>
        /// <param name="appendPixelTypeToFileName">A boolean indicating whether to append the pixel type to the  output file name.</param>
        /// <param name="appendSourceFileOrDescription">A boolean indicating whether to append <see cref="ITestImageProvider.SourceFileOrDescription"/> to the test output file name.</param>
        /// <param name="decoder">A custom decoder.</param>
        /// <returns>The <see cref="Image{TPixel}"/>.</returns>
        public static Image<TPixel> CompareToReferenceOutput<TPixel>(
            this Image<TPixel> image,
            ImageComparer comparer,
            ITestImageProvider provider,
            object testOutputDetails = null,
            string extension = "png",
            bool appendPixelTypeToFileName = true,
            bool appendSourceFileOrDescription = true,
            IImageDecoder decoder = null)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (Image<TPixel> referenceImage = GetReferenceOutputImage<TPixel>(
                provider,
                testOutputDetails,
                extension,
                appendPixelTypeToFileName,
                appendSourceFileOrDescription,
                decoder))
            {
                comparer.VerifySimilarity(referenceImage, image);
            }

            return image;
        }

        public static Image<TPixel> CompareFirstFrameToReferenceOutput<TPixel>(
            this Image<TPixel> image,
            ImageComparer comparer,
            ITestImageProvider provider,
            FormattableString testOutputDetails,
            string extension = "png",
            bool appendPixelTypeToFileName = true,
            bool appendSourceFileOrDescription = true)
            where TPixel : unmanaged, IPixel<TPixel> => image.CompareFirstFrameToReferenceOutput(
                comparer,
                provider,
                (object)testOutputDetails,
                extension,
                appendPixelTypeToFileName,
                appendSourceFileOrDescription);

        public static Image<TPixel> CompareFirstFrameToReferenceOutput<TPixel>(
            this Image<TPixel> image,
            ImageComparer comparer,
            ITestImageProvider provider,
            object testOutputDetails = null,
            string extension = "png",
            bool appendPixelTypeToFileName = true,
            bool appendSourceFileOrDescription = true)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (var firstFrameOnlyImage = new Image<TPixel>(image.Width, image.Height))
            using (Image<TPixel> referenceImage = GetReferenceOutputImage<TPixel>(
                provider,
                testOutputDetails,
                extension,
                appendPixelTypeToFileName,
                appendSourceFileOrDescription))
            {
                firstFrameOnlyImage.Frames.AddFrame(image.Frames.RootFrame);
                firstFrameOnlyImage.Frames.RemoveFrame(0);

                comparer.VerifySimilarity(referenceImage, firstFrameOnlyImage);
            }

            return image;
        }

        public static Image<TPixel> CompareToReferenceOutputMultiFrame<TPixel>(
            this Image<TPixel> image,
            ITestImageProvider provider,
            ImageComparer comparer,
            object testOutputDetails = null,
            string extension = "png",
            bool appendPixelTypeToFileName = true)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (Image<TPixel> referenceImage = GetReferenceOutputImageMultiFrame<TPixel>(
                provider,
                image.Frames.Count,
                testOutputDetails,
                extension,
                appendPixelTypeToFileName))
            {
                comparer.VerifySimilarity(referenceImage, image);
            }

            return image;
        }

        public static Image<TPixel> GetReferenceOutputImage<TPixel>(
            this ITestImageProvider provider,
            object testOutputDetails = null,
            string extension = "png",
            bool appendPixelTypeToFileName = true,
            bool appendSourceFileOrDescription = true,
            IImageDecoder decoder = null)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            string referenceOutputFile = provider.Utility.GetReferenceOutputFileName(
                extension,
                testOutputDetails,
                appendPixelTypeToFileName,
                appendSourceFileOrDescription);

            if (!File.Exists(referenceOutputFile))
            {
                throw new FileNotFoundException("Reference output file missing: " + referenceOutputFile, referenceOutputFile);
            }

            decoder ??= TestEnvironment.GetReferenceDecoder(referenceOutputFile);

            return Image.Load<TPixel>(referenceOutputFile, decoder);
        }

        public static Image<TPixel> GetReferenceOutputImageMultiFrame<TPixel>(
            this ITestImageProvider provider,
            int frameCount,
            object testOutputDetails = null,
            string extension = "png",
            bool appendPixelTypeToFileName = true)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            string[] frameFiles = provider.Utility.GetReferenceOutputFileNamesMultiFrame(
                frameCount,
                extension,
                testOutputDetails,
                appendPixelTypeToFileName);

            var temporaryFrameImages = new List<Image<TPixel>>();

            IImageDecoder decoder = TestEnvironment.GetReferenceDecoder(frameFiles[0]);

            foreach (string path in frameFiles)
            {
                if (!File.Exists(path))
                {
                    throw new Exception("Reference output file missing: " + path);
                }

                var tempImage = Image.Load<TPixel>(path, decoder);
                temporaryFrameImages.Add(tempImage);
            }

            Image<TPixel> firstTemp = temporaryFrameImages[0];

            var result = new Image<TPixel>(firstTemp.Width, firstTemp.Height);

            foreach (Image<TPixel> fi in temporaryFrameImages)
            {
                result.Frames.AddFrame(fi.Frames.RootFrame);
                fi.Dispose();
            }

            // remove the initial empty frame:
            result.Frames.RemoveFrame(0);
            return result;
        }

        public static IEnumerable<ImageSimilarityReport> GetReferenceOutputSimilarityReports<TPixel>(
            this Image<TPixel> image,
            ITestImageProvider provider,
            ImageComparer comparer,
            object testOutputDetails = null,
            string extension = "png",
            bool appendPixelTypeToFileName = true)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (Image<TPixel> referenceImage = provider.GetReferenceOutputImage<TPixel>(
                testOutputDetails,
                extension,
                appendPixelTypeToFileName))
            {
                return comparer.CompareImages(referenceImage, image);
            }
        }

        public static Image<TPixel> ComparePixelBufferTo<TPixel>(
            this Image<TPixel> image,
            Span<TPixel> expectedPixels)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            foreach (ImageFrame<TPixel> imageFrame in image.Frames)
            {
                imageFrame.ComparePixelBufferTo(expectedPixels);
            }

            return image;
        }

        public static void CompareBuffers<T>(Span<T> expected, Span<T> actual)
            where T : struct, IEquatable<T>
        {
            Assert.True(expected.Length == actual.Length, "Buffer sizes are not equal!");

            for (int i = 0; i < expected.Length; i++)
            {
                T x = expected[i];
                T a = actual[i];

                Assert.True(x.Equals(a), $"Buffers differ at position {i}! Expected: {x} | Actual: {a}");
            }
        }

        /// <summary>
        /// All pixels in all frames should be exactly equal to 'expectedPixel'.
        /// </summary>
        /// <returns>The <see cref="Image{TPixel}"/>.</returns>
        public static Image<TPixel> ComparePixelBufferTo<TPixel>(this Image<TPixel> image, TPixel expectedPixel)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            foreach (ImageFrame<TPixel> imageFrame in image.Frames)
            {
                imageFrame.ComparePixelBufferTo(expectedPixel);
            }

            return image;
        }

        /// <summary>
        /// All pixels in all frames should be exactly equal to 'expectedPixelColor.ToPixel()'.
        /// </summary>
        /// <returns>The <see cref="Image{TPixel}"/>.</returns>
        public static Image<TPixel> ComparePixelBufferTo<TPixel>(this Image<TPixel> image, Color expectedPixelColor)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            foreach (ImageFrame<TPixel> imageFrame in image.Frames)
            {
                imageFrame.ComparePixelBufferTo(expectedPixelColor.ToPixel<TPixel>());
            }

            return image;
        }

        /// <summary>
        /// All pixels in the frame should be exactly equal to 'expectedPixel'.
        /// </summary>
        /// <returns>The <see cref="Image{TPixel}"/>.</returns>
        public static ImageFrame<TPixel> ComparePixelBufferTo<TPixel>(this ImageFrame<TPixel> imageFrame, TPixel expectedPixel)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            Assert.True(imageFrame.DangerousTryGetSinglePixelMemory(out Memory<TPixel> actual));
            Span<TPixel> actualPixels = actual.Span;

            for (int i = 0; i < actualPixels.Length; i++)
            {
                Assert.True(expectedPixel.Equals(actualPixels[i]), $"Pixels are different on position {i}!");
            }

            return imageFrame;
        }

        public static ImageFrame<TPixel> ComparePixelBufferTo<TPixel>(
                    this ImageFrame<TPixel> image,
                    Span<TPixel> expectedPixels)
                    where TPixel : unmanaged, IPixel<TPixel>
        {
            Assert.True(image.DangerousTryGetSinglePixelMemory(out Memory<TPixel> actual));
            Span<TPixel> actualPixels = actual.Span;

            Assert.True(expectedPixels.Length == actualPixels.Length, "Buffer sizes are not equal!");

            for (int i = 0; i < expectedPixels.Length; i++)
            {
                Assert.True(expectedPixels[i].Equals(actualPixels[i]), $"Pixels are different on position {i}!");
            }

            return image;
        }

        public static Image<TPixel> CompareToOriginal<TPixel>(
            this Image<TPixel> image,
            ITestImageProvider provider,
            IImageDecoder referenceDecoder = null)
            where TPixel : unmanaged, IPixel<TPixel> => CompareToOriginal(image, provider, ImageComparer.Tolerant(), referenceDecoder);

        public static Image<TPixel> CompareToOriginal<TPixel>(
            this Image<TPixel> image,
            ITestImageProvider provider,
            ImageComparer comparer,
            IImageDecoder referenceDecoder = null)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            string path = TestImageProvider<TPixel>.GetFilePathOrNull(provider);
            if (path == null)
            {
                throw new InvalidOperationException("CompareToOriginal() works only with file providers!");
            }

            var testFile = TestFile.Create(path);

            referenceDecoder ??= TestEnvironment.GetReferenceDecoder(path);

            using (var original = Image.Load<TPixel>(testFile.Bytes, referenceDecoder))
            {
                comparer.VerifySimilarity(original, image);
            }

            return image;
        }

        /// <summary>
        /// Utility method for doing the following in one step:
        /// 1. Executing an operation (taken as a delegate)
        /// 2. Executing DebugSave()
        /// 3. Executing CompareToReferenceOutput()
        /// </summary>
        internal static void VerifyOperation<TPixel>(
            this TestImageProvider<TPixel> provider,
            ImageComparer comparer,
            Action<Image<TPixel>> operation,
            FormattableString testOutputDetails,
            bool appendPixelTypeToFileName = true,
            bool appendSourceFileOrDescription = true)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (Image<TPixel> image = provider.GetImage())
            {
                operation(image);

                image.DebugSave(
                    provider,
                    testOutputDetails,
                    appendPixelTypeToFileName: appendPixelTypeToFileName,
                    appendSourceFileOrDescription: appendSourceFileOrDescription);

                image.CompareToReferenceOutput(
                    comparer,
                    provider,
                    testOutputDetails,
                    appendPixelTypeToFileName: appendPixelTypeToFileName,
                    appendSourceFileOrDescription: appendSourceFileOrDescription);
            }
        }

        /// <summary>
        /// Utility method for doing the following in one step:
        /// 1. Executing an operation (taken as a delegate)
        /// 2. Executing DebugSave()
        /// 3. Executing CompareToReferenceOutput()
        /// </summary>
        internal static void VerifyOperation<TPixel>(
            this TestImageProvider<TPixel> provider,
            Action<Image<TPixel>> operation,
            FormattableString testOutputDetails,
            bool appendPixelTypeToFileName = true,
            bool appendSourceFileOrDescription = true)
            where TPixel : unmanaged, IPixel<TPixel>
            => provider.VerifyOperation(
                ImageComparer.Tolerant(),
                operation,
                testOutputDetails,
                appendPixelTypeToFileName,
                appendSourceFileOrDescription);

        /// <summary>
        /// Utility method for doing the following in one step:
        /// 1. Executing an operation (taken as a delegate)
        /// 2. Executing DebugSave()
        /// 3. Executing CompareToReferenceOutput()
        /// </summary>
        internal static void VerifyOperation<TPixel>(
            this TestImageProvider<TPixel> provider,
            ImageComparer comparer,
            Action<Image<TPixel>> operation,
            bool appendPixelTypeToFileName = true,
            bool appendSourceFileOrDescription = true)
            where TPixel : unmanaged, IPixel<TPixel>
            => provider.VerifyOperation(
                comparer,
                operation,
                $"",
                appendPixelTypeToFileName,
                appendSourceFileOrDescription);

        /// <summary>
        /// Utility method for doing the following in one step:
        /// 1. Executing an operation (taken as a delegate)
        /// 2. Executing DebugSave()
        /// 3. Executing CompareToReferenceOutput()
        /// </summary>
        internal static void VerifyOperation<TPixel>(
            this TestImageProvider<TPixel> provider,
            Action<Image<TPixel>> operation,
            bool appendPixelTypeToFileName = true,
            bool appendSourceFileOrDescription = true)
            where TPixel : unmanaged, IPixel<TPixel>
            => provider.VerifyOperation(operation, $"", appendPixelTypeToFileName, appendSourceFileOrDescription);

        /// <summary>
        /// Loads the expected image with a reference decoder + compares it to <paramref name="image"/>.
        /// Also performs a debug save using <see cref="ImagingTestCaseUtility.SaveTestOutputFile{TPixel}"/>.
        /// </summary>
        internal static void VerifyEncoder<TPixel>(
            this Image<TPixel> image,
            ITestImageProvider provider,
            string extension,
            object testOutputDetails,
            IImageEncoder encoder,
            ImageComparer customComparer = null,
            bool appendPixelTypeToFileName = true,
            IImageDecoder referenceDecoder = null)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            string actualOutputFile = provider.Utility.SaveTestOutputFile(
                image,
                extension,
                encoder,
                testOutputDetails,
                appendPixelTypeToFileName);

            referenceDecoder ??= TestEnvironment.GetReferenceDecoder(actualOutputFile);

            using (var actualImage = Image.Load<TPixel>(actualOutputFile, referenceDecoder))
            {
                ImageComparer comparer = customComparer ?? ImageComparer.Exact;
                comparer.VerifySimilarity(actualImage, image);
            }
        }

        private class MakeOpaqueProcessor : IImageProcessor
        {
            public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
                where TPixel : unmanaged, IPixel<TPixel>
                => new MakeOpaqueProcessor<TPixel>(configuration, source, sourceRectangle);
        }

        private class MakeOpaqueProcessor<TPixel> : ImageProcessor<TPixel>
            where TPixel : unmanaged, IPixel<TPixel>
        {
            public MakeOpaqueProcessor(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
                : base(configuration, source, sourceRectangle)
            {
            }

            protected override void OnFrameApply(ImageFrame<TPixel> source)
            {
                Rectangle sourceRectangle = this.SourceRectangle;
                Configuration configuration = this.Configuration;

                var operation = new RowIntervalOperation(configuration, sourceRectangle, source.PixelBuffer);
                ParallelRowIterator.IterateRowIntervals<RowIntervalOperation, Vector4>(
                    configuration,
                    sourceRectangle,
                    in operation);
            }

            private readonly struct RowIntervalOperation : IRowIntervalOperation<Vector4>
            {
                private readonly Configuration configuration;
                private readonly Rectangle bounds;
                private readonly Buffer2D<TPixel> source;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public RowIntervalOperation(Configuration configuration, Rectangle bounds, Buffer2D<TPixel> source)
                {
                    this.configuration = configuration;
                    this.bounds = bounds;
                    this.source = source;
                }

                /// <inheritdoc/>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Invoke(in RowInterval rows, Span<Vector4> span)
                {
                    for (int y = rows.Min; y < rows.Max; y++)
                    {
                        Span<TPixel> rowSpan = this.source.DangerousGetRowSpan(y).Slice(this.bounds.Left, this.bounds.Width);
                        PixelOperations<TPixel>.Instance.ToVector4(this.configuration, rowSpan, span, PixelConversionModifiers.Scale);
                        for (int i = 0; i < span.Length; i++)
                        {
                            ref Vector4 v = ref span[i];
                            v.W = 1F;
                        }

                        PixelOperations<TPixel>.Instance.FromVector4Destructive(this.configuration, span, rowSpan, PixelConversionModifiers.Scale);
                    }
                }
            }
        }
    }
}
