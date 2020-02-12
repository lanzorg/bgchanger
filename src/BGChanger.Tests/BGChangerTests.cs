using System;
using System.IO;
using Xunit;

namespace BGChanger.Tests
{
    public class BGChangerTests : IDisposable
    {
        public void Dispose()
        {
            BGChanger.ResetBackground();
        }

        [Fact]
        public void Using_non_existent_background_file_should_throw_an_exception()
        {
            Assert.Throws<FileNotFoundException>(() => BGChanger.SetPictureBackground(Guid.NewGuid().ToString()));
        }

        [Fact]
        public void Using_non_existent_background_directory_should_throw_an_exception()
        {
            Assert.Throws<DirectoryNotFoundException>(() => BGChanger.SetSlideshowBackground(Guid.NewGuid().ToString()));
        }
    }
}
