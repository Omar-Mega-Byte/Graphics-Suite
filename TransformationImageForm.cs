using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

public class TransformationForm : Form
{
    private PictureBox pictureBox;
    private Image originalImage;
    private Image transformedImage;
    private NumericUpDown rotationBox, translateXBox, translateYBox, shearXBox, shearYBox, scaleXBox, scaleYBox;
    private Button loadButton, resetButton;

    public TransformationForm()
    {
        Text = "Transformation Tester";
        Size = new Size(600, 400);

        // PictureBox for displaying the image
        pictureBox = new PictureBox
        {
            Location = new Point(10, 10),
            Size = new Size(300, 300),
            BorderStyle = BorderStyle.FixedSingle,
            SizeMode = PictureBoxSizeMode.CenterImage
        };

        // Load Image Button
        loadButton = new Button
        {
            Text = "Load Image",
            Location = new Point(320, 10),
            Width = 100
        };
        loadButton.Click += LoadButton_Click;

        // Rotation
        Label rotationLabel = new Label { Text = "Rotation", Location = new Point(320, 50) };
        Label degreeLabel = new Label { Text = "Degree", Location = new Point(340, 70) };
        rotationBox = new NumericUpDown
        {
            Location = new Point(400, 70),
            Width = 60,
            Minimum = -360,
            Maximum = 360,
            Value = 0
        };
        rotationBox.ValueChanged += ApplyTransformations;

        // Translation
        Label translationLabel = new Label { Text = "Translation", Location = new Point(320, 100) };
        Label translateXLabel = new Label { Text = "X", Location = new Point(340, 120) };
        translateXBox = new NumericUpDown
        {
            Location = new Point(400, 120),
            Width = 60,
            Minimum = -1000,
            Maximum = 1000,
            Value = 0
        };
        translateXBox.ValueChanged += ApplyTransformations;

        Label translateYLabel = new Label { Text = "Y", Location = new Point(340, 150) };
        translateYBox = new NumericUpDown
        {
            Location = new Point(400, 150),
            Width = 60,
            Minimum = -1000,
            Maximum = 1000,
            Value = 0
        };
        translateYBox.ValueChanged += ApplyTransformations;

        // Shear
        Label shearLabel = new Label { Text = "Shear", Location = new Point(320, 180) };
        Label shearXLabel = new Label { Text = "X", Location = new Point(340, 200) };
        shearXBox = new NumericUpDown
        {
            Location = new Point(400, 200),
            Width = 60,
            Minimum = -10,
            Maximum = 10,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 0
        };
        shearXBox.ValueChanged += ApplyTransformations;

        Label shearYLabel = new Label { Text = "Y", Location = new Point(340, 230) };
        shearYBox = new NumericUpDown
        {
            Location = new Point(400, 230),
            Width = 60,
            Minimum = -10,
            Maximum = 10,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 0
        };
        shearYBox.ValueChanged += ApplyTransformations;

        // Scale
        Label scaleLabel = new Label { Text = "Scale", Location = new Point(320, 260) };
        Label scaleXLabel = new Label { Text = "X", Location = new Point(340, 280) };
        scaleXBox = new NumericUpDown
        {
            Location = new Point(400, 280),
            Width = 60,
            Minimum = 0.1m,
            Maximum = 10,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 1
        };
        scaleXBox.ValueChanged += ApplyTransformations;

        Label scaleYLabel = new Label { Text = "Y", Location = new Point(340, 310) };
        scaleYBox = new NumericUpDown
        {
            Location = new Point(400, 310),
            Width = 60,
            Minimum = 0.1m,
            Maximum = 10,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 1
        };
        scaleYBox.ValueChanged += ApplyTransformations;

        // Reset Button
        resetButton = new Button
        {
            Text = "Reset",
            Location = new Point(320, 340),
            Width = 100
        };
        resetButton.Click += ResetButton_Click;

        // Add controls to form
        Controls.AddRange(new Control[] {
            pictureBox, loadButton,
            rotationLabel, degreeLabel, rotationBox,
            translationLabel, translateXLabel, translateXBox, translateYLabel, translateYBox,
            shearLabel, shearXLabel, shearXBox, shearYLabel, shearYBox,
            scaleLabel, scaleXLabel, scaleXBox, scaleYLabel, scaleYBox,
            resetButton
        });
    }

    private void LoadButton_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                originalImage = Image.FromFile(openFileDialog.FileName);
                transformedImage = (Image)originalImage.Clone();
                pictureBox.Image = transformedImage;
                ApplyTransformations(this, EventArgs.Empty);
            }
        }
    }

    private void ApplyTransformations(object sender, EventArgs e)
    {
        if (originalImage == null) return;

        // Get transformation parameters
        float scaleX = (float)scaleXBox.Value;
        float scaleY = (float)scaleYBox.Value;
        float shearX = (float)shearXBox.Value;
        float shearY = (float)shearYBox.Value;
        float rotation = (float)rotationBox.Value;
        float translateX = (float)translateXBox.Value;
        float translateY = -(float)translateYBox.Value; // Invert Y translation (positive Y moves up)

        // Original image corners
        PointF[] corners = new PointF[]
        {
            new PointF(0, 0),
            new PointF(originalImage.Width, 0),
            new PointF(0, originalImage.Height),
            new PointF(originalImage.Width, originalImage.Height)
        };

        // Create transformation matrix (excluding user translation for bounds calculation)
        using (Matrix matrix = new Matrix())
        {
            float centerX = originalImage.Width / 2f;
            float centerY = originalImage.Height / 2f;

            matrix.Translate(-centerX, -centerY); // Move to origin
            matrix.Scale(scaleX, scaleY); // Scale
            matrix.Shear(shearX, shearY); // Shear
            matrix.Rotate(rotation); // Rotate
            matrix.Translate(centerX, centerY); // Move back without user translation

            // Transform the corners to find the new bounds
            matrix.TransformPoints(corners);
        }

        // Calculate the new bitmap size with padding
        float minX = corners.Min(p => p.X);
        float maxX = corners.Max(p => p.X);
        float minY = corners.Min(p => p.Y);
        float maxY = corners.Max(p => p.Y);

        int padding = 50; // Add padding to prevent clipping
        int newWidth = (int)Math.Ceiling(maxX - minX) + 2 * padding;
        int newHeight = (int)Math.Ceiling(maxY - minY) + 2 * padding;

        // Ensure the bitmap is at least as large as the PictureBox
        newWidth = Math.Max(newWidth, pictureBox.Width);
        newHeight = Math.Max(newHeight, pictureBox.Height);

        using (Bitmap bmp = new Bitmap(newWidth, newHeight))
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.Transparent);

            // Offset to center the image within the bitmap based on bounds
            float offsetX = -minX + padding;
            float offsetY = -minY + padding;

            // Apply transformations
            g.TranslateTransform(offsetX, offsetY); // Adjust for new origin
            float centerX = originalImage.Width / 2f;
            float centerY = originalImage.Height / 2f;
            g.TranslateTransform(-centerX, -centerY); // Move to origin
            g.ScaleTransform(scaleX, scaleY); // Scale
            g.MultiplyTransform(new Matrix(1, shearY, shearX, 1, 0, 0)); // Shear
            g.RotateTransform(rotation); // Rotate
            g.TranslateTransform(translateX, translateY); // Apply user translation last

            // Draw the image
            g.DrawImage(originalImage, 0, 0);

            // Update the picture box
            transformedImage?.Dispose();
            transformedImage = (Image)bmp.Clone();
            pictureBox.Image = transformedImage;
        }
    }

    private void ResetButton_Click(object sender, EventArgs e)
    {
        rotationBox.Value = 0;
        translateXBox.Value = 0;
        translateYBox.Value = 0;
        shearXBox.Value = 0;
        shearYBox.Value = 0;
        scaleXBox.Value = 1;
        scaleYBox.Value = 1;

        if (originalImage != null)
        {
            transformedImage?.Dispose();
            transformedImage = (Image)originalImage.Clone();
            pictureBox.Image = transformedImage;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            originalImage?.Dispose();
            transformedImage?.Dispose();
        }
        base.Dispose(disposing);
    }
}