# Imgur Uploader.NET Change Log 📋

## v2.1 *(current)* 🆕

### New Features
- Images can now be uploaded to the user's own Imgur account using OAuth 2 authorization (PIN flow). Previously, all uploads were anonymous.
- The "Upload" button is now a split button that shows the current destination ("Upload to my account" / "Upload anonymously"). The drop-down menu lets you switch between both modes and sign in or out of Imgur.
- New sign-in dialog that guides the user through the Imgur authorization and PIN entry.
- The selected upload mode and the authorized account are remembered between sessions. They are stored per user in "%APPDATA%\ElektroStudios\Imgur Uploader.NET\Account.dat", with the refresh token encrypted using the Windows Data Protection API (DPAPI).
- The access token is renewed automatically when it expires, with no need to sign in again.
- The drop-down arrow area of the split button is now wider to prevent misclicks, and the button supports a custom color.
- Added a check mark to the active option in the dark context menus.

### Fixes
- Fixed images always being uploaded anonymously, even though the application was intended to use the user's account.
- If Imgur rejects the stored authorization, the account is cleared and the user is prompted to sign in again instead of the upload failing repeatedly.

### Other Changes
- Added unit tests for the token parsing, the account storage and the new UI controls.
- README: documented the required Imgur application registration type ("OAuth 2 authorization without a callback URL") and the account authorization flow.
- Added references to System.Security and System.Web.Extensions.

## v2.0 (2026) 🔄

#### 🚀 New Features:
 - New dark-themed custom dropdown selectors (with hover effects and mouse wheel support) to choose the image size and the output format.
 - Added the following sizes: Original, Small Square, Big Square, Small Thumbnail, Medium Thumbnail, Large Thumbnail and Huge Thumbnail.
 - Added the following output formats: Direct URL, Share Link, CSS, HTML, HTML Embed, BBCode, Markdown and Markdown (Image).
 - Added a single result box with "Copy" and "Open" buttons (the latter for URL formats).
 - Added a new "Upload image" button: browsing, dragging and dropping or passing an image by command line now only selects and previews it, and the upload starts after confirming it.
 - Added a panel that shows the name, extension, size and resolution of the selected image.
 - Added a link to the project repository on the status bar.
#### 🌟 Improvements:
 - The user interface has been completely modernized: the 8 textboxes, labels and buttons were replaced by a compact layout with a dark theme, flat controls and hover effects.
 - The window title bar now follows the dark theme of Windows.
 - The image preview now scales with the window and no longer shows black borders.
 - All the generated URLs now use HTTPS.
 - BBCode and Markdown formats are now generated for any selected size.
 - Updated the Imgur file size limits (50 MB for images and 200 MB for animated images).
 - Images that exceed the limit are now compressed in the background (as JPEG) while the window remains responsive, and the process can be aborted.
 - Errors reported by Imgur (invalid credentials, rate limit, etc.) are now shown on the status bar.
 - The message shown when the INI file is missing now explains how to get and configure the API credentials.
 - The window title now shows the application version.
#### 🛠️ Fixes:
 - The program hung when compressing PNG images that exceeded the file size limit.
 - An unwanted error message was shown when aborting an upload.
 - The program could freeze when aborting an upload.
 - The window remained disabled after cancelling or failing the compression of an image.
 - Drag and drop was not re-enabled after finishing an upload.
 - Temporary files created by the compression procedure were not deleted.
 - "System.Resources.Extensions" FileLoadException on startup.

## v1.4.1 (2024) 🔄

#### 🛠️ Fixes:
 - Copy button for Markdow format was copying BBCode url instead.

## v1.4 (2024) 🔄

#### 🛠️ Fixes:
 - The program hangs during the image upload procedure (on Release mode).
#### 🌟 Improvements:
 - Now the program window allows to resize.
 - CButtonLib updated to last version.

## v1.3 (2024) 🔄

#### 🚀 New Features:
 - Added a row to display image url in Markdown format.
#### 🌟 Improvements:
 - Source-code upgraded to .NET Framework 4.8
 - The user interface has been reworked.
 - Improved detection of invalid API key and key Secret values.
 - From now, the uploaded image file name will be used as its default title and description on Imgur.

## v1.1 to v1-2 (2019) 🔄

*Change log information not available.*

## v1.0 (2018) 🔄

Initial Release.
