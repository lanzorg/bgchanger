<p align="center">
  <a href="https://github.com/lanzorg/bgchanger">
    <img height="250" src="./logo.svg" alt="logo">
  </a>
</p>

<h1 align="center">BGChanger</h1>

<p align="center">Change the Windows background settings.</p>

## Usage

```cs
// Set a picture as the desktop background.
BGChanger.SetPictureBackground(@"C:\Users\bg.png", BackgroundStyle.Stretch, "255 255 255");

// Set a solid color as the desktop background.
BGChanger.SetSolidColorBackground("255 255 255");

// Reset the default Windows desktop background.
BGChanger.ResetBackground();
```