using System;
using System.IO;
using Xunit;

namespace BGChanger.Tests
{
    public class BGChangerTests : IDisposable
    {
        public void Dispose()
        {
            BGChanger.ResetCurrentBackground();
        }

        [Fact]
        public void Using_non_existent_background_file_should_throw_an_exception()
        {
            Assert.Throws<FileNotFoundException>(() => BGChanger.SetCurrentBackground(Guid.NewGuid().ToString()));
        }

        [Theory]
        [InlineData("RRRGGGBBB")]
        [InlineData("255255255")]
        [InlineData("RRR GGG BBB")]
        [InlineData("255,255,255")]
        [InlineData("255:255:255")]
        // [InlineData("001 001 001")]
        // [InlineData("256 256 256")]
        public void Using_an_invalid_color_string_should_throw_an_exception(string value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => BGChanger.SetCurrentBackground(color: value));
            Assert.Throws<ArgumentOutOfRangeException>(() => BGChanger.SetCurrentSolidColor(value));
            Assert.Throws<ArgumentOutOfRangeException>(() => BGChanger.SetCurrentSlideshow(color: value));
        }

        [Fact]
        public void Using_non_existent_background_directory_should_throw_an_exception()
        {
            Assert.Throws<DirectoryNotFoundException>(() => BGChanger.SetCurrentSlideshow(Guid.NewGuid().ToString()));
        }
    }
}
