<table>
  <tr>
    <td width="9999px" align="center">
      <p>
        <br>
        <img height="200" src="./logo.svg" alt="logo">
      </p>
      <h1>bgchanger</h1>
      <p>Change the Windows background settings.</p>
    </td>
  </tr>
</table>

## Usage

```cs
// Set a picture as the desktop background.
BGChanger.SetPictureBackground(@"C:\Users\bg.png", BackgroundStyle.Stretch);

// Set a solid color as the desktop background.
BGChanger.SetSolidColorBackground(Color.Black);

// Reset the default Windows desktop background.
BGChanger.ResetBackground();
```
