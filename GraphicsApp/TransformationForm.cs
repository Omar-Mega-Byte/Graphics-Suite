using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

public class TransformationForm : Form
{
    private PictureBox pictureBox;
    private NumericUpDown rotationBox, translateXBox, translateYBox, shearXBox, shearYBox, scaleXBox, scaleYBox;
    private CheckBox reflectXBox, reflectYBox;
    private Button resetButton;

    public TransformationForm()
    {
        Text = "Transformation Tester";
        Size = new Size(600, 400);

        // PictureBox for displaying the square
        pictureBox = new PictureBox
        {
            Location = new Point(10, 10),
            Size = new Size(300, 300),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White
        };
        pictureBox.Paint += PictureBox_Paint;

        // Rotation
        Label rotationLabel = new Label { Text = "Rotation", Location = new Point(320, 10) };
        Label degreeLabel = new Label { Text = "Degree", Location = new Point(340, 30) };
        rotationBox = new NumericUpDown
        {
            Location = new Point(400, 30),
            Width = 60,
            Minimum = -360,
            Maximum = 360,
            Value = 0
        };
        rotationBox.ValueChanged += ApplyTransformations;

        // Translation
        Label translationLabel = new Label { Text = "Translation", Location = new Point(320, 60) };
        Label translateXLabel = new Label { Text = "X", Location = new Point(340, 80) };
        translateXBox = new NumericUpDown
        {
            Location = new Point(400, 80),
            Width = 60,
            Minimum = -1000,
            Maximum = 1000,
            Value = 0
        };
        translateXBox.ValueChanged += ApplyTransformations;

        Label translateYLabel = new Label { Text = "Y", Location = new Point(340, 110) };
        translateYBox = new NumericUpDown
        {
            Location = new Point(400, 110),
            Width = 60,
            Minimum = -1000,
            Maximum = 1000,
            Value = 0
        };
        translateYBox.ValueChanged += ApplyTransformations;

        // Shear
        Label shearLabel = new Label { Text = "Shear", Location = new Point(320, 140) };
        Label shearXLabel = new Label { Text = "X", Location = new Point(340, 160) };
        shearXBox = new NumericUpDown
        {
            Location = new Point(400, 160),
            Width = 60,
            Minimum = -10,
            Maximum = 10,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 0
        };
        shearXBox.ValueChanged += ApplyTransformations;

        Label shearYLabel = new Label { Text = "Y", Location = new Point(340, 190) };
        shearYBox = new NumericUpDown
        {
            Location = new Point(400, 190),
            Width = 60,
            Minimum = -10,
            Maximum = 10,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 0
        };
        shearYBox.ValueChanged += ApplyTransformations;

        // Scale
        Label scaleLabel = new Label { Text = "Scale", Location = new Point(320, 220) };
        Label scaleXLabel = new Label { Text = "X", Location = new Point(340, 240) };
        scaleXBox = new NumericUpDown
        {
            Location = new Point(400, 240),
            Width = 60,
            Minimum = 0.1m,
            Maximum = 10,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 1
        };
        scaleXBox.ValueChanged += ApplyTransformations;

        Label scaleYLabel = new Label { Text = "Y", Location = new Point(340, 270) };
        scaleYBox = new NumericUpDown
        {
            Location = new Point(400, 270),
            Width = 60,
            Minimum = 0.1m,
            Maximum = 10,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 1
        };
        scaleYBox.ValueChanged += ApplyTransformations;

        // Reflection
        Label reflectLabel = new Label { Text = "Reflection", Location = new Point(320, 300) };
        reflectXBox = new CheckBox
        {
            Text = "Over X-Axis",
            Location = new Point(340, 320),
            Width = 100
        };
        reflectXBox.CheckedChanged += ApplyTransformations;

        reflectYBox = new CheckBox
        {
            Text = "Over Y-Axis",
            Location = new Point(340, 340),
            Width = 100
        };
        reflectYBox.CheckedChanged += ApplyTransformations;

        // Reset Button
        resetButton = new Button
        {
            Text = "Reset",
            Location = new Point(320, 360),
            Width = 100
        };
        resetButton.Click += ResetButton_Click;

        // Add controls to form
        Controls.AddRange(new Control[] {
            pictureBox,
            rotationLabel, degreeLabel, rotationBox,
            translationLabel, translateXLabel, translateXBox, translateYLabel, translateYBox,
            shearLabel, shearXLabel, shearXBox, shearYLabel, shearYBox,
            scaleLabel, scaleXLabel, scaleXBox, scaleYLabel, scaleYBox,
            reflectLabel, reflectXBox, reflectYBox,
            resetButton
        });

        // Initial draw
        ApplyTransformations(this, EventArgs.Empty);
    }

    private void PictureBox_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Color.White);

        // Get transformation parameters
        float scaleX = (float)scaleXBox.Value;
        float scaleY = (float)scaleYBox.Value;
        float shearX = (float)shearXBox.Value;
        float shearY = (float)shearYBox.Value;
        float rotation = (float)rotationBox.Value;
        float translateX = (float)translateXBox.Value;
        float translateY = -(float)translateYBox.Value; // Invert Y translation (positive Y moves up)
        bool reflectX = reflectXBox.Checked;
        bool reflectY = reflectYBox.Checked;

        // Define the square (100x100 pixels, centered in the PictureBox initially)
        float squareSize = 100;
        PointF[] squarePoints = new PointF[]
        {
            new PointF(-squareSize / 2, -squareSize / 2), // Top-left
            new PointF(squareSize / 2, -squareSize / 2),  // Top-right
            new PointF(squareSize / 2, squareSize / 2),   // Bottom-right
            new PointF(-squareSize / 2, squareSize / 2)   // Bottom-left
        };

        // Create transformation matrix (excluding user translation for bounds calculation)
        using (Matrix matrix = new Matrix())
        {
            matrix.Translate(-squareSize / 2, -squareSize / 2); // Move to origin
            if (reflectX) matrix.Scale(1, -1); // Reflect over X-axis (vertical flip)
            if (reflectY) matrix.Scale(-1, 1); // Reflect over Y-axis (horizontal flip)
            matrix.Scale(scaleX, scaleY); // Scale
            matrix.Shear(shearX, shearY); // Shear
            matrix.Rotate(rotation); // Rotate
            matrix.Translate(squareSize / 2, squareSize / 2); // Move back without user translation

            // Transform the square points to find the new bounds
            matrix.TransformPoints(squarePoints);
        }

        // Calculate the new bounds with padding
        float minX = squarePoints.Min(p => p.X);
        float maxX = squarePoints.Max(p => p.X);
        float minY = squarePoints.Min(p => p.Y);
        float maxY = squarePoints.Max(p => p.Y);

        int padding = 50; // Add padding to prevent clipping
        float boundsWidth = maxX - minX + 2 * padding;
        float boundsHeight = maxY - minY + 2 * padding;

        // Center the square in the PictureBox
        float offsetX = (pictureBox.Width - boundsWidth) / 2 - minX + padding;
        float offsetY = (pictureBox.Height - boundsHeight) / 2 - minY + padding;

        // Draw X and Y axes (before applying transformations to the square)
        using (Pen axisPen = new Pen(Color.Gray, 1))
        {
            // Center of the PictureBox
            float centerX = pictureBox.Width / 2f;
            float centerY = pictureBox.Height / 2f;

            // Draw X-axis (horizontal)
            g.DrawLine(axisPen, 0, centerY, pictureBox.Width, centerY);
            // Draw Y-axis (vertical)
            g.DrawLine(axisPen, centerX, 0, centerX, pictureBox.Height);

            // Draw tick marks on X-axis
            for (float x = centerX - 100; x <= centerX + 100; x += 20)
            {
                g.DrawLine(axisPen, x, centerY - 5, x, centerY + 5);
            }

            // Draw tick marks on Y-axis
            for (float y = centerY - 100; y <= centerY + 100; y += 20)
            {
                g.DrawLine(axisPen, centerX - 5, y, centerX + 5, y);
            }
        }

        // Apply transformations to the graphics context for the square
        g.TranslateTransform(offsetX, offsetY); // Center in PictureBox
        g.TranslateTransform(-squareSize / 2, -squareSize / 2); // Move to origin
        if (reflectX) g.ScaleTransform(1, -1); // Reflect over X-axis (vertical flip)
        if (reflectY) g.ScaleTransform(-1, 1); // Reflect over Y-axis (horizontal flip)
        g.ScaleTransform(scaleX, scaleY); // Scale
        g.MultiplyTransform(new Matrix(1, shearY, shearX, 1, 0, 0)); // Shear
        g.RotateTransform(rotation); // Rotate
        g.TranslateTransform(translateX, translateY); // Apply user translation last

        // Draw the square
        using (Pen pen = new Pen(Color.Black, 2))
        using (Brush brush = new SolidBrush(Color.LightBlue))
        {
            g.FillPolygon(brush, squarePoints);
            g.DrawPolygon(pen, squarePoints);
        }
    }

    private void ApplyTransformations(object sender, EventArgs e)
    {
        pictureBox.Invalidate(); // Redraw the PictureBox
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
        reflectXBox.Checked = false;
        reflectYBox.Checked = false;

        pictureBox.Invalidate(); // Redraw the PictureBox
    }
}