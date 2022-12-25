using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SimpleImageResizer.Core;

/// <summary>
/// Class for drawing objects.
/// </summary>
public static class Draw
{
    /// <summary>
    /// Convert the text string to a geometry and draw it to the control's DrawingContext.
    /// https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/how-to-draw-text-to-a-control-background.
    /// </summary>
    /// <param name="pixelsPerDip">Pixels per DIP.</param>
    /// <returns>Drawing.</returns>
    public static Drawing WelcomeText(double pixelsPerDip)
    {
        // Create a new DrawingGroup of the control.
        DrawingGroup drawingGroup = new();

        // Open the DrawingGroup in order to access the DrawingContext.
        using DrawingContext drawingContext = drawingGroup.Open();

        // Note that the font is name without a space in resources but here is has a space because the actual name of the font has a space.
        FontFamily fontFamily = new(new Uri("pack://application:,,,/"), "Resources/#Comic Kings");
        FontFamily segoeFont = new("Segeo UI");
        FontFamily fallbackFont = new("Segoe UI");

        FormattedText formattedText = new(
               "Simple Image Resizer!",
               CultureInfo.GetCultureInfo("en-us"),
               FlowDirection.LeftToRight,
               new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal, fallbackFont),
               24,
               Brushes.Black,
               pixelsPerDip);

        FormattedText formattedText2 = new(
           "Drag and Drop your images here!",
           System.Globalization.CultureInfo.GetCultureInfo("en-us"),
           FlowDirection.LeftToRight,
           new Typeface(segoeFont, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal, fallbackFont),
           12,
           Brushes.LightBlue, // This brush does not matter since we use the geometry of the text.
           pixelsPerDip);

        // Build the geometry object that represents the text.
        Geometry textGeometry = formattedText.BuildGeometry(new System.Windows.Point(20, 10));

        // Draw a rounded rectangle under the text that is slightly larger than the text.
        Rect rect = new(new Size(formattedText.Width + 50, formattedText.Height + 10));
        drawingContext.DrawRectangle(Brushes.Transparent, null, rect);

        // Draw the outline based on the properties that are set.
        drawingContext.DrawGeometry(Brushes.LightBlue, new Pen(Brushes.SteelBlue, 1.5), textGeometry);

        drawingContext.DrawText(formattedText2, new Point(80, 40));

        // Return the updated DrawingGroup content to be used by the control.
        return drawingGroup;
    }
}
